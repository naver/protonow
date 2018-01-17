using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Naver.Compass.Module.Model
{
    public class NamedFontFamily : ViewModelBase
    {
        public string Name { get; set; }
        public FontFamily FontFamily { get; set; }
        public bool IsRecent { get; set; }

        private bool _showSplitLine;

        public bool ShowSplitLine
        {
            get { return _showSplitLine; }
            set
            {
                if (_showSplitLine != value)
                {
                    _showSplitLine = value;
                    this.FirePropertyChanged("ShowSplitLine");
                }
            }
        }
        
    }

    public class NamedFontSize
    {
        public string Name { get; set; }
        public double FontSize { get; set; }
    }
}
