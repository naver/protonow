using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialMasterStyle
    {
        [NonSerialized]
        private IMasterStyle _style;

        #region public interface functions
        public SerialMasterStyle(IMasterStyle style, bool bIsPlaced)
        {
            _style = style;
            IsPlaced = bIsPlaced;
            refresh();
        }
        public void refresh(IMasterStyle style, bool bIsPlaced)
        {
            _style = style;
            IsPlaced = bIsPlaced;
            refresh();
        }
        #endregion

        #region private functions
        private void refresh()
        {
            IsVisible = _style.IsVisible;
            X = _style.X;
            Y = _style.Y;
            //Height = _style.Height;
            //Width = _style.Width;
            Height = 0;
            Width = 0;
            Z = _style.Z;
            IsFixed = _style.IsFixed;
        }
        #endregion

        #region Serialize Data
        public bool IsVisible;
        public double X;
        public double Y;
        public double Height;
        public double Width;
        public int Z;
        public bool IsFixed;
        public bool IsPlaced;
        #endregion       
    }
   
}
