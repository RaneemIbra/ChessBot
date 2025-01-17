﻿using ChessBot.Core.Board;
using ChessBot.Core.Agents;
using System.Diagnostics;

namespace ChessBot
{
    internal class Program
    {
        const string DEFAULT_BOARD = "Wa2 Wb2 Wc2 Wd2 We2 Wf2 Wg2 Wh2 Ba7 Bb7 Bc7 Bd7 Be7 Bf7 Bg7 Bh7";

        static void Main(string[] args)
        {
            var setupParts = DEFAULT_BOARD.Split(" ");
            var board = new ChessBoard();
            board.Setup(setupParts);
            Console.WriteLine("Choose mode: 1 for Player vs Player, 2 for Player vs Agent, 3 for Agent vs Agent");
            int mode = GetChoice(new[] { 1, 2, 3});
            IAgent? WhiteAgent = null;
            IAgent? BlackAgent = null;
            if(mode == 2){
                Console.WriteLine("Who plays as white? 1 for Player, 2 for Agent");
                int choice = GetChoice(new[] { 1, 2 });
                if(choice == 2){
                    WhiteAgent = new SimpleAgent();
                }
                else{
                    BlackAgent = new SimpleAgent();
                }
            }
            else if(mode == 3){
                WhiteAgent = new SimpleAgent();
                BlackAgent = new SimpleAgent();
            }

            bool whitesTurn = true;
            while (true)
            {
                board.PrintBoard();

                if (EndGame.IsGameOver(board, out string? message))
                {
                    Console.WriteLine(message);
                    break;
                }

                if (whitesTurn)
                {
                    if(WhiteAgent != null){
                        Console.WriteLine("White's Agent turn");
                        var move = WhiteAgent.GetMove(board, true);
                        board.ExecuteMove(move);
                    }
                    else{
                        ExecutePlayerTurn(board, true);
                    }
                }
                else{
                    if(BlackAgent != null){
                        Console.WriteLine("Black's Agent turn");
                        var move = BlackAgent.GetMove(board, false);
                        board.ExecuteMove(move);
                    }
                    else{
                        ExecutePlayerTurn(board, false);
                    }
                }
                whitesTurn = !whitesTurn;
            }
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

        static void ExecutePlayerTurn(ChessBoard board, bool isWhiteTurn){
            var currentPieces = isWhiteTurn ? board.WhitePieces : board.BlackPieces;
            var chosenPiece = PickPiece(board, currentPieces);

            if (chosenPiece == null)
            {
                Console.WriteLine("No valid moves available. Game over!");
                return;
            }

            var possibleMoves = board.GetPossibleMoves(chosenPiece).ToArray();
            var chosenMove = PickMove(possibleMoves);

            if (chosenMove != null)
            {
                board.ExecuteMove(chosenMove);
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
    }
}
