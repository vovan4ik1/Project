using System;
using System.Text;
using Project2.Checker;
using Project2.Player;

namespace Project2.Game
{
    public class GameUI
    {
        public const int k_SinglePlayer = 1;
        public const int k_MultiPlayer = 2;
        public const int k_Small = 6;
        public const int k_Medium = 8;
        public const int k_Large = 10;
        public int m_BoardSize;

        public static void StartNewGame()
        {
            bool anotherRound = true;
            Player.Player player1 = GetPlayerName();
            int boardSize = ChooseBoardSize();
            int gameMode = ChooseGameMode();
            Player.Player player2 = ReturnPlayer2Instance(gameMode);
            Game game = new Game(boardSize, player1, player2);

            while (anotherRound)
            {
                game.InitializeBoard(boardSize);
                DrawBoard(game.Board);

                while (game.BlacksCounter != 0 && game.WhitesCounter != 0)
                {                     
                    if (game.CurrentPlayer().Type == EPlayerType.Computer)
                    {
                        game.MakeMove();
                    }
                    else
                    {
                        while (!game.MakeMove(GetMoveFromPlayer(game.CurrentPlayer())))
                        {
                            WrongMoveInput();
                        }
                    }

                    while (game.CurrentMove.CheckIfEatingMove())
                    {
                        game.UpdateBoard();

                        if (game.ThereIsEatingMoves(game.CheckAdditionalEatingMove()))
                        {
                            if (game.CurrentPlayer().Type == EPlayerType.Computer)
                            {
                                game.MakeAdditionalEatingMove();
                            }
                            else
                            {
                                game.MakeAdditionalEatingMove(GetMoveFromPlayer(game.CurrentPlayer()));
                            }

                            while (!game.CheckIfCurrentMoveIsPossible(game.CheckAdditionalEatingMove()))
                            {
                                WrongMoveInput();

                                if (game.CurrentPlayer().Type == EPlayerType.Computer)
                                {
                                    game.MakeAdditionalEatingMove();
                                }
                                else
                                {
                                    game.MakeAdditionalEatingMove(GetMoveFromPlayer(game.CurrentPlayer()));
                                }
                            }

                            game.UpdateBoard();
                        }
                        else
                        {
                            break;
                        }
                                                
                        DrawBoard(game.Board);
                    }

                    if (!game.CurrentMove.CheckIfEatingMove())
                    {
                        game.UpdateBoard();
                    }

                    DrawBoard(game.Board);
                    ShowPreviousMove(game.CurrentMove, game.CurrentPlayer());
                    game.UpdateCurrentTurn();
                }

                PrintEndRound(game, player1, player2);
                anotherRound = DoYouWantAnotherRound();
            }
        }

        private static bool DoYouWantAnotherRound()
        {
            bool anotherRound = false;

            string output = String.Format(@"Хочеш зіграти ще один раунд? «N» для виходу з гри або «Y» для YES");
            Console.WriteLine(output);
            string answer = Console.ReadLine();

            while (answer[0] != 'N' && answer[0] != 'Y')
            {
                output = String.Format(@"Неправильне значення, спробуйте ще раз.");
                Console.WriteLine(output);
                answer = Console.ReadLine();
            }

            if (answer[0] == 'Y')
            {
                anotherRound = true;
            }
            else
            {
                output = @"Допобачення";
                Console.WriteLine(output);
            }

            return anotherRound;
        }

        private static int ChooseBoardSize()
        {
            int boardSize;
            string sizeInput;
            string output = @"Будь ласка вибиріть розмір дошки:
            Маленька Дошка: введіть '6'.
            Середня Дошка: введіть '8'.
            Велика Дошка: введіть '10'.";

            Console.WriteLine(output);
            sizeInput = Console.ReadLine();
            int.TryParse(sizeInput, out boardSize);

            while (boardSize != k_Small && boardSize != k_Medium && boardSize != k_Large)
            {
                Console.WriteLine("Неправильне значення, спробуйте ще раз.");
                sizeInput = Console.ReadLine();
                int.TryParse(sizeInput, out boardSize);
            }

            Console.WriteLine("Ваш вибір: " + boardSize);

            return boardSize;
        }

        private static int ChooseGameMode()
        {
            var output = @"Обрати режим гри:
            Грати коп'ютером: введіть '1'
            Грати в вдвох: введіть '2' ";
            Console.WriteLine(output);
            var inputFromUser = Console.ReadLine();
            int.TryParse(inputFromUser, out var gameMode);

            while (gameMode != k_SinglePlayer && gameMode != k_MultiPlayer)
            {                                                                
                Console.WriteLine("Неправильне значення, спробуйте ще раз.");
                inputFromUser = Console.ReadLine();
                int.TryParse(inputFromUser, out gameMode);
            }                                                                                                           

            return gameMode;
        }

        private static Player.Player ReturnPlayer2Instance(int iGameMode)
        {
            Player.Player player2 = new Player.Player();

            if (iGameMode == k_MultiPlayer)
            {
                player2 = GetPlayerName();
            }

            return player2;
        }

        private static void DrawBoard(Checker.Checker[,] iBoard)
        {
            StringBuilder boardBuilder = new StringBuilder();
            int sizeOfBoard = iBoard.GetLength(0);

            for (int i = 0; i < sizeOfBoard; i++)
            {
                boardBuilder.Append(" ").Append(" ").Append(" ").Append((char)('A' + i));               
            }

            boardBuilder.AppendLine();

            for (int i = 0; i < (2 * sizeOfBoard) + 1; i++)
            {                                                                        
                if (i % 2 == 0)
                {
                    boardBuilder.Append(' ').Append('=', (4 * sizeOfBoard) + 1);
                    boardBuilder.AppendLine();
                }
                else if (i % 2 == 1)
                {
                    boardBuilder.Append((char)('a' + (i / 2))).Append("|");
                    DrawLine((i - 1) / 2, sizeOfBoard, iBoard, boardBuilder);
                    boardBuilder.AppendLine();
                }
            }

            Console.Write(boardBuilder.ToString());
        }

        private static void DrawLine(int iLineIndex, int iSizeOfBoard, Checker.Checker[,] iBoardMatrix, StringBuilder iBoardBuilder)
        {
            for (int j = 0; j < iSizeOfBoard; j++)
            {
                if ((j + iLineIndex) % 2 == 1)
                {
                    iBoardBuilder.Append(" ");

                    if (iBoardMatrix[iLineIndex, j] == null)
                    {
                        iBoardBuilder.Append(' ');
                    }
                    else if (iBoardMatrix[iLineIndex, j].Color == EPlayerColor.White)
                    {
                        if (iBoardMatrix[iLineIndex, j].Type == ECheckerType.Soldier)
                        {
                            iBoardBuilder.Append('X');
                        }
                        else
                        {
                            iBoardBuilder.Append('K');
                        }
                    }
                    else if (iBoardMatrix[iLineIndex, j].Color == EPlayerColor.Black)
                    {
                        if (iBoardMatrix[iLineIndex, j].Type == ECheckerType.Soldier)
                        {
                            iBoardBuilder.Append('O');
                        }
                        else
                        {
                            iBoardBuilder.Append('U');
                        } 
                    }
                        
                    iBoardBuilder.Append(" ").Append("|");
                }
                else
                {
                    iBoardBuilder.Append(" ").Append(" ").Append(" ").Append("|");
                }
            }
        }

        private static bool CheckMoveStringFromPlayer(string iMove)
        {
            return iMove.Length == 5 && iMove[2] == '>';
        }

        private static char FigureSign(EPlayerColor iColor)
        {
            char figure = 'O';

            if (iColor == EPlayerColor.White)
            {
                figure = 'X';
            }

            return figure;
        }

        private static string GetMoveFromPlayer(Player.Player iPlayer)
        {
            string output = $@"{iPlayer.Name}'s Turn ({FigureSign(iPlayer.Color)}) : ";
            Console.WriteLine(output);
            string move = Console.ReadLine();

            if (move.Length == 1)
            {
                if (move[0] == 'Q')
                {
                    move = null;    
                }
            }
            else
            {
                while (!CheckMoveStringFromPlayer(move))
                {
                    Console.WriteLine("Неправильний вхід! Введіть ще раз у такому форматі: 'COLrow>Colrow' або 'Q' для виходу\");\r\n}");
                    move = Console.ReadLine();
                }
            }

            return move;
        }

        private static Player.Player GetPlayerName()
        {
            Player.Player player = new Player.Player(EPlayerType.Human);

            string output = String.Format(
            @"<==========Шашки============>
            Введіть свій ігровий нікнейм: ");
            Console.WriteLine(output);
            player.Name = Console.ReadLine();

            while (player.Name.Length > 20 || player.Name.Contains(" ") || string.IsNullOrEmpty(player.Name))
            {
                Console.WriteLine("Неправильно введен нікнейм,повинно мати 20 значеннь,через пробіл ");
                player.Name = Console.ReadLine();
            }

            return player;
        }

        public static void WrongMoveInput()
        {
            Console.WriteLine("Неправильний вхід! Введіть ще раз у такому форматі: 'COLrow>Colrow' або 'Q' для виходу");
        }

        public static void ShowPreviousMove(Move i_Move, Player.Player i_Player)
        {            
            string move = String.Format(@"{0}{1}>{2}{3}", (char)(i_Move.Start.X + 'A'), (char)(i_Move.Start.Y + 'a'), (char)(i_Move.End.X + 'A'), (char)(i_Move.End.Y + 'a'));
                                
            if (move != null)
            {
                string output = String.Format(@"{0} move was ({1}) : {2}", i_Player.Name, FigureSign(i_Player.Color), move);
                Console.WriteLine(output);
            }
        }

        public static void PrintScores(Player.Player iPlayer1, Player.Player iPlayer2, int iCurrentScore)
        {
            string output = $@"{iPlayer1.Name} Виграв раунд!
         Поточний рахунок раунду: {iCurrentScore}

         Загальні результати гри: {iPlayer1.Name} : {iPlayer1.Score}
                                  {iPlayer2.Name} : {iPlayer2.Score}";

            Console.WriteLine(output);
        }

        private static void PrintEndRound(Game iGame, Player.Player iPlayer1, Player.Player iPlayer2)
        {
            int currentRoundScore = iGame.CalculateScore();


            if (iGame.CurrentTurn == iPlayer1.Color)
            {
                iPlayer2.Score += currentRoundScore;
                PrintScores(iPlayer2, iPlayer1, currentRoundScore);
            }
            else
            {
                iPlayer1.Score += currentRoundScore;
                PrintScores(iPlayer1, iPlayer2, currentRoundScore);
            }
        }
    }
}