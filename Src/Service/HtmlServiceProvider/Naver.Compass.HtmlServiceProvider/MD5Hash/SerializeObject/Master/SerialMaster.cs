using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialMaster : SerialMasterBase
    {
        #region public interface functions
        public SerialMaster(IMaster wdg) : base(wdg)
        { 
            InitializeProperty();
        }
        override public void Update(IMaster newWdg)
        {
            _wdg = newWdg;
            base.Update(newWdg);
            InitializeProperty();
        }
        #endregion

        #region private functions
        private void InitializeProperty()
        {
            Name= _wdg.Name;
            MastePageMD5 = MD5HashManager.GetHash(_wdg.MasterPage,true);
        }

        #endregion

        #region Serialize Data
        public string Name;
        public string MastePageMD5;
        // "toastWidgets":[
        #endregion
    }
}
