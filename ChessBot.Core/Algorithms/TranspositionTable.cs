/*
 * TranspositionTable.cs
 * 
 * Summary:
 *   This file implements a simple transposition table for caching evaluations of board positions 
 *   during the search. A transposition table is used to avoid recalculating the evaluation for positions 
 *   that have already been encountered. Each entry in the table (TTEntry) stores the search depth, the 
 *   evaluation score, a flag indicating the type of bound (Exact, LowerBound, or UpperBound), and the best 
 *   move found at that node. The table is keyed by a unique hash (generated using Zobrist hashing) representing 
 *   the board state.
 * 
 *   The TTFlag enumeration defines the type of evaluation stored:
 *     - Exact: The stored evaluation is exactly correct.
 *     - LowerBound: The stored evaluation is a lower bound (the true value is at least this high).
 *     - UpperBound: The stored evaluation is an upper bound (the true value is no more than this).
 * 
 *   The TranspositionTable class provides methods to retrieve (TryGet) and store (Store) entries in a dictionary.
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChessBot.Core.Board;

namespace ChessBot.Core.Algorithms
{
    /// <summary>
    /// Specifies the type of bound stored in a transposition table entry.
    /// </summary>
    public enum TTFlag
    {
        Exact,
        LowerBound,
        UpperBound
    }

    /// <summary>
    /// Represents an entry in the transposition table.
    /// Contains the search depth, evaluation score, a bound flag, and the best move found.
    /// </summary>
    public class TTEntry
    {
        /// <summary>
        /// The depth at which this position was evaluated.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// The evaluation score of the board position.
        /// </summary>
        public int Evaluation { get; set; }

        /// <summary>
        /// The flag indicating whether the evaluation is exact, a lower bound, or an upper bound.
        /// </summary>
        public TTFlag Flag { get; set; }

        /// <summary>
        /// The best move found from this board position.
        /// </summary>
        public Move? BestMove { get; set; }
    }

    /// <summary>
    /// A static class that implements the transposition table using a Dictionary.
    /// The table stores TTEntry objects indexed by a unique board hash.
    /// </summary>
    public static class TranspositionTable
    {
        // The internal dictionary that maps board hashes to transposition table entries.
        private static Dictionary<ulong, TTEntry> _table = new Dictionary<ulong, TTEntry>();

        /// <summary>
        /// Attempts to retrieve a transposition table entry for a given board hash and search depth.
        /// 
        /// The method checks if an entry exists with a depth at least as high as requested.
        /// 
        /// Parameters:
        ///   hash  - The unique hash representing the board state.
        ///   depth - The minimum search depth for which the cached entry is valid.
        ///   entry - Output parameter that will contain the found TTEntry if successful.
        /// 
        /// Returns:
        ///   True if a valid entry was found; otherwise, false.
        /// </summary>
        public static bool TryGet(ulong hash, int depth, [NotNullWhen(true)] out TTEntry? entry)
        {
            if (_table.TryGetValue(hash, out entry))
            {
                if (entry != null && entry.Depth >= depth)
                {
                    return true;
                }
            }
            entry = null;
            return false;
        }

        /// <summary>
        /// Stores or updates a transposition table entry for the given board hash.
        /// 
        /// Parameters:
        ///   hash       - The unique hash representing the board state.
        ///   depth      - The search depth at which this entry is valid.
        ///   evaluation - The evaluation score for the board position.
        ///   flag       - The TTFlag indicating the type of bound (Exact, LowerBound, UpperBound).
        ///   bestMove   - The best move found from this board position.
        /// </summary>
        public static void Store(ulong hash, int depth, int evaluation, TTFlag flag, Move? bestMove)
        {
            TTEntry entry = new TTEntry
            {
                Depth = depth,
                Evaluation = evaluation,
                Flag = flag,
                BestMove = bestMove
            };
            _table[hash] = entry;
        }
    }
}
