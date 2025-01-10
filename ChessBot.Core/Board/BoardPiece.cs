namespace ChessBot.Core.Board
{
    public class BoardPiece
    {
        public ChessRank Rank { get; set; }
        public ChessFile File { get; set; }
        public ChessPiece ChessPiece { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is BoardPiece piece) return piece.Rank == Rank && piece.File == File;
            return false;
        }

        public override int GetHashCode()
        {
            return Rank.GetHashCode() + File.GetHashCode();
        }
    }
}