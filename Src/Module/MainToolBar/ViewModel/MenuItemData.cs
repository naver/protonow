using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainToolBar.ViewModel
{
    public class MenuItemData : SplitButtonData
    {
        public MenuItemData()
            : this(false)
        {
        }

        public MenuItemData(bool isApplicationMenu)
            : base(isApplicationMenu)
        {
        }

        /// <summary>
        /// Just for keeping, not binding to the name of the control
        /// </summary>
        public string Name { get; set; }
    }
}
