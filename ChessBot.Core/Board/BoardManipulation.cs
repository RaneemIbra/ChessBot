/*
 * BoardManipulation.cs
 * 
 * Summary:
 *   This class provides utility methods to manipulate pieces on the chessboard.
 *   It includes functions to move and remove pieces.
 */

namespace ChessBot.Core.Board
{
    /// <summary>
    /// Provides methods to manipulate the chessboard by moving and removing pieces.
    /// </summary>
    public static class BoardManipulation
    {
        /// <summary>
        /// Moves a piece to a new location on the board.
        /// </summary>
        /// <param name="board">The chessboard instance.</param>
        /// <param name="piece">The piece to be moved.</param>
        /// <param name="targetRank">The destination rank.</param>
        /// <param name="targetFile">The destination file.</param>
        /// <returns>The new piece instance after the move.</returns>
        public static BoardPiece MovePiece(ChessBoard board, BoardPiece piece, ChessRank targetRank, ChessFile targetFile)
        {
            // Ensure the piece exists on the board before attempting to move it
            if (board.GetPieceAt(piece.Rank, piece.File) == null)
                throw new InvalidOperationException("The specified piece does not exist on the board.");

            RemovePiece(board, piece); // Remove the piece from its current location

            var newPiece = new BoardPiece
            {
                ChessPiece = piece.ChessPiece,
                Rank = targetRank,
                File = targetFile
            };

            board.AddPiece(newPiece);
            return newPiece;
        }

        /// <summary>
        /// Removes a piece from the board.
        /// </summary>
        /// <param name="board">The chessboard instance.</param>
        /// <param name="piece">The piece to be removed.</param>
        public static void RemovePiece(ChessBoard board, BoardPiece piece)
        {
            var occupant = board.GetPieceAt(piece.Rank, piece.File);
            if (occupant != null)
            {
                board.RemovePiece(occupant);
            }
        }
    }
}
