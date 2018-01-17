using System;
using System.Collections.Generic;
using System.IO;
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

namespace Naver.Compass.WidgetLibrary
{
    /// <summary>
    /// Interaction logic for ImageWidget.xaml
    /// </summary>
    public partial class ImageWidget : UserControl
    {
        public ImageWidget()
        {
            InitializeComponent();
        }

        private void Image_Unloaded(object sender, RoutedEventArgs e)
        {              
            Image img = sender as Image;
            if (img != null)
            {
                //img.Source = new BitmapImage(new Uri("Media/cross.png", UriKind.Relative));
                //img.Visibility = Visibility.Collapsed;  
                
                //img.Source = null;
                //img.UpdateLayout();

                //ImageWidgetViewModel ImageData = img.DataContext as ImageWidgetViewModel;
                //ImageData.NailStream.Seek(0, SeekOrigin.Begin);
                //ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                //img.Source= imageSourceConverter.ConvertFrom(ImageData.NailStream) as BitmapFrame;

                //img.InvalidateMeasure();
            }
        }
    }
}
