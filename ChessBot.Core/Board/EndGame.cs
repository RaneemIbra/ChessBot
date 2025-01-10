namespace ChessBot.Core.Board
{
    public static class EndGame
    {
        public static bool IsGameOver(Board board)
        {
            return IsGameOver(board, out _);
        }
        public static bool IsGameOver(Board board, out string? message)
        {
            if (board.NumOfBlackPieces == 0)
            {
                message = "Game Over! White wins - No black pawns left.";
                return true;
            }
            if (board.NumOfWhitePieces == 0)
            {
                message = "Game Over! Black wins - No white pawns left.";
                return true;
            }
            if (board.WhitePieces.Any(piece => piece.Rank == ChessRank.Eight))
            {
                message = "Game Over! White wins - White pawn reached the last rank.";
                return true;
            }
            if (board.BlackPieces.Any(piece => piece.Rank == ChessRank.One))
            {
                message = "Game Over! Black wins - Black pawn reached the last rank.";
                return true;
            }
            if (!board.BlackPieces.Any(piece => board.GetPossibleMoves(piece).Any()))
            {
                message = "Game Over! White wins - Black has no valid moves left.";
                return true;
            }
            if (!board.WhitePieces.Any(piece => board.GetPossibleMoves(piece).Any()))
            {
                message = "Game Over! Black wins - White has no valid moves left.";
                return true;
            }
            message = null;
            return false;
        }
    }
}