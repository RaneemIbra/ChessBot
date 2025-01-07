﻿using ChessBot.Core.Board;

public static class BoardManipulation
{
    public static BoardPiece MovePiece(Board board, BoardPiece piece, ChessRank targetRank, ChessFile targetFile)
    {
        RemovePiece(board, piece);
        var newPiece = new BoardPiece
        {
            ChessPiece = piece.ChessPiece,
            Rank = targetRank,
            File = targetFile
        };
        board.AddPiece(newPiece);
        return newPiece;
    }

    public static void RemovePiece(Board board, BoardPiece piece)
    {
        board.RemovePiece(piece);
    }
}
