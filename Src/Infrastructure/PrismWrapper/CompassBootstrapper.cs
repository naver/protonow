using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Naver.Compass.Service.Document;
using Naver.Compass.Service;
using Naver.Compass.Service.Html;
using Naver.Compass.Service.Update;
using Naver.Compass.Service.WebServer;
using Naver.Compass.Service.CustomLibrary;


namespace Naver.Compass.InfoStructure
{
    public class CompassBootstrapper<T> : UnityBootstrapper
        where T : Window
    {
        public Action _CustommerResigerHandler;
        public CompassBootstrapper(Action RegisteCustomerService)
        {
            _CustommerResigerHandler = RegisteCustomerService;
        }

        /// <summary>
        ///Attention: return value is null, to ignore the Region manager service
        /// </summary>
        protected override DependencyObject CreateShell()
        {
            Application.Current.MainWindow = Container.Resolve<T>();
            Application.Current.MainWindow.Show();       
            return null;
         }

        /// <summary>
        /// Now, it's not be called by region-manager
        /// </summary>
        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow=(Window)this.Shell;
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Override the function to avoid the default Prism service
        /// User can decide that only necessary service will be loaded
        /// </summary>
        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(false);
        }


        /// <summary>
        /// App call it to register it's self service and instance type.
        /// </summary>
        public void RegisterCustomerService(Type fromType, Type toType, bool isSingleton)
        {
            RegisterTypeIfMissing(fromType, toType, isSingleton);            
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
            RegisterTypeIfMissing(typeof(IModuleInitializer), typeof(ModuleInitializer), true);
            RegisterTypeIfMissing(typeof(IModuleManager), typeof(ModuleManager), true);
            RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);
            _CustommerResigerHandler();      
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            return null;
        }
        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            return null;
        }
        protected override void RegisterFrameworkExceptionTypes()
        {
            return;
        }

        public void RegisterService()
        {
            //Register the Service as singleton
            RegisterCustomerService(typeof(IDocumentService), typeof(DocumentService), true);
            RegisterCustomerService(typeof(ISelectionService), typeof(SelectionServiceProvider), true);
            RegisterCustomerService(typeof(IHtmlServiceProvider), typeof(HtmlServiceProvider), true);
            RegisterCustomerService(typeof(IUpdateService), typeof(UpdateService), true);
            RegisterCustomerService(typeof(IFormatPainterService), typeof(FormatPainterService), true);
            RegisterCustomerService(typeof(IWebServer), typeof(WebService), true);
            RegisterCustomerService(typeof(INClickService), typeof(NClickService), true);
            RegisterCustomerService(typeof(ICustomLibraryService), typeof(CustomLibraryServiceProvider), true);

            //Register the some module as singleton
            RegisterCustomerService(typeof(PreviewModel), typeof(PreviewModel), true);
            RegisterCustomerService(typeof(BusyIndicatorContext), typeof(BusyIndicatorContext), true);
            //Register Memory service and initialize it.
            RegisterCustomerService(typeof(IShareMemoryService), typeof(ShareMemorServiceProvider), true);
        }       

    }
}
