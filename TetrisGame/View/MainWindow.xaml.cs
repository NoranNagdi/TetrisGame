using System.Windows;
using System.Windows.Input;
using TetrisGame.ViewModel;

namespace TetrisGame
{
    public partial class MainWindow : Window
    {
        GameViewModel gameVM;
        public MainWindow()
        {
            InitializeComponent();

            gameVM = new GameViewModel();
            this.DataContext = gameVM;
            this.KeyDown += Keyboard_Event;
        }

        private void Keyboard_Event(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Up)
            {
                gameVM.PlaceTetromino();

            }
            else if (e.Key == Key.Down)
            {
                gameVM.MoveDown();

            }
            else if (e.Key == Key.Right)
            {
                gameVM.MoveRight();

            }
            else if (e.Key == Key.Left)
            {
                gameVM.MoveLeft();
            }
            else if (e.Key == Key.R)
            {
                gameVM.Rotate();
            }
        }
        private void Pause_Button(object sender, RoutedEventArgs e)
        {
            gameVM.StopGame();
            MessageBox.Show("Continue Game");
            gameVM.StartGame();
        }
        private void Restart_Button(object sender, RoutedEventArgs e)
        {
            gameVM.ResetGame();
        }
    }
}