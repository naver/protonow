using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using MainToolBar.ViewModel;
using Naver.Compass.Module;
using Naver.Compass.Common.Helper;
using Naver.Compass.Common.CommonBase;
using System.Windows.Media;
using MainToolBar.Common;
using Naver.Compass.Service.Document;
using System.Diagnostics;
using System.Windows.Markup;
using Naver.Compass.Module.Model;
using System.Globalization;


namespace Naver.Compass.Module
{
    public partial class PropertyPageViewModel
    {

        #region public Function

        public void ResetFontsizeValue()
        {
            FontSizeControlData fData = _dataCollection["Font Size"] as FontSizeControlData;

            if (fData != null)
            {
                fData.ReUpDateFontsize();
            }
        }
        #endregion


        #region Private data

        private static object _lockObject = new object();

        private static Dictionary<string, ControlData> _dataCollection = new Dictionary<string, ControlData>();

        public static Dictionary<string, ControlData> DataCollection
        {
            get { return _dataCollection; }
        }

        #endregion

        #region  Binding Property

        public bool IsBoldCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPropertyBoolValueForText("vFontBold");
                }

                return false;
            }

        }

        public bool IsItalicCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPropertyBoolValueForText("vFontItalic");
                }

                return false;
            }

        }

        public bool IsUnderlineCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPropertyBoolValueForText("vFontUnderLine");
                }

                return false;
            }

        }

        public bool IsStrikeThroughCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPropertyBoolValueForText("vFontStrickeThrough");
                }

                return false;
            }

        }

        public bool IsBulletCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBulletBoolValue();
                }

                return false;
            }

        }

        public bool IsTxtAlignLeft
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextHorAligen") == 0);
                }

                return false;
            }

        }

        public bool IsTxtAlignCenter
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextHorAligen") == 1);
                }

                return false;
            }

        }

        public bool IsTxtAlignRight
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextHorAligen") == 2);
                }

                return false;
            }

        }

        public bool IsTxtAlignTop
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextVerAligen") == 3);
                }

                return false;
            }

        }

        public bool IsTxtAlignMiddle
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextVerAligen") == 1);
                }

                return false;
            }

        }

        public bool IsTxtAlignBottom
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextVerAligen") == 4);
                }

                return false;
            }

        }

        public Brush FontColorView
        {
            get
            {
                if (_model != null)
                {
                    return (new SolidColorBrush(_model.GetFontColorValue()));
                }
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
            set
            {

            }
        }

        public StyleColor BackgroundModifyColorView
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBackgroundColorModifyValue();
                }

                return new StyleColor(ColorFillType.Solid, -1);
            }
        }

        public StyleColor BackgroundColorView
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBackgroundColorValue();
                }

                return new StyleColor(ColorFillType.Solid, -1);
            }

        }

        public StyleColor BorderLineColorView
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBorderlineColorValue();
                }
                return new StyleColor(ColorFillType.Solid, -1);
            }

        }

        public bool BackgroundGradientEnable
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBackgroundGradientEnable();
                }
                return false;
            }
        }

        public bool BorderlineGradientEnable
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBorderlineGradientEnable();
                }
                return false;
            }
        }

        #endregion

        #region Event handler

        private void StyChangeHandler(string EventArg)
        {
            if (EventArg == null)
            {
                return;
            }
            if (EventArg.Equals("vFontFamily", StringComparison.Ordinal))
            {
                FontFamilyControlData cData = _dataCollection["Font Face"] as FontFamilyControlData;
                var fontfamilyname = _model.GetPropertyFontFamilyValue("vFontFamily");
                if (!string.IsNullOrEmpty(fontfamilyname))
                {
                    //cData.SelectedFamily = cData.FontFamilies.FirstOrDefault(x => x.Name == fontfamilyname);
                    cData.SetSelectedFamily(cData.FontFamilies.FirstOrDefault(x => x.Name == fontfamilyname));
                }
                else
                {
                    cData.SelectedFamily = null;
                }
            }
            else if (EventArg.Equals("vFontSize", StringComparison.Ordinal))
            {
                FontSizeControlData fData = _dataCollection["Font Size"] as FontSizeControlData;
                var vFontSize = _model.GetPropertyFontSizeValue("vFontSize");
                 var fsize = 0d;
               
                if (double.TryParse(vFontSize, out fsize))
                {
                    fData.SetSelectedSize(fData.FontSizes.FirstOrDefault(x => x.FontSize == fsize));
                }
                else
                {
                    fData.SelectedSize = null;
                }

                fData.DisplaySize = vFontSize;
            }
        }

        private void UpdateBorderLineWidthButtonParameter()
        {
            SplitMenuItemData ButtonData = _dataCollection["BorderLineType"] as SplitMenuItemData;
            if (ButtonData != null)
            {
                int iVaule = _model.GetBorderLineValue("vBorderLinethinck");
                if (iVaule >= 0)
                {
                    GalleryData<BorderLineWidthData> BLData = _dataCollection["BorderLineType Gallery"] as GalleryData<BorderLineWidthData>;
                    if (BLData != null)
                    {
                        foreach (BorderLineWidthData tData in BLData.CategoryDataCollection[0].GalleryItemDataCollection)
                        {
                            if (tData.Width == iVaule)
                            {
                                ButtonData.CommandParameter = tData;
                            }
                        }
                    }
                }
            }
        }
        private void UpdateBorderLineStyleButtonParameter()
        {
            SplitMenuItemData ButtonData = _dataCollection["BorderLinePattern"] as SplitMenuItemData;
            if (ButtonData != null)
            {
                int iVaule = _model.GetBorderLineValue("vBorderlineStyle");
                if (iVaule < 0)
                {
                    ButtonData.CommandParameter = null;
                }
                else
                {
                    GalleryData<BorderLineStyleData> uData = _dataCollection["BorderLinePattern Gallery"] as GalleryData<BorderLineStyleData>;
                    if (uData != null)
                    {
                        foreach (BorderLineStyleData tData in uData.CategoryDataCollection[0].GalleryItemDataCollection)
                        {
                            if (tData.BLStyle == iVaule)
                            {
                                ButtonData.CommandParameter = tData;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateBorderLineWidthSelectedItem()
        {
            GalleryData<BorderLineWidthData> BLData = _dataCollection["BorderLineType Gallery"] as GalleryData<BorderLineWidthData>;
            if (BLData != null)
            {
                int iVaule = _model.GetBorderLineValue("vBorderLinethinck");
                if (iVaule < 0)
                {
                    BLData.SelectedItem = null;
                }
                else
                {
                    foreach (BorderLineWidthData tData in BLData.CategoryDataCollection[0].GalleryItemDataCollection)
                    {
                        if (tData.Width == iVaule)
                        {
                            BLData.SelectedItem = tData;
                        }
                    }
                }
            }
        }

        private void UpdateBorderLineStyleSelectedItem()
        {
            GalleryData<BorderLineStyleData> uData = _dataCollection["BorderLinePattern Gallery"] as GalleryData<BorderLineStyleData>;
            if (uData != null)
            {
                int iVaule = _model.GetBorderLineValue("vBorderlineStyle");
                if (iVaule < 0)
                {
                    uData.SelectedItem = null;
                }
                else
                {
                    foreach (BorderLineStyleData tData in uData.CategoryDataCollection[0].GalleryItemDataCollection)
                    {
                        if (tData.BLStyle == iVaule)
                        {
                            uData.SelectedItem = tData;
                        }
                    }

                }
            }
        }

        private void UpdateSelectedItem()
        {
            GalleryData<StyleColor> Cdata = _dataCollection["Background Color Gallery"] as GalleryData<StyleColor>;
            if (Cdata != null)
            {
                Cdata.SelectedItem = BackgroundColorView;
                Cdata.GradientEnable = BackgroundGradientEnable;
            }

            var Bdata = _dataCollection["Font Color Gallery"] as GalleryData<Brush>;
            if (Bdata != null)
            {
                Bdata.SelectedItem = FontColorView;
            }

            Cdata = _dataCollection["BorderLine Color Gallery"] as GalleryData<StyleColor>;
            if (Cdata != null)
            {
                Cdata.SelectedItem = BorderLineColorView;
                Cdata.GradientEnable = BorderlineGradientEnable;
            }

            UpdateBorderLineStyleSelectedItem();

            UpdateBorderLineWidthSelectedItem();
        }

        private void CanSupportProperty()
        {
            bool IsSupportText = _model.IsSupportProperty_Panel(PropertyOption.Option_Text);
            bool IsSupportBorder = _model.IsSupportProperty_Panel(PropertyOption.Option_Border); ;
            bool IsSupportBackground = _model.IsSupportProperty_Panel(PropertyOption.OPtion_BackColor);
            bool IsSupportArrowStyle = _model.IsSupportProperty_Panel(PropertyOption.Option_LineArrow);

            ComboBoxData cData = _dataCollection["Font Face"] as ComboBoxData;
            cData.IsEnable = IsSupportText;

            cData = _dataCollection["Font Size"] as ComboBoxData;
            cData.IsEnable = IsSupportText;

            SplitFontColorButton spcData = _dataCollection["Font Color"] as SplitFontColorButton;
            spcData.IsEnable = IsSupportText;

            SplitMenuItemData spData = _dataCollection["BorderLineType"] as SplitMenuItemData;
            spData.IsEnable = IsSupportBorder;

            spData = _dataCollection["BorderLinePattern"] as SplitMenuItemData;
            spData.IsEnable = IsSupportBorder;

            //spData = _dataCollection["LineArrowStyle"] as SplitMenuItemData;
            //spData.IsEnable = IsSupportArrowStyle;

            SplitBackgroundColorButton sbBKData = _dataCollection["BackColor"] as SplitBackgroundColorButton;
            sbBKData.IsEnable = IsSupportBackground;

            SplitBordlineColorButton sbBLData = _dataCollection["BorderLineColor"] as SplitBordlineColorButton;
            sbBLData.IsEnable = IsSupportBorder;
        }

        private void SetStyleCmdTarget(IInputElement target)
        {
            //GalleryData<FontFamily> GDate = _dataCollection["Font Face Gallery"] as GalleryData<FontFamily>;
            //if (GDate != null)
            //{
            //    GDate.CmdTarget = target;
            //}

            //GalleryData<double?> dDate = _dataCollection["Font Size Gallery"] as GalleryData<double?>;

            //if (dDate != null)
            //{
            //    dDate.CmdTarget = target;
            //}



            GalleryData<Brush> Bdata = _dataCollection["Background Color Gallery"] as GalleryData<Brush>;
            if (Bdata != null)
            {
                Bdata.CmdTarget = target;
            }

            Bdata = _dataCollection["BorderLine Color Gallery"] as GalleryData<Brush>;
            if (Bdata != null)
            {
                Bdata.CmdTarget = target;
            }



            SplitBackgroundColorButton sbBKData = _dataCollection["BackColor"] as SplitBackgroundColorButton;
            if (sbBKData != null)
            {
                sbBKData.CmdTarget = target;
            }

            SplitBordlineColorButton sbBLData = _dataCollection["BorderLineColor"] as SplitBordlineColorButton;
            if (sbBLData != null)
            {
                sbBLData.CmdTarget = target;
            }

            SplitMenuItemData spData = _dataCollection["BorderLineType"] as SplitMenuItemData;
            if (spData != null)
            {
                spData.CmdTarget = target;
            }

            GalleryData<BorderLineWidthData> BLDate = _dataCollection["BorderLineType Gallery"] as GalleryData<BorderLineWidthData>;
            if (BLDate != null)
            {
                BLDate.CmdTarget = target;
            }

            spData = _dataCollection["BorderLinePattern"] as SplitMenuItemData;
            if (spData != null)
            {
                spData.CmdTarget = target;
            }

            GalleryData<BorderLineStyleData> uDate = _dataCollection["BorderLinePattern Gallery"] as GalleryData<BorderLineStyleData>;
            if (uDate != null)
            {
                uDate.CmdTarget = target;
            }

            //spData = _dataCollection["LineArrowStyle"] as SplitMenuItemData;
            //if (spData != null)
            //{
            //    spData.CmdTarget = target;
            //}

            //GalleryData<LineArrowStyleDate> LADate = _dataCollection["LineArrowStyleGallery"] as GalleryData<LineArrowStyleDate>;
            //if (LADate != null)
            //{
            //    LADate.CmdTarget = target;
            //}

        }

        #endregion

        #region Binding static data

        #region Font Group Model

        public static ControlData Font
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Font";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GroupData Data = new GroupData(Str)
                        {
                            IsEnable = true
                        };
                        _dataCollection[Str] = Data;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData FontFace
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Font Face";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        FontFamilyControlData comboBoxData = new FontFamilyControlData();
                        var listKorean = new List<NamedFontFamily>();
                        var listEnglish = new List<NamedFontFamily>();
                        var listChineseandJapaniese = new List<NamedFontFamily>();
                        var listOthers = new List<NamedFontFamily>();
                        foreach (FontFamily fontFamily in System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(x => x.ToString()))
                        {
                            try
                            {

                                Typeface ltypFace = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                                var formattedText = new FormattedText(
                                        "김은영",
                                        CultureInfo.CurrentCulture,
                                        FlowDirection.LeftToRight,
                                        ltypFace,
                                        15,
                                        Brushes.Black);

                                Geometry textGeometry = formattedText.BuildGeometry(new Point(0, 0));

                                GlyphTypeface lglyphTypeFace;
                                if (ltypFace.TryGetGlyphTypeface(out lglyphTypeFace))// Try to create a GlyphTypeface object from the TypeFace object
                                {
                                    LanguageSpecificStringDictionary NameCollect = fontFamily.FamilyNames;// trigger the exception

                                    if (NameCollect != null)
                                    {
                                        string localizedName = "";
                                        if ((NameCollect.ContainsKey(XmlLanguage.GetLanguage("ko-KR"))))
                                        {
                                            localizedName = NameCollect[XmlLanguage.GetLanguage("ko-KR")];
                                            if (!String.IsNullOrEmpty(localizedName))
                                            //comboBoxData.FontFamilies.Add(new NamedFontFamily { Name = localizedName, FontFamily = fontFamily });
                                            {
                                                listKorean.Add(new NamedFontFamily { Name = localizedName, FontFamily = fontFamily });
                                            }
                                        }
                                        else if (NameCollect.ContainsKey(XmlLanguage.GetLanguage("zh-CN")))
                                        {
                                            localizedName = NameCollect[XmlLanguage.GetLanguage("zh-CN")];
                                            if (!String.IsNullOrEmpty(localizedName))
                                            //comboBoxData.FontFamilies.Add(new NamedFontFamily { Name = localizedName, FontFamily = fontFamily });
                                            {
                                                listChineseandJapaniese.Add(new NamedFontFamily { Name = localizedName, FontFamily = fontFamily });
                                            }
                                        }
                                        else if (NameCollect.ContainsKey(XmlLanguage.GetLanguage("ja-JP")))
                                        {
                                            localizedName = NameCollect[XmlLanguage.GetLanguage("ja-JP")];
                                            if (!String.IsNullOrEmpty(localizedName))
                                            //comboBoxData.FontFamilies.Add(new NamedFontFamily { Name = localizedName, FontFamily = fontFamily });
                                            {
                                                listChineseandJapaniese.Add(new NamedFontFamily { Name = localizedName, FontFamily = fontFamily });
                                            }
                                        }
                                        else if (NameCollect.ContainsKey(XmlLanguage.GetLanguage("en-US")))
                                        {
                                            localizedName = NameCollect[XmlLanguage.GetLanguage("en-US")];
                                            if (!String.IsNullOrEmpty(localizedName))
                                            //comboBoxData.FontFamilies.Add(new NamedFontFamily { Name = localizedName, FontFamily = fontFamily });
                                            {
                                                listEnglish.Add(new NamedFontFamily { Name = localizedName, FontFamily = fontFamily });
                                            }
                                        }
                                        else
                                        {
                                            //comboBoxData.FontFamilies.Add(new NamedFontFamily { Name = fontFamily.Source, FontFamily = fontFamily });
                                            listOthers.Add(new NamedFontFamily { Name = fontFamily.Source, FontFamily = fontFamily });
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("Illegal Font Glyph:" + fontFamily.Source);
                                }

                            }
                            catch (System.Exception ex)
                            {
                                Debug.WriteLine("Illegal Font family name:" + fontFamily.Source);
                            }
                        }

                        comboBoxData.FontFamilies.AddRange(listKorean);
                        comboBoxData.FontFamilies.AddRange(listEnglish);
                        comboBoxData.FontFamilies.AddRange(listChineseandJapaniese);
                        comboBoxData.FontFamilies.AddRange(listOthers);

                        var recentlyFonts = Naver.Compass.Common.Helper.GlobalData.RecentFonts;
                        if (recentlyFonts != null && recentlyFonts.Count > 0)
                        {
                            var matchRecentlyFonts = comboBoxData
                                .FontFamilies
                                .Where(f => recentlyFonts.Contains(f.Name))
                                .Take(5)
                                .OrderByDescending(f => recentlyFonts.IndexOf(f.Name));
                            foreach (var recent in matchRecentlyFonts)
                            {
                                comboBoxData.FontFamilies.Insert(0, new NamedFontFamily { Name = recent.Name, FontFamily = recent.FontFamily, IsRecent = true });
                            }

                            comboBoxData.PerformRecent(comboBoxData);
                        }

                        comboBoxData.Command = FontCommands.Family;
                        _dataCollection[Str] = comboBoxData;
                    }
                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData FontFaceGallery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Font Face Gallery";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GalleryData<FontFamily> galleryData = new GalleryData<FontFamily>()
                        {
                            SelectedItem = SystemFonts.MessageFontFamily,
                        };

                        //GalleryCategoryData<FontFamily> recentlyUsedCategoryData = new GalleryCategoryData<FontFamily>()
                        //{
                        //    Label = GlobalData.FindResource("Ribbon_FontFamily_RecentlyUsedCategory")
                        //};

                        //galleryData.CategoryDataCollection.Add(recentlyUsedCategoryData);

                        GalleryCategoryData<FontFamily> allFontsCategoryData = new GalleryCategoryData<FontFamily>()
                        {
                            // Label = GlobalData.FindResource("Ribbon_FontFamily_ToolTip_AllFontsCategory")
                        };

                        allFontsCategoryData.GalleryItemDataCollection.OrderBy(x => x.Source);
                        foreach (FontFamily fontFamily in System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(x => x.ToString()))
                        {
                            try
                            {
                                Typeface ltypFace = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

                                GlyphTypeface lglyphTypeFace;

                                if (ltypFace.TryGetGlyphTypeface(out lglyphTypeFace))// Try to create a GlyphTypeface object from the TypeFace object
                                {
                                    LanguageSpecificStringDictionary NameCollect = fontFamily.FamilyNames; // trigger the exception

                                    string localizedName = "";
                                    if ((NameCollect.ContainsKey(XmlLanguage.GetLanguage("ko-KR"))))
                                    {
                                        localizedName = NameCollect[XmlLanguage.GetLanguage("ko-KR")];
                                        if (!String.IsNullOrEmpty(localizedName))
                                            allFontsCategoryData.GalleryItemDataCollection.Add(new FontFamily(localizedName));
                                    }
                                    else if (NameCollect.ContainsKey(XmlLanguage.GetLanguage("zh-CN")))
                                    {
                                        localizedName = NameCollect[XmlLanguage.GetLanguage("zh-CN")];
                                        if (!String.IsNullOrEmpty(localizedName))
                                            allFontsCategoryData.GalleryItemDataCollection.Add(new FontFamily(localizedName));
                                    }
                                    else if (NameCollect.ContainsKey(XmlLanguage.GetLanguage("ja-JP")))
                                    {
                                        localizedName = NameCollect[XmlLanguage.GetLanguage("ja-JP")];
                                        if (!String.IsNullOrEmpty(localizedName))
                                            allFontsCategoryData.GalleryItemDataCollection.Add(new FontFamily(localizedName));
                                    }
                                    else
                                    {
                                        allFontsCategoryData.GalleryItemDataCollection.Add(fontFamily);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("dIllegal Font:" + fontFamily.Source);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.WriteLine("Illegal Font:" + fontFamily.Source);
                            }
                        }

                        galleryData.CategoryDataCollection.Add(allFontsCategoryData);

                        Action<FontFamily> ChangeFontFace = delegate(FontFamily parameter)
                        {
                            //UserControlWord wordControl = WordControl;
                            //if (wordControl != null)
                            //{
                            //    wordControl.ChangeFontFace(parameter);

                            //    if (!recentlyUsedCategoryData.GalleryItemDataCollection.Contains(parameter))
                            //    {
                            //        recentlyUsedCategoryData.GalleryItemDataCollection.Add(parameter);
                            //    }
                            //}
                        };

                        Func<FontFamily, bool> CanChangeFontFace = delegate(FontFamily parameter)
                        {
                            //UserControlWord wordControl = WordControl;
                            //if (wordControl != null)
                            //{
                            //    return wordControl.CanChangeFontFace(parameter);
                            //}

                            return true;
                        };

                        Action<FontFamily> PreviewFontFace = delegate(FontFamily parameter)
                        {
                            //UserControlWord wordControl = WordControl;
                            //if (wordControl != null)
                            //{
                            //    wordControl.PreviewFontFace(parameter);
                            //}
                        };

                        Action CancelPreviewFontFace = delegate()
                        {
                            //UserControlWord wordControl = WordControl;
                            //if (wordControl != null)
                            //{
                            //    wordControl.CancelPreviewFontFace();
                            //}
                        };

                        //galleryData.Command = new PreviewDelegateCommand<FontFamily>(ChangeFontFace, CanChangeFontFace, PreviewFontFace, CancelPreviewFontFace);
                        galleryData.Command = FontCommands.Family;

                        _dataCollection[Str] = galleryData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData FontSize
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Font Size";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        FontSizeControlData comboBoxData = new FontSizeControlData();
                        List<double> collect = FontSizeEnumerator.GetSizeList();
                        foreach (double value in collect)
                        {
                            comboBoxData.FontSizes.Add(new NamedFontSize { FontSize = value, Name = value.ToString() });
                        }

                        comboBoxData.Command = FontCommands.Size;
                        _dataCollection[Str] = comboBoxData;
                    }
                    return _dataCollection[Str];
                }
            }
        }
        public static ControlData FontSizeGallery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Font Size Gallery";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        Action<double?> ChangeFontSize = delegate(double? parameter)
                        {
                            //UserControlWord wordControl = WordControl;
                            //if (wordControl != null)
                            //{
                            //    wordControl.ChangeFontSize(parameter);
                            //}
                        };

                        Func<double?, bool> CanChangeFontSize = delegate(double? parameter)
                        {
                            //UserControlWord wordControl = WordControl;
                            //if (wordControl != null)
                            //{
                            //    return wordControl.CanChangeFontSize(parameter);
                            //}

                            return true;
                        };

                        Action<double?> PreviewFontSize = delegate(double? parameter)
                        {
                            //UserControlWord wordControl = WordControl;
                            //if (wordControl != null)
                            //{
                            //    wordControl.PreviewFontSize(parameter);
                            //}
                        };

                        Action CancelPreviewFontSize = delegate()
                        {
                            //UserControlWord wordControl = WordControl;
                            //if (wordControl != null)
                            //{
                            //    wordControl.CancelPreviewFontSize();
                            //}
                        };

                        GalleryData<double?> galleryData = new GalleryData<double?>()
                        {
                            Command = FontCommands.Size,

                        };

                        GalleryCategoryData<double?> galleryCategoryData = new GalleryCategoryData<double?>();

                        List<double> collect = FontSizeEnumerator.GetSizeList();

                        foreach (double value in collect)
                        {
                            galleryCategoryData.GalleryItemDataCollection.Add(value);
                        }

                        galleryData.CategoryDataCollection.Add(galleryCategoryData);
                        galleryData.Command = FontCommands.Size;
                        _dataCollection[Str] = galleryData;

                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData FontColor
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Font Color";

                    if (!_dataCollection.ContainsKey(Str))
                    {

                        SplitFontColorButton splitMenuItemData = SplitFontColorButton.GetInstance();
                        splitMenuItemData.Command = FontCommands.Color;
                        //{
                        //    Command = FontCommands.Color,
                        //};

                        _dataCollection[Str] = splitMenuItemData;
                    }

                    return _dataCollection[Str];

                }
            }
        }

        public static ControlData BorderLineColorGallery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "BorderLine Color Gallery";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GalleryData<StyleColor> galleryData = new GalleryData<StyleColor>()
                        {
                            SmallImage = new Uri(CommonData.RibbonFontColorSmall, UriKind.Relative),
                            //Command = new PreviewDelegateCommand<Brush>(ChangeFontColor, CanChangeFontColor, PreviewFontColor, CancelPreviewFontColor),
                            //SelectedItem = SystemColors.ControlBrush,
                        };

                        galleryData.Command = BorderCommands.BorderLineColor;


                        _dataCollection[Str] = galleryData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData BackColorGallery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Background Color Gallery";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GalleryData<StyleColor> galleryData = new GalleryData<StyleColor>()
                        {
                            SmallImage = new Uri(CommonData.RibbonFontColorSmall, UriKind.Relative),
                            //Command = new PreviewDelegateCommand<Brush>(ChangeFontColor, CanChangeFontColor, PreviewFontColor, CancelPreviewFontColor),
                            //SelectedItem = SystemColors.ControlBrush,
                        };


                        galleryData.Command = BorderCommands.BackGround;

                        _dataCollection[Str] = galleryData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData FontColorGallery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Font Color Gallery";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GalleryData<StyleColor> galleryData = new GalleryData<StyleColor>()
                        {
                            SmallImage = new Uri(CommonData.RibbonFontColorSmall, UriKind.Relative),
                            //Command = new PreviewDelegateCommand<Brush>(ChangeFontColor, CanChangeFontColor, PreviewFontColor, CancelPreviewFontColor),
                            // SelectedItem = SystemColors.ControlBrush,
                        };


                        galleryData.Command = FontCommands.Color;

                        _dataCollection[Str] = galleryData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData MoreColors
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "_More Colors";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        _dataCollection[Str] = new MenuItemData()
                        {
                            Label = Str,
                            SmallImage = new Uri(CommonData.RibbonMoreColorSmall, UriKind.Relative),
                            // Command = new DelegateCommand(DefaultExecuted, DefaultCanExecute),
                        };
                    }

                    return _dataCollection[Str];
                }
            }
        }

        #endregion Font Group Model

        #region Paragraph Group Model

        public static ControlData BackColor
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "BackColor";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        SplitBackgroundColorButton splitButtonData = SplitBackgroundColorButton.GetInstance();

                        splitButtonData.Command = BorderCommands.BackGround;
                        
                        _dataCollection[Str] = splitButtonData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData BorderLineColor
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "BorderLineColor";

                    if (!_dataCollection.ContainsKey(Str))
                    {

                        SplitBordlineColorButton splitButtonData = SplitBordlineColorButton.GetInstance();
                        splitButtonData.Command = BorderCommands.BorderLineColor;

                        _dataCollection[Str] = splitButtonData;
                    }

                    return _dataCollection[Str];
                }
            }
        }
        public static ControlData LineArrowStyle
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "LineArrowStyle";

                    if (!_dataCollection.ContainsKey(Str))
                    {

                        SplitMenuItemData splitButtonData = new SplitMenuItemData()
                        {
                            IsCheckable = false,
                            IsVerticallyResizable = false,
                            IsHorizontallyResizable = false,
                            Command = BorderCommands.LineArrowStyle,
                            //KeyTip = "U",
                        };
                        _dataCollection[Str] = splitButtonData;
                    }

                    return _dataCollection[Str];
                }
            }

        }

        public static ControlData LineArrowStyleGallery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "LineArrowStyleGallery";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GalleryData<LineArrowStyleDate> galleryData = new GalleryData<LineArrowStyleDate>()
                        {

                        };

                        GalleryCategoryData<LineArrowStyleDate> BorderLineTypeLibraryCategoryData = new GalleryCategoryData<LineArrowStyleDate>()
                        {

                        };

                        LineArrowStyleDate tDate = new LineArrowStyleDate(new Uri("../Images/None.png", UriKind.Relative), ArrowStyle.None);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);

                        tDate = new LineArrowStyleDate(new Uri("../Images/NoneArrow.png", UriKind.Relative), ArrowStyle.NoneArrow);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("../Images/ArrowNone.png", UriKind.Relative), ArrowStyle.ArrowNone);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("../Images/Arrow.png", UriKind.Relative), ArrowStyle.ArrowArrow);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);

                        tDate = new LineArrowStyleDate(new Uri("../Images/NoneOpenarrow.png", UriKind.Relative), ArrowStyle.NoneOpen);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("../Images/OpenarrowNone.png", UriKind.Relative), ArrowStyle.OpenNone);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("../Images/Opearrow.png", UriKind.Relative), ArrowStyle.OpenOpen);

                        tDate = new LineArrowStyleDate(new Uri("../Images/NoneStealtharrow.png", UriKind.Relative), ArrowStyle.NoneStealth);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("../Images/StealtharrowNone.png", UriKind.Relative), ArrowStyle.StealthNone);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("../Images/StealthArrow.png", UriKind.Relative), ArrowStyle.StealthStealth);


                        galleryData.CategoryDataCollection.Add(BorderLineTypeLibraryCategoryData);

                        //Func<Uri, bool> galleryCommandCanExecute = delegate(Uri parameter)
                        //{
                        //    return true;
                        //};

                        // galleryData.Command = new DelegateCommand<Uri>(galleryCommandExecuted, galleryCommandCanExecute);
                        galleryData.Command = BorderCommands.LineArrowStyle;
                        _dataCollection[Str] = galleryData;
                    }

                    return _dataCollection[Str];
                }
            }

        }


        public static ControlData BorderLineType
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "BorderLineType";

                    if (!_dataCollection.ContainsKey(Str))
                    {

                        SplitMenuItemData splitButtonData = new SplitMenuItemData()
                        {
                            IsCheckable = false,
                            IsVerticallyResizable = false,
                            IsHorizontallyResizable = false,
                            Command = BorderCommands.BorderLineThinck,
                            //KeyTip = "U",
                        };
                        _dataCollection[Str] = splitButtonData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData BorderLineTypeGallery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "BorderLineType Gallery";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GalleryData<BorderLineWidthData> galleryData = new GalleryData<BorderLineWidthData>()
                        {

                        };

                        GalleryCategoryData<BorderLineWidthData> BorderLineTypeLibraryCategoryData = new GalleryCategoryData<BorderLineWidthData>()
                        {

                        };

                        BorderLineWidthData tDate = new BorderLineWidthData(new Uri(DataDefine.BorderLineStyle1, UriKind.Relative), 0);
                        tDate.ShowText = true;
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(DataDefine.BorderLineStyle1, UriKind.Relative), 1);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(DataDefine.BorderLineStyle2, UriKind.Relative), 2);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(DataDefine.BorderLineStyle3, UriKind.Relative), 3);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(DataDefine.BorderLineStyle4, UriKind.Relative), 4);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(DataDefine.BorderLineStyle5, UriKind.Relative), 5);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);


                        galleryData.CategoryDataCollection.Add(BorderLineTypeLibraryCategoryData);

                        //Func<Uri, bool> galleryCommandCanExecute = delegate(Uri parameter)
                        //{
                        //    return true;
                        //};

                        // galleryData.Command = new DelegateCommand<Uri>(galleryCommandExecuted, galleryCommandCanExecute);
                        galleryData.Command = BorderCommands.BorderLineThinck;
                        _dataCollection[Str] = galleryData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData BorderLinePattern
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "BorderLinePattern";

                    if (!_dataCollection.ContainsKey(Str))
                    {

                        SplitMenuItemData splitButtonData = new SplitMenuItemData()
                        {
                            SmallImage = new Uri(CommonData.RibbonBorderLinePattern, UriKind.Relative),

                            IsCheckable = false,

                            IsVerticallyResizable = false,
                            IsHorizontallyResizable = false,
                            Command = BorderCommands.BorderLinePattern,
                            //KeyTip = "U",
                        };
                        _dataCollection[Str] = splitButtonData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData BorderLinePatternGallery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "BorderLinePattern Gallery";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GalleryData<BorderLineStyleData> galleryData = new GalleryData<BorderLineStyleData>()
                        {
                        };

                        GalleryCategoryData<BorderLineStyleData> BorderLinePatternLibraryCategoryData = new GalleryCategoryData<BorderLineStyleData>()
                        {

                        };

                        BorderLineStyleData tData = new BorderLineStyleData(new Uri(DataDefine.BorderLinePatterSolid, UriKind.Relative), (int)LineStyle.Solid);
                        BorderLinePatternLibraryCategoryData.GalleryItemDataCollection.Add(tData);
                        tData = new BorderLineStyleData(new Uri(DataDefine.BorderLinePatterDot, UriKind.Relative), (int)LineStyle.Dot);
                        BorderLinePatternLibraryCategoryData.GalleryItemDataCollection.Add(tData);
                        tData = new BorderLineStyleData(new Uri(DataDefine.BorderLinePatterDot2, UriKind.Relative), (int)LineStyle.DashDot);
                        BorderLinePatternLibraryCategoryData.GalleryItemDataCollection.Add(tData);
                        tData = new BorderLineStyleData(new Uri(DataDefine.BorderLinePatterDouble, UriKind.Relative), (int)LineStyle.DashDotDot);
                        BorderLinePatternLibraryCategoryData.GalleryItemDataCollection.Add(tData);

                        galleryData.CategoryDataCollection.Add(BorderLinePatternLibraryCategoryData);

                        galleryData.Command = BorderCommands.BorderLinePattern;
                        _dataCollection[Str] = galleryData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        #endregion Paragraph Group Model

        #endregion

    }
}
