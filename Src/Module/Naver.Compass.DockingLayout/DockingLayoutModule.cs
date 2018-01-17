using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace DockingLayout
{
    public class DockingLayoutModule:IModule
    {
        public DockingLayoutModule(IUnityContainer container)
        {
            Container = container;
        }

        public void Initialize()
        {
            var layoutView = Container.Resolve<DockingLayoutView>();

        }

        public IUnityContainer Container { get; private set; }
    }
}
