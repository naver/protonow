using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
    class NotesViewModel:ViewModelBase
    {
        public NotesViewModel()
        {
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(SelectionChangeEventHandler);
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler);
        }


        #region event handler
        private void SelectionPageChangeHandler(Guid pageGuid)
        {
            if (pageGuid == Guid.Empty)
                return;

            if (_document != null)
            {
                var page = _document.Pages.GetPage(pageGuid);
                if(page == null)
                {
                    page = _document.MasterPages.GetPage(pageGuid);
                }
                selectedPage = page;
                
                LoadNotes();
            }
        }

        private void SelectionChangeEventHandler(string EventArg)
        {
            LoadNotes();
        }
        #endregion

        #region property
        private string widgetNotes;
        public string WidgetNote
        {
            get
            {
                return widgetNotes;
            }
            set
            {
                if(widgetNotes!=value)
                {
                    if (CurrentUndoManager != null)
                    {
                        WidgetNoteCommand cmd = new WidgetNoteCommand(this, "Raw_WidgetNote", widgetNotes, value,
                                                                      _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                        CurrentUndoManager.Push(cmd);
                    }

                    Raw_WidgetNote = value;
                }
            }
        }

        public string Raw_WidgetNote
        {
            set
            {
                if (widgetNotes != value)
                {
                    widgetNotes = value;
                    if (_selectedWidget != null && isSetByUser)
                    {
                        if (_selectedWidget is IWidget)
                        {
                            (_selectedWidget as IWidget).Annotation.SetTextValue("Description", value);
                        }
                        else if(_selectedWidget is IMaster)
                        {
                            (_selectedWidget as IMaster).Annotation.SetTextValue("Description", value);
                        }
                        
                        _document.IsDirty = true;
                    }
                    FirePropertyChanged("WidgetNote");
                }
            }
        }

        private bool isNotesEnabled;
        public bool IsNotesEnabled
        {
            get
            {
                return isNotesEnabled;
            }
            set
            {
                if(isNotesEnabled!=value)
                {
                    isNotesEnabled = value;
                    FirePropertyChanged("IsNotesEnabled");
                }
            }
        }

        #endregion 

        #region private memeber

        private ISelectionService _selectionService;
        private IPage selectedPage;

        private bool isSetByUser = true;
        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        private IRegion _selectedWidget
        {
            get
            {//Edit event only when select one widget, so guids[0].
                List<Guid> guids = _selectionService.GetSelectedWidgetGUIDs();
                if (guids.Count != 1)
                    return null;

                var iwidget = selectedPage.WidgetsAndMasters.GetRegion(guids[0]);
                return iwidget;
            }
        }

        private void LoadNotes()
        {
            if (selectedPage == null || _selectedWidget == null)
            {
                Raw_WidgetNote = string.Empty;
                IsNotesEnabled = false;
                return;
            }

            isSetByUser = false;

            if (_selectedWidget is IWidget && (_selectedWidget as IWidget).Annotation != null)
            {
                Raw_WidgetNote = (_selectedWidget as IWidget).Annotation.GetextValue("Description");
                IsNotesEnabled = true;
               
            }
            else if (_selectedWidget is IMaster && (_selectedWidget as IMaster).Annotation != null)
            {
                Raw_WidgetNote = (_selectedWidget as IMaster).Annotation.GetextValue("Description");
                IsNotesEnabled = true;
            }
            else
            {
                Raw_WidgetNote = string.Empty;
                IsNotesEnabled = false;
            }

            isSetByUser = true;
        }
        #endregion
    }
}
