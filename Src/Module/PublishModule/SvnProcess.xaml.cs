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
using System.Windows.Shapes;
using Naver.Compass.Common;
using System.Diagnostics;

namespace Naver.Compass.Module.PublishModel
{
    /// <summary>
    /// Interaction logic for SvnProcess.xaml
    /// </summary>
    public partial class SvnProcess : BaseWindow
    {

        private bool  _IsAllowClose = true;
        public SvnProcess()
        {
            InitializeComponent();
            this.DataContext = new ProcessViewModel();
            this.Closing += OnClose;
        }


        void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsAllowClose)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }            
        }

       
        public bool IsAllowClose
        {
            get { return _IsAllowClose; }
            set
            {
                if (_IsAllowClose != value)
                {
                    _IsAllowClose = value;
                }
            }
        }
    }
}
