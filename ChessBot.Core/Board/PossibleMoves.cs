using ChessBot.Core.Board;

public static class PossibleMoves
{
    public static IEnumerable<Move> SingleMoves(Board board, BoardPiece piece)
    {
        if (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black)
        {
            yield break;
        }
        int direction = piece.ChessPiece == ChessPiece.White ? 1 : -1;
        ushort targetRankIndex = (ushort)(piece.Rank.ToIndex() + direction);
        if (targetRankIndex < 0 || targetRankIndex >= 8)
        {
            yield break;
        }
        ChessRank targetRank = (ChessRank)targetRankIndex;
        ushort currentFile = piece.File.ToIndex();
        if (board[targetRankIndex, currentFile] == ChessPiece.Empty)
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
        if ((piece.ChessPiece == ChessPiece.White && piece.Rank != ChessRank.Two) ||
            (piece.ChessPiece == ChessPiece.Black && piece.Rank != ChessRank.Seven))
        {
            yield break;
        }
        ushort targetRankIndex = (ushort)(piece.Rank.ToIndex() + direction);
        ushort intermediateRankIndex = (ushort)(piece.Rank.ToIndex() + (direction / 2));
        if (targetRankIndex < 0 || targetRankIndex >= 8 || intermediateRankIndex < 0 || intermediateRankIndex >= 8)
        {
            yield break;
        }
        ChessRank targetRank = (ChessRank)targetRankIndex;
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
        int[] diagonalOffsets = { -1, 1 };
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
                        TargetRank = (ChessRank)targetRankIndex,
                        TargetFile = (ChessFile)targetFileIndex,
                        CapturedPiece = board.GetPieceAt((ChessRank)targetRankIndex, (ChessFile)targetFileIndex)
                    };
                }
            }
        }
    }

    public static IEnumerable<Move> EnPassent(Board board, BoardPiece piece)
    {
        if (board.LastMove == null ||
            (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black))
        {
            yield break;
        }
        int direction = piece.ChessPiece == ChessPiece.White ? 1 : -1;
        ChessRank requiredRank = piece.ChessPiece == ChessPiece.White ? ChessRank.Five : ChessRank.Four;
        ChessPiece opponentPiece = piece.ChessPiece == ChessPiece.White ? ChessPiece.Black : ChessPiece.White;
        if (piece.Rank != requiredRank)
        {
            yield break;
        }
        ushort currentRankIndex = piece.Rank.ToIndex();
        ushort currentFileIndex = piece.File.ToIndex();
        int[] diagonalOffsets = { -1, 1 };
        foreach (int offset in diagonalOffsets)
        {
            int targetFileIndex = currentFileIndex + offset;
            if (targetFileIndex >= 0 && targetFileIndex < 8)
            {
                if (board[currentRankIndex, targetFileIndex] == opponentPiece &&
                    board.LastMove.TargetRank == piece.Rank && board.LastMove.TargetFile == (ChessFile)targetFileIndex)
                {
                    yield return new Move
                    {
                        MovingPiece = piece,
                        TargetRank = (ChessRank)(currentRankIndex + direction),
                        TargetFile = (ChessFile)targetFileIndex,
                        CapturedPiece = board.GetPieceAt(piece.Rank, (ChessFile)targetFileIndex)
                    };
                }
            }
        }
    }
}
