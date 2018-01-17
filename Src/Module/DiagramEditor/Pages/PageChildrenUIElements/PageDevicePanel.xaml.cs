using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Interaction logic for PageDevicePanel.xaml
    /// </summary>
    public partial class PageDevicePanel : UserControl
    {
        public PageDevicePanel()
        {
            InitializeComponent();
        }

        private void PresetsBtn_Click(object sender, RoutedEventArgs e)
        {
            PageEditorViewModel editorVm = this.DataContext as PageEditorViewModel;
            editorVm.EditorCanvas.Focus();
            editorVm.CheckDevices();
            //Point point = PresetsBtn.TransformToAncestor(DiagramEditor).Transform(new Point(0, 0));
            PresetsMenu.IsOpen = true;
            PresetsMenu.PlacementTarget = PresetsBtn;
            //PresetsMenu.PlacementRectangle = new Rect(point.X, point.Y, 0, 0);
            PresetsMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
        }

        private void PresetsBtn_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PresetsMenu.IsOpen = false;
            e.Handled = true;
        }

        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is ComboBox)
            {
                ComboBox control = sender as ComboBox;
                PageEditorViewModel VM = control.DataContext as PageEditorViewModel;

                string sText = control.Text;
                sText = sText.Replace("%", "");
                try
                {
                    double scale = Convert.ToDouble(sText) / 100;

                    VM.EditorScale = scale;
                }
                catch
                {
                    control.Text = VM.EditorScale* 100 + "%"; ;
                }

            }
        }
    }
}
