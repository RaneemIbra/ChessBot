using ChessBot.Core.Board;
using ChessBot.Core.Agents;
using ChessBot.Core;
using System.Diagnostics;
using System.Security.Cryptography;

namespace ChessBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChessBoard board = new ChessBoard();

            Console.WriteLine("Enter setup command (e.g., Setup Wb1 Wb2 Bg6):");
            string? setupCommand = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(setupCommand) || !ParseSetupCommand(setupCommand.Trim(), board))
            {
                Console.WriteLine("Failed to setup board. Exiting.");
                return;
            }

            Console.WriteLine("Choose mode: 1 for Player vs Player, 2 for Player vs Agent, 3 for Agent vs Agent");
            int mode = GetChoice(new[] { 1, 2, 3 });

            IAgent whitePlayer;
            IAgent blackPlayer;

            if (mode == 1)
            {
                whitePlayer = new HumanPlayer { Color = ChessColor.White };
                blackPlayer = new HumanPlayer { Color = ChessColor.Black };
            }
            else if (mode == 2)
            {
                Console.WriteLine("Who plays as White? 1 for Player, 2 for Agent");
                int choice = GetChoice(new[] { 1, 2 });
                if (choice == 1)
                {
                    whitePlayer = new HumanPlayer { Color = ChessColor.White };
                    blackPlayer = new SimpleAgent { Color = ChessColor.Black };
                }
                else
                {
                    whitePlayer = new SimpleAgent { Color = ChessColor.White };
                    blackPlayer = new HumanPlayer { Color = ChessColor.Black };
                }
            }
            else
            {
                whitePlayer = new SimpleAgent { Color = ChessColor.White };
                blackPlayer = new SimpleAgent { Color = ChessColor.Black };
            }

            var game = new Game(board, whitePlayer, blackPlayer);
            game.Run();
        }

        static int GetChoice(int[] validChoices)
        {
            while (true)
            {
                Console.Write("Enter your choice: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && validChoices.Contains(choice))
                {
                    return choice;
                }
                Console.WriteLine("Invalid choice. Try again.");
            }
        }

        static bool ParseSetupCommand(string? setupCommand, ChessBoard board)
        {
            if (string.IsNullOrWhiteSpace(setupCommand) || !setupCommand.StartsWith("Setup ", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string[] parts = setupCommand.Substring(6).Split(" ", StringSplitOptions.RemoveEmptyEntries);
            try
            {
                board.ClearBoard();
                board.Setup(parts);
                return true;
            }
            catch
            {
                Console.WriteLine("Failed to setup board. Please try again with the correct format.");
                return false;
            }
        }
    }
}