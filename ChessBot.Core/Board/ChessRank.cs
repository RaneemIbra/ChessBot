namespace ChessBot.Core.Board
{
    public enum ChessRank : ushort
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    }

    public static class ChessRankExtension
    {
        public static ushort ToIndex(this ChessRank rank)
        {
            return (ushort)(rank - 1);
        }

        public static ChessRank FromIndex(this ushort index)
        {
            return (ChessRank)(index + 1);
        }
    }
}
