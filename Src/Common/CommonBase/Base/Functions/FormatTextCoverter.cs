using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Windows.Media;

namespace Naver.Compass.Common.CommonBase
{
    public class FormatTextCoverter
    {
        public FormatTextCoverter(string Formatstring, bool IsFirstLine = true)
        {
            _isFirstline = IsFirstLine;
            _text = Formatstring;

            _widgeText = new CommonBase.WidgetText();

            DecoderString();
        }

        #region Private string
        bool _isFirstline;

        string _text;

        CommonBase.WidgetText _widgeText;
        #endregion

        #region  Private fuction
        private void DecoderString()
        {
            try
            {
                if (!String.IsNullOrEmpty(_text))
                {
                    Section docment = XamlReader.Parse(_text) as Section;

                    if (docment != null)
                    {
                        foreach (Block block in docment.Blocks)
                        {
                            if (block is Paragraph)
                            {
                                Paragraph para = block as Paragraph;
                                foreach (TextElement control in para.Inlines)
                                {
                                    if (control is Run)
                                    {
                                        Run run = control as Run;

                                        if (run.FontWeight == FontWeights.Bold)
                                        {
                                            _widgeText.Bold = true;
                                        }

                                        if (run.FontStyle == FontStyles.Italic)
                                        {
                                            _widgeText.Italic = true;
                                        }

                                        _widgeText.Underline = false;
                                        _widgeText.StrikeThough= false;
                                        foreach (TextDecoration dec in run.TextDecorations)
                                        {
                                            if (dec.Location == TextDecorationLocation.Underline)
                                            {
                                                _widgeText.Underline = true;
                                            }
                                            else if (dec.Location == TextDecorationLocation.Strikethrough)
                                            {
                                                _widgeText.StrikeThough = true;
                                            }
                                        }

                                        _widgeText.FontSize = run.FontSize;
                                        _widgeText.FontFamily = run.FontFamily.ToString();
                                        _widgeText.Color = CommonFunction.CovertColorToInt(((SolidColorBrush)run.Foreground).Color);
                                        _widgeText.sText = run.Text;

                                        if (_isFirstline)
                                        {
                                            return;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
            }
        }
        #endregion

        #region Public function
        public double GetFontsize()
        {
            return _widgeText.FontSize;
        }

        public Color GetFontColor()
        {
            return CommonFunction.CovertIntToColor(_widgeText.Color);
        }

        public string GetFontFamily()
        {
            return _widgeText.FontFamily;
        }

        public bool GetFontBold()
        {
            return _widgeText.Bold;
        }

        public bool GetFontItalic()
        {
            return _widgeText.Italic;
        }

        public bool GetFontUnderline()
        {
            return _widgeText.Underline;
        }

        public bool GetFontStrikeThough()
        {
            return _widgeText.StrikeThough;
        }
        #endregion
    }
}
