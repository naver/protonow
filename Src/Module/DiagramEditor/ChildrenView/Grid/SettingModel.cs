using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common;
using System.Drawing;

namespace Naver.Compass.Module
{
    public class SettingModel
    {
        private static SettingModel _instance;
        public static SettingModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingModel();
            }
            return _instance;
        }

        #region property:Grid
        public bool IsShowGrid { get; set; }
        public bool IsSnapToGrid { get; set; }
        public string GridSize { get; set; }
        public bool IsLineType { get; set; }
        public StyleColor GridColor { get; set; }

        #endregion

        #region property: Guides

        public bool IsShowGlobalGuides { get; set; }
        public bool IsShowPageGuides { get; set; }
        public bool IsSnapToGuides { get; set; }
        public bool IsLockGuides { get; set; }

        public StyleColor GlobalGuideColor { get; set; }
        public StyleColor LocalGuideColor { get; set; }

        #endregion

        public void InitData()
        {
            IsShowGrid = GlobalData.IsShowGrid;
            IsSnapToGrid = GlobalData.IsSnapToGrid;
            GridSize = GlobalData.GRID_SIZE.ToString();
            IsLineType = GlobalData.IsLineType;
            GridColor = new StyleColor(ColorFillType.Solid, GlobalData.GridColor.ToArgb());

            IsShowGlobalGuides = GlobalData.IsShowGlobalGuide;
            IsShowPageGuides = GlobalData.IsShowPageGuide;
            IsSnapToGuides = GlobalData.IsSnapToGuide;
            IsLockGuides = GlobalData.IsLockGuides;
            GlobalGuideColor = new StyleColor(ColorFillType.Solid, GlobalData.GlobalGuideColor.ToArgb());
            LocalGuideColor = new StyleColor(ColorFillType.Solid, GlobalData.LocalGuideColor.ToArgb());

        }

        public void Save()
        {
            GlobalData.IsShowGrid = IsShowGrid;
            GlobalData.IsSnapToGrid = IsSnapToGrid;
            GlobalData.GRID_SIZE = Convert.ToInt16(GridSize);
            GlobalData.IsLineType = IsLineType;
            GlobalData.GridColor = Color.FromArgb(GridColor.ARGB);

            GlobalData.IsShowGlobalGuide = IsShowGlobalGuides;
            GlobalData.IsShowPageGuide = IsShowPageGuides;
            GlobalData.IsSnapToGuide = IsSnapToGuides;
            GlobalData.IsLockGuides = IsLockGuides;
            GlobalData.GlobalGuideColor = Color.FromArgb(GlobalGuideColor.ARGB);
            GlobalData.LocalGuideColor = Color.FromArgb(LocalGuideColor.ARGB);
        }
    }
}
