using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainToolBar.ViewModel
{
    public class SplitFontColorButton : SplitMenuItemData
    {

        private SplitFontColorButton()
            : this(false)
        {
        }

        private SplitFontColorButton(bool isApplicationMenu)
            : base(isApplicationMenu)
        {
        }

        private static SplitFontColorButton singleInstanse = null;

        public static SplitFontColorButton GetInstance()
        {
            if (singleInstanse == null)
            {
                singleInstanse = new SplitFontColorButton();
            }

            return singleInstanse;
        }
    }


    public class SplitBackgroundColorButton : SplitMenuItemData
    {

        private SplitBackgroundColorButton()
            : this(false)
        {
        }

        private SplitBackgroundColorButton(bool isApplicationMenu)
            : base(isApplicationMenu)
        {
        }

        private static SplitBackgroundColorButton singleInstanse = null;

        public static SplitBackgroundColorButton GetInstance()
        {
            if (singleInstanse == null)
            {
                singleInstanse = new SplitBackgroundColorButton();
            }

            return singleInstanse;
        }
    }

    public class SplitBordlineColorButton : SplitMenuItemData
    {

        private SplitBordlineColorButton()
            : this(false)
        {
        }

        private SplitBordlineColorButton(bool isApplicationMenu)
            : base(isApplicationMenu)
        {
        }

        private static SplitBordlineColorButton singleInstanse = null;

        public static SplitBordlineColorButton GetInstance()
        {
            if (singleInstanse == null)
            {
                singleInstanse = new SplitBordlineColorButton();
            }

            return singleInstanse;
        }
    }
}
