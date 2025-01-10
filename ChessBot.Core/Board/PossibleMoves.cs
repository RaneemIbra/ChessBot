namespace ChessBot.Core.Board
{
    public static class PossibleMoves
    {
        public static IEnumerable<Move> SingleMoves(Board board, BoardPiece piece)
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

        public static IEnumerable<Move> DoubleMove(Board board, BoardPiece piece)
        {
            if (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black)
            {
                yield break;
            }
            int direction = piece.ChessPiece == ChessPiece.White ? 2 : -2;
            if (piece.ChessPiece == ChessPiece.White && piece.Rank != ChessRank.Two ||
                piece.ChessPiece == ChessPiece.Black && piece.Rank != ChessRank.Seven)
            {
                yield break;
            }
            ushort currentRankIndex = piece.Rank.ToIndex();
            int targetRankIndex = currentRankIndex + direction;
            int intermediateRankIndex = currentRankIndex + direction / 2;
            if (targetRankIndex < 0 || targetRankIndex >= 8 ||
                intermediateRankIndex < 0 || intermediateRankIndex >= 8)
            {
                yield break;
            }
            ChessRank targetRank = ((ushort)targetRankIndex).FromIndex();
            ushort currentFile = piece.File.ToIndex();
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

        public static IEnumerable<Move> CapturingMove(Board board, BoardPiece piece)
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

        public static IEnumerable<Move> EnPassent(Board board, BoardPiece piece)
        {
            if (board.LastMove == null ||
                piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black)
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