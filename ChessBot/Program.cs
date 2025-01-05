using System;
using System.Diagnostics;

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var setupString = "Wa2 Wb2 Wc2 Wd2 We2 Wf2 Wg2 Wh2 Ba7 Bb7 Bc7 Bd7 Be7 Bf7 Bg7 Bh7";
            var setupParts = setupString.Split(" ");
            var board = new Board();
            board.Setup(setupParts);

            Stopwatch sw = Stopwatch.StartNew();
            HashSet<Board> cloneBoards = new();
            int copyAmount = 10000000;
            for (int i= 1; i <= copyAmount; i++)
            {
                var nBoard = board.Clone();
                if(i % 1000 == 0)
                {
                    sw.Stop();
                    cloneBoards.Add(nBoard);
                    Console.Clear();
                    Console.WriteLine($"Cloning board {copyAmount} times");
                    Console.WriteLine($"{sw.ElapsedMilliseconds} ms / {i} of {copyAmount} / {(sw.ElapsedMilliseconds / (decimal)i).ToString("0.00000")} ms per clone");
                    sw.Start();
                }
                
            }


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
                var cPiece = currentPieces.ElementAt(pieceIndex);
                var moves = board.GetPossibleMoves(cPiece);
                index = 0;
                foreach (var move in moves)
                {
                    Console.WriteLine($"[{index++}] {move.TargetFile}{(ushort)move.TargetRank} - {move.MoveCommand}");
                }
                var moveIndex = -1;
                while(!(moveIndex >= 0 && moveIndex < moves.Count()))
                {
                    Console.Write("Pick a valid move: ");
                    moveIndex = Console.ReadKey().KeyChar - '0';
                    Console.Write(Environment.NewLine);
                }
                var cMove = moves.ElementAt(moveIndex);
                board.ExecuteMove(cMove);
                whitesTurn = !whitesTurn;
            }

        }
    }
}