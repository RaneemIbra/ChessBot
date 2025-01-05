

using ChessBot.Core.Board;

public class BoardPiece
{
    public ChessRank Rank { get; set; }
    public ChessFile File { get; set; }
    public ChessPiece ChessPiece { get; set; }
}

public class Board
{
    private ChessPiece[,] _board;

    public IEnumerable<BoardPiece> BlackPieces => _pieces.Where(w => w.ChessPiece == ChessPiece.Black);
    public IEnumerable<BoardPiece> WhitePieces => _pieces.Where(w => w.ChessPiece == ChessPiece.White);

    public int NumOfBlackPieces => BlackPieces.Count();
    public int NumOfWhitePieces => WhitePieces.Count();

    private List<BoardPiece> _pieces = new List<BoardPiece>();

    public Board()
    {
        _board = new ChessPiece[8, 8];
        InitEmptyBoard();
    }

    private ushort FileToIndex(ChessFile file)
    {
        return (ushort)file;
    }

    private ushort RankToIndex(ChessRank rank) 
    { 
        return (ushort)(rank - 1); 
    }

    private void InitPiece(ChessRank rank, ChessFile file, ChessPiece piece)
    {
        ushort bRank = RankToIndex(rank);
        ushort bFile = FileToIndex(file);
        var existing = _pieces.FirstOrDefault(a => a.Rank == rank && a.File == file);
        // Check if the piece is valid
        if (existing != null)
        {
            // ... we could also throw an exception here, init shoult not be called with nonsense
            _pieces.Remove(existing);
        }
        if (piece != ChessPiece.Empty)
        {
            var boardPiece = new BoardPiece { File = file, Rank = rank, ChessPiece = piece };
            _pieces.Add(boardPiece);
        }
        _board[RankToIndex(rank), FileToIndex(file)] = piece;
    }

    private void InitEmptyBoard()
    {
        foreach(ChessRank rank in Enum.GetValues(typeof(ChessRank)))
        {
            foreach(ChessFile file in Enum.GetValues(typeof(ChessFile)))
            {
                InitPiece(rank, file, ChessPiece.Empty);
            }
        }
    }
}