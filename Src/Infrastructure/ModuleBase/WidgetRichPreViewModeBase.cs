using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows.Media;

namespace Naver.Compass.InfoStructure
{
   public class WidgetRichPreViewModeBase : WidgetPreViewModeBase
    {
        public WidgetRichPreViewModeBase(IWidget widget)
            : base(widget)
        {
            #region Init private properties

            //_sectionFontFamily = "Arial";

            //_sectionFontSize = 12;

            //_sectionFontColor = Color.FromRgb(0, 0, 0);

            //_sectionFontBold = false;

            //_sectionFontItalic = false;

            //_sectionFontUnderline = false;

            //_sectionStrikethrough = false;

            #endregion

        }

        #region Private properties

        //private string _sectionFontFamily;

        //private double _sectionFontSize;

        //private Color _sectionFontColor;

        //private bool _sectionFontBold;

        //private bool _sectionFontItalic;

        //private bool _sectionFontUnderline;

        //private bool _sectionStrikethrough;

        #endregion

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
                return;
            }
        }

        //Font family
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

        //////Font size
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

        //////Font color
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

        ////Font bold style
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

    }
}
