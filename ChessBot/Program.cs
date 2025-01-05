using System;

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
            bool whitesTurn = true;
            while(true)
            {
                board.Print();
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
                    Console.WriteLine($"[{index++}] {move.TargetFile}{(ushort)move.TargetRank}");
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