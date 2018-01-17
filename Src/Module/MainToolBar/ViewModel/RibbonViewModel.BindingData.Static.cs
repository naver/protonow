using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using System.Windows.Media;
using Naver.Compass.Common.Helper;
using MainToolBar.Common;
using Naver.Compass.Service.Document;
using System.Diagnostics;
using System.Windows.Markup;
using Naver.Compass.Module.Model;
using System.Globalization;


namespace MainToolBar.ViewModel
{
    partial class RibbonViewModel
    {
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
                            SmallImage = new Uri(CommonData.RibbonFontSmall, UriKind.Relative),
                            LargeImage = new Uri(CommonData.RibbonFontLarge, UriKind.Relative),
                            //KeyTip = "ZF",
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

                        var recentlyFonts= Naver.Compass.Common.Helper.GlobalData.RecentFonts;
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
                            //Label = GlobalData.FindResource("Ribbon_FontFamily_ToolTip_AllFontsCategory")
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
                                    LanguageSpecificStringDictionary NameCollect = fontFamily.FamilyNames;// trigger the exception

                                    if (NameCollect != null)
                                    {
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
                            //SelectedItem = 11,
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
                        //SplitFontColorButton splitMenuItemData = new SplitFontColorButton()
                        //{
                        //    SmallImage = new Uri(CommonData.RibbonFontColorSmall, UriKind.Relative),

                        //    ToolTipFooterImage = new Uri(CommonData.RibbonHelpSmall, UriKind.Relative),

                        //    Command = FontCommands.Color,
                        //};

                        SplitFontColorButton splitMenuItemData = SplitFontColorButton.GetInstance();
                        splitMenuItemData.Command = FontCommands.Color;

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
                            Command = new DelegateCommand(DefaultExecuted, DefaultCanExecute),
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

                        LineArrowStyleDate tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/None.png", UriKind.Relative), ArrowStyle.None);
                        tDate.ShowText = true;
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);

                        tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/NoneArrow.png", UriKind.Relative), ArrowStyle.NoneArrow);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/ArrowNone.png", UriKind.Relative), ArrowStyle.ArrowNone);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/Arrow.png", UriKind.Relative), ArrowStyle.ArrowArrow);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);

                        tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/NoneOpenarrow.png", UriKind.Relative), ArrowStyle.NoneOpen);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/OpenarrowNone.png", UriKind.Relative), ArrowStyle.OpenNone);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/Opearrow.png", UriKind.Relative), ArrowStyle.OpenOpen);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);

                        //tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/NoneStealtharrow.png", UriKind.Relative), ArrowStyle.NoneStealth);
                        //BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        //tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/StealtharrowNone.png", UriKind.Relative), ArrowStyle.StealthNone);
                        //BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        //tDate = new LineArrowStyleDate(new Uri("ICON/Arrow/StealthArrow.png", UriKind.Relative), ArrowStyle.StealthStealth);


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
                            SmallImage = new Uri(CommonData.RibbonBorderLineStyle, UriKind.Relative),
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
                        BorderLineWidthData tDate = new BorderLineWidthData(new Uri(CommonData.BorderLineStyle1, UriKind.Relative), 0);
                        tDate.ShowText = true;
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(CommonData.BorderLineStyle1, UriKind.Relative), 1);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(CommonData.BorderLineStyle2, UriKind.Relative), 2);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(CommonData.BorderLineStyle3, UriKind.Relative), 3);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(CommonData.BorderLineStyle4, UriKind.Relative), 4);
                        BorderLineTypeLibraryCategoryData.GalleryItemDataCollection.Add(tDate);
                        tDate = new BorderLineWidthData(new Uri(CommonData.BorderLineStyle5, UriKind.Relative), 5);
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
                        BorderLineStyleData tData = new BorderLineStyleData(new Uri(CommonData.BorderLinePatterSolid, UriKind.Relative), (int)LineStyle.Solid);
                        BorderLinePatternLibraryCategoryData.GalleryItemDataCollection.Add(tData);
                        tData = new BorderLineStyleData(new Uri(CommonData.BorderLinePatterDot, UriKind.Relative), (int)LineStyle.Dot);
                        BorderLinePatternLibraryCategoryData.GalleryItemDataCollection.Add(tData);
                        tData = new BorderLineStyleData(new Uri(CommonData.BorderLinePatterDot2, UriKind.Relative), (int)LineStyle.DashDot);
                        BorderLinePatternLibraryCategoryData.GalleryItemDataCollection.Add(tData);
                        tData = new BorderLineStyleData(new Uri(CommonData.BorderLinePatterDouble, UriKind.Relative), (int)LineStyle.DashDotDot);
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

        #region Windows model

        //public static ControlData Windows
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "Window";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                GroupData Data = new GroupData(Str)
        //                {
        //                    SmallImage = new Uri(CommonData.RibbonFindSmall, UriKind.Relative),
        //                    LargeImage = new Uri(CommonData.RibbonFindLarge, UriKind.Relative),
        //                    ToolTipTitle = GlobalData.FindResource("Toolbar_HomePanesTooltip"),
        //                    ToolTipDescription = GlobalData.FindResource("Toolbar_HomePanesTooltipDes"),
        //                    //KeyTip = "ZN11",
        //                };
        //                _dataCollection[Str] = Data;
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}

        //public static ControlData Panes
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "Panes";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                string TooTipTitle = GlobalData.FindResource("Toolbar_HomePanesTooltip");
        //                string ToolTipDescription = GlobalData.FindResource("Toolbar_HomePanesTooltipDes");
        //                //string DropDownToolTipDescription = "Find and select specific text, formatting or other type of information within the document.";
        //                //string DropDownToolTipFooter = "You can also replace the information with new text or formatting.";

        //                SplitButtonData FindSplitButtonData = new SplitButtonData()
        //                {
        //                    Label = GlobalData.FindResource("Toolbar_HomePanes"),
        //                    // SmallImage = new Uri(CommonData.RibbonFindSmall, UriKind.Relative),
        //                    ToolTipTitle = TooTipTitle,
        //                    ToolTipDescription = ToolTipDescription,
        //                    Command = new DelegateCommand(DefaultExecuted, DefaultCanExecute),
        //                    //KeyTip = "FD",
        //                };
        //                FindSplitButtonData.DropDownButtonData.ToolTipTitle = TooTipTitle;
        //                FindSplitButtonData.DropDownButtonData.ToolTipDescription = ToolTipDescription;
        //                // FindSplitButtonData.DropDownButtonData.ToolTipFooterDescription = DropDownToolTipFooter;
        //                FindSplitButtonData.DropDownButtonData.Command = new DelegateCommand(delegate { });
        //                _dataCollection[Str] = FindSplitButtonData;
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}

        //public static ControlData Sitmap
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "Sitmap";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                _dataCollection[Str] = new MenuItemData()
        //                {
        //                    Name = Str,
        //                    Label = GlobalData.FindResource("Sitemap_Title"),
        //                    IsChecked = true,
        //                    Command = new DelegateCommand<MenuItemData>(PaneMenuItemDefaultExecute, PaneMenuItemDefaultCanExecute),
        //                };
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}
        //public static ControlData Widgets
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "Widgets";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                _dataCollection[Str] = new MenuItemData()
        //                {
        //                    Name = Str,
        //                    Label = GlobalData.FindResource("widgets_Title"),
        //                    IsChecked = true,
        //                    Command = new DelegateCommand<MenuItemData>(PaneMenuItemDefaultExecute, PaneMenuItemDefaultCanExecute),
        //                };
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}
        //public static ControlData Masters
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "Masters";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                _dataCollection[Str] = new MenuItemData()
        //                {
        //                    Name = Str,
        //                    Label = GlobalData.FindResource("Masters_Title"),
        //                    IsChecked = true,
        //                    Command = new DelegateCommand<MenuItemData>(PaneMenuItemDefaultExecute, PaneMenuItemDefaultCanExecute),
        //                };
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}
        //public static ControlData PageProperty
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "PageProperty";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                _dataCollection[Str] = new MenuItemData()
        //                {
        //                    Name = Str,
        //                    Label = GlobalData.FindResource("PageProp_Title"),
        //                    IsChecked = true,
        //                    Command = new DelegateCommand<MenuItemData>(PaneMenuItemDefaultExecute, PaneMenuItemDefaultCanExecute),
        //                };
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}
        //public static ControlData Interaction
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "Interaction";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                _dataCollection[Str] = new MenuItemData()
        //                {
        //                    Name = Str,
        //                    Label = GlobalData.FindResource("Interaction_Title"),
        //                    IsChecked = true,
        //                    Command = new DelegateCommand<MenuItemData>(PaneMenuItemDefaultExecute, PaneMenuItemDefaultCanExecute),
        //                };
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}
        //public static ControlData WidgetProperty
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "WidgetProperty";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                _dataCollection[Str] = new MenuItemData()
        //                {
        //                    Name = Str,
        //                    Label = GlobalData.FindResource("WidgetProp_Title"),
        //                    IsChecked = true,
        //                    Command = new DelegateCommand<MenuItemData>(PaneMenuItemDefaultExecute, PaneMenuItemDefaultCanExecute),
        //                };
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}
        //public static ControlData WidgetManager
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "WidgetManager";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                _dataCollection[Str] = new MenuItemData()
        //                {
        //                    Name = Str,
        //                    Label = GlobalData.FindResource("WidgetManager_Title"),
        //                    IsChecked = true,
        //                    Command = new DelegateCommand<MenuItemData>(PaneMenuItemDefaultExecute, PaneMenuItemDefaultCanExecute),
        //                };
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}

        //private static void PaneMenuItemDefaultExecute(MenuItemData menuItemData)
        //{
        //    MenuItemData itemData = menuItemData;
        //    itemData.IsChecked = !itemData.IsChecked;
        //    ActivityPane pane = new ActivityPane();
        //    pane.Name = itemData.Name;
        //    pane.bOpen = itemData.IsChecked;
        //    eventAggregation.GetEvent<OpenPanesEvent>().Publish(pane);
        //    return;
        //}

        //private static bool PaneMenuItemDefaultCanExecute(MenuItemData menuItemData)
        //{
        //    return true;
        //}

        #endregion

        #region Project
        //public static ControlData Project
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "Project";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                GroupData Data = new GroupData(Str)
        //                {
        //                    Label = GlobalData.FindResource("Toolbar_HomeProject"),
        //                    ToolTipTitle = GlobalData.FindResource("Toolbar_HomeSettingTooltip"),
        //                    ToolTipDescription = GlobalData.FindResource("Toolbar_HomeSettingTooltipDes"),
        //                    SmallImage = new Uri(CommonData.RibbonFindSmall, UriKind.Relative),
        //                    LargeImage = new Uri(CommonData.RibbonFindLarge, UriKind.Relative),
        //                    //KeyTip = "ZN11",
        //                };
        //                _dataCollection[Str] = Data;
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}

        //public static ControlData ProjectSetting
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "ProjectSetting";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                SplitButtonData HtmlSplitButtonData = new SplitButtonData()
        //                {
        //                    Label = GlobalData.FindResource("Toolbar_HomeSetting"), //GlobalData.FindResource("Toolbar_HomePanes"),
        //                    ToolTipTitle = GlobalData.FindResource("Toolbar_HomeSettingTooltip"),
        //                    ToolTipDescription = GlobalData.FindResource("Toolbar_HomeSettingTooltipDes"),
        //                    Command = new DelegateCommand(DefaultExecuted, ProjectSettingCanExecute),
        //                    //KeyTip = "FD",
        //                };
        //                HtmlSplitButtonData.DropDownButtonData.Command = new DelegateCommand(DefaultExecuted, ProjectSettingCanExecute);
        //                HtmlSplitButtonData.DropDownButtonData.ToolTipTitle = GlobalData.FindResource("Toolbar_HomeSettingTooltip");
        //                HtmlSplitButtonData.DropDownButtonData.ToolTipDescription = GlobalData.FindResource("Toolbar_HomeSettingTooltipDes");
        //                _dataCollection[Str] = HtmlSplitButtonData;
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}

        //private static bool ProjectSettingCanExecute()
        //{
        //    return beOpenDocument;
        //}

        //public static ControlData PageNoteField
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "PageNoteField";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                // string ReplaceToolTipTitle = "Replace (Ctrl+H)";
        //                // string ReplaceToolTipDescription = "Replace text in the document.";

        //                ButtonData ReplaceButtonData = new ButtonData()
        //                {
        //                    Label = GlobalData.FindResource("Toolbar_HomePageNoteField"),
        //                    SmallImage = new Uri(CommonData.RibbonReplace, UriKind.Relative),
        //                    Command = new DelegateCommand(PageNoteFieldExecuted, PageNoteFieldCanExecute),
        //                    //KeyTip = "R",
        //                };
        //                _dataCollection[Str] = ReplaceButtonData;
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}

        //private static void PageNoteFieldExecuted()
        //{
        //    eventAggregation.GetEvent<OpenDialogEvent>().Publish(DialogType.PageNoteField);
        //}

        //private static bool PageNoteFieldCanExecute()
        //{
        //    return beOpenDocument;
        //}

        //public static ControlData AdaptiveViewSet
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "AdaptiveViewSet";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                // string ReplaceToolTipTitle = "Replace (Ctrl+H)";
        //                // string ReplaceToolTipDescription = "Replace text in the document.";

        //                ButtonData ReplaceButtonData = new ButtonData()
        //                {
        //                    Label = GlobalData.FindResource("Responsive_Menu"),
        //                    SmallImage = new Uri(CommonData.RibbonReplace, UriKind.Relative),
        //                    Command = new DelegateCommand(AdaptiveViewExecuted, AdaptiveViewCanExecute),
        //                    //KeyTip = "R",
        //                };
        //                _dataCollection[Str] = ReplaceButtonData;
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}

        //private static void AdaptiveViewExecuted()
        //{
        //    eventAggregation.GetEvent<OpenDialogEvent>().Publish(DialogType.AdaptiveView);
        //}

        //private static bool AdaptiveViewCanExecute()
        //{
        //    return beOpenDocument;
        //}
        #endregion

        #region Layout Model

        public static void LoadDefaultLayout()
        {
            // _ListEventAggregator.GetEvent<ChangeLayoutEvent>().Publish("Default");
        }
        public static void SaveCurrentLayout()
        {
            //_ListEventAggregator.GetEvent<ChangeLayoutEvent>().Publish("Save");
        }


        private static void DefaultExecuted()
        {

        }

        private static bool DefaultCanExecute()
        {
            return true;
        }

        private static bool DefaultCustomExcute()
        {
            return false;
        }
        #endregion

    }
}
