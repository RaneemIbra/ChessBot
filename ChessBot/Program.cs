using ChessBot.Core.Board;
using System.Diagnostics;

namespace ChessBot
{
    internal class Program
    {
        const string DEFAULT_BOARD = "Wa2 Wb2 Wc2 Wd2 We2 Wf2 Wg2 Wh2 Ba7 Bb7 Bc7 Bd7 Be7 Bf7 Bg7 Bh7";

        static void Main(string[] args)
        {
            Console.WriteLine($"Programm started with {args.Length} arguments");
            // TODO: use arguments as shown in assignment documentation
            var setupParts = DEFAULT_BOARD.Split(" ");
            var board = new ChessBoard();
            board.Setup(setupParts);
            RunBoardCloneTest(board, 1000000);
            bool whitesTurn = true;
            while (true)
            {
                board.PrintBoard();
                IEnumerable<BoardPiece> currentPieces;
                if (whitesTurn)
                {
                    Console.WriteLine("It's White's turn.");
                    currentPieces = board.WhitePieces;
                }
                else
                {
                    Console.WriteLine("It's Black's turn.");
                    currentPieces = board.BlackPieces;
                }
                if (EndGame.IsGameOver(board, out string? message))
                {
                    Console.WriteLine(message);
                    break;
                }
                var chosenPiece = PickPiece(board, currentPieces);
                if (chosenPiece == null)
                {
                    if (EndGame.IsGameOver(board, out message))
                    {
                        Console.WriteLine(message);
                        break;
                    }
                    break;
                }
                var possibleMoves = board.GetPossibleMoves(chosenPiece).ToArray();
                var chosenMove = PickMove(possibleMoves);
                if (chosenMove != null)
                {
                    board.ExecuteMove(chosenMove);
                    if (EndGame.IsGameOver(board, out message))
                    {
                        Console.WriteLine(message);
                        break;
                    }
                    whitesTurn = !whitesTurn;
                }
            }
        }

        static Move? PickMove(Move[] moves)
        {
            if (moves.Length == 0)
            {
                Console.WriteLine("No valid moves available.");
                return null;
            }
            for (int i = 0; i < moves.Length; i++)
            {
                var move = moves[i];
                Console.WriteLine($"[{i}] {move.TargetFile}{(ushort)move.TargetRank} - {move.MoveCommand}");
            }
            while (true)
            {
                Console.Write("Pick a valid move index: ");
                if (!int.TryParse(Console.ReadLine(), out var moveIndex) ||
                    moveIndex < 0 || moveIndex >= moves.Length)
                {
                    Console.WriteLine("Invalid index. Try again.");
                    continue;
                }
                return moves[moveIndex];
            }
        }

        static BoardPiece? PickPiece(ChessBoard board, IEnumerable<BoardPiece> currentPieces)
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
                    Console.WriteLine("Invalid index. Try again.\n");
                    continue;
                }
                var chosenPiece = piecesArray[choice];
                var chosenMoves = board.GetPossibleMoves(chosenPiece).ToArray();
                if (chosenMoves.Length == 0)
                {
                    Console.WriteLine("That piece has no valid moves. Please pick another piece.\n");
                    continue;
                }
                return chosenPiece;
            }
        }

        static void RunBoardCloneTest(ChessBoard board, long numberOfClones)
        {
            Stopwatch sw = Stopwatch.StartNew();
            HashSet<ChessBoard> cloneBoards = [];
            for (int i = 1; i <= numberOfClones; i++)
            {
                var nBoard = board.Clone();
                if (i % 1000 == 0)
                {
                    sw.Stop();
                    cloneBoards.Add(nBoard);
                    Console.Clear();
                    Console.WriteLine($"Cloning board {numberOfClones} times");
                    Console.WriteLine($"{sw.ElapsedMilliseconds} ms / {i} of {numberOfClones} / {sw.ElapsedMilliseconds / (decimal)i:0.00000} ms per clone");
                    sw.Start();
                }

            }
        }
    }
}