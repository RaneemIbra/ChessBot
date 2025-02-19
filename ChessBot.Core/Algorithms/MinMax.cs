using ChessBot.Core.Board;

namespace ChessBot.Core.Algorithms
{
    public static class Minimax
    {
        public static Move GetBestMove(ChessBoard board, ChessColor color, int depth)
        {
            var (bestMove, _) = MinimaxWithAlphaBeta(
                board,
                colorToOptimize: color,
                currentColor: color,
                depth,
                alpha: int.MinValue,
                beta: int.MaxValue
            );

            int whiteEval = EvaluateBoard(board, ChessColor.White);
            int blackEval = EvaluateBoard(board, ChessColor.Black);
            Console.WriteLine($"After move: {bestMove}, White Evaluation: {whiteEval}, Black Evaluation: {blackEval}");
            return bestMove ?? throw new InvalidOperationException("No valid moves found");
        }

        private static (Move?, int) MinimaxWithAlphaBeta(
            ChessBoard board,
            ChessColor colorToOptimize,
            ChessColor currentColor,
            int depth,
            int alpha,
            int beta)
        {
            ulong hash = board.ComputeZobristHash(colorToOptimize);
            if (TranspositionTable.TryGet(hash, depth, out var ttEntry))
            {
                if (ttEntry.Flag == TTFlag.Exact)
                    return (ttEntry.BestMove, ttEntry.Evaluation);
                if (ttEntry.Flag == TTFlag.LowerBound)
                    alpha = Math.Max(alpha, ttEntry.Evaluation);
                else if (ttEntry.Flag == TTFlag.UpperBound)
                    beta = Math.Min(beta, ttEntry.Evaluation);

                if (alpha >= beta)
                    return (ttEntry.BestMove, ttEntry.Evaluation);
            }

            if (depth == 0 || EndGame.IsGameOver(board))
            {
                int eval = EvaluateBoard(board, colorToOptimize);
                return (null, eval);
            }

            var possibleMoves = currentColor == ChessColor.White
                ? board.WhitePieces.SelectMany(piece => board.GetPossibleMoves(piece))
                : board.BlackPieces.SelectMany(piece => board.GetPossibleMoves(piece));

            if (!possibleMoves.Any())
            {
                int eval = EvaluateBoard(board, colorToOptimize);
                return (null, eval);
            }

            bool isMaximizing = (currentColor == colorToOptimize);
            Move? bestMove = null;
            int bestValue = isMaximizing ? int.MinValue : int.MaxValue;

            int originalAlpha = alpha;

            foreach (var move in possibleMoves)
            {
                var boardClone = board.Clone();
                boardClone.ExecuteMove(move);

                var nextColor = (currentColor == ChessColor.White) ? ChessColor.Black : ChessColor.White;
                var (_, value) = MinimaxWithAlphaBeta(
                    boardClone,
                    colorToOptimize,
                    nextColor,
                    depth - 1,
                    alpha,
                    beta
                );

                if (isMaximizing)
                {
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = move;
                    }
                    alpha = Math.Max(alpha, bestValue);
                }
                else
                {
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestMove = move;
                    }
                    beta = Math.Min(beta, bestValue);
                }

                if (beta <= alpha)
                    break;
            }

            TTFlag flag;
            if (bestValue <= originalAlpha)
                flag = TTFlag.UpperBound;
            else if (bestValue >= beta)
                flag = TTFlag.LowerBound;
            else
                flag = TTFlag.Exact;

            TranspositionTable.Store(hash, depth, bestValue, flag, bestMove);
            return (bestMove, bestValue);
        }


        private static int EvaluateBoard(ChessBoard board, ChessColor color)
        {
            // If the game is over, return a huge score.
            // (Assuming you have a way to determine who won via EndGame.GetWinner)
            if (EndGame.IsGameOver(board))
            {
                var winner = EndGame.GetWinner(board);
                if (winner == ChessColor.none)
                    return 0; // Draw

                // We subtract (or add) the current ply depth so that a win in fewer moves is better.
                // (Make sure your depth is passed in from your search if you want to do this.)
                if (winner == color)
                    return int.MaxValue - 1; // winning board
                else
                    return int.MinValue + 1; // losing board
            }

            int whiteScore = 0;
            int blackScore = 0;

            // Evaluate White pawns.
            foreach (var w in board.WhitePieces)
            {
                // Base value for having a pawn.
                whiteScore += 10;
                // Reward advancing the pawn (the higher the rank, the better).
                whiteScore += (int)w.Rank * 2;

                // Already existing bonus for passed pawns.
                if (IsPassedPawn(w, board, ChessColor.White))
                    whiteScore += 15;
                // Penalize blocked pawns.
                if (IsPawnBlocked(w, board, ChessColor.White))
                    whiteScore -= 5;

                // **** NEW: Winning potential bonus ****
                // Calculate moves remaining until the pawn wins (i.e. reaches rank 8).
                int movesToPromotion = 8 - (int)w.Rank;

                // If the pawn is unblocked along its file (or already passed), assume it has a clear path.
                // You might want to refine this check.
                bool clearPath = true;
                // Check every square between current rank+1 and rank 8.
                for (int r = (int)w.Rank + 1; r <= 8; r++)
                {
                    if (board.GetPieceAt((ChessRank)r, w.File) != null)
                    {
                        clearPath = false;
                        break;
                    }
                }

                // If the pawn is passed and has a clear path,
                // give a bonus that increases as it gets closer to promotion.
                if (clearPath && movesToPromotion <= 3)
                {
                    // For example, if movesToPromotion == 1, bonus = 90; if 2, bonus = 60; if 3, bonus = 30.
                    whiteScore += (4 - movesToPromotion) * 30;
                }
            }

            // Evaluate Black pawns.
            foreach (var b in board.BlackPieces)
            {
                blackScore += 10;
                // For black, the further the pawn is from rank 1 the better it is.
                blackScore += (8 - (int)b.Rank) * 2;

                if (IsPassedPawn(b, board, ChessColor.Black))
                    blackScore += 15;
                if (IsPawnBlocked(b, board, ChessColor.Black))
                    blackScore -= 5;

                // **** NEW: Winning potential bonus for Black ****
                int movesToPromotion = (int)b.Rank - 1;
                bool clearPath = true;
                for (int r = (int)b.Rank - 1; r >= 1; r--)
                {
                    if (board.GetPieceAt((ChessRank)r, b.File) != null)
                    {
                        clearPath = false;
                        break;
                    }
                }
                if (clearPath && movesToPromotion <= 3)
                {
                    blackScore += (4 - movesToPromotion) * 30;
                }
            }

            // Return the evaluation from the perspective of the given color.
            return (color == ChessColor.White)
                ? whiteScore - blackScore
                : blackScore - whiteScore;
        }

        private static bool IsPassedPawn(BoardPiece pawn, ChessBoard board, ChessColor color)
        {
            int direction = color == ChessColor.White ? 1 : -1;
            ChessFile pawnFile = pawn.File;

            foreach (var fileOffset in new[] { -1, 0, 1 })
            {
                ChessFile checkFile = (ChessFile)((int)pawnFile + fileOffset);
                if (checkFile < ChessFile.A || checkFile > ChessFile.H) continue;

                for (int rankOffset = 1; rankOffset <= 7; rankOffset++)
                {
                    ChessRank checkRank = (ChessRank)((int)pawn.Rank + rankOffset * direction);
                    if (checkRank < ChessRank.One || checkRank > ChessRank.Eight) break;

                    var piece = board.GetPieceAt(checkRank, checkFile);

                    if (piece != null && 
                        ((color == ChessColor.White && board.BlackPieces.Contains(piece)) || 
                        (color == ChessColor.Black && board.WhitePieces.Contains(piece))))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsPawnBlocked(BoardPiece pawn, ChessBoard board, ChessColor color)
        {
            int direction = color == ChessColor.White ? 1 : -1;
            ChessRank nextRank = (ChessRank)((int)pawn.Rank + direction);
            return board.GetPieceAt(nextRank, pawn.File) != null;
        }
    }
}
