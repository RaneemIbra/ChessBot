/*
 * Evaluation.cs
 * 
 * Summary:
 *   This file contains the static Evaluation class that provides methods for assessing
 *   the strength of a given board position in our pawn-only chess game. The evaluation
 *   function takes into account multiple factors including material balance, pawn advancement,
 *   mobility, control of the center, and bonus points for passed pawns. The overall score
 *   returned is oriented from the perspective of the root color (the player for whom the
 *   evaluation is performed).
 * 
 *   Key components include:
 *     - EvaluateBoard: The main evaluation function that combines various factors.
 *     - EvaluateCenterControl: Assesses control over the center squares.
 *     - EvaluatePassedPawnBonus: Computes bonus scores for pawns that are not blocked by enemy pawns.
 *     - EvaluateAdvancement: Rewards pawns that are advanced toward promotion.
 *     - GetPawnBitboards: Converts the board's pawn positions into bitboards for fast computations.
 *
 *   The evaluation returns a high positive number if the position is favorable to the root player,
 *   and a high negative number if it is unfavorable. Terminal positions (win/loss) are assigned
 *   scores near positive or negative infinity.
 */

using ChessBot.Core.Board;
using ChessBot.Core.Utils;
using System.Numerics;

namespace ChessBot.Core.Algorithms.Evaluation
{
    public static class Evaluation
    {
        // A constant representing "infinity" used in terminal position evaluation.
        private const int INF = 1000000;

        /// <summary>
        /// Evaluates the given board position from the perspective of the rootColor.
        /// If the game is over, returns a terminal score.
        /// Otherwise, combines various factors such as material, advancement,
        /// mobility, center control, and passed pawn bonuses.
        /// </summary>
        /// <param name="board">The current chess board state.</param>
        /// <param name="rootColor">The color for which the evaluation is performed.</param>
        /// <returns>
        /// An integer score where a higher value indicates a better position for the rootColor.
        /// Terminal wins/losses are indicated by values near INF or -INF.
        /// </returns>
        public static int EvaluateBoard(ChessBoard board, ChessColor rootColor)
        {
            // Check if the game is over. If so, return a near-infinite score.
            if (EndGame.IsGameOver(board, out string? _))
            {
                ChessColor winner = EndGame.GetWinner(board);
                return (winner == rootColor) ? INF - 1 : -INF + 1;
            }

            // Get the bitboard representations for white and black pawns.
            var pb = GetPawnBitboards(board);

            // Calculate material score based on pawn count.
            int whitePawnCount = BitOperations.PopCount(pb.WhitePawns);
            int blackPawnCount = BitOperations.PopCount(pb.BlackPawns);
            int materialScore = (whitePawnCount - blackPawnCount) * 100;

            // Calculate bonus for pawn advancement.
            int whiteAdvancement = EvaluateAdvancement(pb.WhitePawns, true);
            int blackAdvancement = EvaluateAdvancement(pb.BlackPawns, false);

            // Combine both bitboards to know which squares are occupied.
            ulong occupied = pb.WhitePawns | pb.BlackPawns;

            // Calculate mobility: number of possible single moves and capture moves.
            int whiteMobility = BitOperations.PopCount(BitboardHelper.WhiteSinglePush(pb.WhitePawns, ~occupied)) +
                                BitOperations.PopCount(BitboardHelper.WhiteCaptures(pb.WhitePawns, pb.BlackPawns));
            int blackMobility = BitOperations.PopCount(BitboardHelper.BlackSinglePush(pb.BlackPawns, ~occupied)) +
                                BitOperations.PopCount(BitboardHelper.BlackCaptures(pb.BlackPawns, pb.WhitePawns));
            int mobilityScore = (whiteMobility - blackMobility) * 10;

            // Calculate bonus for passed pawns.
            int passedBonusWhite = EvaluatePassedPawnBonus(pb.WhitePawns, pb.BlackPawns, true);
            int passedBonusBlack = EvaluatePassedPawnBonus(pb.BlackPawns, pb.WhitePawns, false);

            // Calculate bonus for controlling the center of the board.
            int centerControlScore = EvaluateCenterControl(pb);

            // Combine all factors into a final score.
            int score = centerControlScore + materialScore + whiteAdvancement - blackAdvancement + mobilityScore + (passedBonusWhite - passedBonusBlack);

            // Return score relative to rootColor perspective.
            return rootColor == ChessColor.White ? score : -score;
        }

        /// <summary>
        /// Evaluates the control over the center of the board.
        /// Uses a center mask to identify key squares and awards points for both occupied and potential control.
        /// </summary>
        /// <param name="pb">PawnBitboards representing white and black pawn positions.</param>
        /// <returns>
        /// An integer representing the center control score, positive if white has an advantage,
        /// negative if black does.
        /// </returns>
        private static int EvaluateCenterControl(PawnBitboards pb)
        {
            // Center squares of interest: these indices correspond to the center of an 8x8 board.
            ulong centerMask = (1UL << 27) | (1UL << 28) | (1UL << 35) | (1UL << 36);

            // Calculate bonus for white: count pawns occupying center squares.
            int whiteOccupiedBonus = BitOperations.PopCount(pb.WhitePawns & centerMask) * 25;

            // Calculate potential control for white by shifting bitboards to simulate capture moves.
            ulong whitePotential = ((pb.WhitePawns & ~BitboardHelper.FileA) << 7) | ((pb.WhitePawns & ~BitboardHelper.FileH) << 9);
            int whitePotentialBonus = BitOperations.PopCount(whitePotential & centerMask) * 10;

            // Similarly for black.
            int blackOccupiedBonus = BitOperations.PopCount(pb.BlackPawns & centerMask) * 25;
            ulong blackPotential = ((pb.BlackPawns & ~BitboardHelper.FileA) >> 9) | ((pb.BlackPawns & ~BitboardHelper.FileH) >> 7);
            int blackPotentialBonus = BitOperations.PopCount(blackPotential & centerMask) * 10;

            // Return the difference in center control.
            return (whiteOccupiedBonus + whitePotentialBonus) - (blackOccupiedBonus + blackPotentialBonus);
        }

        /// <summary>
        /// Evaluates the bonus for passed pawns.
        /// A passed pawn is one that does not have any enemy pawns blocking its advance.
        /// The bonus is higher if the pawn's path is clear.
        /// </summary>
        /// <param name="ownPawns">Bitboard for the player's pawns.</param>
        /// <param name="enemyPawns">Bitboard for the enemy's pawns.</param>
        /// <param name="isWhite">True if evaluating for white, false for black.</param>
        /// <returns>
        /// An integer bonus representing the advantage gained from passed pawns.
        /// </returns>
        private static int EvaluatePassedPawnBonus(ulong ownPawns, ulong enemyPawns, bool isWhite)
        {
            int bonus = 0;
            ulong pawns = ownPawns;
            // Combine own and enemy pawns to know occupied squares.
            ulong occupied = ownPawns | enemyPawns;
            while (pawns != 0)
            {
                // Get the index of the least significant pawn bit.
                int idx = BitOperations.TrailingZeroCount(pawns);
                // Remove that pawn from the bitboard.
                pawns &= pawns - 1;
                int rank = idx / 8;
                int file = idx % 8;

                // Create a mask that covers the squares in front of the pawn (and one file to either side).
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

                // If no enemy pawns are found in the mask, the pawn is passed.
                if ((enemyPawns & mask) == 0)
                {
                    bool clearPath = true;
                    // Check for obstructions in the pawn's direct path.
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
                    // Calculate the distance to promotion.
                    int distance = isWhite ? (7 - rank) : rank;
                    // Give a higher bonus if the path is clear.
                    int pawnBonus = (clearPath ? 300 : 150) + ((7 - distance) * 50);
                    bonus += pawnBonus;
                }
            }
            return bonus;
        }

        /// <summary>
        /// Evaluates the bonus for pawn advancement.
        /// For white, the bonus increases as the pawn moves upward (increasing rank);
        /// for black, as it moves downward (decreasing rank).
        /// </summary>
        /// <param name="pawnBitboard">Bitboard representing the pawns.</param>
        /// <param name="isWhite">True if evaluating for white, false for black.</param>
        /// <returns>An integer bonus based on pawn advancement.</returns>
        private static int EvaluateAdvancement(ulong pawnBitboard, bool isWhite)
        {
            int bonus = 0;
            ulong pawns = pawnBitboard;
            while (pawns != 0)
            {
                int index = BitOperations.TrailingZeroCount(pawns);
                pawns &= pawns - 1;
                int rank = index / 8;
                // For white, bonus is proportional to rank number; for black, to distance from top.
                bonus += isWhite ? rank * 10 : (7 - rank) * 10;
            }
            return bonus;
        }

        /// <summary>
        /// Constructs bitboard representations for white and black pawns.
        /// Each pawn's position is encoded as a bit in a 64-bit unsigned integer.
        /// </summary>
        /// <param name="board">The current chess board state.</param>
        /// <returns>A PawnBitboards structure containing the bitboards for white and black pawns.</returns>
        private static PawnBitboards GetPawnBitboards(ChessBoard board)
        {
            ulong white = 0UL;
            ulong black = 0UL;
            // For every white piece, calculate its bit index and update the white bitboard.
            foreach (var piece in board.WhitePieces)
            {
                int rankIdx = (int)piece.Rank - 1;
                int fileIdx = (int)piece.File;
                int bitIndex = rankIdx * 8 + fileIdx;
                white |= 1UL << bitIndex;
            }
            // Similarly for black pieces.
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
