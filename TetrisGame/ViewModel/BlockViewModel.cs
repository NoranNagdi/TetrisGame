using System.Windows.Media;

namespace TetrisGame.ViewModel
{
    public class BlockViewModel
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Brush? Color { get; set; }
    }
}
