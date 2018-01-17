using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        #region Operation Functions
        public GroupViewModel CreateGroupRender(IGroup targetGroup, CompositeCommand cmds = null)
        {
            //create group element
            GroupViewModel NewGroupVM = WidgetFactory.CreateGroup(targetGroup) as GroupViewModel;

            //remove the older inner group
            List<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true).ToList<GroupViewModel>();
            foreach (GroupViewModel it in AllGroups)
            {
                if (NewGroupVM.IsChild(it.widgetGID, true))
                {
                    it.IsSelected = false;
                    _selectionService.RemoveWidget(it);
                    items.Remove(it);
                }
            }


            //initialize all elements in new group
            foreach (WidgetViewModBase wdg in Items)
            {
                if (true == NewGroupVM.IsChild(wdg.widgetGID, false))
                {
                    wdg.ParentID = NewGroupVM.widgetGID;
                    wdg.IsGroup = false;
                    wdg.IsSelected = false;
                    NewGroupVM.AddChild(wdg);
                }
            }

            #region Reset Zorder

            // We got a crash report which was caused by  the exception System.InvalidOperationException from
            // System.Linq.Enumerable.Max(System.Collections.Generic.IEnumerable`1<Int32>), so make sure WidgetChildren has element first.
            // See http://bts1.nhncorp.com/nhnbts/browse/DSTUDIO-1679
            SetTargetObject2Front(NewGroupVM.WidgetChildren, cmds);
            //SetGroupTargetObjectContinue(, cmds, NewGroupVM);

            #endregion

            //UI Render the new group
            NewGroupVM.IsSelected = true;
            NewGroupVM.Status = GroupStatus.Selected;
            //NewGroupVM.ZOrder = -3;
            Items.Add(NewGroupVM);
            NewGroupVM.Refresh();

            _ListEventAggregator.GetEvent<GroupChangedEvent>().Publish(true);
            return NewGroupVM;
        }
        private void SetGroupTargetObjectContinue(List<WidgetViewModBase> targetWidgets, CompositeCommand cmds, GroupViewModel NewGroupVM)
        {
            if (targetWidgets.Count < 1)
            {
                return;
            }

            int maxZOrder = targetWidgets.Max(a => a.ZOrder);
            int minZOrder = targetWidgets.Min(a => a.ZOrder);
            if (targetWidgets.Count() < (maxZOrder - minZOrder + 1))
            {
                targetWidgets.Sort(delegate(WidgetViewModBase a1, WidgetViewModBase a2)
                {
                    if (a1.ZOrder == a2.ZOrder)
                        return 0;
                    return (a1.ZOrder < a2.ZOrder) ? 1 : -1;
                });

                int maxZ = maxZOrder;
                foreach (WidgetViewModBase item in targetWidgets)
                {
                    //Undo command
                    if (cmds != null)
                    {
                        PropertyChangeCommand cmd = new PropertyChangeCommand(item, "ZOrder", item.ZOrder, maxZ);
                        cmds.AddCommand(cmd);
                    }
                    item.ZOrder = maxZ--;
                }

                List<WidgetViewModBase> widgets = items.OfType<WidgetViewModBase>().Where(a => a.ZOrder > minZOrder && a.ZOrder < maxZOrder && !(NewGroupVM.IsChild(a.widgetGID, false))).ToList<WidgetViewModBase>();
                widgets.Sort(delegate(WidgetViewModBase a1, WidgetViewModBase a2)
                {
                    if (a1.ZOrder == a2.ZOrder)
                        return 0;
                    return (a1.ZOrder > a2.ZOrder) ? 1 : -1;
                });

                int minZ = minZOrder;
                foreach (WidgetViewModBase item in widgets)
                {
                    //Undo command
                    if (cmds != null)
                    {
                        PropertyChangeCommand cmd = new PropertyChangeCommand(item, "ZOrder", item.ZOrder, minZ);
                        cmds.AddCommand(cmd);
                    }
                    item.ZOrder = minZ++;
                }
            }

        }

        public void UnGroup(GroupViewModel groupVM)
        {
            GroupViewModel targetVM = groupVM;
            List<WidgetViewModBase> WidgetChildren = targetVM.WidgetChildren;
            foreach (WidgetViewModBase item in WidgetChildren)
            {
                item.ParentID = Guid.Empty;
            }

            IGroups ChildGroups = targetVM.GroupChildren;
            //======================================================render the group
            //Create the external Groups
            List<WidgetViewModBase> GroupVMs = new List<WidgetViewModBase>();
            foreach (IGroup it in ChildGroups)
            {
                WidgetViewModBase vmItem = WidgetFactory.CreateGroup(it);
                if (vmItem == null)
                {
                    return;
                }
                vmItem.IsGroup = true;
                GroupVMs.Add(vmItem);
            }

            //Remove the parent group
            targetVM.IsSelected = false;
            _selectionService.RemoveWidget(targetVM);
            Items.Remove(targetVM);

            //Initialize the new Group
            //if (GroupVMs.Count <= 0)
            //    return;
            foreach (WidgetViewModBase wdg in Items)
            {
                foreach (GroupViewModel group in GroupVMs)
                {
                    if (true == group.IsChild(wdg.widgetGID, false))
                    {
                        wdg.ParentID = group.widgetGID;
                        wdg.IsGroup = false;
                        wdg.IsSelected = false;
                        group.AddChild(wdg);
                    }
                }
            }

            //UI Render the Groups
            foreach (GroupViewModel group in GroupVMs)
            {
                group.Status = GroupStatus.Selected;
                Items.Add(group);
                group.Refresh();
            }
            //===========================================


            //Select new childreb
            foreach (WidgetViewModBase item in WidgetChildren)
            {
                if (item.ParentID == Guid.Empty)
                {
                    item.IsSelected = true;
                    item.IsGroup = false;
                }
            }

            //Last
            _model.UnGroup(groupVM.WidgetID);
            _ListEventAggregator.GetEvent<GroupChangedEvent>().Publish(false);
        }
        #endregion

        #region IGroupOperation(Editor)
        public GroupStatus GetGroupStatus(Guid GroupID)
        {
            IEnumerable<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true);
            foreach (GroupViewModel it in AllGroups)
            {
                if (it.WidgetID == GroupID)
                {
                    return it.Status;
                }
            }
            return GroupStatus.UnSelect;
        }
        public void SetGroupStatus(Guid GroupID, GroupStatus status)
        {
            IEnumerable<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true);
            foreach (GroupViewModel it in AllGroups)
            {
                if (it.WidgetID == GroupID)
                {
                    it.Status = status;
                    break;
                }
            }

        }
        public void DeselectAllChildren(Guid GroupID)
        {
            IEnumerable<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true);
            foreach (GroupViewModel it in AllGroups)
            {
                if (it.WidgetID == GroupID)
                {
                    it.DeSelectAllChildren();
                    break;
                }
            }
        }
        public void DeselectAllGroups()
        {
            IEnumerable<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true);
            foreach (GroupViewModel it in AllGroups)
            {
                it.Status = GroupStatus.UnSelect;
            }
        }
        public void UpdateGroup(Guid GroupID)
        {
            IEnumerable<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true);
            foreach (GroupViewModel it in AllGroups)
            {
                if (it.WidgetID == GroupID)
                {
                    it.Refresh();
                    break;
                }
            }
        }
        public void MoveSelectedGroup(double OffsetX, double OffsetY)
        {
            IEnumerable<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true);
            foreach (GroupViewModel it in AllGroups)
            {
                if (it.Status == GroupStatus.Selected)
                {
                    it.Move(OffsetX, OffsetY);
                }
            }
        }
        public List<Guid> GetAllSelectedGroups()
        {
            List<Guid> all = new List<Guid>();
            IEnumerable<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true);
            foreach (GroupViewModel it in AllGroups)
            {
                if (it.Status == GroupStatus.Selected)
                {
                    all.Add(it.WidgetID);
                    continue;
                }
            }
            return all;
        }

        public GroupViewModel GetTargetGroup(Guid GroupID)
        {
            IEnumerable<GroupViewModel> AllGroups = items.OfType<GroupViewModel>().Where(c => c.IsGroup == true);
            return AllGroups.FirstOrDefault(c => c.widgetGID == GroupID);
        }
        #endregion IGroupOperation(Editor)
    }
}
