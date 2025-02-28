/*
 * MoveOrdering.cs
 * 
 * Summary:
 *   The MoveOrdering class provides helper methods to improve the efficiency of the search algorithm
 *   by ordering moves based on heuristic scores. Proper move ordering is crucial for effective alpha–beta
 *   pruning, as it helps the algorithm find good moves early and prune unpromising branches.
 * 
 *   This file contains two main methods:
 *     1. OrderMoves: Orders a collection of moves in descending order of their heuristic score.
 *        The heuristic rewards moves that capture an opponent’s piece and moves that advance a pawn.
 *     2. GenerateCaptureMoves: Generates a list of moves that involve capturing an opponent’s piece.
 *        This is useful in quiescence search, where only "noisy" moves like captures are considered.
 */

using ChessBot.Core.Board;

namespace ChessBot.Core.Algorithms.Search
{
    public static class MoveOrdering
    {
        /// <summary>
        /// Orders the given moves in descending order based on a simple heuristic score.
        /// 
        /// The heuristic works as follows:
        ///   - If the move captures an opponent's piece, it adds a large bonus (1000 points).
        ///   - Additionally, it calculates a score based on the rank difference:
        ///       - For white pieces, moves that advance the pawn upward (towards a higher rank)
        ///         are rewarded.
        ///       - For black pieces, moves that advance the pawn downward (towards a lower rank)
        ///         are rewarded.
        ///   - The rank difference is multiplied by 10 and added to the score.
        /// 
        /// A higher total score indicates a move that is more promising.
        /// 
        /// Parameters:
        ///   moves      - A collection of possible moves.
        ///   board      - The current chess board state (not directly used in the heuristic here,
        ///                but provided for potential future enhancements).
        ///   sideToMove - The color of the player whose moves are being ordered.
        /// 
        /// Returns:
        ///   An IEnumerable of moves sorted in descending order by their heuristic score.
        /// </summary>
        public static IEnumerable<Move> OrderMoves(IEnumerable<Move> moves, ChessBoard board, ChessColor sideToMove)
        {
            return moves.OrderByDescending(move =>
            {
                int score = 0;
                // Reward capturing moves with a high score bonus.
                if (move.CapturedPiece != null)
                    score += 1000;

                // Calculate rank difference for pawn advancement.
                int rankDiff = 0;
                if (move.MovingPiece.ChessPiece == ChessPiece.White)
                    // For white, a move that increases the rank is favorable.
                    rankDiff = (int)move.TargetRank - (int)move.MovingPiece.Rank;
                else if (move.MovingPiece.ChessPiece == ChessPiece.Black)
                    // For black, a move that decreases the rank is favorable.
                    rankDiff = (int)move.MovingPiece.Rank - (int)move.TargetRank;

                // Add the rank difference (scaled) to the score.
                score += rankDiff * 10;
                return score;
            });
        }

        /// <summary>
        /// Generates a list of capture moves for the given board and player.
        /// 
        /// This method iterates through all the pieces of the specified side and collects all moves 
        /// that capture an opponent's piece using the CapturingMove method from the PossibleMoves class.
        /// 
        /// Parameters:
        ///   board      - The current chess board state.
        ///   sideToMove - The color of the player for whom capture moves are to be generated.
        /// 
        /// Returns:
        ///   A list of moves that involve capturing an opponent’s piece.
        /// </summary>
        public static List<Move> GenerateCaptureMoves(ChessBoard board, ChessColor sideToMove)
        {
            List<Move> captureMoves = new List<Move>();
            // Select pieces belonging to the current player.
            IEnumerable<BoardPiece> pieces = sideToMove == ChessColor.White ? board.WhitePieces : board.BlackPieces;
            // For each piece, get all capture moves and add them to the list.
            foreach (var piece in pieces)
            {
                var moves = PossibleMoves.CapturingMove(board, piece);
                captureMoves.AddRange(moves);
            }
            return captureMoves;
        }
    }
}
