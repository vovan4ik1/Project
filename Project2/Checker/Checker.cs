using System.Collections.Generic;
using Project2.Player;

namespace Project2.Checker
{
    public class Checker
    {
        public SquarePosition Position { get; set; }
        public List<Move> PossibleMoves { get; private set; }
        public EPlayerColor Color { get; set; }
        public ECheckerType Type { get; set; }

        public Checker(EPlayerColor color, SquarePosition position, ECheckerType type = ECheckerType.Soldier)
        {
            Type = type;
            Color = color;
            Position = position;
            PossibleMoves = new List<Move>(4);
        }

        public EPlayerColor OppositeColor()
        {
            return (EPlayerColor)((int)Color * -1);
        }

        public void InsertPossibleCorrectMovesToList(Checker[,] board)
        {
            PossibleMoves.Clear();
            if (Type == ECheckerType.Soldier)
            {
                InsertSoldierMoves(board);
            }
            else if (Type == ECheckerType.King)
            {
                InsertKingMoves(board);
            }
        }

        public void InsertSoldierMoves(Checker[,] board)
        {
            if (!InsertEatingMoves(board, 1))
            {
                InsertRegularMoves(board, 1);
            }
        }

        public void InsertKingMoves(Checker[,] board)
        {
            if (!InsertEatingMoves(board, 1) && !InsertEatingMoves(board, -1))
            {
                InsertRegularMoves(board, 1);
                InsertRegularMoves(board, -1);
            }
        }

        private bool InsertEatingMoves(Checker[,] board, int direction)
        {
            return CheckEatingMove(board, direction, 1) || CheckEatingMove(board, direction, -1);
        }

        private void InsertRegularMoves(Checker[,] board, int direction)
        {
            CheckRegularMove(board, direction, 1);
            CheckRegularMove(board, direction, -1);
        }

        private bool CheckEatingMove(Checker[,] board, int verticalDirection, int horizontalDirection)
        {
            int color = (int)Color;
            int i = Position.Y;
            int j = Position.X;
            int boardSize = board.GetLength(0);

            if (IsValidPosition(i - (2 * color * verticalDirection), j + (2 * horizontalDirection), boardSize) &&
                IsOpponentChecker(board[i - (color * verticalDirection), j + horizontalDirection]) &&
                IsEmptySquare(board[i - (2 * color * verticalDirection), j + (2 * horizontalDirection)]))
            {
                PossibleMoves.Add(new Move(Position, new SquarePosition(i - (2 * color * verticalDirection), j + (2 * horizontalDirection))));
                return true;
            }

            return false;
        }

        private void CheckRegularMove(Checker[,] board, int verticalDirection, int horizontalDirection)
        {
            int color = (int)Color;
            int i = Position.Y;
            int j = Position.X;
            int boardSize = board.GetLength(0);

            if (IsValidPosition(i - (color * verticalDirection), j + horizontalDirection, boardSize) &&
                IsEmptySquare(board[i - (color * verticalDirection), j + horizontalDirection]))
            {
                PossibleMoves.Add(new Move(Position, new SquarePosition(i - (color * verticalDirection), j + horizontalDirection)));
            }
        }

        private bool IsValidPosition(int i, int j, int boardSize)
        {
            return i >= 0 && i < boardSize && j >= 0 && j < boardSize;
        }

        private bool IsOpponentChecker(Checker checker)
        {
            return checker != null && checker.Color == OppositeColor();
        }

        private bool IsEmptySquare(Checker checker)
        {
            return checker == null;
        }
    }
}
