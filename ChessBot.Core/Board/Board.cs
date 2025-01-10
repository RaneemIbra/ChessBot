namespace ChessBot.Core.Board
{
    public class Board
    {
        #region Members
        private readonly ChessPiece[,] _board;
        private readonly List<BoardPiece> _pieces = [];
        #endregion

        #region Properties
        public IEnumerable<BoardPiece> BlackPieces => _pieces.Where(w => w.ChessPiece == ChessPiece.Black);
        public IEnumerable<BoardPiece> WhitePieces => _pieces.Where(w => w.ChessPiece == ChessPiece.White);
        public Move? LastMove { get; private set; }
        public int NumOfBlackPieces => BlackPieces.Count();
        public int NumOfWhitePieces => WhitePieces.Count();
        #endregion

        #region Constructor / Copy
        public Board()
        {
            _board = new ChessPiece[8, 8];
        }
        public Board Clone()
        {
            var copy = new Board();
            foreach (var bPiece in BlackPieces)
            {
                copy.InitPiece(bPiece.Rank, bPiece.File, bPiece.ChessPiece, true);
            }
            foreach (var wPiece in WhitePieces)
            {
                copy.InitPiece(wPiece.Rank, wPiece.File, wPiece.ChessPiece, true);
            }
            return copy;
        }
        #endregion

        #region Public interface

        public ChessPiece this[int rank, int file]
        {
            get
            {
                if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                {
                    return _board[rank, file];
                }
                return ChessPiece.Invalid;
            }
            set
            {
                if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
                {
                    _board[rank, file] = value;
                }
                else
                {
                    if (rank < 0 || rank >= 8)
                    {
                        throw new ArgumentOutOfRangeException(nameof(rank));
                    }
                    if (file < 0 || file >= 8)
                    {
                        throw new ArgumentOutOfRangeException(nameof(file));
                    }
                }
            }
        }
        public ChessPiece this[ChessRank rank, ChessFile file]
        {
            get
            {
                return this[rank.ToIndex(), file.ToIndex()];
            }
            set
            {
                this[rank.ToIndex(), file.ToIndex()] = value;
            }
        }
        public IEnumerable<Move> GetPossibleMoves(BoardPiece boardPiece)
        {
            return
            [
                .. PossibleMoves.SingleMoves(this, boardPiece),
                .. PossibleMoves.DoubleMove(this, boardPiece),
                .. PossibleMoves.CapturingMove(this, boardPiece),
                .. PossibleMoves.EnPassent(this, boardPiece),
            ];
        }

        public BoardPiece? GetPieceAt(ChessRank rank, ChessFile file)
        {
            return _pieces.SingleOrDefault(a => a.Rank == rank && a.File == file);
        }

        public void Setup(IEnumerable<string> setupParts)
        {
            BoardInitializer.Setup(this, setupParts);
        }

        public void ExecuteMove(Move move)
        {
            if (move.CapturedPiece != null)
            {
                BoardManipulation.RemovePiece(this, move.CapturedPiece);
            }
            BoardManipulation.MovePiece(this, move.MovingPiece, move.TargetRank, move.TargetFile);
            LastMove = move;
            if (EndGame.IsGameOver(this))
            {
                return;
            }
        }

        public void PrintBoard()
        {
            Console.WriteLine("   | A | B | C | D | E | F | G | H |");
            Console.WriteLine("   |-------------------------------|");
            foreach (ChessRank rank in Enum.GetValues<ChessRank>().Reverse())
            {
                Console.Write($" {(ushort)rank} |");
                foreach (ChessFile file in Enum.GetValues<ChessFile>())
                {
                    var chessPiece = this[rank, file];
                    switch (chessPiece)
                    {
                        case ChessPiece.Empty:
                            Console.Write("   |");
                            break;
                        case ChessPiece.White:
                            Console.Write(" W |");
                            break;
                        case ChessPiece.Black:
                            Console.Write(" B |");
                            break;
                    }

                }
                Console.Write($" {(ushort)rank} {Environment.NewLine}");
            }
            Console.WriteLine("   |-------------------------------|");
            Console.WriteLine("   | A | B | C | D | E | F | G | H |");
        }
        public void ClearBoard()
        {
            BoardInitializer.ClearBoard(this);
        }
        #endregion

        #region Private Helpers
        internal void AddPiece(BoardPiece piece)
        {
            var occupant = GetPieceAt(piece.Rank, piece.File);
            if (occupant != null)
            {
                _pieces.Remove(occupant);
            }
            _pieces.Add(piece);
            this[piece.Rank, piece.File] = piece.ChessPiece;
        }

        internal void RemovePiece(BoardPiece piece)
        {
            var occupant = GetPieceAt(piece.Rank, piece.File);
            if (occupant != null)
            {
                _pieces.Remove(occupant);
                this[piece.Rank, piece.File] = ChessPiece.Empty;
            }
        }

        internal void InitPiece(ChessRank rank, ChessFile file, ChessPiece piece, bool unsafeInit = false)
        {
            if (!unsafeInit)
            {
                var existing = _pieces.FirstOrDefault(a => a.Rank == rank && a.File == file);
                // Check if the piece is valid
                if (existing != null)
                {
                    // ... we could also throw an exception here, init shoult not be called with nonsense
                    _pieces.Remove(existing);
                }
            }
            if (piece != ChessPiece.Empty)
            {
                var boardPiece = new BoardPiece { File = file, Rank = rank, ChessPiece = piece };
                _pieces.Add(boardPiece);
            }
            this[rank, file] = piece;
        }
        #endregion
    }
}