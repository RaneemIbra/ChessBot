using ChessBot.Core.Algorithms;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace ChessBot.Core.Board
{
    /// <summary>
    /// Represents the chessboard and manages the state of the game, including pieces, moves, and board manipulation.
    /// </summary>
    public class ChessBoard
    {
        #region Members
        private readonly ChessPiece[,] _board;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of black pieces on the board.
        /// </summary>
        public List<BoardPiece> BlackPieces { get; private set; } = new List<BoardPiece>(8);

        /// <summary>
        /// Gets the list of white pieces on the board.
        /// </summary>
        public List<BoardPiece> WhitePieces { get; private set; } = new List<BoardPiece>(8);

        /// <summary>
        /// Gets the last move made on the board.
        /// </summary>
        public Move? LastMove { get; private set; }

        /// <summary>
        /// Gets the number of black pieces on the board.
        /// </summary>
        public int NumOfBlackPieces => BlackPieces.Count;

        /// <summary>
        /// Gets the number of white pieces on the board.
        /// </summary>
        public int NumOfWhitePieces => WhitePieces.Count;
        #endregion

        #region Constructor / Copy

        /// <summary>
        /// Initializes a new chessboard with an empty 8x8 grid.
        /// </summary>
        public ChessBoard()
        {
            _board = new ChessPiece[8, 8];
        }

        /// <summary>
        /// Creates a clone of the current chessboard.
        /// </summary>
        /// <returns>A new instance of the chessboard that is a copy of the current one.</returns>
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

        /// <summary>
        /// Gets or sets a chess piece at a specified rank and file.
        /// </summary>
        /// <param name="rank">The rank (row) of the chess piece.</param>
        /// <param name="file">The file (column) of the chess piece.</param>
        /// <returns>The chess piece at the specified location.</returns>
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

        /// <summary>
        /// Gets or sets a chess piece at a specified rank and file using <see cref="ChessRank"/> and <see cref="ChessFile"/>.
        /// </summary>
        /// <param name="rank">The rank (row) of the chess piece.</param>
        /// <param name="file">The file (column) of the chess piece.</param>
        /// <returns>The chess piece at the specified location.</returns>
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

        /// <summary>
        /// Determines if the specified piece has any possible moves on the board.
        /// </summary>
        /// <param name="boardPiece">The piece to check for possible moves.</param>
        /// <returns>True if the piece has any possible moves, false otherwise.</returns>
        public bool HasPossibleMove(BoardPiece boardPiece)
        {
            return PossibleMoves.SingleMoves(this, boardPiece).Any() ||
                PossibleMoves.DoubleMove(this, boardPiece).Any() ||
                PossibleMoves.CapturingMove(this, boardPiece).Any() ||
                PossibleMoves.EnPassent(this, boardPiece).Any();
        }

        /// <summary>
        /// Gets all the possible moves for the specified board piece.
        /// </summary>
        /// <param name="boardPiece">The board piece to check for possible moves.</param>
        /// <returns>A collection of all possible moves for the piece.</returns>
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

        /// <summary>
        /// Gets the piece at the specified rank and file.
        /// </summary>
        /// <param name="rank">The rank (row) of the square to check.</param>
        /// <param name="file">The file (column) of the square to check.</param>
        /// <returns>The piece at the specified square, or null if no piece exists there.</returns>
        public BoardPiece? GetPieceAt(ChessRank rank, ChessFile file)
        {
            return WhitePieces.SingleOrDefault(a => a.Rank == rank && a.File == file)
                ?? BlackPieces.SingleOrDefault(a => a.Rank == rank && a.File == file);
        }

        /// <summary>
        /// Sets up the board according to the provided setup string parts.
        /// </summary>
        /// <param name="setupParts">An enumeration of setup string parts defining piece positions.</param>
        public void Setup(IEnumerable<string> setupParts)
        {
            BoardInitializer.Setup(this, setupParts);
        }

        /// <summary>
        /// Computes the Zobrist hash of the current board state.
        /// </summary>
        /// <param name="sideToMove">The side to move (either White or Black).</param>
        /// <returns>The Zobrist hash representing the board state.</returns>
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

        /// <summary>
        /// Executes a move on the board, updating the board state.
        /// </summary>
        /// <param name="move">The move to execute.</param>
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

        /// <summary>
        /// Prints the current state of the board to the console in a human-readable format.
        /// </summary>
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

        /// <summary>
        /// Clears the board by removing all pieces.
        /// </summary>
        public void ClearBoard()
        {
            BoardInitializer.ClearBoard(this);
        }
        #endregion

        #region Private Helpers

        /// <summary>
        /// Adds a piece to the board at the specified location.
        /// </summary>
        /// <param name="piece">The piece to add.</param>
        /// <param name="unsafeInit">Whether to allow overwriting an existing piece without validation.</param>
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
            if (piece.ChessPiece == ChessPiece.White)
            {
                WhitePieces.Add(piece);
            }
            else
            {
                BlackPieces.Add(piece);
            }
            this[piece.Rank, piece.File] = piece.ChessPiece;
        }

        /// <summary>
        /// Removes a piece from the board.
        /// </summary>
        /// <param name="piece">The piece to remove.</param>
        internal void RemovePiece(BoardPiece piece)
        {
            if (piece.ChessPiece == ChessPiece.White)
            {
                WhitePieces.Remove(piece);
            }
            else
            {
                BlackPieces.Remove(piece);
            }
            this[piece.Rank, piece.File] = ChessPiece.Empty;
        }

        /// <summary>
        /// Initializes a piece on the board at the specified location.
        /// </summary>
        /// <param name="rank">The rank (row) of the piece.</param>
        /// <param name="file">The file (column) of the piece.</param>
        /// <param name="piece">The piece to initialize.</param>
        /// <param name="unsafeInit">Whether to allow overwriting an existing piece without validation.</param>
        internal void InitPiece(ChessRank rank, ChessFile file, ChessPiece piece, bool unsafeInit = false)
        {
            if (!unsafeInit)
            {
                var existing = GetPieceAt(rank, file);
                // Check if the piece is valid
                if (existing != null)
                {
                    // ... we could also throw an exception here, init shouldn't be called with nonsense
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
