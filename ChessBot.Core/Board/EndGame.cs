using ChessBot.Core.Board;

public static class EndGame
{
    public static bool IsGameOver(Board board)
    {
        if (board.NumOfBlackPieces == 0)
        {
            Console.WriteLine("Game Over! White wins - No black pawns left.");
            return true;
        }
        if (board.NumOfWhitePieces == 0)
        {
            Console.WriteLine("Game Over! Black wins - No white pawns left.");
            return true;
        }
        if (board.WhitePieces.Any(piece => piece.Rank == ChessRank.Eight))
        {
            Console.WriteLine("Game Over! White wins - White pawn reached the last rank.");
            return true;
        }
        if (board.BlackPieces.Any(piece => piece.Rank == ChessRank.One))
        {
            Console.WriteLine("Game Over! Black wins - Black pawn reached the last rank.");
            return true;
        }
        if (!board.BlackPieces.Any(piece => board.GetPossibleMoves(piece).Any()))
        {
            Console.WriteLine("Game Over! White wins - Black has no valid moves left.");
            return true;
        }
        if (!board.WhitePieces.Any(piece => board.GetPossibleMoves(piece).Any()))
        {
            Console.WriteLine("Game Over! Black wins - White has no valid moves left.");
            return true;
        }
        return false;
    }
}
