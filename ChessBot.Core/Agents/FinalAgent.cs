using ChessBot.Core.Board;
using ChessBot.Core.Algorithms;

namespace ChessBot.Core.Agents
{
    public class FinalAgent : IAgent
    {
        public ChessColor Color { get; set; }
        private readonly int _searchDepth;

        public FinalAgent(int searchDepth = 5)
        {
            _searchDepth = searchDepth;
        }

        public Move GetMove(ChessBoard board)
        {
            return FinalAlgorithm.GetBestMove(board, Color, _searchDepth);
        }
    }
}
