

public class Board
{
    private int[,] board;
    public int NumOfBlackPieces { get; private set; }
    public int NumOfWhitePieces { get; private set; }
    public Board()
    {
        board = new int[8, 8];
        NumOfBlackPieces = 0;
        NumOfWhitePieces = 0;
        InitBoard();
    }

    private void InitBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                board[row, col] = (int)Piece.Empty;
            }
        }
    }
}