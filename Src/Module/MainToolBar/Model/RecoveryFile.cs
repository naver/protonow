using Naver.Compass.Common.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module.Model
{
    public class RecoveryInfo
    {
        public List<string> DocsClosedUnGracefully { get; set; }
        public List<RecoveryFile> RecoveryFiles { get; set; }
        public RecoveryInfo()
        {
            RecoveryFiles = new List<RecoveryFile>();
            DocsClosedUnGracefully = new List<string>();
        }
    }

    public class RecoveryFile
    {
        public Guid Guid { get; set; }
        public DateTime CreateTime { get; set; }
        public RecoveryType Type { get; set; }
        public string Filename { get; set; }
        public string Location { get; set; }
        public int DuplicateNameIndex { get; set; }
        public string FileType { get; set; }

        public string FullFilename
        {
            get
            {
                if (DuplicateNameIndex == 0)
                {
                    return string.Format("{0}_Recovered file{1}", Filename, string.IsNullOrEmpty(FileType) ? ".pn" : FileType);
                }
                else
                {
                    return string.Format("{0}_{1}_Recovered file{2}", Filename, DuplicateNameIndex, string.IsNullOrEmpty(FileType) ? ".pn" : FileType);
                }
            }
        }

        public string LocalizedTime
        {
            get
            {
                var culture = GlobalData.Culture;
                return CreateTime.ToString("f", new CultureInfo(culture));
            }
        }

        public string GetFullPath()
        {
            return Path.Combine(Location, FullFilename);
        }

        public string VersionLabel
        {
            get
            {
                if (Type == RecoveryType.AutoSave)
                {

                    return GlobalData.FindResource("Recovery_LastAutosave");
                }
                else
                {
                    return GlobalData.FindResource("Recovery_UserSaved");
                }
            }
        }
    }

    public enum RecoveryType
    {
        AutoSave,
        UserSave
    }
}
