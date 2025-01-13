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
        public abstract Move GetMove(ChessBoard board, bool IsWhiteTurn);
        protected IEnumerable<BoardPiece> GetCurrentPieces(ChessBoard board, bool IsWhiteTurn)
        {
            return IsWhiteTurn ? board.WhitePieces : board.BlackPieces;
        }
        protected Move? PickRandomStupidMove(ChessBoard board, IEnumerable<BoardPiece> pieces)
        {
            var random = new Random();
            var AllMoves = pieces.SelectMany(piece => board.GetPossibleMoves(piece)).ToList();
            if (AllMoves.Count() == 0)
            {
                return null;
            }
            return AllMoves.ElementAt(random.Next(AllMoves.Count()));
        }
}
