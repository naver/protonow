using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.Helper;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;

namespace Naver.Compass.Module.Property
{
    class ListBoxPropertyViewModel : ListBasePropertyViewModel
    {
        public ListBoxPropertyViewModel()
        {
        }
    

        #region Binding property

        public Visibility ListBoxVisible
        {
            get
            {
                return Visibility.Visible;
            }
        }
        public Visibility DropListVisible
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        #endregion

    }
}
