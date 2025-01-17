using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBot.Core.Board;

namespace ChessBot.Core.Agents
{
    public class HumanPlayer : IAgent
    {
        public ChessColor Color { get; set; }

        public Move GetMove(ChessBoard board)
        {
            var currentPieces = Color == ChessColor.White ? board.WhitePieces : board.BlackPieces;
            var chosenPiece = PickPiece(board, currentPieces);

            if (chosenPiece == null)
                throw new InvalidOperationException("No valid moves available.");

            var possibleMoves = board.GetPossibleMoves(chosenPiece).ToArray();
            var chosenMove = PickMove(possibleMoves);

            return chosenMove ?? throw new InvalidOperationException("Invalid move.");
        }

        private BoardPiece? PickPiece(ChessBoard board, IEnumerable<BoardPiece> currentPieces)
        {
            var piecesArray = currentPieces.ToArray();
            if (piecesArray.Length == 0)
            {
                return null;
            }

            while (true)
            {
                Console.WriteLine("Pick a piece with valid moves:");
                for (int i = 0; i < piecesArray.Length; i++)
                {
                    var piece = piecesArray[i];
                    int moveCount = board.GetPossibleMoves(piece).Count();
                    Console.WriteLine($"[{i}] {piece.File}{(ushort)piece.Rank} - {moveCount} possible moves");
                }

                Console.Write("Enter piece index: ");
                if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 0 || choice >= piecesArray.Length)
                {
                    Console.WriteLine("Invalid index. Try again.");
                    continue;
                }

                var chosenPiece = piecesArray[choice];
                if (board.GetPossibleMoves(chosenPiece).Any())
                {
                    return chosenPiece;
                }

                Console.WriteLine("That piece has no valid moves. Please pick another piece.");
            }
        }

        private Move? PickMove(Move[] moves)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                var move = moves[i];
                Console.WriteLine($"[{i}] {move.TargetFile}{(ushort)move.TargetRank} - {move.MoveCommand}");
            }

            while (true)
            {
                Console.Write("Pick a valid move index: ");
                if (int.TryParse(Console.ReadLine(), out var moveIndex) && moveIndex >= 0 && moveIndex < moves.Length)
                {
                    return moves[moveIndex];
                }

                Console.WriteLine("Invalid index. Try again.");
            }
        }
    }
}
