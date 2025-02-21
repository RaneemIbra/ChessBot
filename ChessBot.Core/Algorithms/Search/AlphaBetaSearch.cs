using ChessBot.Core.Board;
using ChessBot.Core.Algorithms.Evaluation;

namespace ChessBot.Core.Algorithms.Search
{
    public static class AlphaBetaSearch
    {
        private const int INF = 1000000;
        
        public static int AlphaBeta(ChessBoard board, int depth, int alpha, int beta, ChessColor rootColor, ChessColor sideToMove, out Move bestMove)
        {
            bestMove = null!;
            ulong hash = board.ComputeZobristHash(sideToMove);
            if (TranspositionTable.TryGet(hash, depth, out TTEntry? ttEntry))
            {
                if (ttEntry.Flag == TTFlag.Exact)
                {
                    bestMove = ttEntry.BestMove!;
                    return ttEntry.Evaluation;
                }
                else if (ttEntry.Flag == TTFlag.LowerBound)
                {
                    alpha = Math.Max(alpha, ttEntry.Evaluation);
                }
                else if (ttEntry.Flag == TTFlag.UpperBound)
                {
                    beta = Math.Min(beta, ttEntry.Evaluation);
                }
                if (alpha >= beta)
                {
                    bestMove = ttEntry.BestMove!;
                    return ttEntry.Evaluation;
                }
            }

            if (depth == 0)
            {
                return Quiescence(board, alpha, beta, rootColor, sideToMove);
            }

            List<Move> moves = new List<Move>();
            IEnumerable<BoardPiece> pieces = sideToMove == ChessColor.White ? board.WhitePieces : board.BlackPieces;
            foreach (var piece in pieces)
            {
                moves.AddRange(board.GetPossibleMoves(piece));
            }
            if (moves.Count == 0)
            {
                return Evaluation.Evaluation.EvaluateBoard(board, rootColor);
            }

            moves = MoveOrdering.OrderMoves(moves, board, sideToMove);
            int originalAlpha = alpha;
            Move? bestLocalMove = null;
            
            if (sideToMove == rootColor)
            {
                int value = -INF;
                foreach (var move in moves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = AlphaBeta(child, depth - 1, alpha, beta, rootColor, Opponent(sideToMove), out _);
                    if (score > value)
                    {
                        value = score;
                        bestLocalMove = move;
                    }
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
                bestMove = bestLocalMove!;
                TTFlag flag = (value <= originalAlpha) ? TTFlag.UpperBound :
                            (value >= beta) ? TTFlag.LowerBound : TTFlag.Exact;
                TranspositionTable.Store(hash, depth, value, flag, bestMove);
                return value;
            }
            else
            {
                int value = INF;
                foreach (var move in moves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = AlphaBeta(child, depth - 1, alpha, beta, rootColor, Opponent(sideToMove), out _);
                    if (score < value)
                    {
                        value = score;
                        bestLocalMove = move;
                    }
                    beta = Math.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }
                bestMove = bestLocalMove!;
                TTFlag flag = (value <= originalAlpha) ? TTFlag.UpperBound :
                            (value >= beta) ? TTFlag.LowerBound : TTFlag.Exact;
                TranspositionTable.Store(hash, depth, value, flag, bestMove);
                return value;
            }
        }

        private static int Quiescence(ChessBoard board, int alpha, int beta, ChessColor rootColor, ChessColor sideToMove)
        {
            int standPat = Evaluation.Evaluation.EvaluateBoard(board, rootColor);
            if (sideToMove == rootColor)
            {
                if (standPat >= beta)
                    return beta;
                if (alpha < standPat)
                    alpha = standPat;
                List<Move> captureMoves = MoveOrdering.GenerateCaptureMoves(board, sideToMove);
                captureMoves = MoveOrdering.OrderMoves(captureMoves, board, sideToMove);
                foreach (var move in captureMoves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = Quiescence(child, alpha, beta, rootColor, Opponent(sideToMove));
                    alpha = Math.Max(alpha, score);
                    if (alpha >= beta)
                        return beta;
                }
                return alpha;
            }
            else
            {
                if (standPat <= alpha)
                    return alpha;
                if (beta > standPat)
                    beta = standPat;
                List<Move> captureMoves = MoveOrdering.GenerateCaptureMoves(board, sideToMove);
                captureMoves = MoveOrdering.OrderMoves(captureMoves, board, sideToMove);
                foreach (var move in captureMoves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = Quiescence(child, alpha, beta, rootColor, Opponent(sideToMove));
                    beta = Math.Min(beta, score);
                    if (alpha >= beta)
                        return alpha;
                }
                return beta;
            }
        }

        private static ChessColor Opponent(ChessColor color)
        {
            return color == ChessColor.White ? ChessColor.Black : ChessColor.White;
        }
    }
}
