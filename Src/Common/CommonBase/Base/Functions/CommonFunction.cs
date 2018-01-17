using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Input;
using System.Text.RegularExpressions;


namespace Naver.Compass.Common.CommonBase
{
    public class CommonFunction
    {
        public static TextDecorationCollection GetUnderline()
        {
            TextDecorationCollection mycollection = new TextDecorationCollection();
           
            TextDecoration myUnderline = TextDecorations.Underline[0].Clone();

            //myUnderline.PenThicknessUnit = TextDecorationUnit.Pixel;

            mycollection.Add(myUnderline);

            return mycollection;
        }

        public static TextDecorationCollection GetStrikeThough()
        {
            TextDecorationCollection mycollection = new TextDecorationCollection();

            TextDecoration myStrikeThough = TextDecorations.Strikethrough[0].Clone();

            //myStrikeThough.PenThicknessUnit = TextDecorationUnit.Pixel;

            mycollection.Add(myStrikeThough);

            return mycollection;
        }

        public static bool IsImageFilePath(string path)
        {
            string tStr = path;
            tStr = tStr.Substring(tStr.LastIndexOf(".") + 1);

            if (tStr.Equals("jpeg", StringComparison.OrdinalIgnoreCase)
                   || tStr.Equals("bmp", StringComparison.OrdinalIgnoreCase)
                   || tStr.Equals("ico", StringComparison.OrdinalIgnoreCase)
                   || tStr.Equals("png", StringComparison.OrdinalIgnoreCase)
                   || tStr.Equals("gif", StringComparison.OrdinalIgnoreCase)
                   || tStr.Equals("jpg", StringComparison.OrdinalIgnoreCase))
            {
                return true;

            }

            return false;
        }

        public static bool IsProjectFilePath(string path)
        {
            if (path.EndsWith(".pn", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".libpn", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static string BuildQRUrl(string path)
        {

            string sValue = @"http://qrmwig.naver.com/alcatraz_mig/m.jsp?cmd=4&ver=20&size=Q&&res_format=png&refresh=1&only=y&src=" + path + @"&svccode=0077";

            return sValue;
        }

        public static int CovertColorToInt(System.Windows.Media.Color data)
        {
            return (data.A << 24) | (data.R << 16) | (data.G << 8) | data.B;
        }

        public static System.Windows.Media.Color CovertIntToColor(int data)
        {
            return Color.FromArgb((byte)(data >> 24),
                             (byte)(data >> 16),
                             (byte)(data >> 8),
                             (byte)(data));
        }

        private static string sPreviewTempPath = "";
        public static void SetPreViewTempPath(string sPath)
        {
            sPreviewTempPath = sPath;
        }
        public static string GetPreViewTempPath()
        {
            return sPreviewTempPath;
        }

        public static bool IsNetworkAvailable()
        {
            try
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    return true;
                }
            }
            catch
            {
               
            }

            return false;
        }

        public static string MakeRandomPassword(int length)
        {
            string lowChars = "abcdefghijklmnopqrstuvwxyz";
            string numnberChars = "0123456789";
            string specialCahrs = "~!@#$%^&*()_+|-=/[]{}";

            string tmpStr = "";

            int iRandNum;
            Random rnd = new Random();

            length = (length < 6) ? 6 : length; //password need below 6

            //////////////////////////////////////////////////////////////////////////
            int numLength = rnd.Next(1,3);

            int specCharLength = rnd.Next(1,2);

            int charLength = length - numLength - specCharLength;
            string rndStr = "";

            for (int i = 0; i < numLength; i++)
            {
                iRandNum = rnd.Next(numnberChars.Length);
                tmpStr += numnberChars[iRandNum];
            }

            for (int i = 0; i < charLength; i++)
            {
                iRandNum = rnd.Next(lowChars.Length);
                tmpStr += lowChars[iRandNum];
            }

            for (int i = 0; i < specCharLength; i++)
            {
                iRandNum = rnd.Next(specialCahrs.Length);
                tmpStr += specialCahrs[iRandNum];
            }

            for (int i = 0; i < length; i++)
            {
                int n = rnd.Next(tmpStr.Length);
                rndStr += tmpStr[n];
                tmpStr = tmpStr.Remove(n, 1);
            }

            tmpStr = rndStr;


            return tmpStr;

        }

        public static bool IsErrorPassword(string str)
        {
            if (Regex.IsMatch(str, "^[\x21-\x7e]{6,16}$"))
            {

                return !Regex.IsMatch(str, @"(?=.*[a-zA-Z])
                                       (?=([\x21-\x7e]+)[^a-zA-Z])", RegexOptions.IgnorePatternWhitespace);
            }

            return true;
        }

        public static string GetCurrentPath()
        {
            System.Reflection.Assembly _assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string codeBase = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }

        private static Cursor _RotateCur = null;
        public static Cursor GetRotateCur()
        {
            if (_RotateCur == null)
            {
                _RotateCur = new Cursor(GetCurrentPath() + Path.DirectorySeparatorChar + @"Res\rotate.cur");
            }

            return _RotateCur;
        }

        private static Cursor _FormatCur = null;
        public static Cursor GetFormatCur()
        {
            if (_FormatCur == null)
            {
                _FormatCur = new Cursor(GetCurrentPath() + Path.DirectorySeparatorChar + @"Res\Format.cur");
            }

            return _FormatCur;
        }

        private static Cursor _LockCur = null;
        public static Cursor GetLockCur()
        {
            if (_LockCur == null)
            {
                _LockCur = new Cursor(GetCurrentPath() + Path.DirectorySeparatorChar + @"Res\lock.cur");
            }

            return _LockCur;
        }

        private static Cursor _CopyCur = null;
        public static Cursor GetCopytCur()
        {
            if (_CopyCur == null)
            {
                _CopyCur = new Cursor(GetCurrentPath() + Path.DirectorySeparatorChar + @"Res\copy.cur");
            }

            return _CopyCur;
        }


        public static string GetDefaultFontNameByLanguage()
        {
            int LanID = System.Threading.Thread.CurrentThread.CurrentUICulture.LCID;
            switch (LanID)
            {
                case 0x0409:
                    return "Arial";

                case 0x0804:
                    return "MingLiU";

                case 0x0411:
                    return "Meiryo UI";

                case 0x0412:
                    return "나눔고딕";
            }

            return "Arial";//default return Roboto
        }

        public static StringStatus GetTextStatus(string sText)
        {
            string sFirst = sText.Substring(0, 1);
            string sExceptFirst = sText.Substring(1);
            bool IsLowcase = System.Text.RegularExpressions.Regex.IsMatch(sFirst, "[a-z]");
            bool IsUpcase = System.Text.RegularExpressions.Regex.IsMatch(sFirst, "[A-Z]");

            if (IsLowcase || IsUpcase)
            {
                if (IsUpcase)
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(sExceptFirst, "[a-z]"))
                    {
                        return StringStatus.AllUplow;
                    }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(sExceptFirst, "[A-Z]"))
                    {
                        return StringStatus.FirstAndMiddle;
                    }
                    return StringStatus.First;
                }
                else
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(sExceptFirst, "[A-Z]"))
                    {
                        return StringStatus.Middle;
                    }
                }

                return StringStatus.AllSmall;
            }
            else
            {
                //if (System.Text.RegularExpressions.Regex.IsMatch(sExceptFirst, "[A-Z]"))
                //{
                    return StringStatus.Middle;
                //}

                //return StringStatus.AllSmall;
            }

        }


        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static string GetClientCurrentVersion()
        {
            // Display version format is x.x.x
            string productVersion = ConfigFileManager.CurrentVersion;
            if (String.IsNullOrEmpty(productVersion))
            {
                return String.Empty;
            }
            else
            {
                productVersion = productVersion.Substring(0, productVersion.LastIndexOf("."));
            }

            return productVersion;
        }
    }
}
