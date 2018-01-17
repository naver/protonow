using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Interaction logic for WidgetGallery.xaml
    /// </summary>
    public partial class WidgetGallery : UserControl
    {

        public WidgetGallery()
        {

            InitializeComponent();
            
            this.DataContext = new WidgetGalleryViewModel();
        }

        private void libDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(CommonDefine.UrlLibaryDownload);
                ServiceLocator.Current.GetInstance<INClickService>().SendNClick("lud.downloadmore");
                Common.CommonBase.NLogger.Info("Send nclick: {0}", "lud.downloadmore");
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Open URL error : {0}.", exp.Message);
            }
        }

    }
}
