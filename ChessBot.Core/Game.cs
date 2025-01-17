using ChessBot.Core.Board;
using ChessBot.Core.Agents;

namespace ChessBot.Core
{
    public class Game
    {
        private readonly ChessBoard _board;
        private readonly IAgent _whitePlayer;
        private readonly IAgent _blackPlayer;

        public Game(ChessBoard board, IAgent whitePlayer, IAgent blackPlayer)
        {
            _board = board;
            _whitePlayer = whitePlayer;
            _blackPlayer = blackPlayer;
        }

        public void Run()
        {
            bool whitesTurn = true;
            while (true)
            {
                _board.PrintBoard();

                if (EndGame.IsGameOver(_board, out string? message))
                {
                    Console.WriteLine(message);
                    break;
                }

                var currentPlayer = whitesTurn ? _whitePlayer : _blackPlayer;
                Console.WriteLine($"{(whitesTurn ? "White" : "Black")}'s turn...");

                var move = currentPlayer.GetMove(_board);
                _board.ExecuteMove(move);

                whitesTurn = !whitesTurn;
            }
        }
    }
}
