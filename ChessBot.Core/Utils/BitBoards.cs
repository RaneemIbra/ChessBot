using System;

namespace ChessBot.Core.Utils
{
    public struct PawnBitboards
    {
        public ulong WhitePawns;
        public ulong BlackPawns;

        public PawnBitboards(ulong white, ulong black)
        {
            WhitePawns = white;
            BlackPawns = black;
        }
    }

    public static class BitboardHelper
    {
        public const ulong FileA = 0x0101010101010101;
        public const ulong FileH = 0x8080808080808080;
        public const ulong Rank2 = 0x000000000000FF00;
        public const ulong Rank7 = 0x00FF000000000000;

        public static ulong WhiteSinglePush(ulong whitePawns, ulong emptySquares)
        {
            ulong oneStep = whitePawns << 8;
            return oneStep & emptySquares;
        }

        public static ulong WhiteDoublePush(ulong whitePawns, ulong emptySquares)
        {
            ulong oneStep = WhiteSinglePush(whitePawns, emptySquares);
            ulong pawnsOnRank2 = whitePawns & Rank2;
            ulong twoSteps = (pawnsOnRank2 << 16);
            ulong intermediate = pawnsOnRank2 << 8;
            return (twoSteps & emptySquares) & (intermediate & emptySquares);
        }

        public static ulong WhiteCaptures(ulong whitePawns, ulong enemyPieces)
        {
            ulong leftCaptures = (whitePawns & ~FileA) << 7;
            ulong rightCaptures = (whitePawns & ~FileH) << 9;
            return (leftCaptures | rightCaptures) & enemyPieces;
        }

        public static ulong BlackSinglePush(ulong blackPawns, ulong emptySquares)
        {
            ulong oneStep = blackPawns >> 8;
            return oneStep & emptySquares;
        }

        public static ulong BlackDoublePush(ulong blackPawns, ulong emptySquares)
        {
            ulong oneStep = BlackSinglePush(blackPawns, emptySquares);
            ulong pawnsOnRank7 = blackPawns & Rank7;
            ulong twoSteps = (pawnsOnRank7 >> 16);
            ulong intermediate = pawnsOnRank7 >> 8;
            return (twoSteps & emptySquares) & (intermediate & emptySquares);
        }

        public static ulong BlackCaptures(ulong blackPawns, ulong enemyPieces)
        {
            ulong leftCaptures = (blackPawns & ~FileA) >> 9;
            ulong rightCaptures = (blackPawns & ~FileH) >> 7;
            return (leftCaptures | rightCaptures) & enemyPieces;
        }
    }
}
