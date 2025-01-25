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
            SendResponse("OK");
            
            while (true)
            {
                string serverMessage = ReadMessage();
                if (string.IsNullOrEmpty(serverMessage)) continue;

                Console.WriteLine($"Server: {serverMessage}");

                if (serverMessage.StartsWith("Setup"))
                {
                    HandleSetupCommand(serverMessage);
                    SendResponse("OK");
                }
                else if (serverMessage.StartsWith("Time"))
                {
                    SendResponse("OK");
                }
                else if (serverMessage.StartsWith("Begin"))
                {
                    _assignedColor = ChessColor.White;
                    MakeFirstMove();
                }
                else if (serverMessage.StartsWith("Black"))
                {
                    _assignedColor = ChessColor.Black;
                }
                else if (serverMessage.StartsWith("White"))
                {
                    _assignedColor = ChessColor.White;
                }
                else if (IsMoveCommand(serverMessage))
                {
                    HandleOpponentMove(serverMessage);
                    MakeAgentMove();
                }
                else if (serverMessage == "exit")
                {
                    break;
                }
            }

            _stream.Close();
            _client.Close();
        }

        private void HandleSetupCommand(string setupCommand)
        {
            string[] parts = setupCommand.Split(' ');
            _board.ClearBoard();
            _board.Setup(parts[1..]);
        }

        private void MakeFirstMove()
        {
            if (_assignedColor == ChessColor.White)
            {
                MakeAgentMove();
            }
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
            var move = NotationHelper.FromNotation(moveNotation, _board, 
                _assignedColor == ChessColor.White ? ChessColor.Black : ChessColor.White);
            if (move != null)
            {
                _board.ExecuteMove(move);
            }
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
    }
}