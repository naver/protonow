using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;

namespace Naver.Compass.Module
{
    class DeleteWidgetInGroupSubCommand
    {
        public DeleteWidgetInGroupSubCommand(PageEditorViewModel pageVM, WidgetViewModBase widgetVM)
        {
            _pageVM = pageVM;
            _widgetVM = widgetVM;
            _parentGroupInVM = _pageVM.PageEditorModel.Groups[widgetVM.ParentID];
            _parentGroupInDOM = _widgetVM.WidgetModel.RealParentGroup;

            foreach (IPageView pageView in _pageVM.PageEditorModel.PageViews)
            {
                if (pageView.Widgets.Contains(_widgetVM.WidgetID))
                {
                    _placedPageViewGuids.Add(pageView.Guid);
                }
            }
        }

        public void Undo()
        {
            _pageVM.AddWidgetItem(_widgetVM);

            foreach (Guid guid in _placedPageViewGuids)
            {
                IPageView pageView = _pageVM.PageEditorModel.PageViews[guid];

                if (pageView != null)
                {
                    pageView.PlaceWidget(_widgetVM.WidgetModel.WdgDom.Guid);
                }
            }

            if (_parentGroupInDOM != null)
            {
                // Add the widget parent group to page. If the group was removed when the widget was deleted, 
                // this will restore the group and the group tree.
                _pageVM.PageEditorModel.AddClonedGroup2Dom(_parentGroupInDOM);
            }

            _pageVM.CreateGroupRender(_parentGroupInVM);
        }

        public void Redo()
        {
            _widgetVM.IsSelected = false;

            // Delete widgets in group
            GroupViewModel groupVM = _pageVM.Items.OfType<GroupViewModel>().Where(c => c.WidgetID == _widgetVM.ParentID).FirstOrDefault<GroupViewModel>();
            if (groupVM != null)
            {
                IGroup parentGroup = groupVM.ExternalGroup;
                if (parentGroup == null)
                {
                    return;
                }

                _widgetVM.ParentID = Guid.Empty;
                _pageVM.DeleteItem(_widgetVM);

                if (_pageVM.PageEditorModel.Groups.Contains(parentGroup.Guid))
                {
                    // The old group still exists, recreate this group
                    _pageVM.CreateGroupRender(parentGroup);
                }
                else
                {
                    // The old group doesn't exist in DOM, which means the old group is split to its child groups or widgets.

                    // Set the rest widgets VM ParentID to Guid.Empty.
                    foreach (WidgetViewModBase childWdgItem in groupVM.WidgetChildren)
                    {
                        childWdgItem.ParentID = Guid.Empty;
                    }

                    // Delete old group VM.
                    _pageVM.Items.Remove(groupVM);

                    // Then create sub group if necessary
                    foreach (IGroup childGroup in parentGroup.Groups)
                    {
                        _pageVM.CreateGroupRender(childGroup);
                    }
                }
            }
            
        }

        private PageEditorViewModel _pageVM;
        private WidgetViewModBase _widgetVM;
        private IGroup _parentGroupInVM;
        private IGroup _parentGroupInDOM;
        private List<Guid> _placedPageViewGuids = new List<Guid>();
    }
}

