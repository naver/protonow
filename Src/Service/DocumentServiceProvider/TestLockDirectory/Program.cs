using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;

namespace TestLockDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "TestLockDirectory"));
            if (!info.Exists)
            {
                info.Create();
            }

            DirectoryInfo workingDirectory = info.CreateSubdirectory(Guid.NewGuid().ToString());

            SafeFileHandle directorySafeHandle = Win32Wrapper.CreateFile(workingDirectory.FullName, Win32Wrapper.DESIRED_ACCESS_GENERIC_WRITE,
                                                           Win32Wrapper.SHARE_MODE_NONE/*Win32Wrapper.SHARE_MODE_READ*/ , IntPtr.Zero,
                                                           Win32Wrapper.OPEN_EXISTING, Win32Wrapper.FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);
            if (directorySafeHandle.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), workingDirectory.FullName);
            }

            DirectoryInfo subWorkingDirectory = workingDirectory.CreateSubdirectory("Sub_WorkingDirectory");

            // It is ok to enumerate sub directories
            foreach (DirectoryInfo pageDir in subWorkingDirectory.EnumerateDirectories("*", SearchOption.AllDirectories))
            {

            }

            // Why exception raised? Same process cannot enumerate directories?
            foreach (DirectoryInfo pageDir in workingDirectory.EnumerateDirectories("*", SearchOption.AllDirectories))
            {

            }
        }
    }
}
