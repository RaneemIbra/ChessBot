namespace ChessBot.Core.Board
{
    public static class BoardManipulation
    {
        public static BoardPiece MovePiece(Board board, BoardPiece piece, ChessRank targetRank, ChessFile targetFile)
        {
            RemovePiece(board, piece);
            var newPiece = new BoardPiece
            {
                ChessPiece = piece.ChessPiece,
                Rank = targetRank,
                File = targetFile
            };
            board.AddPiece(newPiece);
            return newPiece;
        }

        public static void RemovePiece(Board board, BoardPiece piece)
        {
            var occupant = board.GetPieceAt(piece.Rank, piece.File);
            if (occupant != null)
            {
                board.RemovePiece(occupant);
            }
        }
    }
}