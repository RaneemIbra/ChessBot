/*
 * IterativeDeepeningSearch.cs
 * 
 * Summary:
 *   This file implements the IterativeDeepeningSearch class, which uses an iterative deepening
 *   strategy to determine the best move within a given time constraint. The search starts at depth 1 
 *   and incrementally increases the search depth until either the maximum depth is reached or the 
 *   allocated move time (in milliseconds) expires. An aspiration window is used to narrow the alpha–beta 
 *   search bounds for efficiency. If the evaluation falls outside the aspiration window, the search 
 *   is re-run with full bounds. The best move found so far is returned when time runs out or the search 
 *   completes.
 * 
 * Usage:
 *   - Call GetBestMove by providing the current board state, the root player's color, the maximum search 
 *     depth, and the move time limit in milliseconds.
 *   - The method returns the best move computed within the allotted time.
 */

using ChessBot.Core.Board;
using ChessBot.Core.Algorithms.Search;
using System.Diagnostics;

namespace ChessBot.Core.Algorithms.Search
{
    public static class IterativeDeepeningSearch
    {
        /// <summary>
        /// Performs an iterative deepening search to find the best move within a given time limit.
        /// 
        /// The method starts at a depth of 1 and increases the search depth until it reaches the maximum 
        /// depth or the time limit is exceeded. An aspiration window is used to set initial alpha and beta 
        /// values. If the search returns a score outside of this window, a re-search is conducted with full 
        /// window bounds. The best move found so far is returned.
        /// 
        /// Parameters:
        ///   board                   - The current chess board state.
        ///   rootColor               - The color of the player for whom the move is being calculated.
        ///   maxDepth                - The maximum search depth to attempt.
        ///   moveTimeLimitMilliseconds - The time limit (in milliseconds) allocated for making the move.
        /// 
        /// Returns:
        ///   The best move found as a Move object.
        /// </summary>
        public static Move GetBestMove(ChessBoard board, ChessColor rootColor, int maxDepth, int moveTimeLimitMilliseconds)
        {
            // Best move found so far.
            Move bestMove = null!;
            // A guess for the evaluation score to set the aspiration window.
            int scoreGuess = 0;
            // The aspiration window size narrows the search window around the score guess.
            int aspirationWindow = 50;
            // Start a stopwatch to enforce the move time limit.
            Stopwatch sw = Stopwatch.StartNew();

            // Iterate over increasing depths from 1 to maxDepth.
            for (int depth = 1; depth <= maxDepth; depth++)
            {
                // If the elapsed time exceeds the allocated time for the move, exit the loop.
                if (sw.ElapsedMilliseconds >= moveTimeLimitMilliseconds)
                    break;

                // Set initial alpha and beta using the aspiration window around the score guess.
                int alpha = scoreGuess - aspirationWindow;
                int beta = scoreGuess + aspirationWindow;
                Move currentBestMove;
                // Perform the alpha-beta search at the current depth.
                int score = AlphaBetaSearch.AlphaBeta(board, depth, alpha, beta, rootColor, rootColor, 0, out currentBestMove);

                // Check again if time is up after completing the search at the current depth.
                if (sw.ElapsedMilliseconds >= moveTimeLimitMilliseconds)
                    break;

                // If the score is outside the aspiration window, redo the search with full bounds.
                if (score <= alpha || score >= beta)
                {
                    score = AlphaBetaSearch.AlphaBeta(board, depth, -1000000, 1000000, rootColor, rootColor, 0, out currentBestMove);
                }
                // Update our score guess and best move.
                scoreGuess = score;
                bestMove = currentBestMove;
            }
            // Return the best move found within the time constraint.
            return bestMove;
        }
    }
}
