/*
 * ChessPiece.cs
 * 
 * Summary:
 *   Defines the different types of chess pieces and their corresponding values.
 */

namespace ChessBot.Core.Board
{
    /// <summary>
    /// Represents the different types of chess pieces on the board.
    /// </summary>
    public enum ChessPiece : ushort
    {
        /// <summary>
        /// Indicates no piece at a given position on the board.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Represents a white chess piece.
        /// </summary>
        White = 1,

        /// <summary>
        /// Represents a black chess piece.
        /// </summary>
        Black = 2,

        /// <summary>
        /// Represents an invalid piece (used for error handling).
        /// </summary>
        Invalid = 3
    }
}
