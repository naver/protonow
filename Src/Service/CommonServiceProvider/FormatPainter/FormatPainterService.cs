using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System.Windows.Media;
using System.Windows.Markup;
using Naver.Compass.Common.CommonBase;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Events;

namespace Naver.Compass.Service
{
    public class PaintFormat
    {
        public StyleColor BorderColor { get; set; }

        public StyleColor BackColor { get; set; }

        public double LineWidth { get; set; }

        public LineStyle Linestyle { get; set; }

        public string Fontfamily { get; set; }

        public double FontSize { get; set; }

        public Color Fontcolor { get; set; }

        public bool IsBold { get; set; }

        public bool IsItalic { get; set; }

        public bool IsUnderline { get; set; }

        public bool IsStrikeThough { get; set; }
    }

    public class FormatPainterService : IFormatPainterService
    {
        public FormatPainterService()
        {
            _Format = new PaintFormat();

            _EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }

        #region private member

        private bool _IsInMode = false;

        private IEventAggregator _EventAggregator;

        private PaintFormat _Format;

        #endregion

        private void SetFontStyle(WidgetViewModelDate wdg)
        {
            FormatTextCoverter coverter = new FormatTextCoverter(wdg.vTextContent);

            _Format.Fontfamily = coverter.GetFontFamily();

            _Format.FontSize = coverter.GetFontsize();

            _Format.Fontcolor = coverter.GetFontColor();

            _Format.IsUnderline = coverter.GetFontUnderline();

            _Format.IsStrikeThough = coverter.GetFontStrikeThough();

            _Format.IsItalic = coverter.GetFontItalic();

            _Format.IsBold = coverter.GetFontBold();
        }

        #region Public Function

        public void SetPaintFormat(WidgetViewModelDate wdg)
        {

            _Format.BackColor = wdg.vBackgroundColor;

            _Format.BorderColor = wdg.vBorderLineColor;

            _Format.LineWidth = wdg.vBorderLinethinck;

            _Format.Linestyle = wdg.vBorderlineStyle;

            if (wdg.WidgetType == WidgetType.Shape)//shape widget use format string
            {
                SetFontStyle(wdg);
            }
            else
            {
                _Format.Fontfamily = wdg.vFontFamily;

                _Format.FontSize = wdg.vFontSize;

                _Format.Fontcolor = wdg.vFontColor;

                _Format.IsBold = wdg.vFontBold;

                _Format.IsItalic = wdg.vFontItalic;

                _Format.IsUnderline = wdg.vFontUnderLine;

                _Format.IsStrikeThough = wdg.vFontStrickeThrough;
            }

        }

        public PaintFormat GetPaintFormat()
        {
            if (_IsInMode)
            {
                return _Format;
            }

            return null;
        }

        public bool GetMode()
        {
            return _IsInMode;
        }

        public void SetMode(bool val)
        {
            if (_IsInMode != val)
            {
                _IsInMode = val;

                _EventAggregator.GetEvent<TBUpdateEvent>().Publish(TBUpdateType.FormatPaint);
            }
        }

        #endregion

    }
}
