/*
 * BoardInitializer.cs
 * 
 * Summary:
 *   This file provides methods for initializing and resetting the chessboard.
 *   The Setup method places pieces on the board based on a given setup configuration,
 *   while the ClearBoard method resets the board to an empty state.
 */

namespace ChessBot.Core.Board
{
    /// <summary>
    /// Provides methods for setting up and clearing the chessboard.
    /// </summary>
    public static class BoardInitializer
    {
        /// <summary>
        /// Initializes the board with the specified setup configuration.
        /// Each setup string should have the format: "w[a-h][1-8]" or "b[a-h][1-8]" (e.g., "wa2" for a white pawn on a2).
        /// </summary>
        /// <param name="board">The chessboard to initialize.</param>
        /// <param name="setupParts">A collection of strings defining the initial piece placements.</param>
        public static void Setup(ChessBoard board, IEnumerable<string> setupParts)
        {
            foreach (var part in setupParts)
            {
                if (part.Length != 3)
                    continue; // Ignore malformed setup strings

                ChessPiece piece = char.ToLower(part[0]) == 'w' ? ChessPiece.White : ChessPiece.Black;
                ChessFile file = (ChessFile)(part[1] - 'a'); // Convert 'a'-'h' to 0-7
                ChessRank rank = (ChessRank)(part[2] - '1' + 1); // Convert '1'-'8' to 1-8

                board.InitPiece(rank, file, piece);
            }
        }

        /// <summary>
        /// Clears the board by setting all squares to empty.
        /// </summary>
        /// <param name="board">The chessboard to clear.</param>
        public static void ClearBoard(ChessBoard board)
        {
            foreach (ChessRank rank in Enum.GetValues<ChessRank>())
            {
                foreach (ChessFile file in Enum.GetValues<ChessFile>())
                {
                    board.InitPiece(rank, file, ChessPiece.Empty);
                }
            }
        }
    }
}
