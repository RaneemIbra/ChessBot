using System;
using System.Diagnostics;
using System.Reflection;

namespace MyApp
{
    internal class Program
    {
        const string DEFAULT_BOARD = "Wa2 Wb2 Wc2 Wd2 We2 Wf2 Wg2 Wh2 Ba7 Bb7 Bc7 Bd7 Be7 Bf7 Bg7 Bh7";

        static void Main(string[] args)
        {
            var setupParts = DEFAULT_BOARD.Split(" ");
            var board = new Board();
            board.Setup(setupParts);
            RunBoardCloneTest(board, 1000000);
            bool whitesTurn = true;
            while(true)
            {
                board.PrintBoard();
                IEnumerable<BoardPiece> currentPieces = board.WhitePieces;
                if (whitesTurn)
                {
                    Console.WriteLine("It's whites turn. Possible pieces:");
                    currentPieces = board.WhitePieces;
                }
                else
                {
                    Console.WriteLine("It's blacks turn. Possible pieces:");
                    currentPieces = board.BlackPieces;
                }
                var cPiece = PickPiece(currentPieces);
                var cMove = PickMove(board.GetPossibleMoves(cPiece));
                board.ExecuteMove(cMove);
                whitesTurn = !whitesTurn;
            }

        }

        static Move PickMove(IEnumerable<Move> moves)
        {
            int index = 0;
            foreach (var move in moves)
            {
                Console.WriteLine($"[{index++}] {move.TargetFile}{(ushort)move.TargetRank} - {move.MoveCommand}");
            }
            var moveIndex = -1;
            while (!(moveIndex >= 0 && moveIndex < moves.Count()))
            {
                Console.Write("Pick a valid move: ");
                moveIndex = Console.ReadKey().KeyChar - '0';
                Console.Write(Environment.NewLine);
            }
            return moves.ElementAt(moveIndex);
        }

        static BoardPiece PickPiece(IEnumerable<BoardPiece> currentPieces)
        {
            int index = 0;
            foreach (var piece in currentPieces)
            {
                Console.WriteLine($"[{index++}] {piece.File}{(ushort)piece.Rank}");
            }
            var pieceIndex = -1;
            while (!(pieceIndex >= 0 && pieceIndex < currentPieces.Count()))
            {
                Console.Write("Pick a valid piece: ");
                pieceIndex = Console.ReadKey().KeyChar - '0';
                Console.Write(Environment.NewLine);
            }
            return currentPieces.ElementAt(pieceIndex);
        }

        static void RunBoardCloneTest(Board board, long numberOfClones)
        {
            Stopwatch sw = Stopwatch.StartNew();
            HashSet<Board> cloneBoards = new();
            for (int i = 1; i <= numberOfClones; i++)
            {
                var nBoard = board.Clone();
                if (i % 1000 == 0)
                {
                    sw.Stop();
                    cloneBoards.Add(nBoard);
                    Console.Clear();
                    Console.WriteLine($"Cloning board {numberOfClones} times");
                    Console.WriteLine($"{sw.ElapsedMilliseconds} ms / {i} of {numberOfClones} / {(sw.ElapsedMilliseconds / (decimal)i).ToString("0.00000")} ms per clone");
                    sw.Start();
                }

            }
        }
    }
}