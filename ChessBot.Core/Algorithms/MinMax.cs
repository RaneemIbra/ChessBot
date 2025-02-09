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
            ulong hash = board.ComputeZobristHash();
            if (TranspositionTable.TryGet(hash, depth, out var ttEntry))
            {
                if (ttEntry.Flag == TTFlag.Exact)
                    return (null, ttEntry.Evaluation);
                if (ttEntry.Flag == TTFlag.LowerBound)
                    alpha = Math.Max(alpha, ttEntry.Evaluation);
                else if (ttEntry.Flag == TTFlag.UpperBound)
                    beta = Math.Min(beta, ttEntry.Evaluation);

                if (alpha >= beta)
                    return (null, ttEntry.Evaluation);
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

            TranspositionTable.Store(hash, depth, bestValue, flag);
            return (bestMove, bestValue);
        }


        private static int EvaluateBoard(ChessBoard board, ChessColor color)
        {
            int whiteScore = 0;
            int blackScore = 0;

            foreach (var w in board.WhitePieces)
            {
                whiteScore += 10;                    
                whiteScore += (int)w.Rank*2;
                if (IsPassedPawn(w, board, ChessColor.White)) whiteScore += 15;
                if (IsPawnBlocked(w, board, ChessColor.White)) whiteScore -= 5;
            }
            foreach (var b in board.BlackPieces)
            {
                blackScore += 10;
                blackScore += (8 - (int)b.Rank)*2;
                if (IsPassedPawn(b, board, ChessColor.Black)) blackScore += 15;
                if (IsPawnBlocked(b, board, ChessColor.Black)) blackScore -= 5;
            }

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
