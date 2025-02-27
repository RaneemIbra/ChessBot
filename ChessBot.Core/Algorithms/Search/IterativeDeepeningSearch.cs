using ChessBot.Core.Board;
using ChessBot.Core.Algorithms.Search;
using System.Diagnostics;

namespace ChessBot.Core.Algorithms.Search
{
    public static class IterativeDeepeningSearch
    {
        public static Move GetBestMove(ChessBoard board, ChessColor rootColor, int maxDepth, int moveTimeLimitMilliseconds)
        {
            Move bestMove = null!;
            int scoreGuess = 0;
            int aspirationWindow = 50;
            Stopwatch sw = Stopwatch.StartNew();

            for (int depth = 1; depth <= maxDepth; depth++)
            {
                if (sw.ElapsedMilliseconds >= moveTimeLimitMilliseconds)
                    break;

                int alpha = scoreGuess - aspirationWindow;
                int beta = scoreGuess + aspirationWindow;
                Move currentBestMove;
                int score = AlphaBetaSearch.AlphaBeta(board, depth, alpha, beta, rootColor, rootColor, 0, out currentBestMove);

                if (sw.ElapsedMilliseconds >= moveTimeLimitMilliseconds)
                    break;

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
