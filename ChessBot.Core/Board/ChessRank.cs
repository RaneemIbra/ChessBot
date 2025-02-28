/*
 * ChessRank.cs
 * 
 * Summary:
 *   Defines the ranks of a chessboard (1 to 8) and provides extension methods to convert them to indices.
 */

namespace ChessBot.Core.Board
{
    /// <summary>
    /// Represents the ranks (rows) on a chessboard, from 1 to 8.
    /// </summary>
    public enum ChessRank : ushort
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    }

    /// <summary>
    /// Extension methods for converting between ChessRank and array index.
    /// </summary>
    public static class ChessRankExtension
    {
        /// <summary>
        /// Converts a ChessRank enum value to its corresponding 0-based index.
        /// </summary>
        /// <param name="rank">The ChessRank value to convert.</param>
        /// <returns>The corresponding 0-based index.</returns>
        public static ushort ToIndex(this ChessRank rank)
        {
            return (ushort)(rank - 1);
        }

        /// <summary>
        /// Converts a 0-based index to its corresponding ChessRank enum value.
        /// </summary>
        /// <param name="index">The 0-based index to convert.</param>
        /// <returns>The corresponding ChessRank value.</returns>
        public static ChessRank FromIndex(this ushort index)
        {
            return (ChessRank)(index + 1);
        }
    }
}
