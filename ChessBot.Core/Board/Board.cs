

using System.IO.Pipelines;
using ChessBot.Core.Board;

public class Move
{
    public required BoardPiece MovingPiece { get; set; }
    public BoardPiece? CapturedPiece { get; set; }
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

    public IEnumerable<Move> SingleMoves(BoardPiece piece)
    {
        // TODO: Apply game logic and calculate all possible moves for the given piece
        ushort bRank = RankToIndex(piece.Rank);
        ushort bFile = FileToIndex(piece.File);
        if (piece.ChessPiece == ChessPiece.White)
        {
            if(bRank + 1 < 8)
            {
                if (_board[bRank + 1, bFile] == ChessPiece.Empty)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File };
                }
            }
        }
        else if (piece.ChessPiece == ChessPiece.Black)
        {
            if(bRank - 1 >= 0)
            {
                if (_board[bRank - 1, bFile] == ChessPiece.Empty)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File };
                }
            }
        }
    }

    public IEnumerable<Move> DoubleMove(BoardPiece piece)
    {
        if (piece.ChessPiece == ChessPiece.White && piece.Rank == ChessRank.Two)
        {
            // Check if the piece can move two steps forward
            if (_board[RankToIndex(piece.Rank) + 1, FileToIndex(piece.File)] == ChessPiece.Empty && _board[RankToIndex(piece.Rank) + 2, FileToIndex(piece.File)] == ChessPiece.Empty)
            {
                yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 2, TargetFile = piece.File };
            }
        }
        else if (piece.ChessPiece == ChessPiece.Black && piece.Rank == ChessRank.Seven)
        {
            // Check if the piece can move two steps forward
            if (_board[RankToIndex(piece.Rank) - 1, FileToIndex(piece.File)] == ChessPiece.Empty && _board[RankToIndex(piece.Rank) - 2, FileToIndex(piece.File)] == ChessPiece.Empty)
            {
                yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 2, TargetFile = piece.File };
            }
        }
    }
    public IEnumerable<Move> CapturingMove(BoardPiece piece)
    {
        ushort bRank = RankToIndex(piece.Rank);
        ushort bFile = FileToIndex(piece.File);
        if (piece.ChessPiece == ChessPiece.White)
        {
            if(bRank + 1 < 8)
            {
                if(bFile + 1 < 8 && _board[bRank + 1, bFile + 1] == ChessPiece.Black)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File + 1, CapturedPiece = GetPiece(piece.Rank + 1, piece.File + 1) };
                }
                if (bFile - 1 >= 0 && _board[bRank + 1, bFile - 1] == ChessPiece.Black)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File - 1, CapturedPiece = GetPiece(piece.Rank + 1, piece.File - 1) };
                }
            }
            // Check if the piece can move two steps forward
        }
        else if (piece.ChessPiece == ChessPiece.Black)
        {
            if (bRank - 1 >= 0)
            {
                if (bFile + 1 < 8 && _board[bRank - 1, bFile + 1] == ChessPiece.White)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File + 1, CapturedPiece = GetPiece(piece.Rank - 1, piece.File + 1) };
                }
                if (bFile - 1 >= 0 && _board[bRank - 1, bFile - 1] == ChessPiece.White)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File - 1, CapturedPiece = GetPiece(piece.Rank - 1, piece.File - 1) };
                }
            }
        }
    }

    public IEnumerable<Move> enPassent(BoardPiece piece)
    {
        ushort bRank = RankToIndex(piece.Rank);
        ushort bFile = FileToIndex(piece.File);
        if(piece.ChessPiece == ChessPiece.White)
        {
            if (bRank == 4)
            {
                if (bFile + 1 < 8 && _board[bRank, bFile + 1] == ChessPiece.Black)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File + 1, CapturedPiece = GetPiece(piece.Rank, piece.File + 1) };
                }
                if (bFile - 1 >= 0 && _board[bRank, bFile - 1] == ChessPiece.Black)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File - 1, CapturedPiece = GetPiece(piece.Rank, piece.File - 1) };
                }
            }
        }
        else if (piece.ChessPiece == ChessPiece.Black)
        {
            if (bRank == 3)
            {
                if (bFile + 1 < 8 && _board[bRank, bFile + 1] == ChessPiece.White)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File + 1, CapturedPiece = GetPiece(piece.Rank, piece.File + 1) };
                }
                if (bFile - 1 >= 0 && _board[bRank, bFile - 1] == ChessPiece.White)
                {
                    yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File - 1, CapturedPiece = GetPiece(piece.Rank, piece.File - 1) };
                }
            }
        }
    }

    public BoardPiece GetPiece(ChessRank rank, ChessFile file)
    {
        return _pieces.Single(a => a.Rank == rank && a.File == file);
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
                //var moves = GetPossibleMoves(piece);
                //foreach(var move in moves)
                //{
                //    // Create a new leef in the tree
                //    // leef -> Execute -> do again
                //}
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