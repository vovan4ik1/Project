using System;
using System.Collections.Generic;
using Project2.Checker;
using Project2.Player;

namespace Project2.Game
{

    class Game
    {
        public EPlayerColor CurrentTurn { get; set; }

        public Move CurrentMove { get; set; }

        public Checker.Checker[,] Board { get; set; }

        public int WhitesCounter { get; set; }

        public int BlacksCounter { get; set; }

        private Player.Player Player1 { get; set; }

        private Player.Player Player2 { get; set; }

        public Game(int iBoardSize, Player.Player iPlayer1, Player.Player iPlayer2)
        {
            Player1 = iPlayer1;
            Player2 = iPlayer2;
            InitializeBoard(iBoardSize);
        }



        public void InitializeBoard(int boardSize)
        {
            InitializeVariables(boardSize);
            Player.Player.TossColor();
            InitializeCheckers(boardSize);
        }

        private void InitializeVariables(int boardSize)
        {
            CurrentMove = new Move();
            CurrentTurn = EPlayerColor.White;
            Board = new Checker.Checker[boardSize, boardSize];
            BlacksCounter = 0;
            WhitesCounter = 0;
        }

        private void InitializeCheckers(int boardSize)
        {
            InitializeBlackCheckers(boardSize);
            InitializeWhiteCheckers(boardSize);
        }

        private void InitializeBlackCheckers(int boardSize)
        {
            for (int i = 0; i < (boardSize / 2) - 1; i++)
            {
                for (int j = (i + 1) % 2; j < boardSize; j += 2)
                {
                    Board[i, j] = new Checker.Checker(EPlayerColor.Black, new SquarePosition(i, j));
                    BlacksCounter++;
                }
            }
        }

        private void InitializeWhiteCheckers(int boardSize)
        {
            for (int i = boardSize - 1; i > boardSize / 2; i--)
            {
                for (int j = (i + 1) % 2; j < boardSize; j += 2)
                {
                    Board[i, j] = new Checker.Checker(EPlayerColor.White, new SquarePosition(i, j));
                    WhitesCounter++;
                }
            }
        }

        public bool MakeMove(string iMoveString = null)
        {
            UpdateCurrentPossibleMovesInCheckers();
            List<Move> possibleMoves = TotalPossibleCurrentMoves();
            bool isLegalMove = true;

            if (possibleMoves.Count > 0)
            {
                if (iMoveString == null)
                {
                    Random index = new Random();
                    CurrentMove = possibleMoves[index.Next(0, possibleMoves.Count)];
                }
                else
                {
                    isLegalMove = CurrentMove.QuitRoundOrConvertStringToMove(iMoveString) && CheckIfCurrentMoveIsPossible(possibleMoves);
                }
            }
            else
            {
                EndRound();
            }

            return isLegalMove;
        }


        private List<Move> TotalPossibleCurrentMoves()
        {
            List<Move> totalPossibleMoves = new List<Move>();

            foreach (Checker.Checker checker in Board)
            {
                if (checker != null && checker.Color == CurrentTurn && checker.PossibleMoves != null)
                {
                    foreach (Move move in checker.PossibleMoves)
                    {
                        if (!totalPossibleMoves.Contains(move))
                        {
                            totalPossibleMoves.Add(move);
                        }
                    }
                }
            }

            return totalPossibleMoves;
        }

        public List<Move> CheckAdditionalEatingMove()
        {
            Checker.Checker eatingChecker = Board[CurrentMove.End.Y, CurrentMove.End.X];

            if (eatingChecker.Type == ECheckerType.Soldier)
            {
                eatingChecker.InsertSoldierMoves(Board);
            }
            else if (eatingChecker.Type == ECheckerType.King)
            {
                eatingChecker.InsertKingMoves(Board);
            }

            return eatingChecker.PossibleMoves;
        }

        public bool ThereIsEatingMoves(List<Move> iPossibleMoves)
        {
            bool thereIs = iPossibleMoves.Count > 0;

            return thereIs;

        }

        public bool MakeAdditionalEatingMove(string iCurrentMove = null)
        {
            bool isLegal = false;

            if (CurrentMove.QuitRoundOrConvertStringToMove(iCurrentMove))
            {
                if (CheckIfCurrentMoveIsPossible(CheckAdditionalEatingMove()))
                {
                    isLegal = true;
                }
            }
            else
            {
                EndRound();
            }

            return isLegal;
        }

        public void UpdateBoard()
        {
            MoveChecker();
            PromoteToKing();
            HandleEatingMove();
        }

        private void MoveChecker()
        {
            var checkerToMove = Board[CurrentMove.Start.Y, CurrentMove.Start.X];
            Board[CurrentMove.End.Y, CurrentMove.End.X] = new Checker.Checker(CurrentTurn, CurrentMove.End, checkerToMove.Type);
            Board[CurrentMove.Start.Y, CurrentMove.Start.X] = null;
        }

        private void PromoteToKing()
        {
            if (CurrentMove.End.Y == 0 || CurrentMove.End.Y == (Board.GetLength(0) - 1))
            {
                Board[CurrentMove.End.Y, CurrentMove.End.X].Type = ECheckerType.King;
            }
        }

        private void HandleEatingMove()
        {
            if (CurrentMove.CheckIfEatingMove())
            {
                int i = (CurrentMove.End.Y + CurrentMove.Start.Y) / 2;
                int j = (CurrentMove.End.X + CurrentMove.Start.X) / 2;

                Board[i, j] = null;

                if (CurrentTurn == EPlayerColor.White)
                {
                    WhitesCounter--;
                }
                else
                {
                    BlacksCounter--;
                }
            }
        }

        public void UpdateCurrentTurn()
        {
            CurrentTurn = (EPlayerColor)((int)CurrentTurn * -1);
        }

        private void UpdateCurrentPossibleMovesInCheckers()
        {
            foreach (Checker.Checker checker in Board)
            {
                if (checker != null && checker.Color == CurrentTurn)
                {
                    checker.InsertPossibleCorrectMovesToList(Board);
                }
            }
        }

        public bool CheckIfCurrentMoveIsPossible(List<Move> possibleMoves)
        {
            return possibleMoves.Contains(CurrentMove);
        }


        public Player.Player CurrentPlayer()
        {
            return (CurrentTurn == Player1.Color) ? Player1 : Player2;
        }

        public int CalculateScore()
        {
            int score = 0;

            foreach (Checker.Checker checker in Board)
            {
                if (checker != null)
                {
                    int value = (checker.Color != CurrentTurn) ? 1 : -1;
                    score += (checker.Type == ECheckerType.Soldier) ? value : 4 * value;
                }
            }

            return Math.Max(0, score);
        }

        private void EndRound()
        {
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    Board[i, j] = null;
                }
            }
        }
    }
}
