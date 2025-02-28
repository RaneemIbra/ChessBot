/*
 * Zobrist.cs
 * 
 * Summary:
 *   This file implements Zobrist hashing, which is used for efficiently computing unique hash values
 *   for different board states. This technique allows quick lookups in the transposition table.
 * 
 *   The Zobrist class contains:
 *     - A 3D array `Table` that holds precomputed random bitstrings for each (rank, file, piece) combination.
 *     - Two additional random values `WhiteToMove` and `BlackToMove` to encode turn information.
 * 
 *   Each board position's unique hash is computed by XOR-ing the values corresponding to occupied squares
 *   and the side to move.
 */

using System;

namespace ChessBot.Core.Algorithms
{
    /// <summary>
    /// Implements Zobrist hashing for efficiently computing unique hash values for chess board positions.
    /// </summary>
    public static class Zobrist
    {
        /// <summary>
        /// Precomputed random values for each (rank, file, piece) combination.
        /// The third dimension represents different piece types (assuming only pawns are used in this bot).
        /// </summary>
        public static readonly ulong[,,] Table;

        /// <summary>
        /// Random value to indicate it's White's turn to move.
        /// </summary>
        public static readonly ulong WhiteToMove;

        /// <summary>
        /// Random value to indicate it's Black's turn to move.
        /// </summary>
        public static readonly ulong BlackToMove;

        /// <summary>
        /// Static constructor to initialize Zobrist hash table with random values.
        /// Uses a fixed seed for reproducibility.
        /// </summary>
        static Zobrist()
        {
            Table = new ulong[8, 8, 3]; // 8x8 board with 3 possible pieces (e.g., empty, white pawn, black pawn)
            Random random = new Random(123456); // Fixed seed for deterministic behavior

            // Generate random values for each board position and piece type
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    for (int piece = 0; piece < 3; piece++)
                    {
                        byte[] buffer = new byte[8];
                        random.NextBytes(buffer);
                        Table[rank, file, piece] = BitConverter.ToUInt64(buffer, 0);
                    }
                }
            }

            // Generate random values for turn indication
            {
                byte[] buffer = new byte[8];
                random.NextBytes(buffer);
                WhiteToMove = BitConverter.ToUInt64(buffer, 0);
            }
            {
                byte[] buffer = new byte[8];
                random.NextBytes(buffer);
                BlackToMove = BitConverter.ToUInt64(buffer, 0);
            }
        }
    }
}
