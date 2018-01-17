using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.WebServer
{
   public  interface IWebServer : IDisposable
    {
        bool StartWebService();

        void StopWebService();

        string GetWebUrl();
    }
}
