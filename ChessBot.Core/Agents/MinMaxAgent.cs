using ChessBot.Core.Board;
using ChessBot.Core.Algorithms;

namespace ChessBot.Core.Agents
{
    public class MinimaxAgent : IAgent
    {
        public ChessColor Color { get; set; }
        private readonly int _searchDepth;

        public MinimaxAgent(int searchDepth = 5)
        {
            _searchDepth = searchDepth;
        }

        public Move GetMove(ChessBoard board)
        {
            Console.WriteLine("we are making a move1");
            Console.WriteLine(_searchDepth);
            Console.WriteLine(Color);
            return Minimax.GetBestMove(board, Color, _searchDepth);
        }
    }
}
