/*
 * AlphaBetaSearch.cs
 * 
 * Summary:
 *   This file implements the alpha–beta search algorithm used to evaluate the game tree
 *   for our pawn-only chess game. The algorithm recursively searches for the best move by
 *   exploring possible moves up to a specified depth while pruning branches that cannot
 *   improve the outcome (alpha–beta pruning). It uses a transposition table (via Zobrist hashing)
 *   to cache previously computed evaluations, reducing redundant computations. When the search
 *   depth reaches zero, a quiescence search is performed to extend the evaluation in "noisy"
 *   positions (positions with potential capture moves) to avoid the horizon effect.
 * 
 *   The main method, AlphaBeta, returns an evaluation score and outputs the best move found
 *   at that node. The algorithm differentiates between maximizing (root player) and minimizing
 *   nodes (the opponent) and employs move ordering heuristics to enhance pruning efficiency.
 *   The helper method Opponent simply returns the opposite color.
 */

using ChessBot.Core.Board;
using ChessBot.Core.Algorithms.Evaluation;

namespace ChessBot.Core.Algorithms.Search
{
    public static class AlphaBetaSearch
    {
        // Define a constant for "infinity" used in terminal state evaluations.
        private const int INF = 1000000;

        /// <summary>
        /// Recursively performs an alpha–beta search on the game tree.
        /// 
        /// The method evaluates the board state up to a given depth using alpha–beta pruning.
        /// It checks for terminal positions and, at depth 0, performs a quiescence search to handle
        /// unstable tactical positions. Transposition table lookups are used to retrieve cached evaluations.
        /// 
        /// Parameters:
        ///   board      - The current chess board state.
        ///   depth      - Remaining depth to search (in plies).
        ///   alpha      - Lower bound value for the maximizer.
        ///   beta       - Upper bound value for the minimizer.
        ///   rootColor  - The color for which the evaluation is performed (maximizing side).
        ///   sideToMove - The color of the current player whose move is being considered.
        ///   ply        - The current depth level (number of moves from the root).
        ///   bestMove   - Output parameter that will contain the best move found.
        /// 
        /// Returns:
        ///   An integer evaluation score for the board state.
        /// </summary>
        public static int AlphaBeta(ChessBoard board, int depth, int alpha, int beta,
            ChessColor rootColor, ChessColor sideToMove, int ply, out Move bestMove)
        {
            // Initialize bestMove (will be updated during search).
            bestMove = null!;

            // Compute a unique hash for the board state using Zobrist hashing.
            ulong hash = board.ComputeZobristHash(sideToMove);

            // Look up the current position in the transposition table.
            if (TranspositionTable.TryGet(hash, depth, out TTEntry? ttEntry))
            {
                // If an exact evaluation is stored, return it directly.
                if (ttEntry.Flag == TTFlag.Exact)
                {
                    bestMove = ttEntry.BestMove!;
                    return ttEntry.Evaluation;
                }
                // Otherwise, adjust alpha or beta based on stored lower or upper bounds.
                else if (ttEntry.Flag == TTFlag.LowerBound)
                {
                    alpha = Math.Max(alpha, ttEntry.Evaluation);
                }
                else if (ttEntry.Flag == TTFlag.UpperBound)
                {
                    beta = Math.Min(beta, ttEntry.Evaluation);
                }
                // If the bounds have converged, return the stored evaluation.
                if (alpha >= beta)
                {
                    bestMove = ttEntry.BestMove!;
                    return ttEntry.Evaluation;
                }
            }

            // Terminal position: if the game is over, return a very high or low score.
            if (EndGame.IsGameOver(board, out _))
            {
                ChessColor winner = EndGame.GetWinner(board);
                if (winner == rootColor)
                    return INF - ply;
                else
                    return -INF + ply;
            }

            // If depth limit reached, perform a quiescence search to refine evaluation.
            if (depth == 0)
            {
                return Quiescence(board, alpha, beta, rootColor, sideToMove, ply);
            }

            // Generate all possible moves for the current player.
            List<Move> moves = new List<Move>();
            IEnumerable<BoardPiece> pieces = sideToMove == ChessColor.White ? board.WhitePieces : board.BlackPieces;
            foreach (var piece in pieces)
            {
                moves.AddRange(board.GetPossibleMoves(piece));
            }
            // If no moves are available, evaluate the board position.
            if (moves.Count == 0)
            {
                return Evaluation.Evaluation.EvaluateBoard(board, rootColor);
            }

            // Order the moves using heuristics to improve pruning efficiency.
            var orderedMoves = MoveOrdering.OrderMoves(moves, board, sideToMove);
            // Save the original alpha value for later use in determining the transposition table flag.
            int originalAlpha = alpha;
            Move? bestLocalMove = null;

            // If the current side is the maximizer (rootColor).
            if (sideToMove == rootColor)
            {
                int value = -INF;
                // Iterate over each ordered move.
                foreach (var move in orderedMoves)
                {
                    // Clone the board and apply the move.
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    // Recursively evaluate the move for the opponent.
                    int score = AlphaBeta(child, depth - 1, alpha, beta, rootColor, Opponent(sideToMove), ply + 1, out _);
                    // Update the best score and move if this move is better.
                    if (score > value)
                    {
                        value = score;
                        bestLocalMove = move;
                    }
                    // Update alpha with the maximum value found.
                    alpha = Math.Max(alpha, value);
                    // Prune the search if the current branch cannot improve the outcome.
                    if (alpha >= beta)
                        break;
                }
                bestMove = bestLocalMove!;
                // Determine the flag to store in the transposition table.
                TTFlag flag = (value <= originalAlpha) ? TTFlag.UpperBound :
                            (value >= beta) ? TTFlag.LowerBound : TTFlag.Exact;
                // Cache the evaluation and best move.
                TranspositionTable.Store(hash, depth, value, flag, bestMove);
                return value;
            }
            // Else, if the current side is the minimizer (opponent).
            else
            {
                int value = INF;
                foreach (var move in orderedMoves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = AlphaBeta(child, depth - 1, alpha, beta, rootColor, Opponent(sideToMove), ply + 1, out _);
                    if (score < value)
                    {
                        value = score;
                        bestLocalMove = move;
                    }
                    // Update beta with the minimum value found.
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

        /// <summary>
        /// Performs a quiescence search at leaf nodes to extend the evaluation
        /// of "noisy" positions. Instead of stopping at depth zero, this method explores
        /// capture moves to avoid the horizon effect, yielding a more stable evaluation.
        /// </summary>
        /// <param name="board">The current chess board state.</param>
        /// <param name="alpha">Current lower bound.</param>
        /// <param name="beta">Current upper bound.</param>
        /// <param name="rootColor">The color for which evaluation is performed.</param>
        /// <param name="sideToMove">The color of the current player.</param>
        /// <param name="ply">Current depth level.</param>
        /// <returns>An evaluation score for the quiet position.</returns>
        private static int Quiescence(ChessBoard board, int alpha, int beta,
            ChessColor rootColor, ChessColor sideToMove, int ply)
        {
            // Stand-pat evaluation: assess the board without making any additional moves.
            int standPat = Evaluation.Evaluation.EvaluateBoard(board, rootColor);
            if (sideToMove == rootColor)
            {
                // If the static evaluation is already too high, prune by returning beta.
                if (standPat >= beta)
                    return beta;
                // Otherwise, update alpha if the stand-pat value is higher.
                if (alpha < standPat)
                    alpha = standPat;
                // Generate and order only capture moves (the "noisy" moves).
                IEnumerable<Move> captureMoves = MoveOrdering.GenerateCaptureMoves(board, sideToMove);
                captureMoves = MoveOrdering.OrderMoves(captureMoves, board, sideToMove);
                foreach (var move in captureMoves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = Quiescence(child, alpha, beta, rootColor, Opponent(sideToMove), ply + 1);
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
                IEnumerable<Move> captureMoves = MoveOrdering.GenerateCaptureMoves(board, sideToMove);
                captureMoves = MoveOrdering.OrderMoves(captureMoves, board, sideToMove);
                foreach (var move in captureMoves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = Quiescence(child, alpha, beta, rootColor, Opponent(sideToMove), ply + 1);
                    beta = Math.Min(beta, score);
                    if (alpha >= beta)
                        return alpha;
                }
                return beta;
            }
        }

        /// <summary>
        /// Returns the opponent's color.
        /// </summary>
        /// <param name="color">The current player's color.</param>
        /// <returns>If input is White, returns Black; otherwise, returns White.</returns>
        private static ChessColor Opponent(ChessColor color)
        {
            return color == ChessColor.White ? ChessColor.Black : ChessColor.White;
        }
    }
}
