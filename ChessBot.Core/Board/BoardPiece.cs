/*
 * BoardPiece.cs
 * 
 * Summary:
 *   Represents a single piece on the chessboard, including its rank, file, and type.
 */

namespace ChessBot.Core.Board
{
    /// <summary>
    /// Represents a piece on the chessboard with its position and type.
    /// </summary>
    public class BoardPiece
    {
        /// <summary>
        /// The rank (row) of the piece on the board.
        /// </summary>
        public ChessRank Rank { get; set; }

        /// <summary>
        /// The file (column) of the piece on the board.
        /// </summary>
        public ChessFile File { get; set; }

        /// <summary>
        /// The type of the chess piece (e.g., White, Black, or Empty).
        /// </summary>
        public ChessPiece ChessPiece { get; set; }

        /// <summary>
        /// Checks if two BoardPiece instances are equal based on their position.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the pieces have the same position; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not BoardPiece piece) return false;
            return piece.Rank == Rank && piece.File == File && piece.ChessPiece == ChessPiece;
        }

        /// <summary>
        /// Generates a hash code for the BoardPiece based on rank, file, and piece type.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Rank, File, ChessPiece);
        }
    }
}
