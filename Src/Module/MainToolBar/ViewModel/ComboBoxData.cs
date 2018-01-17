using Naver.Compass.Common.CommonBase;
using Naver.Compass.Module.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MainToolBar.ViewModel
{
    public class ComboBoxData : MenuButtonData, INotifyPropertyChanged
    {
        public string sText
        {
            get
            {
                return _sText;
            }

            set
            {
                if (_sText != value)
                {
                    _sText = value;
                    FirePropertyChanged("sText");
                }
            }
        }
        private string _sText;
    }

    public class FontFamilyControlData : ComboBoxData
    {
        private RangeObservableCollection<NamedFontFamily> _fontFamilies;

        public RangeObservableCollection<NamedFontFamily> FontFamilies
        {
            get { return _fontFamilies; }
            set
            {
                if (_fontFamilies != value)
                {
                    _fontFamilies = value;
                    FirePropertyChanged("FontFamilies");
                }
            }
        }

        private bool _isSettingFamily;
        private NamedFontFamily _selectedFamily;

        public NamedFontFamily SelectedFamily
        {
            get { return _selectedFamily; }
            set
            {
                if (_selectedFamily != value && !_isSettingFamily)
                {
                    _isSettingFamily = true;
                    this.SetFontStaticFontFamilies(value, this);
                    _selectedFamily = value;
                    FirePropertyChanged("SelectedFamily");
                    if (_selectedFamily != null)
                    {
                        if (this.Command.CanExecute(_selectedFamily.Name))
                        {
                            this.Command.Execute(_selectedFamily.Name);
                        }
                    }

                    _isSettingFamily = false;
                }
            }
        }

        private void SetFontStaticFontFamilies(NamedFontFamily value, FontFamilyControlData fontdata)
        {
            if (value != null && fontdata != null)
            {
                var matchFont = fontdata.FontFamilies.Where(f => f.Name == value.Name && f.IsRecent).FirstOrDefault();
                if (matchFont != null)
                {
                    var index = fontdata.FontFamilies.IndexOf(matchFont);
                    if (index != 0)
                    {
                        fontdata.FontFamilies.Move(index, 0);
                        this.PerformRecent(fontdata);
                        this.SaveRecentFont();
                    }
                }
                else
                {
                    if (fontdata.FontFamilies.Count(f => f.IsRecent) >= 5)
                    {
                        fontdata.FontFamilies.RemoveAt(4);
                    }

                    fontdata.FontFamilies.Insert(0, new NamedFontFamily { Name = value.Name, IsRecent = true, FontFamily = value.FontFamily });
                    this.PerformRecent(fontdata);
                    this.SaveRecentFont();
                }
            }
        }

        private void SaveRecentFont()
        {
            if (this == RibbonViewModel.FontFace)
            {
                var recentFonts = new System.Collections.Specialized.StringCollection();
                recentFonts.AddRange(this.FontFamilies.Where(f => f.IsRecent).Select(x => x.Name).ToArray());
                Naver.Compass.Common.Helper.GlobalData.RecentFonts = recentFonts;
            }
        }

        public void PerformRecent(FontFamilyControlData fontdata)
        {
            foreach (var f in fontdata.FontFamilies)
            {
                if (f.ShowSplitLine)
                {
                    f.ShowSplitLine = false;
                }
            }

            var lastRecent = fontdata.FontFamilies.LastOrDefault(f => f.IsRecent);
            if (lastRecent != null)
            {
                lastRecent.ShowSplitLine = true;
            }
        }

        public FontFamilyControlData()
        {
            _fontFamilies = new RangeObservableCollection<NamedFontFamily>();
        }

        public void SetSelectedFamily(NamedFontFamily namedFontFamily)
        {
            if (_selectedFamily != namedFontFamily)
            {
                if (!_isSettingFamily && _selectedFamily != null)
                {
                    _isSettingFamily = true;
                    this.SetFontStaticFontFamilies(namedFontFamily, this);
                    _isSettingFamily = false;
                }
                _selectedFamily = namedFontFamily;
                FirePropertyChanged("SelectedFamily");

            }
        }
    }

    public class FontSizeControlData : ComboBoxData
    {
        private List<NamedFontSize> _fontSizes;

        public List<NamedFontSize> FontSizes
        {
            get { return _fontSizes; }
            set
            {
                if (_fontSizes != value)
                {
                    _fontSizes = value;
                    FirePropertyChanged("FontSizes");
                }
            }
        }

        private string _displaySize = "";
        public string DisplaySize
        {
            get { return _displaySize; }

            set
            {
                if (_displaySize != value)
                {
                    _displaySize = value;
                    FirePropertyChanged("DisplaySize");
                }
            }
        }

        private NamedFontSize _selectedSize;

        public NamedFontSize SelectedSize
        {
            get { return _selectedSize; }
            set
            {
                if (_selectedSize != value)
                {
                    _selectedSize = value;
                    FirePropertyChanged("SelectedSize");
                    if (value != null)
                    {
                        if (this.Command.CanExecute(_selectedSize.FontSize))
                        {
                            this.Command.Execute(_selectedSize.FontSize);
                        }
                    }
                }
            }
        }

        public FontSizeControlData()
        {
            _fontSizes = new List<NamedFontSize>();
        }

        public void SetSelectedSize(NamedFontSize size)
        {
            if (_selectedSize != size)
            {
                _selectedSize = size;
                FirePropertyChanged("SelectedSize");
            }
        }

        public void ReUpDateFontsize()
        {
            FirePropertyChanged("DisplaySize");
        }
    }
}
