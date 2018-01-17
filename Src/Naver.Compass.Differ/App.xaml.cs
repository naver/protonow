using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Service.WebServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace Naver.Compass.Differ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CompassBootstrapper<MainWindow> _bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            InitEviroment();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                GlobalData.Culture = args[1];
            }
            else
            {
                GlobalData.GetCulture();
            }

            base.OnStartup(e);           

            _bootstrapper = new CompassBootstrapper<MainWindow>(new Action(RegisterService));
            _bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            IWebServer httpServer = ServiceLocator.Current.GetInstance<IWebServer>();
            httpServer.Dispose();

            IDocumentService docService = ServiceLocator.Current.GetInstance<IDocumentService>();
            docService.Dispose();
        }

        private void RegisterService()
        {
            _bootstrapper.RegisterService();
        }

        #region Init Environment path    
        void InitEviroment()
        {
            try
            {
                string sPath = Environment.GetEnvironmentVariable("TEMP");
                if(!sPath.EndsWith(@"\protoNow\"))
                {
                    sPath += @"\protoNow\";
                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }

                    Environment.SetEnvironmentVariable("TEMP", sPath);
                    Environment.SetEnvironmentVariable("TMP", sPath);
                }                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
            }

        }
        #endregion
    }
}
