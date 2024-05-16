using System;

namespace Project2
{
    public class Move
    {
        public SquarePosition Start { get; set; }
        public SquarePosition End { get; set; }
        
        public Move(SquarePosition iStart, SquarePosition iEnd)
        {
            Start = iStart;
            End = iEnd;
        }
        
        public Move()
        {
            
        }

        public bool QuitRoundOrConvertStringToMove(string move)
        {
            bool isQuit = false;

            if (!string.IsNullOrEmpty(move) && move.Length == 5)
            {
                Start.X = move[0] - 'A';
                Start.Y = move[1] - 'a';
                End.X = move[3] - 'A';
                End.Y = move[4] - 'a';
            }
            else
            {
                isQuit = true;
            }

            return isQuit;
        }

        public bool CheckIfEatingMove()
        {
            return Math.Abs(End.X - Start.X) == 2;
        }
    }
}