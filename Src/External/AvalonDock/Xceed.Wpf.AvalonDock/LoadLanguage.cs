using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Globalization;
using Naver.Compass.Common.Helper;

namespace Xceed.Wpf.AvalonDock
{
    /// <summary>
    /// Load language
    /// Get current language from GlobalData.
    /// </summary>
    public static class LoadLanguage
    {
        public static void GetCulture()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(GlobalData.Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(GlobalData.Culture);
        }

    }
}
