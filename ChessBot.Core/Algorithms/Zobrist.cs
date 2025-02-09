using System;

namespace ChessBot.Core.Algorithms
{
    public static class Zobrist
    {
        public static readonly ulong[,,] Table;

        static Zobrist()
        {
            Table = new ulong[8, 8, 3];
            Random random = new Random(123456);
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
        }
    }
}
