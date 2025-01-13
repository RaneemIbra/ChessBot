using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBot.Core.Board;

namespace ChessBot.Core.Agents
{
    public class SimpleAgent : AgentsBase
    {
        public override Move GetMove(ChessBoard board, bool IsWhiteTurn)
        {
            var pieces = GetCurrentPieces(board, IsWhiteTurn);
            return PickRandomStupidMove(board, pieces) ?? throw new InvalidOperationException("No valid moves");
        }
    }
}
