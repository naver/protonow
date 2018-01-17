using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Naver.Compass.Common.CommonBase
{
    public class NetEnvironmentCheck
    {
        public static RegistryKey HKLM = Registry.LocalMachine;
        public const string NetFrameworkV4Client = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client";
        public const string NetFrameworkV4Full = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full";
        public const string MicrosoftUpdates = @"SOFTWARE\Microsoft\Updates";
        public const string EssentialPackageName = "KB2468871";
        public static bool IsValid()
        {
            var is4_5OrNewerExist = Is4_5OrNewer(NetFrameworkV4Client) || Is4_5OrNewer(NetFrameworkV4Full);
            if (is4_5OrNewerExist)
            {
                ///Don't need to check KB2468871 for net4.5 or newer
                return true;
            }

            var kbRegex = new Regex("KB[0-9]{7}");
            using (var updateNode = HKLM.OpenSubKey(MicrosoftUpdates))
            {
                ///get all subkeys like kb[0-9]{7}
                try
                {
                    var allkbkeys = GetAllSubKeyName(updateNode, kbRegex);
                    return allkbkeys.Any(k => k.Contains(EssentialPackageName));
                }
                catch
                {
                    return false;
                }
            }
        }

        private static List<string> GetAllSubKeyName(RegistryKey rootkey, Regex regex)
        {
            var rtnList = new List<string>();
            if (rootkey == null || regex == null)
            {
                return rtnList;
            }

            var subkeyName = rootkey.GetSubKeyNames();
            if (subkeyName != null && subkeyName.Length > 0)
            {
                foreach (var keyname in subkeyName)
                {
                    if (regex.Match(keyname).Success)
                    {
                        rtnList.Add(keyname);
                    }
                    else
                    {
                        rtnList.AddRange(GetAllSubKeyName(rootkey.OpenSubKey(keyname), regex));
                    }
                }
            }

            return rtnList;
        }

        private static bool Is4_5OrNewer(string subkeyName)
        {
            using (var ndpNode = HKLM.OpenSubKey(subkeyName))
            {
                if (ndpNode == null)
                {
                    /// no such subkey SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client or Full
                    return false;
                }

                var releaseKey = ndpNode.GetValue("Release");
                if (!(releaseKey is int))
                {
                    /// no such subkey SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client or Full\Release
                    return false;
                }

                ///The existence of the Release DWORD indicates that the .NET Framework 4.5 or newer has been installed on that computer.
                return true;
            }
        }
    }
}
