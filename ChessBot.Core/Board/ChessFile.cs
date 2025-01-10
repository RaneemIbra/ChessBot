namespace ChessBot.Core.Board
{
    public enum ChessFile : ushort
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7
    }

    public static class ChessFilExtension
    {
        public static ushort ToIndex(this ChessFile file)
        {
            return (ushort)file;
        }
    }
}
