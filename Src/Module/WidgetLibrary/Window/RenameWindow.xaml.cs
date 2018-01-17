using Naver.Compass.Common;
using Naver.Compass.Common.Helper;
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

namespace Naver.Compass.Module
{
    /// <summary>
    /// Interaction logic for RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow : BaseWindow
    {
        public bool? Result { get; set; }

        public string NewName
        {
            get { return editBox.Text.Trim(); }
            set { editBox.Text = value; }
        }
        public RenameWindow()
        {
            InitializeComponent();
            editBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewName))
            {
                var valid = IsValidFileName(NewName);
                if(!valid)
                {
                    MessageBox.Show(GlobalData.FindResource("Alert_Filename_Invalid"));
                    editBox.Focus();
                    return;
                }
                Result = true;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            this.Close();
        }

        private bool IsValidFileName(string fileName)
        {
            bool isValid = true;
            string errChar = "\\/:*?\"<>|";  //
            for (int i = 0; i < errChar.Length; i++)
            {
                if (fileName.Contains(errChar[i].ToString()))
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }
    }
}
