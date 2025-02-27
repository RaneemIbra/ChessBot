using System.IO.Pipelines;

namespace ChessBot.Core.Board
{
    public static class EndGame
    {
        public static bool IsGameOver(ChessBoard board)
        {
            return IsGameOver(board, out _);
        }
        public static bool IsGameOver(ChessBoard board, out string? message)
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
            bool hasPossibleMove = false;
            foreach (var piece in board.WhitePieces)
            {
                if (piece.Rank == ChessRank.Eight)
                {
                    message = "Game Over! White wins - White pawn reached the last rank.";
                    return true;
                }
                if (!hasPossibleMove)
                {
                    hasPossibleMove |= board.HasPossibleMove(piece);
                }
            }
            if(!hasPossibleMove)
            {
                message = "Game Over! Black wins - White has no valid moves left.";
                return true;
            }
            hasPossibleMove = false;
            foreach (var piece in board.BlackPieces)
            {
                if (piece.Rank == ChessRank.One)
                {
                    message = "Game Over! Black wins - Black pawn reached the last rank.";
                    return true;
                }
                if (!hasPossibleMove)
                {
                    hasPossibleMove |= board.HasPossibleMove(piece);
                }
            }
            if (!hasPossibleMove)
            {
                message = "Game Over! White wins - Black has no valid moves left.";
                return true;
            }
            message = null;
            return false;
        }
        
        public static ChessColor GetWinner(ChessBoard board)
        {
            if (board.NumOfBlackPieces == 0 || board.WhitePieces.Any(piece => piece.Rank == ChessRank.Eight) || !board.BlackPieces.Any(piece => board.HasPossibleMove(piece)))
            {
                return ChessColor.White;
            }
            if (board.NumOfWhitePieces == 0 || board.BlackPieces.Any(piece => piece.Rank == ChessRank.One) || !board.WhitePieces.Any(piece => board.HasPossibleMove(piece)))
            {
                return ChessColor.Black;
            }
            throw new InvalidOperationException("The game is not over yet.");
        }
    }
}