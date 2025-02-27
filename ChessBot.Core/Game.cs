using ChessBot.Core.Board;
using ChessBot.Core.Agents;
using System.Diagnostics;
using ChessBot.Core.Utils;

namespace ChessBot.Core
{
    public class Game
    {
        private readonly ChessBoard _board;
        private readonly IAgent _whitePlayer;
        private readonly IAgent _blackPlayer;
        private TimeSpan _whiteTimeLeft;
        private TimeSpan _blackTimeLeft;
        public Game(ChessBoard board, IAgent whitePlayer, IAgent blackPlayer, int timeForAgents = 1)
        {
            _board = board;
            _whitePlayer = whitePlayer;
            _blackPlayer = blackPlayer;
            _whiteTimeLeft = TimeSpan.FromMinutes(timeForAgents);
            _blackTimeLeft = TimeSpan.FromMinutes(timeForAgents);
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

                IAgent currentPlayer = whitesTurn ? _whitePlayer : _blackPlayer;

                int moveTimeAllocatedMilliseconds;
                if (whitesTurn)
                    moveTimeAllocatedMilliseconds = (int)(_whiteTimeLeft.TotalMilliseconds / 10);
                else
                    moveTimeAllocatedMilliseconds = (int)(_blackTimeLeft.TotalMilliseconds / 10);

                Console.WriteLine($"Time allocated for move: {moveTimeAllocatedMilliseconds / 1000.0:F2} seconds");

                if (currentPlayer is FinalAgent fa)
                {
                    fa.MoveTimeLimitInMilliSeconds = moveTimeAllocatedMilliseconds;
                }

                var stopwatch = Stopwatch.StartNew();
                Console.WriteLine($"{(whitesTurn ? "White" : "Black")}'s turn.");

                Move move;
                try
                {
                    move = currentPlayer.GetMove(_board);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception during move: " + ex.Message);
                    break;
                }

                stopwatch.Stop();
                if (whitesTurn)
                {
                    _whiteTimeLeft -= stopwatch.Elapsed;
                    if (_whiteTimeLeft <= TimeSpan.Zero)
                    {
                        Console.WriteLine("White's time is up! Black wins by time.");
                        break;
                    }
                }
                else
                {
                    _blackTimeLeft -= stopwatch.Elapsed;
                    if (_blackTimeLeft <= TimeSpan.Zero)
                    {
                        Console.WriteLine("Black's time is up! White wins by time.");
                        break;
                    }
                }

                string notation = NotationHelper.ToNotation(move);
                Console.WriteLine($"{(whitesTurn ? "White" : "Black")} plays {notation}");

                _board.ExecuteMove(move);
                whitesTurn = !whitesTurn;
            }
        }
    }
}
