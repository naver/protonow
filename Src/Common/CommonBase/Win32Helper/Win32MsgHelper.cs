using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Naver.Compass.Common.Win32
{
    public static class Win32MsgHelper
    {
        public const int HWND_BROADCAST = 0xFFFF;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32", CharSet = CharSet.Auto)]
        //Mode-1
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        //Mode-2
        //public static extern bool PostMessage(IntPtr hwnd, int msg, int wparam, [MarshalAs(UnmanagedType.LPWStr)] ref string lParam);
    }
}
