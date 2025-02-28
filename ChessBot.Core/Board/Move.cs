namespace ChessBot.Core.Board
{
    public class Move
    {
        /// <summary>
        /// Gets or sets the piece being moved.
        /// </summary>
        public required BoardPiece MovingPiece { get; set; }

        /// <summary>
        /// Gets or sets the captured piece (if any) during this move.
        /// </summary>
        public BoardPiece? CapturedPiece { get; set; }

        /// <summary>
        /// Gets or sets the target rank (row) to which the piece is moved.
        /// </summary>
        public ChessRank TargetRank { get; set; }

        /// <summary>
        /// Gets or sets the target file (column) to which the piece is moved.
        /// </summary>
        public ChessFile TargetFile { get; set; }

        /// <summary>
        /// Generates the target command string representing the move (e.g., "a2").
        /// </summary>
        public string TargetCommand => $"{TargetFile}{(ushort)TargetRank}".ToLower();

        /// <summary>
        /// Generates the full move command (e.g., "e2e4").
        /// </summary>
        public string MoveCommand => $"{MovingPiece.File}{(ushort)MovingPiece.Rank}{TargetCommand}".ToLower();
    }
}
