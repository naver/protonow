using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

/// <summary>
/// this file support to save and load recent file list in registry
/// support follow interface.
/// 
/// List<string> RecentFiles()       // get the recent file list.
/// 
/// void InsertFile(string filepath) //Insert one path to the list
/// 
/// void RemoveFile(string filepath) //remove one path from list
/// 
/// void RemoveAllFile()             //remove all the path from list
/// </summary>

namespace Naver.Compass.Common.CommonBase
{
    
  public class ConfigFileManager
    {

        public static void SetValue(string property,string value)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryConfigKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey, true);

            k.SetValue(property, value);
        }

        public static void SetValue(string property, bool value)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryConfigKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey, true);

            k.SetValue(property, value);
        }

        public static void SetValue(string property, double value)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryConfigKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey, true);

            k.SetValue(property, value);
        }

        public static string GetValue(string property)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryConfigKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey, true);

           string value =Convert.ToString(k.GetValue(property, ""));

           return value;
        }

        public static bool GetBoolValue(string property)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryConfigKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey, true);

            return Convert.ToBoolean(k.GetValue(property, false));
        }

        public static double GetdoubleValue(string property)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryConfigKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryConfigKey, true);

            return Convert.ToDouble(k.GetValue(property, Double.MinValue));
        }

        public static void SetSectionValue(string section, string property, string value)
        {
            string sRegistryPath = BaseRegistryKey + "\\" + section;

            RegistryKey k = Registry.CurrentUser.OpenSubKey(sRegistryPath);
            if (k == null) Registry.CurrentUser.CreateSubKey(sRegistryPath);
            k = Registry.CurrentUser.OpenSubKey(sRegistryPath, true);

            k.SetValue(property, value);
        }

        public static string GetSectionValue(string section, string property)
        {
            string sRegistryPath = BaseRegistryKey + "\\" + section;

            RegistryKey k = Registry.CurrentUser.OpenSubKey(sRegistryPath);
            if (k == null) Registry.CurrentUser.CreateSubKey(sRegistryPath);
            k = Registry.CurrentUser.OpenSubKey(sRegistryPath, true);

            string value = Convert.ToString(k.GetValue(property, ""));

            return value;
        }

  
        public static List<string> RecentFiles()
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) k = Registry.CurrentUser.CreateSubKey(RegistryKey);

            List<string> list = new List<string>(MaxNumberOfFiles);

            for (int i = 0; i < MaxNumberOfFiles; i++)
            {
                string filename = (string)k.GetValue(Key(i));

                if (String.IsNullOrEmpty(filename)) break;

                list.Add(filename);
            }

            return list;
        }

        public static void InsertFile(string filepath)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryKey, true);

            RemoveFile(filepath);

            for (int i = MaxNumberOfFiles - 2; i >= 0; i--)
            {
                string sThis = Key(i);
                string sNext = Key(i + 1);

                object oThis = k.GetValue(sThis);
                if (oThis == null) continue;

                k.SetValue(sNext, oThis);
            }

            k.SetValue(Key(0), filepath);
        }

        public static void RemoveFile(string filepath)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) return;

            for (int i = 0; i < MaxNumberOfFiles; )
            {
                string s = (string)k.GetValue(Key(i));
                if (s != null && s.Equals(filepath, StringComparison.Ordinal))
                {
                    RemoveFile(i);
                    continue;
                }
                i++;
            }
        }

        public static void RemoveAllFile()
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) return;

            for (int i = 0; i < MaxNumberOfFiles; )
            {
                string s = (string)k.GetValue(Key(i));
                if (!String.IsNullOrEmpty(s))
                {
                    RemoveFile(i);
                    continue;
                }
                i++;
            }
        }

        private static void RemoveFile(int index)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
            if (k == null) return;

            k.DeleteValue(Key(index), false);

            for (int i = index; i < MaxNumberOfFiles - 1; i++)
            {
                string sThis = Key(i);
                string sNext = Key(i + 1);

                object oNext = k.GetValue(sNext);
                if (oNext == null) 
                    break;

                k.SetValue(sThis, oNext);
                k.DeleteValue(sNext,false);
            }
        }

        public static string BaseRegistryKey
        {
            get
            {
                return ("Software\\" +
                ApplicationAttributes.CompanyName + "\\" +
                ApplicationAttributes.ProductName);
            }
        }

        public static string RegistryKey
        {
            get
            {
                return ("Software\\" +
                ApplicationAttributes.CompanyName + "\\" +
                ApplicationAttributes.ProductName + "\\" +
                "RecentFileList");
            }
        }

        public static string RegistryConfigKey
        {
            get
            {
                return ("Software\\" +
                ApplicationAttributes.CompanyName + "\\" +
                ApplicationAttributes.ProductName + "\\" +
                "Config");
            }
        }

        public static string CurrentVersion
        {
            get
            {
                RegistryKey compassKey = Registry.LocalMachine.OpenSubKey(@"Software\Design Studio");

                if (compassKey == null)
                {
                    return String.Empty;
                }

                return compassKey.GetValue("CurrentVersion").ToString();
            }
        }

        public static int MaxNumberOfFiles
        {
            get
            {
                return 10;
            }

        }

        public ConfigFileManager()
        {

        }

        static string Key(int i) { return i.ToString("00"); }


       static class ApplicationAttributes
		{
            //static readonly Assembly _Assembly = null;

            //static readonly AssemblyTitleAttribute _Title = null;
            //static readonly AssemblyCompanyAttribute _Company = null;
            //static readonly AssemblyCopyrightAttribute _Copyright = null;
            //static readonly AssemblyProductAttribute _Product = null;

            //public static string Title { get; private set; }
			public static string CompanyName { get; private set; }
            //public static string Copyright { get; private set; }
			public static string ProductName { get; private set; }

            //static Version _Version = null;
            //public static string Version { get; private set; }

			static ApplicationAttributes()
			{
				try
				{
                    //Title = String.Empty;
					CompanyName = String.Empty;
                    //Copyright = String.Empty;
					ProductName = String.Empty;
                    //Version = String.Empty;

                    //_Assembly = Assembly.GetEntryAssembly();

                    //if ( _Assembly != null )
                    //{
                    //    object[] attributes = _Assembly.GetCustomAttributes( false );

                    //    foreach ( object attribute in attributes )
                    //    {
                    //        Type type = attribute.GetType();

                    //        if ( type == typeof( AssemblyTitleAttribute ) ) _Title = ( AssemblyTitleAttribute ) attribute;
                    //        if ( type == typeof( AssemblyCompanyAttribute ) ) _Company = ( AssemblyCompanyAttribute ) attribute;
                    //        if ( type == typeof( AssemblyCopyrightAttribute ) ) _Copyright = ( AssemblyCopyrightAttribute ) attribute;
                    //        if ( type == typeof( AssemblyProductAttribute ) ) _Product = ( AssemblyProductAttribute ) attribute;
                    //    }

                    //    _Version = _Assembly.GetName().Version;
                    //}

                    //if ( _Title != null ) 
                    //    Title = _Title.Title;
                    //if ( _Company != null )
                    CompanyName = "Naver Corporation";
                    //if ( _Copyright != null ) 
                    //    Copyright = _Copyright.Copyright;
                    //if ( _Product != null )
                    ProductName = "Design Studio 2";
                    //if ( _Version != null ) 
                    //    Version = _Version.ToString();
				}
				catch { }
			}
		}
       public static string ShortenPathname(string pathname, int maxLength)
       {
           if (pathname.Length <= maxLength)
               return pathname;

           string root = Path.GetPathRoot(pathname);
           if (root.Length > 3)
               root += Path.DirectorySeparatorChar;

           string[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

           int filenameIndex = elements.GetLength(0) - 1;

           if (elements.GetLength(0) == 1) // pathname is just a root and filename
           {
               if (elements[0].Length > 5) // long enough to shorten
               {
                   // if path is a UNC path, root may be rather long
                   if (root.Length + 6 >= maxLength)
                   {
                       return root + elements[0].Substring(0, 3) + "...";
                   }
                   else
                   {
                       return pathname.Substring(0, maxLength - 3) + "...";
                   }
               }
           }
           else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
           {
               root += "...\\";

               int len = elements[filenameIndex].Length;
               if (len < 6)
                   return root + elements[filenameIndex];

               if ((root.Length + 6) >= maxLength)
               {
                   len = 3;
               }
               else
               {
                   len = maxLength - root.Length - 3;
               }
               return root + elements[filenameIndex].Substring(0, len) + "...";
           }
           else if (elements.GetLength(0) == 2)
           {
               return root + "...\\" + elements[1];
           }
           else
           {
               int len = 0;
               int begin = 0;

               for (int i = 0; i < filenameIndex; i++)
               {
                   if (elements[i].Length > len)
                   {
                       begin = i;
                       len = elements[i].Length;
                   }
               }

               int totalLength = pathname.Length - len + 3;
               int end = begin + 1;

               while (totalLength > maxLength)
               {
                   if (begin > 0)
                       totalLength -= elements[--begin].Length - 1;

                   if (totalLength <= maxLength)
                       break;

                   if (end < filenameIndex)
                       totalLength -= elements[++end].Length - 1;

                   if (begin == 0 && end == filenameIndex)
                       break;
               }

               // assemble final string

               for (int i = 0; i < begin; i++)
               {
                   root += elements[i] + '\\';
               }

               root += "...\\";

               for (int i = end; i < filenameIndex; i++)
               {
                   root += elements[i] + '\\';
               }

               return root + elements[filenameIndex];
           }
           return pathname;
       }
    }

}
