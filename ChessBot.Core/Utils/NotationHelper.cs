using ChessBot.Core.Board;

namespace ChessBot.Core.Utils
{
    /// <summary>
    /// A static helper class that provides methods for converting chess moves to and from standard algebraic notation.
    /// </summary>
    public static class NotationHelper
    {
        /// <summary>
        /// Converts a move to algebraic notation.
        /// </summary>
        /// <param name="move">The move to convert.</param>
        /// <returns>A string representing the move in algebraic notation (e.g., "e2e4").</returns>
        public static string ToNotation(Move move)
        {
            return $"{ConvertFile(move.MovingPiece.File)}{(ushort)move.MovingPiece.Rank}"
                + $"{ConvertFile(move.TargetFile)}{(ushort)move.TargetRank}";
        }

        /// <summary>
        /// Converts a move from algebraic notation to a <see cref="Move"/> object.
        /// </summary>
        /// <param name="notation">The algebraic notation of the move (e.g., "e2e4").</param>
        /// <param name="board">The current state of the chessboard.</param>
        /// <param name="color">The color of the player making the move.</param>
        /// <returns>A <see cref="Move"/> object representing the move, or null if the notation is invalid.</returns>
        public static Move? FromNotation(string notation, ChessBoard board, ChessColor color)
        {
            if (string.IsNullOrWhiteSpace(notation) || notation.Length < 4)
                return null;

            string sourceFileStr = notation[0].ToString().ToLower();
            string sourceRankStr = notation[1].ToString();
            string targetFileStr = notation[2].ToString().ToLower();
            string targetRankStr = notation[3].ToString();

            if (!TryConvertFile(sourceFileStr, out ChessFile sourceFile) ||
                !TryConvertFile(targetFileStr, out ChessFile targetFile))
            {
                return null;
            }
            if (!TryConvertRank(sourceRankStr, out ChessRank sourceRank) ||
                !TryConvertRank(targetRankStr, out ChessRank targetRank))
            {
                return null;
            }

            var pieceAtSource = board.GetPieceAt(sourceRank, sourceFile);
            if (pieceAtSource == null) return null;

            bool colorMismatch = (color == ChessColor.White && pieceAtSource.ChessPiece != ChessPiece.White)
                            || (color == ChessColor.Black && pieceAtSource.ChessPiece != ChessPiece.Black);
            if (colorMismatch) return null;

            var possibleMoves = board.GetPossibleMoves(pieceAtSource);
            var chosenMove = possibleMoves.FirstOrDefault(m =>
                m.TargetRank == targetRank && m.TargetFile == targetFile);

            return chosenMove;
        }

        /// <summary>
        /// Tries to convert a file (column) in chess notation (e.g., "a", "b", etc.) to a <see cref="ChessFile"/> enum.
        /// </summary>
        /// <param name="fileStr">The string representing the file (column) in chess notation.</param>
        /// <param name="file">The resulting <see cref="ChessFile"/> value.</param>
        /// <returns>True if the conversion was successful, false otherwise.</returns>
        private static bool TryConvertFile(string fileStr, out ChessFile file)
        {
            file = ChessFile.A;
            switch (fileStr)
            {
                case "a": file = ChessFile.A; break;
                case "b": file = ChessFile.B; break;
                case "c": file = ChessFile.C; break;
                case "d": file = ChessFile.D; break;
                case "e": file = ChessFile.E; break;
                case "f": file = ChessFile.F; break;
                case "g": file = ChessFile.G; break;
                case "h": file = ChessFile.H; break;
                default: return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to convert a rank (row) in chess notation (e.g., "1", "2", etc.) to a <see cref="ChessRank"/> enum.
        /// </summary>
        /// <param name="rankStr">The string representing the rank (row) in chess notation.</param>
        /// <param name="rank">The resulting <see cref="ChessRank"/> value.</param>
        /// <returns>True if the conversion was successful, false otherwise.</returns>
        private static bool TryConvertRank(string rankStr, out ChessRank rank)
        {
            rank = ChessRank.One;
            if (!int.TryParse(rankStr, out int rankInt)) return false;
            if (rankInt < 1 || rankInt > 8) return false;
            rank = (ChessRank)rankInt;
            return true;
        }

        /// <summary>
        /// Converts a <see cref="ChessFile"/> enum to its corresponding string representation in chess notation.
        /// </summary>
        /// <param name="file">The <see cref="ChessFile"/> value.</param>
        /// <returns>The string representing the file (column) in chess notation (e.g., "a", "b", etc.).</returns>
        private static string ConvertFile(ChessFile file)
        {
            switch (file)
            {
                case ChessFile.A: return "a";
                case ChessFile.B: return "b";
                case ChessFile.C: return "c";
                case ChessFile.D: return "d";
                case ChessFile.E: return "e";
                case ChessFile.F: return "f";
                case ChessFile.G: return "g";
                case ChessFile.H: return "h";
            }
            return "?";
        }
    }
}
