using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Markup;
using Naver.Compass.Common.CommonBase;


namespace Naver.Compass.InfoStructure
{
    public class WidgetMultiTextViewModBase : WidgetRotateViewModBase
    {
        public WidgetMultiTextViewModBase()
        {
            //Init private properties
            //_sectionFontFamily = "Arial";
            //_sectionFontSize = 12;
            //_sectionFontColor = Color.FromRgb(0, 0, 0);
            //_sectionFontBold = false;
            //_sectionFontItalic = false;
            //_sectionFontUnderline = false;
            //_sectionStrikethrough = false;
        }

        #region Private members
        //private string _sectionFontFamily;
        //private double _sectionFontSize;
        //private Color _sectionFontColor;
        //private bool _sectionFontBold;
        //private bool _sectionFontItalic;
        //private bool _sectionFontUnderline;
        //private bool _sectionStrikethrough;
        #endregion

        #region Override properties
      

        // Workaround for text undo/redo, vTextContent is bond to control and it update automatically 
        // when you change it via double click. Add  Raw_TextContent to set text without handling undo/redo.
        public string Raw_RichTextContent
        {
            set
            {
                _model.sRichTextContent = value;
                FirePropertyChanged("vTextContent");
            }
        }
        public string Input_SimpleTextContent
        {
            set
            {
                _model.InputSimpleText = value;
                FirePropertyChanged("vTextContent");
            }
        }
        override public string vTextContent
        {
            get
            {
                if (_model == null)
                    return null;
                return _model.sRichTextContent;
            }
            set
            {
                if (_model.sRichTextContent != value)
                {
                    if (CanEdit)
                    {
                        CompositeCommand cmds = new CompositeCommand();
                        PropertyChangeCommand cmd = new PropertyChangeCommand(this, "Raw_RichTextContent", _model.sRichTextContent, value);

                        Raw_RichTextContent = value;

                        if (CurrentUndoManager != null)
                        {
                            cmds.DeselectAllWidgetsFirst();
                            cmds.AddCommand(cmd);

                            CurrentUndoManager.Push(cmds);
                        }
                    }
                    else
                    {
                        Raw_RichTextContent = value;
                    }
                   
                }
            }
        }
        #endregion

        #region Override Binging Font Property
        //override public string vFontFamily
        //{
        //    get
        //    {
        //        return _sectionFontFamily;
        //    }
        //    set
        //    {

        //        if (_sectionFontFamily != value)
        //        {
        //            _sectionFontFamily = value;
        //            FirePropertyChanged("vFontFamily");
        //        }
        //    }
        //}
        //override public double vFontSize
        //{
        //    get
        //    {
        //        return _sectionFontSize;
        //    }
        //    set
        //    {
        //        if (_sectionFontSize != value)
        //        {
        //            _sectionFontSize = Convert.ToDouble(value);
        //            FirePropertyChanged("vFontSize");
        //        }
        //    }
        //}
        //override public Color vFontColor
        //{
        //    get
        //    {
        //        return _sectionFontColor;
        //    }
        //    set
        //    {

        //        if (_sectionFontColor != value)
        //        {
        //            _sectionFontColor = (Color)value;
        //            FirePropertyChanged("vFontColor");
        //        }

        //    }
        //}
        //override public bool vFontBold
        //{
        //    get
        //    {

        //        return _sectionFontBold;
        //    }
        //    set
        //    {
        //        if (_sectionFontBold != value)
        //        {
        //            _sectionFontBold = value;
        //            FirePropertyChanged("vFontBold");
        //        }
        //    }
        //}
        //override public bool vFontItalic
        //{
        //    get
        //    {
        //        return _sectionFontItalic;
        //    }
        //    set
        //    {
        //        if (_sectionFontItalic != value)
        //        {
        //            _sectionFontItalic = value;
        //            FirePropertyChanged("vFontItalic");
        //        }
        //    }
        //}
        //override public bool vFontUnderLine
        //{
        //    get
        //    {
        //        return _sectionFontUnderline;
        //    }
        //    set
        //    {
        //        if (_sectionFontUnderline != value)
        //        {
        //            _sectionFontUnderline = value;
        //            FirePropertyChanged("vFontUnderLine");
        //        }
        //    }
        //}

        ////Font stringthroug style
        //override public bool vFontStrickeThrough
        //{
        //    get
        //    {
        //        return _sectionStrikethrough;
        //    }
        //    set
        //    {
        //        if (_sectionStrikethrough != value)
        //        {
        //            _sectionStrikethrough = value;
        //            FirePropertyChanged("vFontStrickeThrough");
        //        }
        //    }
        //}

        public void OnFontSizeInDeCrease(bool bInCrease)
        {
            if (viewInput != null)
            {
                TextCommands.OptionCommand.Execute(bInCrease, viewInput);
            }
        }

       
        #endregion
    }
}
