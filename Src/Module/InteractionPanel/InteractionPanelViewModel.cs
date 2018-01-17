using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    public class InteractionPanelViewModel:ViewModelBase
    {
        public InteractionPanelViewModel()
        {
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(SelectionChangeEventHandler);
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeEventHandler);
            _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Subscribe(SelectionPropertyEventHandler);

            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
        }

        private void SelectionChangeEventHandler(string EventArg)
        {

            FirePropertyChanged("Name");
            FirePropertyChanged("Type");
            if (_selectionService.GetSelectedWidgets().Count == 0)
                CanEdit = false;
            else
                CanEdit = true;
        }

        private void SelectionPageChangeEventHandler(Guid EventArg)
        {
            IPagePropertyData page = _selectionService.GetCurrentPage();
            if (page == null)
            {
                return;
            }
            if (CmdTarget == page.EditorCanvas)
            {
                return;
            }
            CmdTarget = _selectionService.GetCurrentPage().EditorCanvas;
        }

        private void SelectionPropertyEventHandler(string EventArg)
        {
            if(EventArg == "Name")
            {
                FirePropertyChanged("Name");
            }
        }
        public string Type
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return string.Empty;

                var val = (wdgs[0] as WidgetViewModBase).Type;
                foreach (WidgetViewModBase item in wdgs)
                {
                    if (val != item.Type)
                        return null;
                }
                return ConvertObject.GetTypeName(val);
            }
        }   
        public string Name
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return string.Empty;

                string val = (wdgs[0] as WidgetViewModelDate).Name;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.Name)
                        return string.Empty;
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.Name.Execute(value, CmdTarget);
                _document.IsDirty = true;
                FirePropertyChanged("Name");
            }
        }

        private bool canEdit;
        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                if(canEdit!=value)
                {
                    canEdit = value;
                    FirePropertyChanged("CanEdit");
                }
            }
        }

        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        public IInputElement CmdTarget
        {
            get;
            set;
        }

        private ISelectionService _selectionService;
    }


}
