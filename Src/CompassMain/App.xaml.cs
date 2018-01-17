using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Naver.Compass.InfoStructure;
using MainToolBar.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System.Threading;
using System.Globalization;
using Naver.Compass.Service.Document;
using Naver.Compass.Service;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Html;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Naver.Compass.Common.CommonBase;
using Microsoft.Practices.Prism.Events;
using Naver.Compass.Service.Update;
using Naver.Compass.Service.WebServer;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Naver.Compass.Service.CustomLibrary;
using Naver.Compass.Main.Properties;

namespace Naver.Compass.Main
{

    public enum AppStartType
    {
        Normal,
        OpenFile,
        CreateLibrary,
        EditLibrary,
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string WidgetLibraryPath { get; set; }
        private CompassBootstrapper<SplashWindow> _spashBootstrapper;
        private CompassBootstrapper<MainIntegrationWindow> _mainBootstrapper;
        private bool _bIsMemoryExceptionOccured = false;
        private bool _bIsExcepting = false;
        public static bool IsNormalProcess = true;
        public static AppStartType StartType;
        protected override void OnStartup(StartupEventArgs e)
        {      
            InitEviroment();
            //Logger.info("Compass start up");
            //Get Current Cultrue
            CheckSetting();

            GlobalData.GetCulture();

            //Base Startup
            base.OnStartup(e);

            //var bootstrapper = new Bootstrapper();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                StartType = AppStartType.OpenFile;
            }
            else if ((args.Length == 3 && args[1] == "create"))
            {
                StartType = AppStartType.CreateLibrary;
            }
            else if (args.Length == 4 && args[2] == "edit")
            {
                StartType = AppStartType.EditLibrary;
            }
            else
            {
                StartType = AppStartType.Normal;
            }


            if (StartType == AppStartType.CreateLibrary || StartType == AppStartType.EditLibrary)
            {
                //create/edit Custom Library
                //don't show splash window
                IsNormalProcess = false;
                GlobalData.IsStandardMode = false;
                _mainBootstrapper = new CompassBootstrapper<MainIntegrationWindow>(new Action(RegisterMainService));
                _mainBootstrapper.Run();
                 UpdateWindowStyle();
            }
            else
            {
                IsNormalProcess = true;
                _spashBootstrapper = new CompassBootstrapper<SplashWindow>(new Action(RegisterSplashService));
                _spashBootstrapper.Run();
            }
          
            //Capture the exception
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

        }

        private void UpdateWindowStyle()
        {
            try
            {
                Application.Current.Resources["WindowTitleColor"] = CommonDefine.LibraryWindowBarColor;
                Application.Current.Resources["WindowBorderColor"] = CommonDefine.LibraryWindowBorderColor;
            }
            catch
            {

            }
        }


        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();           
            if (_bIsExcepting == true)
            {
                e.Handled = true;
                return;
            }                
            _bIsExcepting = true;

            if(e.Exception is OutOfMemoryException)
            {
                //e.Handled = true;
                if (_bIsMemoryExceptionOccured==false)
                {
                    _bIsMemoryExceptionOccured = true;
                    MessageBoxResult res = MessageBox.Show(GlobalData.FindResource("Warn_COM_Exception"), GlobalData.FindResource("Common_Warning"), MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        this.Shutdown();
                    }
                    else
                    {
                        e.Handled = true;
                    }                    
                }
                else
                {
                    e.Handled = true;
                }
                
            }
            else if (e.Exception is COMException)
            {
                //e.Handled = true;
                COMException eCom = e.Exception as COMException;
                if((uint)eCom.ErrorCode == 0x80070008)
                {
                    if (_bIsMemoryExceptionOccured == false)
                    {
                        _bIsMemoryExceptionOccured = true;
                        MessageBoxResult res = MessageBox.Show(GlobalData.FindResource("Warn_COM_Exception"), GlobalData.FindResource("Common_Warning"), MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes)
                        {
                            this.Shutdown();
                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = false;
                    MessageBoxResult res = MessageBox.Show(GlobalData.FindResource("Common_Exception") + e.Exception.Message, GlobalData.FindResource("Common_Error"));     
                }
            }
            else 
            {
                e.Handled = false;
                MessageBoxResult res = MessageBox.Show(GlobalData.FindResource("Common_Exception") + e.Exception.Message, GlobalData.FindResource("Common_Error"));             
            }
            _bIsExcepting = false;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            GlobalData.SetCulture();

            base.OnExit(e);

            IUpdateService updateService = ServiceLocator.Current.GetInstance<IUpdateService>();
            updateService.Dispose();

            IWebServer httpServer = ServiceLocator.Current.GetInstance<IWebServer>();
            httpServer.Dispose();

            IDocumentService docService = ServiceLocator.Current.GetInstance<IDocumentService>();
            docService.Dispose();

        }

        private void RegisterMainService()
        {
            _mainBootstrapper.RegisterService();
        }
        private void RegisterSplashService()
        {
            _spashBootstrapper.RegisterService();
        }

        #region Init Environment path
        void InitEviroment()
        {
            try
            {
                string sPath = Environment.GetEnvironmentVariable("TEMP");
                sPath += @"\protoNow\";

                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }

                Environment.SetEnvironmentVariable("TEMP", sPath);
                Environment.SetEnvironmentVariable("TMP", sPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
            }

        }
        #endregion

        #region localisition Method

        public static void CheckSetting()
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
            catch (ConfigurationException ex)
            {
                string fileName = ex.Filename;
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

            }
        }
        #endregion
    }
}
