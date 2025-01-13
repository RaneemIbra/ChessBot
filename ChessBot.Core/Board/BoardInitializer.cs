namespace ChessBot.Core.Board
{
    public static class BoardInitializer
    {
        public static void Setup(ChessBoard board, IEnumerable<string> setupParts)
        {
            foreach (var part in setupParts)
            {
                ChessPiece piece = part[0].ToString().Equals("w", StringComparison.CurrentCultureIgnoreCase) ? ChessPiece.White : ChessPiece.Black;
                ChessFile file = (ChessFile)(part[1] - 'a');
                ChessRank rank = (ChessRank)(part[2] - '1' + 1);
                board.InitPiece(rank, file, piece);
            }
        }

        public static void ClearBoard(ChessBoard board)
        {
            foreach (ChessRank rank in Enum.GetValues<ChessRank>())
            {
                foreach (ChessFile file in Enum.GetValues<ChessFile>())
                {
                    board.InitPiece(rank, file, ChessPiece.Empty);
                }
            }
        }
    }
}