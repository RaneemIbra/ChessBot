using ChessBot.Core.Board;
using ChessBot.Core.Algorithms.Search;

namespace ChessBot.Core.Agents
{
    public class FinalAgent : IAgent
    {
        public ChessColor Color { get; set; }
        private readonly int _searchDepth;
        public int MoveTimeLimitInMilliSeconds { get; set; }

        public FinalAgent(int searchDepth = 5)
        {
            _searchDepth = searchDepth;
            MoveTimeLimitInMilliSeconds = 5000;
        }

        public Move GetMove(ChessBoard board)
        {
            return IterativeDeepeningSearch.GetBestMove(board, Color, _searchDepth, MoveTimeLimitInMilliSeconds);
        }
    }
}
