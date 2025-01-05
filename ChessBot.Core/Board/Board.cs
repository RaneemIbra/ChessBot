

using ChessBot.Core.Board;

public class Move
{
    public required BoardPiece MovingPiece { get; set; }
    public BoardPiece? DefeatedPiece { get; set; }
    public ChessRank TargetRank { get; set; }
    public ChessFile TargetFile { get; set; }
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


    public IEnumerable<Move> GetPossibleMoves(BoardPiece piece)
    {
        // TODO: Apply game logic and calculate all possible moves for the given piece

        return new List<Move>();
    }

    public Board CrateBoardFromMove(Move execute)
    {
        return this; // TODO: Create a deep copy of the board and execute the given move on it
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

    private void GameLoop()
    {
        var currentPlayer = "white";
        if(currentPlayer == "white")
        {
            foreach(var piece in WhitePieces)
            {
                var moves = GetPossibleMoves(piece);
                foreach(var move in moves)
                {
                    // Create a new leef in the tree
                    // leef -> Execute -> do again
                }
            }
        }
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