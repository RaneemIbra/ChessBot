using System;
using System.Collections.Generic;
using System.Linq;
using ChessBot.Core.Board;

namespace ChessBot.Core.Algorithms
{
    public static class MinMax2
    {
        private const int INF = 100000;

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
                int score = AlphaBeta(board, depth, alpha, beta, rootColor, rootColor, out currentBestMove);

                if (score <= alpha || score >= beta)
                {
                    score = AlphaBeta(board, depth, -INF, INF, rootColor, rootColor, out currentBestMove);
                }

                scoreGuess = score;
                if (currentBestMove != null)
                    bestMove = currentBestMove;
            }
            return bestMove;
        }

        private static int AlphaBeta(ChessBoard board, int depth, int alpha, int beta, ChessColor rootColor, ChessColor sideToMove, out Move bestMove)
        {
            bestMove = null!;

            if (EndGame.IsGameOver(board, out string? _))
            {
                ChessColor winner = EndGame.GetWinner(board);
                return (winner == rootColor) ? 10000 : -10000;
            }

            if (depth == 0)
            {
                return Quiescence(board, alpha, beta, rootColor, sideToMove);
            }

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

            List<Move> moves = new List<Move>();
            IEnumerable<BoardPiece> pieces = sideToMove == ChessColor.White ? board.WhitePieces : board.BlackPieces;
            foreach (var piece in pieces)
            {
                moves.AddRange(board.GetPossibleMoves(piece));
            }
            if (moves.Count == 0)
            {
                return (sideToMove == rootColor) ? -10000 : 10000;
            }

            moves = OrderMoves(moves, board, sideToMove);

            int originalAlpha = alpha;
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
                        bestMove = move;
                    }
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
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
                        bestMove = move;
                    }
                    beta = Math.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }
                TTFlag flag = (value <= originalAlpha) ? TTFlag.UpperBound :
                            (value >= beta) ? TTFlag.LowerBound : TTFlag.Exact;
                TranspositionTable.Store(hash, depth, value, flag, bestMove);
                return value;
            }
        }

        private static int Quiescence(ChessBoard board, int alpha, int beta, ChessColor rootColor, ChessColor sideToMove)
        {
            int standPat = Evaluate(board, rootColor);
            if (sideToMove == rootColor)
            {
                if (standPat >= beta)
                    return beta;
                if (alpha < standPat)
                    alpha = standPat;
                List<Move> captureMoves = GenerateCaptureMoves(board, sideToMove);
                captureMoves = OrderMoves(captureMoves, board, sideToMove);
                foreach (var move in captureMoves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = Quiescence(child, alpha, beta, rootColor, Opponent(sideToMove));
                    if (score > alpha)
                        alpha = score;
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
                List<Move> captureMoves = GenerateCaptureMoves(board, sideToMove);
                captureMoves = OrderMoves(captureMoves, board, sideToMove);
                foreach (var move in captureMoves)
                {
                    ChessBoard child = board.Clone();
                    child.ExecuteMove(move);
                    int score = Quiescence(child, alpha, beta, rootColor, Opponent(sideToMove));
                    if (score < beta)
                        beta = score;
                    if (alpha >= beta)
                        return alpha;
                }
                return beta;
            }
        }

        private static List<Move> GenerateCaptureMoves(ChessBoard board, ChessColor sideToMove)
        {
            List<Move> captureMoves = new List<Move>();
            IEnumerable<BoardPiece> pieces = sideToMove == ChessColor.White ? board.WhitePieces : board.BlackPieces;
            foreach (var piece in pieces)
            {
                var moves = PossibleMoves.CapturingMove(board, piece);
                captureMoves.AddRange(moves);
            }
            return captureMoves;
        }

        private static List<Move> OrderMoves(List<Move> moves, ChessBoard board, ChessColor sideToMove)
        {
            var ordered = moves.Select(move =>
            {
                int score = 0;
                if (move.CapturedPiece != null)
                {
                    score += 100;
                }
                int currentRank = (int)move.MovingPiece.Rank;
                int targetRank = (int)move.TargetRank;
                if (move.MovingPiece.ChessPiece == ChessPiece.White)
                {
                    score += targetRank - currentRank;
                }
                else if (move.MovingPiece.ChessPiece == ChessPiece.Black)
                {
                    score += currentRank - targetRank;
                }
                return new { move, score };
            })
            .OrderByDescending(x => x.score)
            .Select(x => x.move)
            .ToList();
            return ordered;
        }

        private static ChessColor Opponent(ChessColor color)
        {
            return color == ChessColor.White ? ChessColor.Black : ChessColor.White;
        }

        private static int Evaluate(ChessBoard board, ChessColor rootColor)
        {
            if (EndGame.IsGameOver(board, out string? _))
            {
                ChessColor winner = EndGame.GetWinner(board);
                return (winner == rootColor) ? 10000 : -10000;
            }
            int score = 0;
            foreach (var piece in board.WhitePieces)
            {
                score += (int)piece.Rank;
            }
            foreach (var piece in board.BlackPieces)
            {
                score -= (9 - (int)piece.Rank);
            }
            int whiteMoves = board.WhitePieces.SelectMany(p => board.GetPossibleMoves(p)).Count();
            int blackMoves = board.BlackPieces.SelectMany(p => board.GetPossibleMoves(p)).Count();
            score += (whiteMoves - blackMoves);

            if (rootColor == ChessColor.Black)
                score = -score;
            return score;
        }
    }
}
