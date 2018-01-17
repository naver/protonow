using Naver.Compass.Common.CommonBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Naver.Compass.Service
{

    /// <summary>
    ///Send NClick when click widget in widgetgallery.
    ///1. Set image's tag in widget: lbw.image,lbw.link....lbr.materialdesign... (WidgetGallery)
    ///2. Get tag and send nClick when create widget. (ToolboxItem)
    /// </summary>
    public class NClickService:INClickService
    {
        private const string NClickServer = "http://XXX";


        /// <summary>
        /// Send click info to server for statistics
        /// </summary>
        /// <param name="action"></param>
        public void SendNClick(string action)
        {
            if (string.IsNullOrEmpty(action))
                return;
            try
            {
                //string strURL = NClickServer + "&nsc=protonow.win&a=" + action;
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);

                //request.Referer = "client://protoNow";

                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //if(response.StatusDescription!="OK")
                //{
                //}

                //response.Close();
            }
            catch(Exception e)
            {
            }
        }
    }
}
