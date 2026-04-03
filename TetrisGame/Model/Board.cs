namespace TetrisGame.Model
{
    public class Board
    {
        public int Rows { get; }
        public int Columns { get; }
        public bool[,] cells { get; private set; }

        public Board(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            cells = new bool[rows, columns];

        }
    }
}
