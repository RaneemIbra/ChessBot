using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBot.Core.Board;
using ChessBot.Core.Utils;

namespace ChessBot.Core.Agents
{
    public class HumanPlayer : IAgent
    {
        public ChessColor Color { get; set; }

        public Move GetMove(ChessBoard board)
        {
            while (true)
            {
                Console.WriteLine("Enter your move in standard notation (e.g. e2e4):");
                string? input = Console.ReadLine();
                var move = NotationHelper.FromNotation(input ?? string.Empty, board, Color);
                if (move == null)
                {
                    Console.WriteLine("Invalid move. Please try again.");
                    continue;
                }
                return move;
            }
        }
    }
}
