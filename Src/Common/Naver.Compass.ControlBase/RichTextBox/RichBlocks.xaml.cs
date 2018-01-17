using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;
using System.Linq;
using Naver.Compass.Common.CommonBase;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Naver.Compass.Common
{

    public partial class RichBlocks : Label
    {

        public RichBlocks()
        {
            InitializeComponent();
            Unloaded += RichBlocks_UnLoaded;
            Loaded +=RichBlocks_Loaded;
        }

        private string BulletSymol = "•";

        #region Override Method

        void RichBlocks_Loaded(object sender, RoutedEventArgs e)
        {
            CommandBindings.Add(new CommandBinding(TextCommands.OptionCommand, OnTextOptionCommand));
            CommandBindings.Add(new CommandBinding(TextCommands.UpDownCaseControl, OnUpDownCaseCommand));
        }


        void RichBlocks_UnLoaded(object sender, RoutedEventArgs e)
        {
            vPanel.Children.Clear();

            CommandBindings.Clear();
            BindingOperations.ClearAllBindings(this);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
           // CommandBindings.Add(new CommandBinding(TextCommands.OptionCommand, OnTextOptionCommand));
           // CommandBindings.Add(new CommandBinding(TextCommands.UpDownCaseControl, OnUpDownCaseCommand));
        }

        #endregion

        #region Define Fuction

        #region  Text to Control

        void TranslateTextToControl(string sText)
        {
            sText = System.Text.RegularExpressions.Regex.Replace(sText, "&#x(0?[0-8B-F]|1[0-9A-F]|7F);", "");

            if (sText.Length < 1)
            {
                return;
            }

            vPanel.Children.Clear();

            using (MemoryStream ms = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(sText)))
            {
                object obj = XamlReader.Load(ms);
                if (obj is Section)
                {
                    TranlateSection(obj as Section, vPanel.Children);
                }
                else if (obj is Span)
                {
                    NLogger.Error("TranslateTextToControl Invalid  span\n" + sText.ToString());
                }
                else
                {
                    NLogger.Error("TranslateTextToControl  Invalid object\n" + sText.ToString());
                }
            }
        }

        void TranlateSection(Section section, UIElementCollection ElemList)
        {
            foreach (Block block in section.Blocks)
            {
                if (block is Paragraph)
                {
                    TranlateParagrah(block as Paragraph, ElemList);
                }
                else if (block is List)
                {
                    TranslateList(block as List, ElemList);
                }
                else if (block is Section)
                {
                    TranlateSection(block as Section, ElemList);
                }
            }
        }

        void TranslateList(List listData, UIElementCollection ElemList)
        {
            foreach (ListItem item in listData.ListItems)
            {
                foreach (Block itemblock in item.Blocks)
                {
                    if (itemblock is Paragraph)
                    {
                        BulletDecorator bull = GetBulletStyle();
                        TextBlock single = new TextBlock();
                        Paragraph temp = itemblock as Paragraph;
                        foreach (Run run in temp.Inlines)
                        {
                            Run newrun = new Run();
                            newrun.Text = run.Text;
                            newrun.TextDecorations = run.TextDecorations;
                            newrun.FontFamily = run.FontFamily;
                            newrun.FontSize = run.FontSize;
                            newrun.FontStyle = run.FontStyle;
                            newrun.FontWeight = run.FontWeight;
                            newrun.Foreground = run.Foreground;

                            single.Inlines.Add(newrun);
                        }

                        single.TextAlignment = itemblock.TextAlignment;
                        single.TextWrapping = TextWrapping.Wrap;
                        single.Margin = new Thickness(2, 0, 0, 0);
                        bull.Child = single;
                        ElemList.Add(bull);
                    }
                }
            }
        }

        void TranlateParagrah(Paragraph paraData, UIElementCollection ElemList)
        {
            TextBlock single = new TextBlock();
            single.TextWrapping = TextWrapping.Wrap;
            foreach (Run run in paraData.Inlines)
            {
                Run newrun = new Run();
                newrun.Text = run.Text;
                newrun.TextDecorations = run.TextDecorations;
                newrun.FontFamily = run.FontFamily;
                newrun.FontSize = run.FontSize;
                newrun.FontStyle = run.FontStyle;
                newrun.FontWeight = run.FontWeight;
                newrun.Foreground = run.Foreground;

                single.Inlines.Add(newrun);
            }
            single.TextAlignment = paraData.TextAlignment;
            single.LineHeight = paraData.LineHeight;
            ElemList.Add(single);
        }

        #endregion

        #region Control to Text

        void TranslateControlToText()
        {
            if (vPanel.Children.Count < 1)
            {
                return;
            }
            
            FlowDocument section = new FlowDocument();
            
            UIElement[] elemlist = new UIElement[vPanel.Children.Count];
            vPanel.Children.CopyTo(elemlist, 0);

            foreach (UIElement Elem in elemlist)
            {
                TextBlock txtblock;
                if (Elem is BulletDecorator)
                {
                    txtblock = ((BulletDecorator)Elem).Child as TextBlock;

                    Paragraph textPara = new Paragraph();
                    textPara.Margin = new Thickness(0);
                    foreach (Run run in txtblock.Inlines)
                    {
                        Run tRun = new Run();
                        tRun.Text = run.Text;
                        tRun.TextDecorations = run.TextDecorations;
                        tRun.FontFamily = run.FontFamily;
                        tRun.FontSize = run.FontSize;
                        tRun.FontStyle = run.FontStyle;
                        tRun.FontWeight = run.FontWeight;
                        tRun.Foreground = run.Foreground;
                        textPara.Inlines.Add(tRun);
                    }
                    textPara.TextAlignment = txtblock.TextAlignment;

                    List list = new List();
                    ListItem listitem = new ListItem(textPara);
                    list.MarkerStyle = TextMarkerStyle.Disc;
                    list.ListItems.Add(listitem);

                    section.Blocks.Add(list);
                }
                else
                {
                    txtblock = Elem as TextBlock;

                    Paragraph textPara = new Paragraph();
                    textPara.Margin = new Thickness(0);
                    foreach (Run run in txtblock.Inlines)
                    {
                        Run tRun = new Run();
                        tRun.Text = run.Text;
                        tRun.TextDecorations = run.TextDecorations;
                        tRun.FontFamily = run.FontFamily;
                        tRun.FontSize = run.FontSize;
                        tRun.FontStyle = run.FontStyle;
                        tRun.FontWeight = run.FontWeight;
                        tRun.Foreground = run.Foreground;
                        textPara.Inlines.Add(tRun);
                    }
                    textPara.TextAlignment = txtblock.TextAlignment;
                    textPara.LineHeight = txtblock.LineHeight;
                    section.Blocks.Add(textPara);
                }

            }

            using (MemoryStream ms = new MemoryStream())
            {
               // XamlWriter.Save(section, ms);

                //RichString = ASCIIEncoding.UTF8.GetString(ms.ToArray());

                TextRange rang = new TextRange(section.ContentStart, section.ContentEnd);
                rang.Save(ms, DataFormats.Xaml,true);

                RichString = ASCIIEncoding.UTF8.GetString(ms.ToArray());

               // System.Diagnostics.Debug.WriteLine(RichString);
            }

        }

        void FlashPropertyAfterStringSet()
        {

            bool GlobBold = true;
            bool GlobItalic = true;
            bool GlobUnderline = true;
            bool GlobStrikeThough = true;
            string GlobFamily = "Init";
            double GlobSize = -2;
            Color GlobColor = Color.FromRgb(0, 0, 1);

            int index = 0;

            UIElement[] elemlist = new UIElement[vPanel.Children.Count];
            vPanel.Children.CopyTo(elemlist, 0);

            foreach (UIElement Elem in elemlist)
            {
                TextBlock txtblock;
                if (Elem is BulletDecorator)
                {
                    txtblock = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    txtblock = Elem as TextBlock;
                }

                foreach (Run run in txtblock.Inlines)
                {
                    index++;
                    if (index == 1)
                    {
                        GlobFamily = run.FontFamily.ToString();
                        GlobSize = run.FontSize;
                        GlobColor = ((SolidColorBrush)run.Foreground).Color;
                    }
                    else
                    {
                        if (GlobFamily.Length > 0 && GlobFamily.CompareTo(run.FontFamily.ToString()) != 0)
                        {
                            GlobFamily = "";
                        }

                        if (GlobSize != -1 && GlobSize != run.FontSize)
                        {
                            GlobSize = -1;
                        }

                        if (GlobColor == Color.FromRgb(0, 0, 1) && GlobColor != ((SolidColorBrush)run.Foreground).Color)
                        {
                            GlobColor = Color.FromRgb(0, 0, 1);
                        }
                    }

                    bool bUnerline = false;
                    bool bStrikeThough = false;
                    foreach (TextDecoration de in run.TextDecorations)
                    {
                        if (de.Location == TextDecorationLocation.Underline)
                        {
                            bUnerline = true;
                        }
                        if (de.Location == TextDecorationLocation.Strikethrough)
                        {
                            bStrikeThough = true;
                        }
                    }

                    GlobUnderline &= bUnerline;
                    GlobStrikeThough &= bStrikeThough;

                    GlobItalic &= (run.FontStyle == FontStyles.Italic);
                    GlobBold &= (run.FontWeight == FontWeights.Bold);

                }
                //textPara.TextAlignment = txtblock.TextAlignment;
            }

            if (elemlist.Count() > 0)
            {
                if (DFontBold != GlobBold)
                    DFontBold = GlobBold;

                if (DFontItalic != GlobItalic)
                    DFontItalic = GlobItalic;

                if (DFontUnderline != GlobUnderline)
                    DFontUnderline = GlobUnderline;

                if (DFontStrikeThough != GlobStrikeThough)
                    DFontStrikeThough = GlobStrikeThough;

                if (DFontFamily.CompareTo(GlobFamily) != 0)
                    DFontFamily = GlobFamily;

                if (DisplayFontsize != GlobSize)
                    DisplayFontsize = GlobSize;

                if (DFontcolor != GlobColor)
                    DFontcolor = GlobColor;

            }


        }

        #endregion

        private void OnTextOptionCommand(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {

                if (DisplayFontsize < 0)
                {
                    foreach (UIElement Elem in vPanel.Children)
                    {
                        TextBlock block;
                        if (Elem is BulletDecorator)
                        {
                            block = ((BulletDecorator)Elem).Child as TextBlock;
                        }
                        else
                        {
                            block = Elem as TextBlock;
                        }

                        foreach (Run run in block.Inlines)
                        {
                            run.FontSize = FontSizeEnumerator.GetValue(run.FontSize, (bool)e.Parameter);
                        }
                    }
                }
                else
                {
                    DisplayFontsize = FontSizeEnumerator.GetValue(DisplayFontsize, (bool)e.Parameter);
                    OndisplayFontSizeChange(DisplayFontsize);
                }

                ResetLineHeight();

                TranslateControlToText();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void OnUpDownCaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OnChangeCharactorCase();

            TranslateControlToText();
        }

        private BulletDecorator GetBulletStyle()
        {
            BulletDecorator rValue = new BulletDecorator();
            Ellipse bull = new Ellipse();
            bull.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            bull.Width = 4;
            bull.Height = 4;

            rValue.Bullet = bull;

            return rValue;
        }

        private void ResetLineHeight()
        {
            int index = 0;
            double dLineHeight = 0.0;
            try
            {

                foreach (UIElement Elem in vPanel.Children)
                {
                    TextBlock block;
                    if (Elem is BulletDecorator)
                    {
                        block = ((BulletDecorator)Elem).Child as TextBlock;
                    }
                    else
                    {
                        block = Elem as TextBlock;
                    }

                    index = 0;
                    dLineHeight = 0.0;
                    foreach (Run run in block.Inlines)
                    {
                        double oldHeight = run.FontSize * run.FontFamily.LineSpacing;
                        oldHeight += (oldHeight * 3 / 8);
                        dLineHeight += oldHeight;
                        index++;
                    }
                    block.LineHeight = Math.Round(dLineHeight / index);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


        }

        private bool ResetLineHeightAndGetSaveFlag()
        {
            bool bReturn = false;
            int index = 0;
            double dLineHeight = 0.0;
            try
            {

                foreach (UIElement Elem in vPanel.Children)
                {
                    TextBlock block;
                    if (Elem is BulletDecorator)
                    {
                        block = ((BulletDecorator)Elem).Child as TextBlock;
                    }
                    else
                    {
                        block = Elem as TextBlock;
                    }

                    index = 0;
                    dLineHeight = 0.0;
                    foreach (Run run in block.Inlines)
                    {
                        double oldHeight = run.FontSize * run.FontFamily.LineSpacing;
                        oldHeight += (oldHeight * 3 / 8);
                        dLineHeight += oldHeight;
                        index++;
                    }

                    double NewHeight = Math.Round(dLineHeight / index);
                    if (!block.LineHeight.Equals(NewHeight))
                    {
                        block.LineHeight = NewHeight;
                        bReturn = true;
                    }
                    
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return bReturn;
        }

        private void OnChangeCharactorCase()
        {
          
            string sText = "";
            foreach (TextBlock block in vPanel.Children)
            {
                foreach (Inline line in block.Inlines)
                {
                    sText += ((Run)line).Text;
                }
            }

            if (String.IsNullOrEmpty(sText))
            {
                return;
            }

            StringStatus status = CommonFunction.GetTextStatus(sText);

            if (status == StringStatus.First)
            {
                UpCaseText();
            }
            else if (status == StringStatus.Middle || status == StringStatus.AllUplow)
            {
                LowCaseText();
            }
            else //if (status == StringStatus.FirstAndMiddle || status == StringStatus.AllSmall)
            {
                UpCaseFirstCharofText();
            }
            
        }

        private void UpCaseText()
        {
            for (int i = 0; i < vPanel.Children.Count; i++)
            {
                TextBlock block = vPanel.Children[i] as TextBlock;
               
                Inline line = block.Inlines.FirstInline;
                while(line != null)
                {
                    Run run = line as Run;
             
                    run.Text = run.Text.ToUpper();

                    line = line.NextInline;
                }
            }
        }

        private void UpCaseFirstCharofText()
        {
            for (int i = 0; i < vPanel.Children.Count; i++)
            {
                TextBlock block = vPanel.Children[i] as TextBlock;

                if (i == 0)
                {
                    Inline line = block.Inlines.FirstInline;
                    while (line != null)
                    {
                        if (line.PreviousInline == null)
                        {
                            Run run = line as Run;
                            string sFirst = run.Text.Substring(0, 1);
                            string sExceptFirst = run.Text.Substring(1);
                            sFirst = sFirst.ToUpper();
                            sExceptFirst = sExceptFirst.ToLower();

                            run.Text = (sFirst + sExceptFirst);
                        }
                        else
                        {
                            Run run = line as Run;
                            run.Text = run.Text.ToLower();
                        }

                        line = line.NextInline;
                    }
                }
                else
                {
                    Inline line = block.Inlines.FirstInline;
                    while (line != null)
                    {
                        Run run = line as Run;
                        run.Text = run.Text.ToLower();

                        line = line.NextInline;
                    }
                }

            }
        }

        private void LowCaseText()
        {
            for (int i = 0; i < vPanel.Children.Count; i++)
            {
                TextBlock block = vPanel.Children[i] as TextBlock;

                Inline line = block.Inlines.FirstInline;
                while (line != null)
                {
                    Run run = line as Run;
                    
                    run.Text = run.Text.ToLower();

                    line = line.NextInline;
                }
            }
        }

        #endregion

        #region Property Change CallBack

        private void OnTargetUpdated(Object sender, DataTransferEventArgs args)
        {
            if (args != null && args.Property != null && args.Property.Name != null)
            {
                try
                {

                    if (vPanel.Children.Count > 0)
                    {
                        if (args.Property.Name == DFontcolorProperty.Name)
                        {
                            OnDFontColorChange(GetValue(args.Property));

                            TranslateControlToText();
                        }
                        else if (args.Property.Name == DFontFamilyProperty.Name)
                        {
                            OnDFontFamilyChange(GetValue(args.Property));

                            ResetLineHeight();

                            TranslateControlToText();
                        }
                        else if (args.Property.Name == DTextHorAligenProperty.Name)
                        {
                            OnDTextHorticalChange(GetValue(args.Property));

                            TranslateControlToText();
                        }
                        else if (args.Property.Name == DisplayFontsizeProperty.Name)
                        {
                            OndisplayFontSizeChange((double)GetValue(args.Property));

                            ResetLineHeight();

                            TranslateControlToText();
                        }
                        else if (args.Property.Name == DFontBoldProperty.Name)
                        {
                            OnDFontBoldChange((bool)GetValue(args.Property));
                        }
                        else if (args.Property.Name == DFontItalicProperty.Name)
                        {
                            OnDFontItalicChange((bool)GetValue(args.Property));
                        }
                        else if (args.Property.Name == DFontUnderlineProperty.Name)
                        {
                            OnDFontUnderline((bool)GetValue(args.Property));
                        }
                        else if (args.Property.Name == DFontStrikeThoughProperty.Name)
                        {
                            OnDFontStrikeThoughChange((bool)GetValue(args.Property));
                        }
                        else if (args.Property.Name == TextBulletStyleProperty.Name)
                        {
                            OnBulletChange(GetValue(args.Property));
                        }
                    }

                    if (args.Property.Name == RichStringProperty.Name)
                    {
                        TranslateTextToControl((string)GetValue(args.Property));

                        FlashPropertyAfterStringSet();

                     //   ResetLineHeight();

                        if (ResetLineHeightAndGetSaveFlag())
                        {
                            TranslateControlToText();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        #endregion

        #region RichString

        public string RichString
        {
            get
            {
                return (string)GetValue(RichStringProperty);
            }
            set
            {
                SetValue(RichStringProperty, value);
            }
        }

        public static readonly DependencyProperty RichStringProperty =
          DependencyProperty.Register("RichString", typeof(string), typeof(RichBlocks), new PropertyMetadata(""));

        #endregion

        #region Font size

        public double DisplayFontsize
        {
            get
            {
                return (double)GetValue(DisplayFontsizeProperty);
            }
            set
            {
                SetValue(DisplayFontsizeProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayFontsizeProperty =
            DependencyProperty.Register("DisplayFontsize", typeof(double), typeof(RichBlocks));

        private void OndisplayFontSizeChange(double value)
        {
            if (value < 1)
            {
                return;
            }

            foreach (UIElement Elem in vPanel.Children)
            {
                TextBlock block;
                if (Elem is BulletDecorator)
                {
                    block = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    block = Elem as TextBlock;
                }

                foreach (Run run in block.Inlines)
                {
                    run.FontSize = value;
                }
            }

           

        }
        #endregion

        #region Font family

        public string DFontFamily
        {
            get
            {
                return (string)GetValue(DFontFamilyProperty);
            }
            set
            {
                SetValue(DFontFamilyProperty, value);
            }
        }

        public static readonly DependencyProperty DFontFamilyProperty =
            DependencyProperty.Register("DFontFamily", typeof(string), typeof(RichBlocks),
            new PropertyMetadata("Arial"));

        private void OnDFontFamilyChange(object newValue)
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                TextBlock block;
                if (Elem is BulletDecorator)
                {
                    block = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    block = Elem as TextBlock;
                }
                foreach (Run run in block.Inlines)
                {
                    string sFamily = Convert.ToString(newValue);
                    if (sFamily.Length > 0)
                    {
                        run.FontFamily = new System.Windows.Media.FontFamily(sFamily);
                    }

                }
            }

            
        }
        #endregion

        #region Fontcolor

        public Color DFontcolor
        {
            get
            {
                return (Color)GetValue(DFontcolorProperty);
            }
            set
            {
                SetValue(DFontcolorProperty, value);
            }
        }

        public static readonly DependencyProperty DFontcolorProperty = DependencyProperty.Register("DFontcolor", typeof(Color), typeof(RichBlocks), new PropertyMetadata(Color.FromRgb(0, 0, 0)));

        private void OnDFontColorChange(object newValue)
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                TextBlock block;
                if (Elem is BulletDecorator)
                {
                    block = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    block = Elem as TextBlock;
                }
                foreach (Run run in block.Inlines)
                {
                    run.Foreground = new SolidColorBrush((Color)newValue);
                }
            }

            
        }

        #endregion

        #region Font Bold

        public bool DFontBold
        {
            get
            {
                return (bool)GetValue(DFontBoldProperty);
            }
            set
            {
                SetValue(DFontBoldProperty, value);
            }
        }

        public static readonly DependencyProperty DFontBoldProperty =
           DependencyProperty.Register(
           "DFontBold",
           typeof(bool),
           typeof(RichBlocks),
           new PropertyMetadata(false));

        private void OnDFontBoldChange(bool newValue)
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                TextBlock block;
                if (Elem is BulletDecorator)
                {
                    block = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    block = Elem as TextBlock;
                }

                foreach (Run run in block.Inlines)
                {
                    if (newValue)
                    {
                        run.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        run.FontWeight = FontWeights.Normal;
                    }
                }
            }


            TranslateControlToText();
        }
        #endregion

        #region Font Italic

        public bool DFontItalic
        {
            get { return (bool)GetValue(DFontItalicProperty); }
            set
            {
                SetValue(DFontItalicProperty, value);
            }
        }

        public static readonly DependencyProperty DFontItalicProperty =
           DependencyProperty.Register(
           "DFontItalic",
           typeof(bool),
           typeof(RichBlocks),
           new PropertyMetadata(false));

        private void OnDFontItalicChange(bool newValue)
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                TextBlock block;
                if (Elem is BulletDecorator)
                {
                    block = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    block = Elem as TextBlock;
                }

                foreach (Run run in block.Inlines)
                {
                    if (newValue)
                    {
                        run.FontStyle = FontStyles.Italic;
                    }
                    else
                    {
                        run.FontStyle = FontStyles.Normal;
                    }
                }
            }

            TranslateControlToText();
        }

        #endregion

        #region FontUnderline

        public bool DFontUnderline
        {
            get { return (bool)GetValue(DFontUnderlineProperty); }
            set
            {
                SetValue(DFontUnderlineProperty, value);
            }
        }

        public static readonly DependencyProperty DFontUnderlineProperty =
           DependencyProperty.Register(
           "DFontUnderline",
           typeof(bool),
           typeof(RichBlocks));

        private void OnDFontUnderline(bool newValue)
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                TextBlock block;
                if (Elem is BulletDecorator)
                {
                    block = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    block = Elem as TextBlock;
                }
                foreach (Run run in block.Inlines)
                {
                    if (newValue)
                    {
                        bool isExit = false;

                        foreach (TextDecoration de in run.TextDecorations)
                        {
                            if (de.Location == TextDecorationLocation.Underline)
                            {
                                isExit = true;
                                break;
                            }
                        }

                        if (!isExit)
                        {
                            run.TextDecorations.Add(CommonFunction.GetUnderline()[0]);
                        }
                    }
                    else
                    {
                        int index = -1;
                        foreach (TextDecoration de in run.TextDecorations)
                        {
                            index++;
                            if (de.Location == TextDecorationLocation.Underline)
                            {
                                run.TextDecorations.RemoveAt(index);
                                break;
                            }
                        }
                    }
                }
            }

            TranslateControlToText();
        }

        #endregion

        #region FontStrikeThrough

        public bool DFontStrikeThough
        {
            get { return (bool)GetValue(DFontStrikeThoughProperty); }
            set
            {
                SetValue(DFontStrikeThoughProperty, value);
            }
        }

        public static readonly DependencyProperty DFontStrikeThoughProperty =
           DependencyProperty.Register(
           "DFontStrikeThough",
           typeof(bool),
           typeof(RichBlocks));

        private void OnDFontStrikeThoughChange(bool newValue)
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                TextBlock block;
                if (Elem is BulletDecorator)
                {
                    block = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    block = Elem as TextBlock;
                }
                foreach (Run run in block.Inlines)
                {
                    if (newValue)
                    {
                        bool isExit = false;

                        foreach (TextDecoration de in run.TextDecorations)
                        {
                            if (de.Location == TextDecorationLocation.Strikethrough)
                            {
                                isExit = true;
                                break;
                            }
                        }
                        if (!isExit)
                        {
                            run.TextDecorations.Add(CommonFunction.GetStrikeThough()[0]);
                        }
                    }
                    else
                    {
                        int index = -1;
                        foreach (TextDecoration de in run.TextDecorations)
                        {
                            index++;
                            if (de.Location == TextDecorationLocation.Strikethrough)
                            {
                                run.TextDecorations.RemoveAt(index);
                                break;
                            }
                        }
                    }
                }
            }

            TranslateControlToText();
        }

        #endregion

        #region Hor Alignment

        public TextAlignment DTextHorAligen
        {
            get
            {
                return (TextAlignment)GetValue(DTextHorAligenProperty);
            }
            set
            {
                SetValue(DTextHorAligenProperty, value);
            }
        }

        public static readonly DependencyProperty DTextHorAligenProperty =
           DependencyProperty.Register(
           "DTextHorAligen",
           typeof(TextAlignment),
           typeof(RichBlocks));

        private void OnDTextHorticalChange(object newValue)
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                TextBlock block;
                if (Elem is BulletDecorator)
                {
                    block = ((BulletDecorator)Elem).Child as TextBlock;
                }
                else
                {
                    block = Elem as TextBlock;
                }

                block.TextAlignment = (TextAlignment)newValue;
            }

            
        }
        #endregion

        #region TextMarkerStyle

        public TextMarkerStyle TextBulletStyle
        {
            get { return (TextMarkerStyle)GetValue(TextBulletStyleProperty); }
            set { SetValue(TextBulletStyleProperty, value); }
        }

        public static readonly DependencyProperty TextBulletStyleProperty =
           DependencyProperty.Register(
           "TextBulletStyle",
           typeof(TextMarkerStyle),
           typeof(RichBlocks),
           new PropertyMetadata(TextMarkerStyle.None));

        private void BulletToNone()
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                if (Elem is TextBlock)
                {
                    TextBlock block = Elem as TextBlock;
                    if (block.Inlines.Count > 0)
                    {
                        Run run = block.Inlines.FirstInline as Run;

                        string sText = run.Text.Replace(BulletSymol, "");
                        sText = sText.TrimStart();
                        run.Text = sText;
                    }
                }
               

            }
        }

        private void NoneToBullet()
        {
            foreach (UIElement Elem in vPanel.Children)
            {
                
                    if (Elem is TextBlock)
                    {
                        TextBlock block = Elem as TextBlock;

                        if (block.Inlines.Count > 0)
                        {
                            Run run = block.Inlines.FirstInline as Run;

                            if (run.Text.IndexOf(BulletSymol) < 0)
                            {
                                string sText = run.Text.TrimStart();

                                run.Text = BulletSymol + "  " + sText;
                            }
                        }
                        else
                        {
                            block.Inlines.Add(new Run(BulletSymol+"  "));
                        }

                    }
                
            }
        }

      

        private void OnBulletChange(object newValue)
        {
            TextMarkerStyle style = (TextMarkerStyle)newValue;

            try
            {
                if (style == TextMarkerStyle.None)
                {
                    BulletToNone();
                }
                else if (style == TextMarkerStyle.Disc)
                {
                    NoneToBullet();
                }
            }
            catch (System.Exception ex)
            {
                NLogger.Warn("RichBlocks OnBulletChange error," + ex.Message.ToString());
            }

            TranslateControlToText();
        }

        private TextBlock CopyTextBlock(TextBlock block)
        {
            TextBlock rValue = new TextBlock();
            rValue.Margin = block.Margin;
            rValue.TextWrapping = block.TextWrapping;
            rValue.TextAlignment = block.TextAlignment;

            foreach (Run run in block.Inlines)
            {
                Run newrun = new Run();
                newrun.Text = run.Text;
                newrun.TextDecorations = run.TextDecorations;
                newrun.FontFamily = run.FontFamily;
                newrun.FontSize = run.FontSize;
                newrun.FontStyle = run.FontStyle;
                newrun.FontWeight = run.FontWeight;
                newrun.Foreground = run.Foreground;

                rValue.Inlines.Add(newrun);
            }

            return rValue;
        }

        #endregion

    }
}
