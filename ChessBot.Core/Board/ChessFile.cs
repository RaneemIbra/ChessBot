/*
 * ChessFile.cs
 * 
 * Summary:
 *   Defines the chessboard files (columns) and provides a helper extension method.
 */

namespace ChessBot.Core.Board
{
    /// <summary>
    /// Represents the files (columns) on a chessboard, ranging from A to H.
    /// </summary>
    public enum ChessFile : ushort
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7
    }

    /// <summary>
    /// Provides extension methods for the ChessFile enum.
    /// </summary>
    public static class ChessFileExtensions
    {
        /// <summary>
        /// Converts a ChessFile to its corresponding zero-based index.
        /// </summary>
        /// <param name="file">The ChessFile value.</param>
        /// <returns>The zero-based index of the file.</returns>
        public static ushort ToIndex(this ChessFile file)
        {
            return (ushort)file;
        }
    }
}
