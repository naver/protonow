using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Regions;

namespace MainToolBar
{
    public class MainToolBarModule:IModule
    {
        public MainToolBarModule(IUnityContainer container,IRegionManager regionManager)
        {
            Container = container;
            RegionManager = regionManager;
        }

        public void Initialize()
        {
            var RibbonView = Container.Resolve<RibbonView>();
           // RegionManager.Regions["RibbonRegion"].Add(RibbonView);
        }

        public IUnityContainer Container { get; private set; }
        public IRegionManager RegionManager { get; private set; }

    }
}
