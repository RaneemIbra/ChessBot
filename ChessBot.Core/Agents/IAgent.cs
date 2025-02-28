/*
 * IAgent.cs
 * 
 * Summary:
 *   The IAgent interface defines the contract for any chess-playing agent in the project.
 *   Any class implementing this interface must specify the agent's chess color (White or Black)
 *   and implement the GetMove method, which takes the current board state as input and returns 
 *   a Move object representing the agent's chosen move.
 *
 * Usage:
 *   - Both human-controlled players (e.g., HumanPlayer) and computer-controlled agents 
 *     (e.g., FinalAgent, SimpleAgent) implement this interface.
 *   - The game loop interacts with agents through this interface, allowing it to call GetMove 
 *     on any agent regardless of its underlying implementation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBot.Core.Board;

namespace ChessBot.Core.Agents
{
    public interface IAgent
    {
        // The chess color assigned to the agent (e.g., White or Black).
        ChessColor Color { get; set; }

        /// <summary>
        /// Given the current chess board state, returns the move chosen by the agent.
        /// For human players, this may involve prompting for input, while for computer agents,
        /// it involves calculating the best move using search algorithms.
        /// </summary>
        /// <param name="board">The current state of the chess board.</param>
        /// <returns>A Move object representing the agent's chosen move.</returns>
        Move GetMove(ChessBoard board);
    }
}
