using ChessBot.Core.Board;

public class Board
{
    #region Members
    private ChessPiece[,] _board;
    private List<BoardPiece> _pieces = new List<BoardPiece>();
    #endregion

    #region Properties
    public IEnumerable<BoardPiece> BlackPieces => _pieces.Where(w => w.ChessPiece == ChessPiece.Black);
    public IEnumerable<BoardPiece> WhitePieces => _pieces.Where(w => w.ChessPiece == ChessPiece.White);
    public Move? LastMove { get; private set; }

    public int NumOfBlackPieces => BlackPieces.Count();
    public int NumOfWhitePieces => WhitePieces.Count();
    #endregion

    #region Constructor / Copy
    public Board()
    {
        _board = new ChessPiece[8, 8];
    }
    public Board Clone()
    {
        var copy = new Board();
        foreach (var bPiece in BlackPieces)
        {
            copy.InitPiece(bPiece.Rank, bPiece.File, bPiece.ChessPiece, true);
        }
        foreach (var wPiece in WhitePieces)
        {
            copy.InitPiece(wPiece.Rank, wPiece.File, wPiece.ChessPiece, true);
        }
        return copy;
    }
    #endregion

    #region Public interface

    public ChessPiece this[int rank, int file]
    {
        get
        {
            if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
            {
                return _board[rank, file];
            }
            return ChessPiece.Invalid;
        }
        set
        {
            if (rank >= 0 && rank < 8 && file >= 0 && file < 8)
            {
                _board[rank, file] = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
    public ChessPiece this[ChessRank rank, ChessFile file]
    {
        get
        {
            return this[rank.ToIndex(), file.ToIndex()];
        }
        set
        {
            this[rank.ToIndex(), file.ToIndex()] = value;
        }
    }
    public IEnumerable<Move> GetPossibleMoves(BoardPiece boardPiece)
    {
        List<Move> possibleMoves = [.. SingleMoves(boardPiece)];
        if (possibleMoves.Count > 0)
        {
            possibleMoves.AddRange(DoubleMove(boardPiece));
        }
        possibleMoves.AddRange(CapturingMove(boardPiece));
        possibleMoves.AddRange(EnPassent(boardPiece));
        return possibleMoves;
    }

    public BoardPiece? GetPieceAt(ChessRank rank, ChessFile file)
    {
        return _pieces.SingleOrDefault(a => a.Rank == rank && a.File == file);
    }

    public void Setup(IEnumerable<string> setupParts)
    {
        foreach (var part in setupParts)
        {
            ChessPiece piece = ChessPiece.White;
            var cPart = part.ToLower();
            if (cPart.Length != 3)
            {
                throw new InvalidDataException($"Setup part '{part}' is malformed");
            }
            if (cPart[0] == 'w' || cPart[0] == 'b')
            {
                if (cPart[0] == 'b')
                {
                    piece = ChessPiece.Black;
                }
            }
            else
            {
                throw new InvalidDataException($"Setup part '{part}' is malformed");
            }
            var filePart = cPart[1];
            if (filePart < 'a' || filePart > 'h')
            {
                throw new InvalidDataException($"Setup part '{part}' is malformed");
            }
            ChessFile file = (ChessFile)(filePart - 'a');
            var rankPart = cPart[2];
            if (rankPart < '1' || rankPart > '8')
            {
                throw new InvalidDataException($"Setup part '{part}' is malformed");
            }
            ChessRank rank = (ChessRank)(rankPart - '1' + 1);
            InitPiece(rank, file, piece);
        }
    }

    public void ExecuteMove(Move move)
    {
        // TODO: impelment
        LastMove = move;
        Console.WriteLine("Sorry, I don't know how to execute moves :(");
    }
    public void PrintBoard()
    {
        Console.WriteLine("   | A | B | C | D | E | F | G | H |");
        Console.WriteLine("   |-------------------------------|");
        foreach (ChessRank rank in Enum.GetValues<ChessRank>().Reverse())
        {
            Console.Write($" {(ushort)rank} |");
            foreach (ChessFile file in Enum.GetValues<ChessFile>())
            {
                var chessPiece = this[rank, file];
                switch (chessPiece)
                {
                    case ChessPiece.Empty:
                        Console.Write("   |");
                        break;
                    case ChessPiece.White:
                        Console.Write(" W |");
                        break;
                    case ChessPiece.Black:
                        Console.Write(" B |");
                        break;
                }

            }
            Console.Write($" {(ushort)rank} {Environment.NewLine}");
        }
        Console.WriteLine("   |-------------------------------|");
        Console.WriteLine("   | A | B | C | D | E | F | G | H |");
    }
    public void ClearBoard()
    {
        foreach (ChessRank rank in Enum.GetValues(typeof(ChessRank)))
        {
            foreach (ChessFile file in Enum.GetValues(typeof(ChessFile)))
            {
                InitPiece(rank, file, ChessPiece.Empty);
            }
        }
    }
    #endregion

    #region Private Helpers
    internal void InitPiece(ChessRank rank, ChessFile file, ChessPiece piece, bool unsafeInit = false)
    {
        if (!unsafeInit)
        {
            var existing = _pieces.FirstOrDefault(a => a.Rank == rank && a.File == file);
            // Check if the piece is valid
            if (existing != null)
            {
                // ... we could also throw an exception here, init shoult not be called with nonsense
                _pieces.Remove(existing);
            }
        }
        if (piece != ChessPiece.Empty)
        {
            var boardPiece = new BoardPiece { File = file, Rank = rank, ChessPiece = piece };
            _pieces.Add(boardPiece);
        }
        this[rank, file] = piece;
    }
    #endregion

    #region Moves
    private IEnumerable<Move> SingleMoves(BoardPiece piece)
    {
        if (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black)
        {
            yield break;
        }

        int direction = piece.ChessPiece == ChessPiece.White ? 1 : -1;
        ushort targetRankIndex = (ushort)(piece.Rank.ToIndex() + direction);
        ChessRank targetRank = (ChessRank)(targetRankIndex + 1);
        ushort currentFile = piece.File.ToIndex();

        if (this[targetRankIndex, currentFile] == ChessPiece.Empty)
        {
            yield return new Move
            {
                MovingPiece = piece,
                TargetRank = targetRank,
                TargetFile = piece.File
            };
        }
    }

    private IEnumerable<Move> DoubleMove(BoardPiece piece)
    {
        if (piece.ChessPiece != ChessPiece.White && piece.ChessPiece != ChessPiece.Black)
        {
            yield break;
        }

        int direction = piece.ChessPiece == ChessPiece.White ? 2 : -2;
        ushort targetRankIndex = (ushort)(piece.Rank.ToIndex() + direction);
        ChessRank targetRank = (ChessRank)(targetRankIndex + 1);
        ushort currentFile = piece.File.ToIndex();

        if (this[targetRankIndex, currentFile] == ChessPiece.Empty)
        {
            yield return new Move
            {
                MovingPiece = piece,
                TargetRank = targetRank,
                TargetFile = piece.File
            };
        }
    }
    private IEnumerable<Move> CapturingMove(BoardPiece piece)
    {
        ushort bRank = piece.Rank.ToIndex();
        ushort bFile = piece.File.ToIndex();
        if (piece.ChessPiece == ChessPiece.White)
        {
            if (this[bRank + 1, bFile + 1] == ChessPiece.Black)
            {
                yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File + 1, CapturedPiece = GetPieceAt(piece.Rank + 1, piece.File + 1) };
            }
            if (this[bRank + 1, bFile - 1] == ChessPiece.Black)
            {
                yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File - 1, CapturedPiece = GetPieceAt(piece.Rank + 1, piece.File - 1) };
            }

        }
        else if (piece.ChessPiece == ChessPiece.Black)
        {
            if (this[bRank - 1, bFile + 1] == ChessPiece.White)
            {
                yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File + 1, CapturedPiece = GetPieceAt(piece.Rank - 1, piece.File + 1) };
            }
            if (this[bRank - 1, bFile - 1] == ChessPiece.White)
            {
                yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File - 1, CapturedPiece = GetPieceAt(piece.Rank - 1, piece.File - 1) };
            }
        }
    }
    private IEnumerable<Move> EnPassent(BoardPiece piece)
    {
        if(LastMove == null)
        {
            yield break;
        }
        ushort bRank = piece.Rank.ToIndex();
        ushort bFile = piece.File.ToIndex();
        if (piece.ChessPiece == ChessPiece.White)
        {
            if (piece.Rank == ChessRank.Five)
            {
                if (this[bRank, bFile + 1] == ChessPiece.Black)
                {
                    if (LastMove.TargetRank == piece.Rank && LastMove.TargetFile == piece.File + 1)
                    {
                        yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File + 1, CapturedPiece = GetPieceAt(piece.Rank, piece.File + 1) };
                    }
                }
                if (this[bRank, bFile - 1] == ChessPiece.Black)
                {
                    if (LastMove.TargetRank == piece.Rank && LastMove.TargetFile == piece.File - 1)
                    {
                        yield return new Move { MovingPiece = piece, TargetRank = piece.Rank + 1, TargetFile = piece.File - 1, CapturedPiece = GetPieceAt(piece.Rank, piece.File - 1) };
                    }
                }
            }
        }
        else if (piece.ChessPiece == ChessPiece.Black)
        {
            if (piece.Rank == ChessRank.Four)
            {
                if (this[bRank, bFile + 1] == ChessPiece.White)
                {
                    if (LastMove.TargetRank == piece.Rank && LastMove.TargetFile == piece.File + 1)
                    {
                        yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File + 1, CapturedPiece = GetPieceAt(piece.Rank, piece.File + 1) };
                    }
                }
                if (this[bRank, bFile - 1] == ChessPiece.White)
                {
                    if (LastMove.TargetRank == piece.Rank && LastMove.TargetFile == piece.File - 1)
                    {
                        yield return new Move { MovingPiece = piece, TargetRank = piece.Rank - 1, TargetFile = piece.File - 1, CapturedPiece = GetPieceAt(piece.Rank, piece.File - 1) };
                    }
                }
            }
        }
    }
    #endregion

}