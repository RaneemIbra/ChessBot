using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBot.Core.Board;

namespace ChessBot.Core.Agents
{
    public abstract class AgentsBase : IAgent
    {
        public ChessColor Color { get; set; }
        public abstract Move GetMove(ChessBoard board);
        protected IEnumerable<BoardPiece> GetCurrentPieces(ChessBoard board)
        {
            return Color == ChessColor.White ? board.WhitePieces : board.BlackPieces;
        }
        protected Move? PickRandomStupidMove(ChessBoard board, IEnumerable<BoardPiece> pieces)
        {
            var random = new Random();
            var allMoves = pieces.SelectMany(piece => board.GetPossibleMoves(piece)).ToList();
            return allMoves.Count == 0 ? null : allMoves[random.Next(allMoves.Count)];
        }
    }
}
