namespace Project2
{
    public class SquarePosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public SquarePosition(int y, int x)
        {
            X = x;
            Y = y;
        }
        
        public SquarePosition(SquarePosition squarePosition)
        {
            X = squarePosition.X;
            Y = squarePosition.Y;
        }
    }
}