using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainToolBar.ViewModel
{
    public class ApplicationSplitMenuItemData : SplitMenuItemData
    {
        public ApplicationSplitMenuItemData()
            : this(false)
        {
        }

        public ApplicationSplitMenuItemData(bool isApplicationMenu)
            : base(isApplicationMenu)
        {
        }
    }
}
