using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.Helper;
using System.Windows.Controls;
using System.Windows.Documents;
using Naver.Compass.Service;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Naver.Compass.Service.CustomLibrary;
using Microsoft.Win32;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        #region pirvate member


        #endregion

        #region public function for widget mananger operation
        private List<WidgetViewModBase> CreateVMObjects(IObjectContainer container)
        {
            List<WidgetViewModBase> all = new List<WidgetViewModBase>();
            //common widget
            foreach (IWidget newItem in container.WidgetList)
            {
                if (newItem != null && newItem.ParentGroup == null)
                {
                    WidgetViewModBase vmItem = WidgetFactory.CreateWidget(newItem);
                    vmItem.ParentPageVM = this;
                    all.Add(vmItem); 
                }
            }

            //common master
            foreach (IMaster newItem in container.MasterList)
            {
                if (newItem != null && newItem.ParentGroup == null)
                {
                    WidgetViewModBase vmItem = WidgetFactory.CreateWidget(newItem);
                    vmItem.ParentPageVM = this;
                    all.Add(vmItem);
                }
            }

            //master and it's all children
            foreach (IGroup newGroup in container.GroupList)
            {
                GroupViewModel vmGroup = null;
                if (newGroup != null && newGroup.ParentGroup == null)
                {
                    vmGroup = WidgetFactory.CreateGroup(newGroup) as GroupViewModel;
                    vmGroup.IsGroup = true;
                    vmGroup.Status = GroupStatus.Selected;                    
                }

                List<IRegion> children = new List<IRegion>();
                GetWidgetChildren(newGroup, children);
                if (children.Count < 1 || vmGroup==null)
                {
                    continue;
                }
                all.Add(vmGroup);

                foreach (IRegion item in children)
                {
                    WidgetViewModBase vmChild = WidgetFactory.CreateWidget(item);
                    if (vmChild == null)
                    {
                        continue;
                    }

                    vmChild.IsGroup = false;
                    vmChild.ParentID = newGroup.Guid;
                    vmChild.IsSelected = false;
                    vmGroup.AddChild(vmChild);
                    all.Add(vmChild);
                }
            }
            return all;
        }
        private void GetWidgetChildren(IGroup targetGroup, List<IRegion> children)
        {
            if (targetGroup == null)
                return;

            if (targetGroup.Widgets.Count>0)
            {
                children.AddRange(targetGroup.Widgets);
            }

            if (targetGroup.Masters.Count > 0)
            {
                children.AddRange(targetGroup.Masters);
            }



            if (targetGroup.Groups == null || targetGroup.Groups.Count <= 0)
            {
                return;
            }

            foreach (IGroup item in targetGroup.Groups)
            {
                GetWidgetChildren(item, children);
            }
        }
        #endregion

        
    }
}
