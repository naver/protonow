
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module.Property
{
    class DropListPropertyViewModel : ListBasePropertyViewModel
    {

        public DropListPropertyViewModel()
        {
        }

        #region Binding property

        public Visibility ListBoxVisible
        {
            get
            {
                return Visibility.Collapsed;
            }
        }
        public Visibility DropListVisible
        {
            get
            {
                return Visibility.Visible;
            }
        }

        #endregion
    }
}
