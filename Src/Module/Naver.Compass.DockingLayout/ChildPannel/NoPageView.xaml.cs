using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
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

namespace DockingLayout
{
    /// <summary>
    /// Interaction logic for NoPagePannel.xaml
    /// </summary>
    public partial class NoPageView : UserControl
    {
        public NoPageView()
        {
            InitializeComponent();
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string path in files)
                {
                    if (CommonFunction.IsProjectFilePath(path))
                    {
                        ExcuteOpenProject(path);
                        break;
                    }
                }
                e.Handled = true;
            }
        }

        private void ExcuteOpenProject(string path)
        {
            IEventAggregator listEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            if (listEventAggregator != null)
            {
                listEventAggregator.GetEvent<OpenFileEvent>().Publish(path);
            }
        }



    }
}
