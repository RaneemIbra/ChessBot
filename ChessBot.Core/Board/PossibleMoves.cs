namespace ChessBot.Core.Board
{
    public static class PossibleMoves
    {
        /// <summary>
        /// Generates the possible single move for a given piece.
        /// A single move is a move by one rank forward for pawns.
        /// </summary>
        /// <param name="board">The current chess board.</param>
        /// <param name="piece">The piece to move.</param>
        /// <returns>Yields possible single moves.</returns>
        public static IEnumerable<Move> SingleMoves(ChessBoard board, BoardPiece piece)
        {
            if (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black)
            {
                yield break;
            }

            int direction = piece.ChessPiece == ChessPiece.White ? 1 : -1;
            ushort currentRankIndex = piece.Rank.ToIndex();
            int nextRankIndex = currentRankIndex + direction;

            if (nextRankIndex < 0 || nextRankIndex >= 8)
            {
                yield break;
            }

            ChessRank targetRank = ((ushort)nextRankIndex).FromIndex();
            ushort currentFile = piece.File.ToIndex();

            if (board[nextRankIndex, currentFile] == ChessPiece.Empty)
            {
                yield return new Move
                {
                    MovingPiece = piece,
                    TargetRank = targetRank,
                    TargetFile = piece.File
                };
            }
        }

        /// <summary>
        /// Generates the possible double move for a given piece.
        /// A double move is a two-square move for pawns if it's their initial move.
        /// </summary>
        /// <param name="board">The current chess board.</param>
        /// <param name="piece">The piece to move.</param>
        /// <returns>Yields possible double moves.</returns>
        public static IEnumerable<Move> DoubleMove(ChessBoard board, BoardPiece piece)
        {
            if (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black)
            {
                yield break;
            }

            int direction = piece.ChessPiece == ChessPiece.White ? 2 : -2;

            // Ensure it's the piece's initial rank to allow the double move
            if ((piece.ChessPiece == ChessPiece.White && piece.Rank != ChessRank.Two) ||
                (piece.ChessPiece == ChessPiece.Black && piece.Rank != ChessRank.Seven))
            {
                yield break;
            }

            ushort currentRankIndex = piece.Rank.ToIndex();
            int targetRankIndex = currentRankIndex + direction;
            int intermediateRankIndex = currentRankIndex + (direction / 2);

            if (targetRankIndex < 0 || targetRankIndex >= 8 ||
                intermediateRankIndex < 0 || intermediateRankIndex >= 8)
            {
                yield break;
            }

            ChessRank targetRank = ((ushort)targetRankIndex).FromIndex();
            ushort currentFile = piece.File.ToIndex();

            // Ensure both the intermediate and target squares are empty
            if (board[intermediateRankIndex, currentFile] == ChessPiece.Empty &&
                board[targetRankIndex, currentFile] == ChessPiece.Empty)
            {
                yield return new Move
                {
                    MovingPiece = piece,
                    TargetRank = targetRank,
                    TargetFile = piece.File
                };
            }
        }

        /// <summary>
        /// Generates the possible capturing moves for a given piece.
        /// A capturing move is a move where the piece captures an opponent's piece diagonally.
        /// </summary>
        /// <param name="board">The current chess board.</param>
        /// <param name="piece">The piece to move.</param>
        /// <returns>Yields possible capturing moves.</returns>
        public static IEnumerable<Move> CapturingMove(ChessBoard board, BoardPiece piece)
        {
            if (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black)
            {
                yield break;
            }

            int direction = piece.ChessPiece == ChessPiece.White ? 1 : -1;
            ChessPiece opponentPiece = piece.ChessPiece == ChessPiece.White ? ChessPiece.Black : ChessPiece.White;
            ushort currentRankIndex = piece.Rank.ToIndex();
            ushort currentFileIndex = piece.File.ToIndex();
            int[] diagonalOffsets = [-1, 1];

            foreach (int offset in diagonalOffsets)
            {
                int targetFileIndex = currentFileIndex + offset;
                int targetRankIndex = currentRankIndex + direction;

                // Ensure the target square is within bounds
                if (targetRankIndex >= 0 && targetRankIndex < 8 && targetFileIndex >= 0 && targetFileIndex < 8)
                {
                    if (board[targetRankIndex, targetFileIndex] == opponentPiece)
                    {
                        yield return new Move
                        {
                            MovingPiece = piece,
                            TargetRank = ((ushort)targetRankIndex).FromIndex(),
                            TargetFile = (ChessFile)targetFileIndex,
                            CapturedPiece = board.GetPieceAt(
                                ((ushort)targetRankIndex).FromIndex(),
                                (ChessFile)targetFileIndex
                            )
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Generates the possible en passant capture move for a given piece.
        /// En passant is a special pawn capture move under specific conditions.
        /// </summary>
        /// <param name="board">The current chess board.</param>
        /// <param name="piece">The piece to move.</param>
        /// <returns>Yields possible en passant moves.</returns>
        public static IEnumerable<Move> EnPassent(ChessBoard board, BoardPiece piece)
        {
            if (board.LastMove == null ||
                (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black))
            {
                yield break;
            }

            int direction = piece.ChessPiece == ChessPiece.White ? 1 : -1;
            ChessRank requiredRank = piece.ChessPiece == ChessPiece.White ? ChessRank.Five : ChessRank.Four;

            if (piece.Rank != requiredRank)
            {
                yield break;
            }

            ChessPiece opponentPiece = piece.ChessPiece == ChessPiece.White ? ChessPiece.Black : ChessPiece.White;
            ushort currentRankIndex = piece.Rank.ToIndex();
            ushort currentFileIndex = piece.File.ToIndex();
            int[] diagonalOffsets = [-1, 1];

            foreach (int offset in diagonalOffsets)
            {
                int targetFileIndex = currentFileIndex + offset;
                if (targetFileIndex >= 0 && targetFileIndex < 8)
                {
                    if (board[currentRankIndex, targetFileIndex] == opponentPiece &&
                        board.LastMove.TargetRank == piece.Rank &&
                        board.LastMove.TargetFile == (ChessFile)targetFileIndex)
                    {
                        ushort newRankIndex = (ushort)(currentRankIndex + direction);
                        yield return new Move
                        {
                            MovingPiece = piece,
                            TargetRank = newRankIndex.FromIndex(),
                            TargetFile = (ChessFile)targetFileIndex,
                            CapturedPiece = board.GetPieceAt(piece.Rank, (ChessFile)targetFileIndex)
                        };
                    }
                }
            }
        }
    }
}
