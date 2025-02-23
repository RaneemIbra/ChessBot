using ChessBot.Core.Board;
using ChessBot.Core.Algorithms.Search;

namespace ChessBot.Core.Algorithms.Search
{
    public static class IterativeDeepeningSearch
    {
        public static Move GetBestMove(ChessBoard board, ChessColor rootColor, int maxDepth)
        {
            Move bestMove = null!;
            int scoreGuess = 0;
            int aspirationWindow = 50;
            
            for (int depth = 1; depth <= maxDepth; depth++)
            {
                int alpha = scoreGuess - aspirationWindow;
                int beta = scoreGuess + aspirationWindow;
                Move currentBestMove;
                int score = AlphaBetaSearch.AlphaBeta(board, depth, alpha, beta, rootColor, rootColor, 0, out currentBestMove);
                
                if (score <= alpha || score >= beta)
                {
                    score = AlphaBetaSearch.AlphaBeta(board, depth, -1000000, 1000000, rootColor, rootColor, 0, out currentBestMove);
                }
                scoreGuess = score;
                bestMove = currentBestMove;
            }
            return bestMove;
        }
    }
}
