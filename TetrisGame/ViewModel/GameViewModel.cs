using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Threading;
using TetrisGame.Enums;
using TetrisGame.Model;


namespace TetrisGame.ViewModel
{
    class GameViewModel : INotifyPropertyChanged
    {
        private bool _isGameOver;
        public bool IsGameOver
        {
            get => _isGameOver;
            set
            {
                if (_isGameOver != value)
                {
                    _isGameOver = value;
                    OnPropertyChanged(nameof(IsGameOver));
                }
            }
        }

        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged(nameof(Score));
                }
            }
        }

        private DispatcherTimer _gameTimer;
        private Tetromino _current_tetromino;
        private Board _board;

        private Dictionary<(int row, int col), Brush> _lockedBlockColors = new Dictionary<(int row, int col), Brush>();
        public ObservableCollection<BlockViewModel> TetrominoBlocks { get; } = new ObservableCollection<BlockViewModel>();

        public GameViewModel()
        {
            _gameTimer = new DispatcherTimer();
            InitializeTimer(_gameTimer);

            _board = new Board(20, 20);
            _current_tetromino = new Tetromino(GenerateRandomShape());
            UpdateTetromino();
        }

        void InitializeTimer(DispatcherTimer timer)
        {
            timer.Interval = TimeSpan.FromSeconds(0.7);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        void Timer_Tick(object? sender, EventArgs e)
        {
            if (IsGameOver)
            {
                return;
            }
            _current_tetromino.Row++;

            if (!IsValidPosition())
            {
                _current_tetromino.Row--;
                LockTetromino();
                ClearCompletedLines();

                _gameTimer.Interval = TimeSpan.FromSeconds(0.7);
                _current_tetromino = new Tetromino(GenerateRandomShape());

                if (!IsValidPosition())
                {
                    IsGameOver = true;
                    GameOver();
                    return;
                }
            }
            UpdateTetromino();
        }

        private List<(int Row, int Col)> GetTetrominoBlocks()
        {
            var tetrominoBlocks = new List<(int Row, int Col)>();

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (_current_tetromino.Blocks[r, c])
                    {
                        tetrominoBlocks.Add((_current_tetromino.Row + r, _current_tetromino.Col + c));
                    }
                }
            }
            return tetrominoBlocks;
        }

        private bool[,] GetRotatedTetromino()
        {
            bool[,] rotatedTetromino = new bool[4, 4];

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    rotatedTetromino[r, c] = _current_tetromino.Blocks[3 - c, r];
                }
            }
            return rotatedTetromino;
        }

        private void UpdateTetromino()
        {
            TetrominoBlocks.Clear();

            for (int row = 0; row < _board.Rows; row++)
            {
                for (int col = 0; col < _board.Columns; col++)
                {
                    if (!IsCellEmpty(row, col))
                    {
                        TetrominoBlocks.Add(new BlockViewModel
                        {
                            Row = row,
                            Column = col,
                            Color = _lockedBlockColors[(row, col)]
                        });
                    }
                }
            }

            var tetrominoBlocks = GetTetrominoBlocks();
            foreach (var block in tetrominoBlocks)
            {
                if (IsInside(block.Row, block.Col))
                {
                    TetrominoBlocks.Add(new BlockViewModel
                    {
                        Row = block.Row,
                        Column = block.Col,
                        Color = GetTetrominoColor(_current_tetromino.Type)
                    });
                }
            }
        }

        public void Rotate()
        {
            var originalBlocks = _current_tetromino.Blocks;

            _current_tetromino.Blocks = GetRotatedTetromino();

            if (!IsValidPosition())
            {
                _current_tetromino.Blocks = originalBlocks;
            }
            UpdateTetromino();
        }

        private void LockTetromino()
        {
            var blocks = GetTetrominoBlocks();

            foreach (var block in blocks)
            {
                if (IsInside(block.Row, block.Col))
                {
                    _board.cells[block.Row, block.Col] = true;
                    _lockedBlockColors[(block.Row, block.Col)] = GetTetrominoColor(_current_tetromino.Type);
                }
            }
        }

        private bool IsInside(int row, int col)
        {
            return (row >= 0 && row < _board.Rows && col >= 0 && col < _board.Columns);
        }

        private bool IsCellEmpty(int row, int col)
        {
            if (!IsInside(row, col))
            {
                return false;
            }
            return (_board.cells[row, col] == false);
        }

        private bool IsValidPosition()
        {
            var blocks = GetTetrominoBlocks();

            foreach (var block in blocks)
            {
                if (!IsInside(block.Row, block.Col))
                {
                    return false;
                }

                if (!IsCellEmpty(block.Row, block.Col))
                {
                    return false;
                }
            }
            return true;
        }

        public void MoveRight()
        {
            _current_tetromino.Col++;
            if (!IsValidPosition())
            {
                _current_tetromino.Col--;
            }
            UpdateTetromino();
        }

        public void MoveLeft()
        {
            _current_tetromino.Col--;
            if (!IsValidPosition())
            {
                _current_tetromino.Col++;
            }
            UpdateTetromino();
        }
        public void MoveDown()
        {
            _current_tetromino.Row++;
            if (!IsValidPosition())
            {
                _current_tetromino.Row--;
            }
            UpdateTetromino();
        }

        public void PlaceTetromino()
        {
            _gameTimer.Interval = TimeSpan.FromSeconds(0);
            if (IsValidPosition())
            {
                Score += 10;
            }
        }

        public void StopGame()
        {
            _gameTimer.Stop();
        }

        public void StartGame()
        {
            _gameTimer.Start();
        }

        public void ResetGame()
        {
            StopGame();
            IsGameOver = false;
            Score = 0;

            // Clear board
            TetrominoBlocks.Clear();
            for (int row = 0; row < _board.Rows; row++)
            {
                for (int col = 0; col < _board.Columns; col++)
                {
                    _board.cells[row, col] = false;
                    _lockedBlockColors.Remove((row, col));
                }
            }
            _gameTimer.Interval = TimeSpan.FromSeconds(0.7);
            StartGame();
        }

        private bool IsRowFull(int row)
        {
            for (int col = 0; col < _board.Columns; col++)
            {
                if (IsCellEmpty(row, col))
                    return false;
            }
            return true;
        }

        private void ClearLine(int row)
        {
            for (int col = 0; col < _board.Columns; col++)
            {
                if (IsCellEmpty(row, col))
                {
                    return;
                }
            }
            for (int col = 0; col < _board.Columns; col++)
            {
                _board.cells[row, col] = false;
                _lockedBlockColors.Remove((row, col));
            }
        }

        private void ShiftRowsDown(int clearedRow)
        {
            for (int row = clearedRow; row > 0; row--)
            {
                for (int col = 0; col < _board.Columns; col++)
                {
                    _board.cells[row, col] = _board.cells[row - 1, col];

                    if (_lockedBlockColors.ContainsKey((row - 1, col)))
                    {
                        _lockedBlockColors[(row, col)] = _lockedBlockColors[(row - 1, col)];
                        _lockedBlockColors.Remove((row - 1, col));
                    }
                    else
                    {
                        _lockedBlockColors.Remove((row, col));
                    }
                }
            }
            // CLEAR TOP ROW
            for (int col = 0; col < _board.Columns; col++)
            {
                _board.cells[0, col] = false;
                _lockedBlockColors.Remove((0, col));
            }

        }

        private void ClearCompletedLines()
        {
            for (int row = _board.Rows - 1; row >= 0; row--)
            {
                if (IsRowFull(row))
                {
                    ClearLine(row);
                    ShiftRowsDown(row);
                    Score += 100;
                    row++; // Re-check the same row after shifting
                }
            }
        }

        private void GameOver()
        {
            IsGameOver = true;
            StopGame();
        }

        private ShapeType GenerateRandomShape()
        {
            var random = new Random();
            return (ShapeType)random.Next(1, 8);
        }

        private Brush GetTetrominoColor(ShapeType type)
        {
            return type switch
            {
                ShapeType.I => Brushes.Cyan,
                ShapeType.O => Brushes.Yellow,
                ShapeType.T => Brushes.Purple,
                ShapeType.J => Brushes.Blue,
                ShapeType.L => Brushes.Orange,
                ShapeType.S => Brushes.Green,
                ShapeType.Z => Brushes.Red,
                _ => Brushes.Gray
            };
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
