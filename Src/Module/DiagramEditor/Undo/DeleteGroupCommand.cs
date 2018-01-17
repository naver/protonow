using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Service.Document;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Module
{
    class DeleteGroupCommand : IUndoableCommand
    {
        public DeleteGroupCommand(PageEditorViewModel pageVM, GroupViewModel groupVM)
        {
            _pageVM = pageVM;
            _group = groupVM.ExternalGroup;

            _childWidgetVMs = new List<WidgetViewModBase>(groupVM.WidgetChildren);

            foreach (WidgetViewModBase widgetVM in _childWidgetVMs)
            {
                List<Guid> placedPageViewGuids = new List<Guid>();
                foreach (IPageView pageView in _pageVM.PageEditorModel.PageViews)
                {
                    if (pageView.Widgets.Contains(widgetVM.WidgetID))
                    {
                        placedPageViewGuids.Add(pageView.Guid);
                    }
                }

                _placedPageViewGuidsMap[widgetVM.WidgetID] = placedPageViewGuids;
            }
        }

        public void Undo()
        {
            // Add the group to document
            _pageVM.PageEditorModel.AddClonedGroup2Dom(_group);

            // Add child widgetVM
            foreach (WidgetViewModBase widgetVM in _childWidgetVMs)
            {
                _pageVM.AddWidgetItem(widgetVM);

                List<Guid> placedPageViewGuids = _placedPageViewGuidsMap[widgetVM.WidgetID];
                foreach (Guid guid in placedPageViewGuids)
                {
                    IPageView pageView = _pageVM.PageEditorModel.PageViews[guid];

                    if (pageView != null)
                    {
                        pageView.PlaceWidget(widgetVM.WidgetModel.WdgDom.Guid);
                    }
                }
            }

            // Create new GroupViewModel with same group
            _pageVM.CreateGroupRender(_group);
        }

        public void Redo()
        {
            GroupViewModel groupVM = _pageVM.Items.FirstOrDefault(x => x.WidgetID == _group.Guid) as GroupViewModel;
            if (groupVM != null)
            {
                groupVM.IsSelected = false;
                _pageVM.DeleteItem(groupVM);
            }
        }

        private PageEditorViewModel _pageVM;
        private IGroup _group;
        List<WidgetViewModBase> _childWidgetVMs;
        Dictionary<Guid, List<Guid>> _placedPageViewGuidsMap = new Dictionary<Guid, List<Guid>>();
    }
}
