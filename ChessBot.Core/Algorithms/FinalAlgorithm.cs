using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChessBot.Core.Board;
using ChessBot.Core.Utils;

namespace ChessBot.Core.Algorithms
{
    public static class FinalAlgorithm
    {
        private const int INF = 1000000;

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
                bestMove = currentBestMove;
            }
            return bestMove;
        }

        private static int AlphaBeta(ChessBoard board, int depth, int alpha, int beta, ChessColor rootColor, ChessColor sideToMove, out Move bestMove)
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
                return EvaluateBoard(board, rootColor);
            }

            moves = OrderMoves(moves, board, sideToMove);
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
            int standPat = EvaluateBoard(board, rootColor);
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
                List<Move> captureMoves = GenerateCaptureMoves(board, sideToMove);
                captureMoves = OrderMoves(captureMoves, board, sideToMove);
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

        private static List<Move> GenerateCaptureMoves(ChessBoard board, ChessColor sideToMove)
        {
            List<Move> captureMoves = new List<Move>();
            IEnumerable<BoardPiece> pieces = sideToMove == ChessColor.White ? board.WhitePieces : board.BlackPieces;
            foreach (var piece in pieces)
            {
                var moves = board.GetPossibleMoves(piece).Where(m => m.CapturedPiece != null);
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
                    score += 1000;
                int rankDiff = 0;
                if (move.MovingPiece.ChessPiece == ChessPiece.White)
                    rankDiff = (int)move.TargetRank - (int)move.MovingPiece.Rank;
                else if (move.MovingPiece.ChessPiece == ChessPiece.Black)
                    rankDiff = (int)move.MovingPiece.Rank - (int)move.TargetRank;
                score += rankDiff * 10;
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

        private static int EvaluateBoard(ChessBoard board, ChessColor rootColor)
        {
            if (EndGame.IsGameOver(board, out string? _))
            {
                ChessColor winner = EndGame.GetWinner(board);
                return (winner == rootColor) ? INF - 1 : -INF + 1;
            }
            PawnBitboards pb = GetPawnBitboards(board);
            int whitePawnCount = BitOperations.PopCount(pb.WhitePawns);
            int blackPawnCount = BitOperations.PopCount(pb.BlackPawns);
            int materialScore = (whitePawnCount - blackPawnCount) * 100;
            int whiteAdvancement = EvaluateAdvancement(pb.WhitePawns, true);
            int blackAdvancement = EvaluateAdvancement(pb.BlackPawns, false);
            ulong occupied = pb.WhitePawns | pb.BlackPawns;
            int whiteMobility = BitOperations.PopCount(BitboardHelper.WhiteSinglePush(pb.WhitePawns, ~occupied)) +
                                BitOperations.PopCount(BitboardHelper.WhiteCaptures(pb.WhitePawns, pb.BlackPawns));
            int blackMobility = BitOperations.PopCount(BitboardHelper.BlackSinglePush(pb.BlackPawns, ~occupied)) +
                                BitOperations.PopCount(BitboardHelper.BlackCaptures(pb.BlackPawns, pb.WhitePawns));
            int mobilityScore = (whiteMobility - blackMobility) * 10;
            
            int passedBonusWhite = EvaluatePassedPawnBonus(pb.WhitePawns, pb.BlackPawns, true);
            int passedBonusBlack = EvaluatePassedPawnBonus(pb.BlackPawns, pb.WhitePawns, false);
            
            int score = materialScore + whiteAdvancement - blackAdvancement + mobilityScore + (passedBonusWhite - passedBonusBlack);
            return rootColor == ChessColor.White ? score : -score;
        }

        // Passed pawns still need to be fixed and checked
        private static int EvaluatePassedPawnBonus(ulong ownPawns, ulong enemyPawns, bool isWhite)
        {
            int bonus = 0;
            ulong pawns = ownPawns;
            ulong occupied = ownPawns | enemyPawns;
            while (pawns != 0)
            {
                int idx = BitOperations.TrailingZeroCount(pawns);
                pawns &= pawns - 1;
                int rank = idx / 8;
                int file = idx % 8;
                
                ulong mask = 0UL;
                if (isWhite)
                {
                    for (int r = rank + 1; r < 8; r++)
                    {
                        for (int f = file - 1; f <= file + 1; f++)
                        {
                            if (f < 0 || f >= 8) continue;
                            int bitIndex = r * 8 + f;
                            mask |= 1UL << bitIndex;
                        }
                    }
                }
                else
                {
                    for (int r = rank - 1; r >= 0; r--)
                    {
                        for (int f = file - 1; f <= file + 1; f++)
                        {
                            if (f < 0 || f >= 8) continue;
                            int bitIndex = r * 8 + f;
                            mask |= 1UL << bitIndex;
                        }
                    }
                }
                
                if ((enemyPawns & mask) == 0)
                {
                    bool clearPath = true;
                    if (isWhite)
                    {
                        for (int r = rank + 1; r < 8; r++)
                        {
                            int bitIndex = r * 8 + file;
                            if ((occupied & (1UL << bitIndex)) != 0)
                            {
                                clearPath = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int r = rank - 1; r >= 0; r--)
                        {
                            int bitIndex = r * 8 + file;
                            if ((occupied & (1UL << bitIndex)) != 0)
                            {
                                clearPath = false;
                                break;
                            }
                        }
                    }
                    int distance = isWhite ? (7 - rank) : rank;
                    int pawnBonus = (clearPath ? 300 : 150) + ((7 - distance) * 50);
                    bonus += pawnBonus;
                }
            }
            return bonus;
        }

        private static int EvaluateAdvancement(ulong pawnBitboard, bool isWhite)
        {
            int bonus = 0;
            ulong pawns = pawnBitboard;
            while (pawns != 0)
            {
                int index = BitOperations.TrailingZeroCount(pawns);
                pawns &= pawns - 1;
                int rank = index / 8;
                bonus += isWhite ? rank * 10 : (7 - rank) * 10;
            }
            return bonus;
        }

        private static PawnBitboards GetPawnBitboards(ChessBoard board)
        {
            ulong white = 0UL;
            ulong black = 0UL;
            foreach (var piece in board.WhitePieces)
            {
                int rankIdx = (int)piece.Rank - 1;
                int fileIdx = (int)piece.File;
                int bitIndex = rankIdx * 8 + fileIdx;
                white |= 1UL << bitIndex;
            }
            foreach (var piece in board.BlackPieces)
            {
                int rankIdx = (int)piece.Rank - 1;
                int fileIdx = (int)piece.File;
                int bitIndex = rankIdx * 8 + fileIdx;
                black |= 1UL << bitIndex;
            }
            return new PawnBitboards(white, black);
        }
    }
}
