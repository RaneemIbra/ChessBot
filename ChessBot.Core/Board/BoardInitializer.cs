using ChessBot.Core.Board;

public static class BoardInitializer
{
    public static void Setup(Board board, IEnumerable<string> setupParts)
    {
        foreach (var part in setupParts)
        {
            ChessPiece piece = part[0].ToString().ToLower() == "w" ? ChessPiece.White : ChessPiece.Black;
            ChessFile file = (ChessFile)(part[1] - 'a');
            ChessRank rank = (ChessRank)(part[2] - '1' + 1);
            board.InitPiece(rank, file, piece);
        }
    }

    public static void ClearBoard(Board board)
    {
        foreach (ChessRank rank in Enum.GetValues(typeof(ChessRank)))
        {
            foreach (ChessFile file in Enum.GetValues(typeof(ChessFile)))
            {
                board.InitPiece(rank, file, ChessPiece.Empty);
            }
        }
    }
}
