using ChessBot.Core.Board;
using ChessBot.Core.Utils;
using System.Numerics;

namespace ChessBot.Core.Algorithms.Evaluation
{
    public static class Evaluation
    {
        private const int INF = 1000000;

        public static int EvaluateBoard(ChessBoard board, ChessColor rootColor)
        {
            if (EndGame.IsGameOver(board, out string? _))
            {
                ChessColor winner = EndGame.GetWinner(board);
                return (winner == rootColor) ? INF - 1 : -INF + 1;
            }
            
            var pb = GetPawnBitboards(board);
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

            int centerControlScore = EvaluateCenterControl(board);
            int pawnProtectionScore = EvaluatePawnProtection(board);
            score += centerControlScore + pawnProtectionScore;

            return rootColor == ChessColor.White ? score : -score;
        }

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

        private static bool IsCenter(ChessRank rank, ChessFile file){
            return (rank == ChessRank.Four || rank == ChessRank.Five) &&
                    (file == ChessFile.D || file == ChessFile.E);
        }

        private static int EvaluateCenterControl(ChessBoard board){
            int whiteCenter = 0;
            int blackCenter = 0;

            foreach(var pawn in board.WhitePieces){
                if(IsCenter(pawn.Rank, pawn.File)){
                    whiteCenter += 25;
                }
                if(pawn.Rank != ChessRank.Eight){
                    if(pawn.File != ChessFile.A){
                        ChessRank targetRank = (ChessRank)(((int)pawn.Rank)+1);
                        ChessFile targetFile = (ChessFile)(((int)pawn.File)-1);
                        if(IsCenter(targetRank, targetFile)){
                            whiteCenter += 10;
                        }
                    }
                    if(pawn.File != ChessFile.H){
                        ChessRank targetRank = (ChessRank)(((int)pawn.Rank)+1);
                        ChessFile targetFile = (ChessFile)(((int)pawn.File)+1);
                        if(IsCenter(targetRank, targetFile)){
                            whiteCenter += 10;
                        }
                    }
                }
            }

            foreach(var pawn in board.BlackPieces){
                if(IsCenter(pawn.Rank, pawn.File)){
                    whiteCenter += 25;
                }
                if(pawn.Rank != ChessRank.One){
                    if(pawn.File != ChessFile.A){
                        ChessRank targetRank = (ChessRank)(((int)pawn.Rank)-1);
                        ChessFile targetFile = (ChessFile)(((int)pawn.File)-1);
                        if(IsCenter(targetRank, targetFile)){
                            whiteCenter += 10;
                        }
                    }
                    if(pawn.File != ChessFile.H){
                        ChessRank targetRank = (ChessRank)(((int)pawn.Rank)-1);
                        ChessFile targetFile = (ChessFile)(((int)pawn.File)+1);
                        if(IsCenter(targetRank, targetFile)){
                            whiteCenter += 10;
                        }
                    }
                }
            }

            return whiteCenter - blackCenter;
        }

        private static int EvaluatePawnProtection(ChessBoard board){
            int whiteProtection = 0;
            int blackProtection = 0;

            foreach(var pawn in board.WhitePieces){
                bool isProtected = false;
                if(pawn.Rank != ChessRank.One){
                    if(pawn.File != ChessFile.A){
                        ChessRank defenderRank = (ChessRank)(((int)pawn.Rank) -1);
                        ChessFile defenderFile = (ChessFile)(((int)pawn.File) -1);
                        var defender = board.GetPieceAt(defenderRank, defenderFile);
                        if(defender != null && defender.ChessPiece == ChessPiece.White){
                            isProtected = true;
                        }
                    }
                    if(!isProtected && pawn.File != ChessFile.H){
                        ChessRank defenderRank = (ChessRank)(((int)pawn.Rank) - 1);
                        ChessFile defenderFile = (ChessFile)(((int)pawn.File) + 1);
                        var defender = board.GetPieceAt(defenderRank, defenderFile);
                        if(defender != null && defender.ChessPiece == ChessPiece.White){
                            isProtected = true;
                        }
                    }
                }
                if(isProtected){
                    whiteProtection += 20;
                }
            }

            foreach (var pawn in board.BlackPieces)
            {
                bool isProtected = false;
                if (pawn.Rank != ChessRank.Eight)
                {
                    if (pawn.File != ChessFile.A)
                    {
                        ChessRank defenderRank = (ChessRank)(((int)pawn.Rank) + 1);
                        ChessFile defenderFile = (ChessFile)(((int)pawn.File) - 1);
                        var defender = board.GetPieceAt(defenderRank, defenderFile);
                        if (defender != null && defender.ChessPiece == ChessPiece.Black)
                            isProtected = true;
                    }
                    if (!isProtected && pawn.File != ChessFile.H)
                    {
                        ChessRank defenderRank = (ChessRank)(((int)pawn.Rank) + 1);
                        ChessFile defenderFile = (ChessFile)(((int)pawn.File) + 1);
                        var defender = board.GetPieceAt(defenderRank, defenderFile);
                        if (defender != null && defender.ChessPiece == ChessPiece.Black)
                            isProtected = true;
                    }
                }
                if (isProtected)
                    blackProtection += 20;
            }

            return whiteProtection - blackProtection;
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
