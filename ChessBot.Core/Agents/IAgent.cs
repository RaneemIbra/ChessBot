using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBot.Core.Board;

namespace ChessBot.Core.Agents
{
    public interface IAgent
    {
        ChessColor Color { get; set;}
        Move GetMove(ChessBoard board);
    }
}
