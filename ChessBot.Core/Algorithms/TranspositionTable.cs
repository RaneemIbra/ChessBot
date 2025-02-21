using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChessBot.Core.Board;

namespace ChessBot.Core.Algorithms
{
    public enum TTFlag
    {
        Exact,
        LowerBound,
        UpperBound
    }

    public class TTEntry
    {
        public int Depth { get; set; }
        public int Evaluation { get; set; }
        public TTFlag Flag { get; set; }
        public Move? BestMove { get; set; }
    }

    public static class TranspositionTable
    {
        private static Dictionary<ulong, TTEntry> _table = new Dictionary<ulong, TTEntry>();

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
