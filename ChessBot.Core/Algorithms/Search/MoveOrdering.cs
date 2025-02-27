using ChessBot.Core.Board;

namespace ChessBot.Core.Algorithms.Search
{
    public static class MoveOrdering
    {
        public static IEnumerable<Move> OrderMoves(IEnumerable<Move> moves, ChessBoard board, ChessColor sideToMove)
        {
            return moves.OrderByDescending(move =>                   
                {
                    int score = 0;
                    if (move.CapturedPiece != null)
                        score += 1000;
                    int rankDiff = 0;
                    if (move.MovingPiece.ChessPiece == ChessPiece.White)
                        rankDiff = (int)move.TargetRank - (int)move.MovingPiece.Rank;
                    else if (move.MovingPiece.ChessPiece == ChessPiece.Black)
                        rankDiff = (int)move.MovingPiece.Rank - (int)move.TargetRank;
                    score += rankDiff * 10;
                    return score ;
                });        
        }

        public static List<Move> GenerateCaptureMoves(ChessBoard board, ChessColor sideToMove)
        {
            List<Move> captureMoves = new List<Move>();
            IEnumerable<BoardPiece> pieces = sideToMove == ChessColor.White ? board.WhitePieces : board.BlackPieces;
            foreach (var piece in pieces)
            {
                var moves = PossibleMoves.CapturingMove(board, piece);
                captureMoves.AddRange(moves);
            }
            return captureMoves;
        }
    }
}
