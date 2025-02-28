/*
 * ChessColor.cs
 * 
 * Summary:
 *   Defines the possible colors for chess pieces and players.
 */

namespace ChessBot.Core.Board
{
    /// <summary>
    /// Represents the color of a chess piece or player.
    /// </summary>
    public enum ChessColor
    {
        /// <summary>Represents a white piece or player.</summary>
        White,

        /// <summary>Represents a black piece or player.</summary>
        Black,

        /// <summary>Represents the absence of color (e.g., an empty square).</summary>
        None
    }
}
