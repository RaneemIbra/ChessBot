using System;

namespace ChessBot.Core.Utils
{
    /// <summary>
    /// A struct that holds the bitboards representing the positions of the white and black pawns on the chessboard.
    /// </summary>
    public struct PawnBitboards
    {
        public ulong WhitePawns;  // Bitboard for white pawns
        public ulong BlackPawns;  // Bitboard for black pawns

        /// <summary>
        /// Constructor to initialize the PawnBitboards struct with white and black pawns bitboards.
        /// </summary>
        /// <param name="white">The bitboard representing white pawns.</param>
        /// <param name="black">The bitboard representing black pawns.</param>
        public PawnBitboards(ulong white, ulong black)
        {
            WhitePawns = white;
            BlackPawns = black;
        }
    }

    /// <summary>
    /// A helper class that provides methods to manipulate and evaluate pawn bitboards for white and black pawns.
    /// </summary>
    public static class BitboardHelper
    {
        // Constants representing files and ranks on the chessboard.
        public const ulong FileA = 0x0101010101010101;
        public const ulong FileH = 0x8080808080808080;
        public const ulong Rank2 = 0x000000000000FF00;
        public const ulong Rank7 = 0x00FF000000000000;

        /// <summary>
        /// Generates a bitboard for the white pawns' single step forward move.
        /// </summary>
        /// <param name="whitePawns">The bitboard representing white pawns.</param>
        /// <param name="emptySquares">The bitboard representing empty squares on the board.</param>
        /// <returns>A bitboard representing the possible single forward push for white pawns.</returns>
        public static ulong WhiteSinglePush(ulong whitePawns, ulong emptySquares)
        {
            ulong oneStep = whitePawns << 8;
            return oneStep & emptySquares;
        }

        /// <summary>
        /// Generates a bitboard for the white pawns' double step forward move, if they are on their starting rank.
        /// </summary>
        /// <param name="whitePawns">The bitboard representing white pawns.</param>
        /// <param name="emptySquares">The bitboard representing empty squares on the board.</param>
        /// <returns>A bitboard representing the possible double forward push for white pawns.</returns>
        public static ulong WhiteDoublePush(ulong whitePawns, ulong emptySquares)
        {
            ulong oneStep = WhiteSinglePush(whitePawns, emptySquares);
            ulong pawnsOnRank2 = whitePawns & Rank2;
            ulong twoSteps = (pawnsOnRank2 << 16);
            ulong intermediate = pawnsOnRank2 << 8;
            return (twoSteps & emptySquares) & (intermediate & emptySquares);
        }

        /// <summary>
        /// Generates a bitboard for the white pawns' capturing moves.
        /// </summary>
        /// <param name="whitePawns">The bitboard representing white pawns.</param>
        /// <param name="enemyPieces">The bitboard representing the opponent's pieces.</param>
        /// <returns>A bitboard representing the possible capturing moves for white pawns.</returns>
        public static ulong WhiteCaptures(ulong whitePawns, ulong enemyPieces)
        {
            ulong leftCaptures = (whitePawns & ~FileA) << 7;
            ulong rightCaptures = (whitePawns & ~FileH) << 9;
            return (leftCaptures | rightCaptures) & enemyPieces;
        }

        /// <summary>
        /// Generates a bitboard for the black pawns' single step forward move.
        /// </summary>
        /// <param name="blackPawns">The bitboard representing black pawns.</param>
        /// <param name="emptySquares">The bitboard representing empty squares on the board.</param>
        /// <returns>A bitboard representing the possible single forward push for black pawns.</returns>
        public static ulong BlackSinglePush(ulong blackPawns, ulong emptySquares)
        {
            ulong oneStep = blackPawns >> 8;
            return oneStep & emptySquares;
        }

        /// <summary>
        /// Generates a bitboard for the black pawns' double step forward move, if they are on their starting rank.
        /// </summary>
        /// <param name="blackPawns">The bitboard representing black pawns.</param>
        /// <param name="emptySquares">The bitboard representing empty squares on the board.</param>
        /// <returns>A bitboard representing the possible double forward push for black pawns.</returns>
        public static ulong BlackDoublePush(ulong blackPawns, ulong emptySquares)
        {
            ulong oneStep = BlackSinglePush(blackPawns, emptySquares);
            ulong pawnsOnRank7 = blackPawns & Rank7;
            ulong twoSteps = (pawnsOnRank7 >> 16);
            ulong intermediate = pawnsOnRank7 >> 8;
            return (twoSteps & emptySquares) & (intermediate & emptySquares);
        }

        /// <summary>
        /// Generates a bitboard for the black pawns' capturing moves.
        /// </summary>
        /// <param name="blackPawns">The bitboard representing black pawns.</param>
        /// <param name="enemyPieces">The bitboard representing the opponent's pieces.</param>
        /// <returns>A bitboard representing the possible capturing moves for black pawns.</returns>
        public static ulong BlackCaptures(ulong blackPawns, ulong enemyPieces)
        {
            ulong leftCaptures = (blackPawns & ~FileA) >> 9;
            ulong rightCaptures = (blackPawns & ~FileH) >> 7;
            return (leftCaptures | rightCaptures) & enemyPieces;
        }
    }
}
