using ChessBot.Core.Board;

public class Move
{
    public required BoardPiece MovingPiece { get; set; }
    public BoardPiece? CapturedPiece { get; set; }
    public ChessRank TargetRank { get; set; }
    public ChessFile TargetFile { get; set; }
    public string TargetCommand => $"{TargetFile}{(ushort)TargetRank}".ToLower();
    public string MoveCommand => $"{MovingPiece.File}{(ushort)MovingPiece.Rank}{TargetCommand}".ToLower();
}
