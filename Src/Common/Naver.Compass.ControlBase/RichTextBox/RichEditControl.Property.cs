using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Naver.Compass.Common.CommonBase;
using System.Windows.Media;

namespace Naver.Compass.Common
{
    public partial class ExRichTextBox : RichTextBox
    {

        void ExRichTextBox_TargetUpdated(object sender, DataTransferEventArgs args)
        {
            if (args != null && args.Property != null && args.Property.Name != null)
            {
                if (Document.Blocks.Count > 0)
                {
                    if (args.Property.Name == SectionFontcolorProperty.Name)
                    {
                        OnFontColorChange(GetValue(args.Property));
                    }
                    else if (args.Property.Name == SectionFontFamilyProperty.Name)
                    {
                        OnFontFamilyChange((string)GetValue(args.Property));

                        UpdateLineSpace();
                    }
                    else if (args.Property.Name == FontAlignmentProperty.Name)
                    {
                        OnFontAlignmentChange(GetValue(args.Property));
                    }
                    else if (args.Property.Name == SectionFontsizeProperty.Name)
                    {
                        OnFontSizeChange(GetValue(args.Property));

                        UpdateLineSpace();
                    }
                    else if (args.Property.Name == SectionFontBoldProperty.Name)
                    {
                        OnFontBoldChange(GetValue(args.Property));
                    }
                    else if (args.Property.Name == SectionFontItalicProperty.Name)
                    {
                        OnFontItalicChange(GetValue(args.Property));
                    }
                    else if (args.Property.Name == SectionFontUnderlineProperty.Name)
                    {
                        OnFontUnderlineChange(GetValue(args.Property));
                    }
                    else if (args.Property.Name == SectionFontStrikeThoughProperty.Name)
                    {
                        OnFontStrikeThoughChange(GetValue(args.Property));
                    }
                    else if (args.Property.Name == TextBulletStyleProperty.Name)
                    {
                        OnBulletStyleChange(GetValue(args.Property));
                    }
                }

                if (args.Property.Name == TextProperty.Name)
                {
                    UpdateDocumentFromText();

                    UpdateLineSpaceAndFixProperty();

                    SetRootDefaultProperty();
                }
            }
        }

        void SetRootDefaultProperty()
        {
            if (String.IsNullOrEmpty(SectionFontFamily))
            {
                Document.FontFamily = new System.Windows.Media.FontFamily(CommonFunction.GetDefaultFontNameByLanguage());
            }
            else
            {
                Document.FontFamily = new System.Windows.Media.FontFamily(SectionFontFamily);
            }

            if (SectionFontsize < 0)
            {
                Document.FontSize = 13;
            }
            else
            {
                Document.FontSize = SectionFontsize;
            }

            if (SectionFontItalic)
            {
                Document.FontStyle = FontStyles.Italic;
            }
            else
            {
                Document.FontStyle = FontStyles.Normal;
            }

            if (SectionFontBold)
            {
                Document.FontWeight = FontWeights.Bold;
            }
            else
            {
                Document.FontWeight = FontWeights.Normal;
            }

            Document.Foreground = new SolidColorBrush(SectionFontcolor);
            

            Document.TextAlignment = FontAlignment;
        }
      

        #region Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ExRichTextBox), new FrameworkPropertyMetadata(String.Empty));

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        #endregion //Text

        #region Section binding properties

        #region FontFamily

        public string SectionFontFamily
        {
            get
            {
                return (string)GetValue(SectionFontFamilyProperty);
            }
            set
            {
                if (!SectionFontFamily.Equals(value))
                {
                    SetValue(SectionFontFamilyProperty, value);
                }

            }
        }

        public static readonly DependencyProperty SectionFontFamilyProperty =
            DependencyProperty.Register(
            "SectionFontFamily",
            typeof(string),
            typeof(ExRichTextBox),
            new PropertyMetadata("Arial"));

        private void OnFontFamilyChange(string Data)
        {
            GetOptionRange().ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(Data));
          
            //////////////////////////////////////////////////////////////////////////
            foreach (Block block in Document.Blocks)
            {
                if (block is Paragraph)
                {
                    Paragraph para = block as Paragraph;

                    foreach (Inline line in para.Inlines)
                    {
                        line.FontFamily = line.FontFamily;
                    }
                }
            }
           
            Document.FontFamily = new System.Windows.Media.FontFamily(Data);
            
            //////////////////////////////////////////////////////////////////////////
        }

        #endregion

        #region Fontsize

        public double SectionFontsize
        {
            get
            {
                return (double)GetValue(SectionFontsizeProperty);
            }
            set
            {
                if (SectionFontsize != Convert.ToDouble(value))
                {
                    SetValue(SectionFontsizeProperty, value);
                }
            }
        }

        public static readonly DependencyProperty SectionFontsizeProperty =
            DependencyProperty.Register(
            "SectionFontsize",
            typeof(double),
            typeof(ExRichTextBox));

        private void OnFontSizeChange(object data)
        {
            TextRange rang = GetOptionRange();
            double fontsize = 0;
            try
            {
                fontsize = Convert.ToDouble(data);
            }
            catch (System.Exception ex)
            {
                NLogger.Error("Format of font size variable is not correct,will reset to 12.");
                fontsize = 12;
            }

            if (fontsize > 0)
            {
               rang.ApplyPropertyValue(TextElement.FontSizeProperty, fontsize);

               //////////////////////////////////////////////////////////////////////////
               foreach (Block block in Document.Blocks)
               {
                   if (block is Paragraph)
                   {
                       Paragraph para = block as Paragraph;

                       foreach (Inline line in para.Inlines)
                       {
                           line.FontSize = line.FontSize;
                       }
                   }
               }

               Document.FontSize = fontsize;

                //////////////////////////////////////////////////////////////////////////
            }
        }

        #endregion

        #region Fontcolor

        public Color SectionFontcolor
        {
            get
            {
                return (Color)GetValue(SectionFontcolorProperty);
            }
            set
            {
                SetValue(SectionFontcolorProperty, value);
            }
        }

        public static readonly DependencyProperty SectionFontcolorProperty =
            DependencyProperty.Register(
            "SectionFontcolor",
            typeof(Color),
            typeof(ExRichTextBox),
            new PropertyMetadata(Color.FromRgb(0, 0, 0)));

        private void OnFontColorChange(object newValue)
        {

            //TextRange rang = new TextRange(Document.ContentStart, Document.ContentEnd);

            TextRange rang = GetOptionRange();

            rang.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush((Color)newValue));

        }

        #endregion

        #region nFontBold

        public bool SectionFontBold
        {
            get
            {
                return (bool)GetValue(SectionFontBoldProperty);
            }
            set
            {
                SetValue(SectionFontBoldProperty, value);
            }
        }

        public static readonly DependencyProperty SectionFontBoldProperty =
           DependencyProperty.Register(
           "SectionFontBold",
           typeof(bool),
           typeof(ExRichTextBox),
           new PropertyMetadata(false));


        private void OnFontBoldChange(object data)
        {
            TextRange rang = GetOptionRange();

            if (Convert.ToBoolean(data))
            {
                rang.ApplyPropertyValue(FontWeightProperty, FontWeights.Bold);
            }
            else
            {
                rang.ApplyPropertyValue(FontWeightProperty, FontWeights.Normal);
            }

        }

        #endregion

        #region FontItalic

        public bool SectionFontItalic
        {
            get { return (bool)GetValue(SectionFontItalicProperty); }
            set
            {
                SetValue(SectionFontItalicProperty, value);
            }
        }

        public static readonly DependencyProperty SectionFontItalicProperty =
           DependencyProperty.Register(
           "SectionFontItalic",
           typeof(bool),
           typeof(ExRichTextBox),
           new PropertyMetadata(false));

       
        private void OnFontItalicChange(object data)
        {
            TextRange rang = GetOptionRange();

            if (Convert.ToBoolean(data))
            {
                rang.ApplyPropertyValue(FontStyleProperty, FontStyles.Italic);
            }
            else
            {
                rang.ApplyPropertyValue(FontStyleProperty, FontStyles.Normal);
            }

        }

        #endregion

        #region FontUnderline

        public bool SectionFontUnderline
        {
            get {

                return (bool)GetValue(SectionFontUnderlineProperty);
            }
            set
            {
               
                SetValue(SectionFontUnderlineProperty, value);
               
            }
        }

        public static readonly DependencyProperty SectionFontUnderlineProperty =
           DependencyProperty.Register(
           "SectionFontUnderline",
           typeof(bool),
           typeof(ExRichTextBox),
           new PropertyMetadata(false));

        private void OnFontUnderlineChange(object data)
        {
            SetfontDecoration(TextDecorationLocation.Underline, Convert.ToBoolean(data));

        }

        #endregion

        #region FontStrikeThrough

        public bool SectionFontStrikeThough
        {
            get { return (bool)GetValue(SectionFontStrikeThoughProperty); }
            set
            {
               SetValue(SectionFontStrikeThoughProperty, value);
            }
        }

        public static readonly DependencyProperty SectionFontStrikeThoughProperty =
           DependencyProperty.Register(
           "SectionFontStrikeThough",
           typeof(bool),
           typeof(ExRichTextBox),
           new PropertyMetadata(false));


        private void OnFontStrikeThoughChange(object data)
        {
            SetfontDecoration(TextDecorationLocation.Strikethrough, Convert.ToBoolean(data));
        }

        #endregion

        #region FontAlignment

        public TextAlignment FontAlignment
        {
            get { return (TextAlignment)GetValue(FontAlignmentProperty); }
            set
            {
                SetValue(FontAlignmentProperty, value);
            }
        }

        public static readonly DependencyProperty FontAlignmentProperty =
           DependencyProperty.Register(
           "FontAlignment",
           typeof(TextAlignment),
           typeof(ExRichTextBox),
           new PropertyMetadata(TextAlignment.Left));

        private void OnFontAlignmentChange(object data)
        {

            BeginChange();

            TextRange rang = new TextRange(Document.ContentStart, Document.ContentEnd);;

            rang.ApplyPropertyValue(Paragraph.TextAlignmentProperty, data);

            Document.TextAlignment = (TextAlignment)data;

            EndChange();

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
           typeof(ExRichTextBox),
           new PropertyMetadata(TextMarkerStyle.None));

        private void AddBullet(Paragraph para)
        {
            if (para.Inlines.Count > 0)
            {
                Run run = para.Inlines.FirstInline as Run;
                if (run.Text.IndexOf(BulletSymol)  < 0)
                {
                    string sText = run.Text.TrimStart();

                    run.Text = BulletSymol + "  " + sText;
                }
            }
            else
            {
                para.Inlines.Add(new Run(BulletSymol + " "));
            }
        }

        private void DeleteBullet(Paragraph para)
        {
            if (para.Inlines.Count > 0)
            {
                Run run = para.Inlines.FirstInline as Run;
                if (run.Text.IndexOf(BulletSymol) > -1 )
                {
                    string sText = run.Text.Replace(BulletSymol, "");
                    sText = sText.TrimStart();
                    run.Text = sText;
                }
            }
        }

        private void ProcessParagraph(TextMarkerStyle style, Paragraph para)
        {
            if (style == TextMarkerStyle.None)
            {
               DeleteBullet(para);
            }
            else
            {
                AddBullet(para); 
            }
        }

        private void OnBulletStyleChange(object obj)
        {
            BeginChange();
            try
            {
                TextMarkerStyle style = (TextMarkerStyle)obj;

                if (Selection != null)
                {
                    if (Selection.Text.Length == 0)
                    {
                        if (CaretPosition != null && CaretPosition.Paragraph != null)
                        {
                            Paragraph para = CaretPosition.Paragraph;

                            ProcessParagraph(style, para);

                            CaretPosition = para.ContentEnd;
                        }
                    }
                    else if (Selection.Text.Length > 0)
                    {
                        TextPointer pStart = Selection.Start;
                        TextPointer pEnd = Selection.End;

                        Paragraph para = pStart.Paragraph;

                        while (para != null && para.ContentStart.CompareTo(pEnd) < 0)
                        {
                            ProcessParagraph(style, para);

                            para = para.NextBlock as Paragraph;
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                NLogger.Warn("RichEditControl OnBulletStyleChange error," + ex.Message.ToString());
            }

            EndChange();

        }

        #endregion

        #endregion
    }
}
