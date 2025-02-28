/*
 * HumanAgent.cs
 * 
 * Summary:
 *   The HumanPlayer class implements the IAgent interface and represents a human-controlled
 *   player in the chess game. Instead of computing moves via an algorithm, this class prompts 
 *   the user to enter a move in standard chess notation (for example, "e2e4") via the console.
 *   It then parses the input using the NotationHelper to convert it into a Move object. If the 
 *   input is invalid, the user is repeatedly prompted until a valid move is entered.
 * 
 * Usage:
 *   - Set the player's color through the Color property.
 *   - Call GetMove(board) to prompt the user for input and retrieve the corresponding move.
 *   - The class continuously requests input until a valid move is provided.
 */

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
        // The chess color (e.g., White or Black) assigned to this human player.
        public ChessColor Color { get; set; }

        /// <summary>
        /// Prompts the human user to enter a move in standard notation (e.g. "e2e4").
        /// The method uses the NotationHelper to convert the input string into a Move object.
        /// If the input is invalid, the user is prompted again until a valid move is entered.
        /// </summary>
        /// <param name="board">The current chess board state.</param>
        /// <returns>The Move object representing the user's chosen move.</returns>
        public Move GetMove(ChessBoard board)
        {
            while (true)
            {
                Console.WriteLine("Enter your move in standard notation (e.g. e2e4):");
                string? input = Console.ReadLine();

                // Use the NotationHelper to parse the user's input into a Move.
                var move = NotationHelper.FromNotation(input ?? string.Empty, board, Color);

                // If the move is not valid, prompt the user to try again.
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
