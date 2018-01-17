using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace Naver.Compass.Common.Helper
{
    public static class GlobalData
    {
        //language type.
        private static string culture;
        public static string Culture
        {
            get
            {
                return string.IsNullOrEmpty(culture) ? "ko-KR" : culture;
            }
            set
            {
                if (culture != value)
                {
                    culture = value;
                    try
                    {
                        UpdateCulture();
                    }
                    catch (Exception)
                    {
                       
                    }
                    
                }
            }
        }

        public static void GetCulture()
        {
            try
            {
                string culture = Properties.Settings.Default.Culture.Trim();
                if (string.IsNullOrEmpty(culture))
                {
                    culture = CurrentLanguage;
                }
                Culture = culture;
            }
            catch (Exception)
            {

            }
        } 
        public static void SetCulture()
        {
            try
            {
                Properties.Settings.Default.Culture = GlobalData.Culture;
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
             
            }
        }

        private static string CurrentLanguage
        {
            get
            {
                RegistryKey compassKey = Registry.LocalMachine.OpenSubKey(@"Software\Design Studio"); ;

                if (compassKey == null)
                {
                    return "ko-KR";
                }
                string lan;
                try
                {
                    lan = compassKey.GetValue("InstallLanguage").ToString();
                }
                catch (Exception)
                {
                    return "ko-KR";
                }

                switch (lan)
                {

                    case "2052":
                        return "zh-CN";
                    case "1033":
                        return "en-US";
                    case "1041":
                        return "ja-JP";
                    case "1042":
                        return "ko-KR";
                    default:
                        return "ko-KR";
                }

            }
        }

        //find language resource in main project.
        public static string FindResource(string resourceKey)
        {
            return Application.Current.TryFindResource(resourceKey).ToString();
        }

        public static void UpdateCulture()
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format(@"/Naver.Compass.Common.Helper;component/Resources/StringResource.{0}.xaml", Culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            if (resourceDictionary == null)
            {
                requestedCulture = @"/Naver.Compass.Common.Helper;component/Resources/StringResource.ko-KR.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            }
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Culture);

        }

        public static bool IsShowGrid
        {
            get
            {
                return Properties.Settings.Default.IsShowGrid;
            }
            set
            {
                Properties.Settings.Default.IsShowGrid = value;
                SaveSetting();
            }
        }
        public static bool IsSnapToGrid
        {
            get
            {
                return Properties.Settings.Default.IsSnapToGrid;
            }
            set
            {
                Properties.Settings.Default.IsSnapToGrid = value;
                SaveSetting();
            }
        }

        public static bool IsShowPageGuide
        {
            get
            {
                return Properties.Settings.Default.IsShowPageGuide;
            }
            set
            {
                Properties.Settings.Default.IsShowPageGuide = value;
                SaveSetting();
            }
        }
        public static bool IsShowGlobalGuide
        {
            get
            {
                return Properties.Settings.Default.IsShowGlobalGuide;
            }
            set
            {
                Properties.Settings.Default.IsShowGlobalGuide = value;
                SaveSetting();
            }
        }

        public static bool IsSnapToGuide
        {
            get
            {
                return Properties.Settings.Default.IsSnapToGuide;
            }
            set
            {
                Properties.Settings.Default.IsSnapToGuide = value;
                SaveSetting();
            }
        }

        public static bool IsLockGuides
        {
            get
            {
                return Properties.Settings.Default.IsLockGuides;
            }
            set
            {

                Properties.Settings.Default.IsLockGuides = value;
                SaveSetting();

            }
        }

        public static bool IsDefaultPreview
        {
            get
            {
                return Properties.Settings.Default.IsFullPreview;
            }
            set
            {
                Properties.Settings.Default.IsFullPreview = value;
                SaveSetting();
            }
        }

        public static bool IsHideWelcomOnStart
        {
            get
            {
                return Properties.Settings.Default.IsHideWelcomOnStart;
            }
            set
            {
                Properties.Settings.Default.IsHideWelcomOnStart = value;
                SaveSetting();
            }
        }
        public static int GRID_SIZE
        {
            get
            {
                return Properties.Settings.Default.GRID_SIZE;
            }
            set
            {
                Properties.Settings.Default.GRID_SIZE = value;
                SaveSetting();
            }
        }
        public static Color GridColor
        {
            get
            {
                return Properties.Settings.Default.GridColor;
            }
            set
            {
                Properties.Settings.Default.GridColor = value;
                SaveSetting();
            }
        }
        public static Color LocalGuideColor
        {
            get
            {
                return Properties.Settings.Default.LocalGuideColor;
            }
            set
            {
                Properties.Settings.Default.LocalGuideColor = value;
                SaveSetting();
            }
        }
        public static Color GlobalGuideColor
        {
            get
            {
                return Properties.Settings.Default.GlobalGuideColor;
            }
            set
            {
                Properties.Settings.Default.GlobalGuideColor = value;
                SaveSetting();
            }
        }

        /// <summary>
        /// Grid line type, true: line, false: Intersection
        /// </summary>
        public static bool IsLineType
        {
            get
            {
                return Properties.Settings.Default.IsLineType;
            }
            set
            {
                Properties.Settings.Default.IsLineType = value;
                SaveSetting();
            }
        }

        public static bool IsAutoSaveEnable
        {
            get
            {
                return Properties.Settings.Default.IsAutoSaveEnable;
            }
            set
            {

                Properties.Settings.Default.IsAutoSaveEnable = value;
                SaveSetting();
            }
        }

        public static int AutoSaveTick
        {
            get
            {
                return Properties.Settings.Default.AutoSaveTick;
            }
            set
            {
                Properties.Settings.Default.AutoSaveTick = value;
                SaveSetting();

            }
        }

        public static bool IsKeepLastAutoSaved
        {
            get
            {
                return Properties.Settings.Default.IsKeepLastAutoSaved;
            }
            set
            {

                Properties.Settings.Default.IsKeepLastAutoSaved = value;
                SaveSetting();
            }
        }

        public static string AutoSaveFileLocation
        {
            get
            {
                return Properties.Settings.Default.AutoSaveFileLocation;
            }
            set
            {
                Properties.Settings.Default.AutoSaveFileLocation = value;
                SaveSetting();
            }
        }

        //Is width/height keep ratio when resize.
        public static bool IsLockRatio
        {
            get
            {
                return Properties.Settings.Default.IsLockRatio;
            }
            set
            {

                Properties.Settings.Default.IsLockRatio = value;
                SaveSetting();
            }
        }

        public static string LibrariesExpanded
        {
            get
            {
                return Properties.Settings.Default.LibrariesExpanded;
            }
            set
            {
                Properties.Settings.Default.LibrariesExpanded = value;
                SaveSetting();
            }
        }

        public static System.Collections.Specialized.StringCollection RecentFonts
        {
            get
            {
                return Properties.Settings.Default.RecentFonts;
            }
            set
            {
                Properties.Settings.Default.RecentFonts = value;
                SaveSetting();
            }
        }

        private static void SaveSetting()
        {
            try
            {
                Properties.Settings.Default.Save();
            }
            catch (ConfigurationException)
            {
                ReloadSetting();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private static void ReloadSetting()
        {
            try
            {
                Properties.Settings.Default.Reload();
            }
            catch(Exception)
            {
            }
        }

        public static bool IsStandardMode = true;
    }
}
