using ChessBot.Core.Algorithms;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace ChessBot.Core.Board
{
    public class ChessBoard
    {
        #region Members
        private readonly ChessPiece[,] _board;
        #endregion

        #region Properties
        public List<BoardPiece> BlackPieces { get; private set; } = new List<BoardPiece>(8);
        public List<BoardPiece> WhitePieces { get; private set; } = new List<BoardPiece>(8);
        public Move? LastMove { get; private set; }
        public int NumOfBlackPieces => BlackPieces.Count;
        public int NumOfWhitePieces => WhitePieces.Count;
        #endregion

        #region Constructor / Copy
        public ChessBoard()
        {
            _board = new ChessPiece[8, 8];
        }
        public ChessBoard Clone()
        {
            var copy = new ChessBoard();
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

        public bool HasPossibleMove(BoardPiece boardPiece)
        {
            return PossibleMoves.SingleMoves(this, boardPiece).Any() ||
                PossibleMoves.DoubleMove(this, boardPiece).Any() ||
                PossibleMoves.CapturingMove(this, boardPiece).Any() ||
                PossibleMoves.EnPassent(this, boardPiece).Any();
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

            return WhitePieces.SingleOrDefault(a => a.Rank == rank && a.File == file) 
                ?? BlackPieces.SingleOrDefault(a => a.Rank == rank && a.File == file);
        }

        public void Setup(IEnumerable<string> setupParts)
        {
            BoardInitializer.Setup(this, setupParts);
        }

        public ulong ComputeZobristHash(ChessColor sideToMove)
        {
            ulong hash = 0;
            foreach (var piece in BlackPieces.Concat(WhitePieces))
            {
                if (piece.ChessPiece == ChessPiece.Empty)
                    continue;

                int rankIndex = piece.Rank.ToIndex();
                int fileIndex = piece.File.ToIndex();
                int pieceIndex = (int)piece.ChessPiece;
                hash ^= Zobrist.Table[rankIndex, fileIndex, pieceIndex];
            }

            hash ^= sideToMove == ChessColor.White ? Zobrist.WhiteToMove : Zobrist.BlackToMove;
            return hash;
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
        internal void AddPiece(BoardPiece piece, bool unsafeInit = false)
        {
            if (!unsafeInit)
            {
                var occupant = GetPieceAt(piece.Rank, piece.File);
                if (occupant != null)
                {
                    RemovePiece(occupant);
                }
            }
            if(piece.ChessPiece == ChessPiece.White)
            {
                WhitePieces.Add(piece);
            }
            else
            {
                BlackPieces.Add(piece);
            }
            this[piece.Rank, piece.File] = piece.ChessPiece;
        }

        internal void RemovePiece(BoardPiece piece)
        {
            if(piece.ChessPiece == ChessPiece.White)
            {
                WhitePieces.Remove(piece);
            }
            else
            {
                BlackPieces.Remove(piece);
            }
            this[piece.Rank, piece.File] = ChessPiece.Empty;
        }

        internal void InitPiece(ChessRank rank, ChessFile file, ChessPiece piece, bool unsafeInit = false)
        {
            if (!unsafeInit)
            {
                var existing = GetPieceAt(rank, file);
                // Check if the piece is valid
                if (existing != null)
                {
                    // ... we could also throw an exception here, init shoult not be called with nonsense
                    RemovePiece(existing);
                }
            }
            if (piece != ChessPiece.Empty)
            {
                var boardPiece = new BoardPiece { File = file, Rank = rank, ChessPiece = piece };
                AddPiece(boardPiece, unsafeInit);
            }
            this[rank, file] = piece;
        }
        #endregion
    }
}