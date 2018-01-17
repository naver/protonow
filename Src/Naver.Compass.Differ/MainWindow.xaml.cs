using Naver.Compass.Common;
using System.Windows.Input;

namespace Naver.Compass.Differ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MainBase
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 1 && e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
        }
    }
}
