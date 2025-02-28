using System;
using System.Linq;

namespace ChessBot.Core.Board
{
    public static class EndGame
    {
        /// <summary>
        /// Checks if the game is over. Returns a boolean indicating the game state.
        /// </summary>
        /// <param name="board">The chessboard instance.</param>
        /// <returns>True if the game is over, otherwise false.</returns>
        public static bool IsGameOver(ChessBoard board)
        {
            return IsGameOver(board, out _);
        }

        /// <summary>
        /// Checks if the game is over and provides a message indicating why.
        /// </summary>
        /// <param name="board">The chessboard instance.</param>
        /// <param name="message">The message explaining the result of the game.</param>
        /// <returns>True if the game is over, otherwise false.</returns>
        public static bool IsGameOver(ChessBoard board, out string? message)
        {
            // Check if there are no pieces for either player
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

            // Check if any white pawn has reached the last rank
            foreach (var piece in board.WhitePieces)
            {
                if (piece.Rank == ChessRank.Eight)
                {
                    message = "Game Over! White wins - White pawn reached the last rank.";
                    return true;
                }
            }

            // Check if White has no valid moves left
            bool whiteHasPossibleMove = board.WhitePieces.Any(piece => board.HasPossibleMove(piece));
            if (!whiteHasPossibleMove)
            {
                message = "Game Over! Black wins - White has no valid moves left.";
                return true;
            }

            // Check if any black pawn has reached the first rank
            foreach (var piece in board.BlackPieces)
            {
                if (piece.Rank == ChessRank.One)
                {
                    message = "Game Over! Black wins - Black pawn reached the last rank.";
                    return true;
                }
            }

            // Check if Black has no valid moves left
            bool blackHasPossibleMove = board.BlackPieces.Any(piece => board.HasPossibleMove(piece));
            if (!blackHasPossibleMove)
            {
                message = "Game Over! White wins - Black has no valid moves left.";
                return true;
            }

            message = null; // Game is not over
            return false;
        }

        /// <summary>
        /// Determines the winner of the game.
        /// </summary>
        /// <param name="board">The chessboard instance.</param>
        /// <returns>The winner's color (White or Black).</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game is not over yet.</exception>
        public static ChessColor GetWinner(ChessBoard board)
        {
            // Check if White wins
            if (board.NumOfBlackPieces == 0 ||
                board.WhitePieces.Any(piece => piece.Rank == ChessRank.Eight) ||
                !board.BlackPieces.Any(piece => board.HasPossibleMove(piece)))
            {
                return ChessColor.White;
            }

            // Check if Black wins
            if (board.NumOfWhitePieces == 0 ||
                board.BlackPieces.Any(piece => piece.Rank == ChessRank.One) ||
                !board.WhitePieces.Any(piece => board.HasPossibleMove(piece)))
            {
                return ChessColor.Black;
            }

            // If none of the above, the game is not over yet
            throw new InvalidOperationException("The game is not over yet.");
        }
    }
}
