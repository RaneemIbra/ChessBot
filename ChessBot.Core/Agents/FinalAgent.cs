/*
 * FinalAgent.cs
 * 
 * Summary:
 *   The FinalAgent class implements the IAgent interface and represents an advanced
 *   chess-playing agent that selects moves using an iterative deepening search algorithm.
 *   It is designed to work under time constraints by allocating a specified number of 
 *   milliseconds per move. The agent uses a configurable search depth and a move time limit 
 *   to determine the best move for the current board state by calling the IterativeDeepeningSearch
 *   method. This design allows the agent to return the best move found so far if the search 
 *   cannot be completed within the allocated time.
 * 
 * Usage:
 *   - Set the agent's color via the Color property.
 *   - Optionally, adjust the search depth and move time limit (in milliseconds) via the constructor
 *     or by setting the MoveTimeLimitInMilliSeconds property.
 *   - Call GetMove(board) to retrieve the best move from the current board state.
 */

using ChessBot.Core.Board;
using ChessBot.Core.Algorithms.Search;

namespace ChessBot.Core.Agents
{
    public class FinalAgent : IAgent
    {
        // The chess color assigned to this agent (e.g., White or Black).
        public ChessColor Color { get; set; }

        // The depth to which the search algorithm will explore the game tree.
        private readonly int _searchDepth;

        // The maximum time (in milliseconds) allocated for calculating a move.
        public int MoveTimeLimitInMilliSeconds { get; set; }

        /// <summary>
        /// Initializes a new instance of the FinalAgent class with an optional search depth.
        /// By default, the search depth is set to 5 and the move time limit is set to 5000 ms.
        /// </summary>
        /// <param name="searchDepth">The maximum depth for the iterative deepening search.</param>
        public FinalAgent(int searchDepth = 5)
        {
            _searchDepth = searchDepth;
            MoveTimeLimitInMilliSeconds = 5000; // Default move time limit set to 5 seconds.
        }

        /// <summary>
        /// Determines the best move for the current board state using iterative deepening search.
        /// The method uses the agent's color, search depth, and move time limit to compute the move.
        /// If the search does not complete within the allotted time, it returns the best move found so far.
        /// </summary>
        /// <param name="board">The current chess board state.</param>
        /// <returns>The chosen move as a Move object.</returns>
        public Move GetMove(ChessBoard board)
        {
            return IterativeDeepeningSearch.GetBestMove(board, Color, _searchDepth, MoveTimeLimitInMilliSeconds);
        }
    }
}
