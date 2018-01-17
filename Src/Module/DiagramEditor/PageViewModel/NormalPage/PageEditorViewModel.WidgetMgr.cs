using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

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
using UndoCompositeCommand = Naver.Compass.InfoStructure.CompositeCommand;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        #region pirvate member
        private Guid _targetLoadingWdgGID = Guid.Empty;
        private bool _targetLoadingWdgIsSelect = false;
        private bool _targetLoadingWdgIsGroup = false;

        #endregion

        #region public function for widget mananger operation
        public void WdgMgrSetTargeComponent(Guid targetGuid,bool bIsSelected, bool bIsGroup)
        {
            _targetLoadingWdgGID = targetGuid;
            _targetLoadingWdgIsSelect = bIsSelected;
            _targetLoadingWdgIsGroup = bIsGroup;
        }
        public void SelectTargetComponent()
        {
            if (_targetLoadingWdgGID == Guid.Empty)
                return;
          
            foreach (WidgetViewModBase wdg in Items)
            {
                if (wdg.widgetGID == _targetLoadingWdgGID)
                {
                    if (wdg.ParentID == Guid.Empty && wdg.IsShowInPageView2Adaptive == true)
                    {
                        if (wdg.IsGroup == false)
                        {
                            wdg.IsSelected = _targetLoadingWdgIsSelect;
                        }
                        else
                        {
                            if (_targetLoadingWdgIsSelect==true)
                                SetGroupStatus(wdg.widgetGID, GroupStatus.Selected);
                            else
                                SetGroupStatus(wdg.widgetGID, GroupStatus.UnSelect);
                        }
                    }
                    else if(wdg.ParentID != Guid.Empty && wdg.IsShowInPageView2Adaptive == true)
                    {
                        wdg.IsSelected = _targetLoadingWdgIsSelect;
                        if (_targetLoadingWdgIsSelect == true)
                        {
                            SetGroupStatus(wdg.ParentID, GroupStatus.Edit);
                        }
                        else
                        {
                            GroupViewModel group = GetTargetGroup(wdg.ParentID);
                            if(group!=null)
                            {
                                if(group.WidgetChildren.Exists(c =>c.IsSelected==true))
                                {
                                    SetGroupStatus(wdg.ParentID, GroupStatus.Edit);
                                }
                                else
                                {
                                    SetGroupStatus(wdg.ParentID, GroupStatus.UnSelect);
                                }

                            }
                            //SetGroupStatus(wdg.ParentID, GroupStatus.UnSelect);
                        }                            
                     }
                    break;
                }
                else
                {
                    continue;
                }                    
            }
            _targetLoadingWdgGID = Guid.Empty;    

        }

        //this function is not used now, ignore it
        public void WdgMgrChangeSelection(object parameter)
        {
            WidgetSelectionInfo info = (WidgetSelectionInfo)parameter;

            List<IWidgetPropertyData> allwidgets = this.GetAllWidgets();
            foreach (IWidgetPropertyData item in allwidgets)
            {
                if (item.WidgetID.Equals(info.WidgetID))
                {
                    ((WidgetViewModBase)item).IsSelected = info.bSelected;
                    break;
                }
            }
        }


        public void WdgMgrDeleteSelection(List<Guid> list)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }

            List<WidgetViewModBase> DeleteList = new List<WidgetViewModBase>();
            foreach(Guid id in list)
            {
                WidgetViewModBase widget = Items.FirstOrDefault(a => a.WidgetID == id);

                if (widget != null)
                {
                    DeleteList.Add(widget);
                }
            }

           
            OnItemListDelete(DeleteList);
        }

        public void WdgMgrHideSelection(bool bIsHide)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            WidgetsHideChangeCommandHandler(bIsHide);
        }
        public void WdgMgrHideAllWidgets(bool bIsHide)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            

            //TODO:
            if (Items.Count == 0)
                return;
           
            Naver.Compass.InfoStructure.CompositeCommand cmds = new Naver.Compass.InfoStructure.CompositeCommand();
            foreach (WidgetViewModBase item in Items)
            {
                if (bIsHide != item.IsHidden)
                {                    
                    CreatePropertyChangeUndoCommand(cmds, item, "IsHidden", item.IsHidden, bIsHide);
                    item.IsHidden = bIsHide;
                }
            }
            PushToUndoStack(cmds);
        }
        public void WdgMgrEditSelection()
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            
            //TODO:
            if(_selectionService.WidgetNumber!=1)
            {
                return;
            }

            WidgetViewModBase wdg = _selectionService.GetSelectedWidgets()[0] as WidgetViewModBase;
            if(wdg==null||wdg is  GroupViewModel)
            {
                return;                
            }

            wdg.CanEdit = true;
            //TODO:2015/09 edit mode for special 
            //wdg.type!=WidgetType.SVG
            
           
        }

        public void WdgMgrZOrderChangeSelection(bool bIsForword)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            
            if(bIsForword==true)
            {
                WidgetsBringForwardCommandHandler(null);
            }
            else
            {
                WidgetsBringBackwardCommandHandler(null);
            }
        }

        public void  WdgMgrPlaceTargets(List<Guid> targets)
        {
            PlaceFromViewCommandHandler(targets);
        }

        public void WdgMgrReZOrderSelection(int tarZorder, Guid tarWdgGuid)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
           // tarWdgGuid = Guid.NewGuid();
            WidgetViewModBase wdg = items.FirstOrDefault(c => c.WidgetID == tarWdgGuid);
            if (wdg == null)
            {
                return;
            }
            int orgZorder = wdg.ZOrder;
            int nOriCount = 1;
            if (wdg.IsGroup==true)
            {
                nOriCount = (wdg as GroupViewModel).WidgetChildren.Count();
            }


            CompositeCommand cmds = new CompositeCommand();
            bool bIsZOrderChanged = false;

            //up drag to other widgets
            List<WidgetViewModBase> allothers;
            int nOtherNum = 0;
            if (tarZorder > orgZorder)
            {
                allothers=items.Where(c => c.ZOrder < tarZorder && c.ZOrder > orgZorder && c.IsGroup == false).ToList<WidgetViewModBase>();
                foreach(WidgetViewModBase it in allothers)
                {
                    CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder, it.ZOrder - nOriCount);
                    it.ZOrder = it.ZOrder - nOriCount;
                    bIsZOrderChanged = true;
                }
                nOtherNum = allothers.Count();
            }            
            else if(tarZorder < orgZorder)//down drag to other widgets
            {
                allothers=items.Where(c => c.ZOrder < (orgZorder - nOriCount + 1) && c.ZOrder >= tarZorder && c.IsGroup == false).ToList<WidgetViewModBase>();
                foreach (WidgetViewModBase it in allothers)
                {
                    CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder, it.ZOrder + nOriCount);
                    it.ZOrder = it.ZOrder + nOriCount;
                    bIsZOrderChanged = true;
                }
                nOtherNum = -allothers.Count();
            }


            //adjust the drag item's zorder
            if (wdg.IsGroup == true)
            {
                foreach (WidgetViewModBase it in (wdg as GroupViewModel).WidgetChildren)
                {
                    CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder, it.ZOrder + nOtherNum);
                    it.ZOrder = it.ZOrder + nOtherNum;
                    bIsZOrderChanged = true;
                }
            }
            else
            {
                CreatePropertyChangeUndoCommand(cmds, wdg, "ZOrder", wdg.ZOrder, wdg.ZOrder + nOtherNum);
                wdg.ZOrder = wdg.ZOrder + nOtherNum;
                bIsZOrderChanged = true;
            }
            _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);

            //Push Command to Undo/Redo stack
            if (bIsZOrderChanged == true)
            {
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }   

            //items.Where(c => c.IsSelected == true).ToList<WidgetViewModBase>();
            //List<WidgetViewModBase> AllChildren = ParentGroup.WidgetChildren;
            //List<WidgetViewModBase> targetWidgets = ParentGroup.WidgetChildren.Where(c => c.IsSelected == true).ToList<WidgetViewModBase>();
            //if (targetWidgets.Count() == AllChildren.Count())
            //{
            //    return;
            //}
            //targetWidgets = targetWidgets.OrderByDescending(s => s.ZOrder).ToList<WidgetViewModBase>();
            //int maxZOrder = AllChildren.Max(a => a.ZOrder);

        }



        #endregion

        
    }
}
