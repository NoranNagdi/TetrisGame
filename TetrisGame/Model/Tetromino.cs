using TetrisGame.Enums;

namespace TetrisGame.Model
{

    public class Tetromino
    {
        public bool[,] Blocks { get; set; }
        public ShapeType Type { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }

        public Tetromino(ShapeType type)
        {
            Blocks = new bool[4, 4];
            Row = 0;
            Col = 8;
            Type = type;
            InitializaShape();
        }


        private void InitializaShape()
        {
            switch (Type)
            {
                case ShapeType.I:
                    Blocks = new bool[,] {
                    {false, false, false, false },
                    {true, true, true, true },
                    {false, false, false, false },
                    {false, false, false, false }
                    };
                    break;
                case ShapeType.O:
                    Blocks = new bool[,] {
                    {false, false, false, false },
                    {false, true, true, false },
                    {false, true, true, false },
                    {false, false, false, false }
                    };
                    break;
                case ShapeType.T:
                    Blocks = new bool[,] {
                    {true, true, true, false },
                    {false, true, false, false },
                    {false, false, false, false },
                    {false, false, false, false }
                    };
                    break;
                case ShapeType.J:
                    Blocks = new bool[,] {
                    {false, true, false, false },
                    {false, true, false, false },
                    {true, true, false, false },
                    {false, false, false, false }
                    };
                    break;
                case ShapeType.L:
                    Blocks = new bool[,] {
                    {false, true, false, false },
                    {false, true, false, false },
                    {false, true, true, false },
                    {false, false, false, false }
                    };
                    break;
                case ShapeType.S:
                    Blocks = new bool[,] {
                    {false, false, true, true },
                    {false, true, true, false },
                    {false, false, false, false },
                    {false, false, false, false }
                    };
                    break;
                case ShapeType.Z:
                    Blocks = new bool[,] {
                    {false, true, true, false },
                    {false, false, true, true },
                    {false, false, false, false },
                    {false, false, false, false }
                    };
                    break;
            }
        }

      
    }
}
