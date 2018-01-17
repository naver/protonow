using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Naver.Compass.Service.Document
{
    internal static class Win32Wrapper
    {
        public const uint DESIRED_ACCESS_GENERIC_ALL     = 0x10000000;
        public const uint DESIRED_ACCESS_GENERIC_EXECUTE = 0x20000000;
        public const uint DESIRED_ACCESS_GENERIC_WRITE   = 0x40000000;
        public const uint DESIRED_ACCESS_GENERIC_READ    = 0x80000000;

        public const uint SHARE_MODE_NONE   = 0x00000000;
        public const uint SHARE_MODE_READ   = 0x00000001;
        public const uint SHARE_MODE_WRITE  = 0x00000002;
        public const uint SHARE_MODE_DELETE = 0x00000004;

        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;

        public const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
                                                       uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
                                                       uint dwFlagsAndAttributes, IntPtr hTemplateFile);
    }
}
