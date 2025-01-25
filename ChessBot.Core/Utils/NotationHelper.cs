using ChessBot.Core.Board;

namespace ChessBot.Core.Utils
{
    public static class NotationHelper
    {
        public static string ToNotation(Move move)
        {
            return $"{ConvertFile(move.MovingPiece.File)}{(ushort)move.MovingPiece.Rank}"
                + $"{ConvertFile(move.TargetFile)}{(ushort)move.TargetRank}";
        }

        public static Move? FromNotation(string notation, ChessBoard board, ChessColor color)
        {
            if (string.IsNullOrWhiteSpace(notation) || notation.Length < 4)
                return null;

            string sourceFileStr  = notation[0].ToString().ToLower();
            string sourceRankStr  = notation[1].ToString();
            string targetFileStr  = notation[2].ToString().ToLower();
            string targetRankStr  = notation[3].ToString();

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

        private static bool TryConvertRank(string rankStr, out ChessRank rank)
        {
            rank = ChessRank.One;
            if (!int.TryParse(rankStr, out int rankInt)) return false;
            if (rankInt < 1 || rankInt > 8) return false;
            rank = (ChessRank)rankInt;
            return true;
        }

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
