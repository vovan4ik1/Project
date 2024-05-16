using System;
using System.Collections.Generic;
using System.Text;
using Project2;
using Project2.Checker;
using Project2.Player;

namespace GameCheckers
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
            Player player1 = GetPlayerName();
            int boardSize = ChooseBoardSize();
            int gameMode = ChooseGameMode();
            Player player2 = returnPlayer2Instance(gameMode);
            Game game = new Game(boardSize, player1, player2);

            while (anotherRound)
            {
                game.initializeBoard(boardSize);
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

                        if (game.thereIsEatingMoves(game.CheckAdditionalEatingMove()))
                        {
                            if (game.CurrentPlayer().Type == EPlayerType.Computer)
                            {
                                game.MakeAdditionalEatingMove();
                            }
                            else
                            {
                                game.MakeAdditionalEatingMove(GetMoveFromPlayer(game.CurrentPlayer()));
                            }

                            while (!game.checkIfCurrentMoveIsPossible(game.CheckAdditionalEatingMove()))
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
                    game.updateCurrentTurn();
                }

                printEndRound(game, player1, player2);
                anotherRound = DoYouWantAnotherRound();
            }
        }

        public static bool DoYouWantAnotherRound()
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
                output = String.Format(@"Допобачення");
                Console.WriteLine(output);
                anotherRound = false;
            }

            return anotherRound;
        }

        public static int ChooseBoardSize()
        {
            int boardSize;
            string sizeInput;
            string output = string.Format(
            @"Будь ласка вибиріть розмір дошки:
            Маленька Дошка: введіть '6'.
            Середня Дошка: введіть '8'.
            Велика Дошка: введіть '10'.");

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

        public static int ChooseGameMode()
        {
            string output;
            string inputFromUser;
            int gameMode;
            output = string.Format(
            @"Обрати режим гри:
            Грати коп'ютером: введіть '1'
            Грати в вдвох: введіть '2' ");
            Console.WriteLine(output);
            inputFromUser = Console.ReadLine();
            int.TryParse(inputFromUser, out gameMode);

            while (gameMode != k_SinglePlayer && gameMode != k_MultiPlayer)
            {                                                                
                Console.WriteLine("Неправильне значення, спробуйте ще раз.");
                inputFromUser = Console.ReadLine();
                int.TryParse(inputFromUser, out gameMode);
            }                                                                                                           

            return gameMode;
        }

        public static Player returnPlayer2Instance(int i_GameMode)
        {
            Player player2 = new Player();

            if (i_GameMode == k_MultiPlayer)
            {
                player2 = GetPlayerName();
            }

            return player2;
        }

        public static void DrawBoard(Checker[,] i_Board)
        {
            StringBuilder boardBuilder = new StringBuilder();
            int sizeOfBoard = i_Board.GetLength(0);

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
                    DrawLine((i - 1) / 2, sizeOfBoard, i_Board, boardBuilder);
                    boardBuilder.AppendLine();
                }
            }

            Console.Write(boardBuilder.ToString());
        }

        private static void DrawLine(int i_LineIndex, int i_SizeOfBoard, Checker[,] i_BoardMatrix, StringBuilder i_BoardBuilder)
        {
            for (int j = 0; j < i_SizeOfBoard; j++)
            {
                if ((j + i_LineIndex) % 2 == 1)
                {
                    i_BoardBuilder.Append(" ");

                    if (i_BoardMatrix[i_LineIndex, j] == null)
                    {
                        i_BoardBuilder.Append(' ');
                    }
                    else if (i_BoardMatrix[i_LineIndex, j].Color == EPlayerColor.White)
                    {
                        if (i_BoardMatrix[i_LineIndex, j].Type == ECheckerType.Soldier)
                        {
                            i_BoardBuilder.Append('X');
                        }
                        else
                        {
                            i_BoardBuilder.Append('K');
                        }
                    }
                    else if (i_BoardMatrix[i_LineIndex, j].Color == EPlayerColor.Black)
                    {
                        if (i_BoardMatrix[i_LineIndex, j].Type == ECheckerType.Soldier)
                        {
                            i_BoardBuilder.Append('O');
                        }
                        else
                        {
                            i_BoardBuilder.Append('U');
                        } 
                    }
                        
                    i_BoardBuilder.Append(" ").Append("|");
                }
                else
                {
                    i_BoardBuilder.Append(" ").Append(" ").Append(" ").Append("|");
                }
            }
        }

        private static bool CheckMoveStringFromPlayer(string i_move)
        {
            bool correctInput = false;
                        
            if (i_move.Length == 5 && i_move[2] == '>')
            {
                correctInput = true;
            }

            return correctInput;
        }

        private static char figureSign(EPlayerColor i_Color)
        {
            char figure = 'O';

            if (i_Color == EPlayerColor.White)
            {
                figure = 'X';
            }

            return figure;
        }

        public static string GetMoveFromPlayer(Player i_Player)
        {
            string output = String.Format(@"{0}'s Turn ({1}) : ", i_Player.Name, figureSign(i_Player.Color));
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

        public static Player GetPlayerName()
        {
            Player player = new Player(EPlayerType.Human);

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

        public static void ShowPreviousMove(Move i_Move, Player i_Player)
        {            
            string move = String.Format(@"{0}{1}>{2}{3}", (char)(i_Move.Start.X + 'A'), (char)(i_Move.Start.Y + 'a'), (char)(i_Move.End.X + 'A'), (char)(i_Move.End.Y + 'a'));
                                
            if (move != null)
            {
                string output = String.Format(@"{0} move was ({1}) : {2}", i_Player.Name, figureSign(i_Player.Color), move);
                Console.WriteLine(output);
            }
        }

        public static void PrintScores(Player i_Player1, Player i_Player2, int i_currentScore)
        {
            string output = String.Format(
         @"{0} Виграв раунд!
         Поточний рахунок раунду: {1}

         Загальні результати гри: {2} : {3}
                                  {4} : {5}", i_Player1.Name, i_currentScore, i_Player1.Name, i_Player1.Score, i_Player2.Name, i_Player2.Score);

            Console.WriteLine(output);
        }

        private static void printEndRound(Game i_Game, Player i_Player1, Player i_Player2)
        {
            int currentRoundScore = i_Game.calculateScore();


            if (i_Game.CurrentTurn == (EPlayerColor)i_Player1.Color)
            {
                i_Player2.Score += currentRoundScore;
                PrintScores(i_Player2, i_Player1, currentRoundScore);
            }
            else
            {
                i_Player1.Score += currentRoundScore;
                PrintScores(i_Player1, i_Player2, currentRoundScore);
            }
        }
    }
}