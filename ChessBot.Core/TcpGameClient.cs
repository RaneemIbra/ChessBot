using System;
using System.Net.Sockets;
using System.Text;
using ChessBot.Core.Board;
using ChessBot.Core.Agents;
using ChessBot.Core.Utils;

namespace ChessBot.Core
{
    public class TcpGameClient
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly ChessBoard _board;
        private readonly IAgent _agent;
        private ChessColor _assignedColor;
        public TcpGameClient(string ip, int port, IAgent agent)
        {
            _client = new TcpClient(ip, port);
            _stream = _client.GetStream();
            _board = new ChessBoard();
            _agent = agent;
        }

        public void Start()
        {
            Console.WriteLine("Connected to server!");
            while (true)
            {
                string serverMessage = ReadMessage();
                if (string.IsNullOrEmpty(serverMessage)) continue;

                Console.WriteLine($"Server: {serverMessage}");

                if (serverMessage.StartsWith("ASSIGN"))
                {
                    _assignedColor = serverMessage.Contains("White") 
                        ? ChessColor.White 
                        : ChessColor.Black;
                    _agent.Color = _assignedColor;
                    SendResponse("OK");
                    continue;
                }
                else if (serverMessage.StartsWith("TIME"))
                {
                    Console.WriteLine("Time here");
                    SendResponse("OK");
                    continue;
                }
                else if(serverMessage.StartsWith("Setup")){
                    ParseSetupCommand(serverMessage.Trim(),_board);
                    Console.WriteLine("Setup Complete");
                    SendResponse("OK");
                    continue;
                }
                else if(serverMessage.StartsWith("BEGIN")){
                    Console.WriteLine("Game began");
                    SendResponse("OK");
                }
                else if(serverMessage.Equals("move")){
                    Console.WriteLine(serverMessage);
                    MakeAgentMove();
                }
                else if (serverMessage == "EXIT")
                {
                    Console.WriteLine("Exit!");
                    break;
                }
                else if(serverMessage.StartsWith("MOVE")){
                    Console.WriteLine(serverMessage);
                    string[] parts = serverMessage.Substring(5).Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    Console.WriteLine("parts: " + parts[0]);
                    HandleOpponentMove(parts[0]);
                    _board.PrintBoard();
                }
                else{
                    Console.WriteLine("Hello from the other side");
                }
            }

            _stream.Close();
            _client.Close();
        }

        private void MakeAgentMove()
        {
            var move = _agent.GetMove(_board);
            string moveNotation = NotationHelper.ToNotation(move);
            SendResponse(moveNotation);
            _board.ExecuteMove(move);
        }

        private void HandleOpponentMove(string moveNotation)
        {
            ChessColor opponentColor = _assignedColor == ChessColor.White 
                ? ChessColor.Black 
                : ChessColor.White;
            var move = NotationHelper.FromNotation(moveNotation, _board, opponentColor);
            if(move != null){
                _board.ExecuteMove(move);
            }
            SendResponse("OK");
        }

        private string ReadMessage()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = _stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
        }

        private void SendResponse(string response)
        {
            byte[] data = Encoding.ASCII.GetBytes(response);
            _stream.Write(data, 0, data.Length);
        }

        private static bool IsMoveCommand(string input)
        {
            return input.Length == 4 &&
                char.IsLetter(input[0]) &&
                char.IsDigit(input[1]) &&
                char.IsLetter(input[2]) &&
                char.IsDigit(input[3]);
        }

        private bool ParseSetupCommand(string? setupCommand, ChessBoard board)
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