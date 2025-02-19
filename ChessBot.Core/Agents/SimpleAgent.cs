using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBot.Core.Board;
using ChessBot.Core.Algorithms;

namespace ChessBot.Core.Agents
{
    public class SimpleAgent : IAgent
    {
        public ChessColor Color { get; set; }
        private readonly int _searchDepth;

        public SimpleAgent(int searchDepth = 5)
        {
            _searchDepth = searchDepth;
        }

        public Move GetMove(ChessBoard board)
        {
            return MinMax2.GetBestMove(board, Color, _searchDepth);
        }
    }
}
