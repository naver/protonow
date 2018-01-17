using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Naver.Compass.WidgetLibrary
{
    class AddManyViewModel
    {
        public AddManyViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(LoadedExecute);
            this.OKCommand = new DelegateCommand<Window>(OKExecute);
            this.CancelCommand = new DelegateCommand<Window>(CancelExecute);
        }

        public DelegateCommand<object> LoadedCommand { get; private set; }
        public DelegateCommand<Window> OKCommand { get; set; }
        public DelegateCommand<Window> CancelCommand { get; set; }

        public string StringAdded { get; set; }
        private void LoadedExecute(object obj)
        {
            TextBox box = obj as TextBox;
            if(box!=null)
            {
                box.Focus();
            }
        }

        public void OKExecute(Window win)
        {
            if (win != null)
            {
                win.DialogResult = true;

                TextBox txtBox = win.FindName("addBox") as TextBox;
                StringAdded = txtBox.Text;
               
                win.Close();
            }
        }

        public void CancelExecute(Window win)
        {
            if (win != null)
            {
                win.DialogResult = false;
                win.Close();
            }
        }
    }
}
