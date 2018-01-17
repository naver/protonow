using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using Naver.Compass.Common.CommonBase;
using Microsoft.Practices.Prism.Events;
using System.Collections.ObjectModel;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.Module
{
    public class PagePropertyViewModel:ViewModelBase
    {
        public PagePropertyViewModel()
        {
            if (this.IsInDesignMode)
                return;

            _model = PageNoteModel.GetInstance();

            this.OpenPageNoteFieldCommand = new DelegateCommand<object>(OpenPageNoteFieldExecute);

            _ListEventAggregator.GetEvent<UpdateNoteEvent>().Subscribe(UpdateNoteHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<OpenDialogEvent>().Subscribe(OpenDialogEventHandler, ThreadOption.UIThread);
           // _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Subscribe(UpdateLanguageEventHandler);
        }

        #region command
        public DelegateCommand<object> OpenPageNoteFieldCommand { get; private set; }

        private void OpenPageNoteFieldExecute(object obj)
        {
            PageNoteFieldsView window = new PageNoteFieldsView();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void UpdateNoteHandler(string type)
        {
            if (type == "PageNote")
            {
                FirePropertyChanged("NotesList");
                if(SelectedValue==null)
                {
                    SelectedValue = NotesList.ElementAt(0).NoteName;
                }
            }
        }
        private void SelectionPageChangeHandler(Guid pageGuid)
        {
            if(pageGuid == Guid.Empty)
            {
                bSet2Document = false;
                Raw_NoteTexts = string.Empty;
                return;
            }

            if (_document != null)
            {
                IPage page = _document.Pages.GetPage(pageGuid);
                if(page != null)
                {
                    _pageAnnotation = page.Annotation;

                    if (selectVlaue != null)
                    {
                        bSet2Document = false;
                        Raw_NoteTexts = _pageAnnotation.GetextValue(selectVlaue);
                    }
                    bSet2Document = true;

                    PageNoteEanbled = true;
                }

                IMasterPage masterPage = _document.MasterPages.GetPage(pageGuid);
                if (masterPage != null)
                {
                    PageNoteEanbled = false;
                }
            }
        }

        public void DomLoadedEventHandler(FileOperationType loadType)
        {
            switch (loadType)
            {
                case FileOperationType.Create:
                case FileOperationType.Open:
                    if (_document != null)
                    {
                        _model.LoadNoteFields();
                        InitParameter(true);
                    }
                    break;
                case FileOperationType.Close:
                    InitParameter(false);
                    break;
            }
        }

        private void OpenDialogEventHandler(DialogType dialogtype)
        {
            if(dialogtype ==DialogType.PageNoteField)
            {
                OpenPageNoteFieldExecute(null);
            }
        }

        private void UpdateLanguageEventHandler(string str)
        {
            FirePropertyChanged("NotesList");

            SelectedValue = NotesList.ElementAt(0).NoteName;
        }
        #endregion

        #region private member

        private IAnnotation _pageAnnotation;
        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        private PageNoteModel _model;

        /// <summary>
        /// Set if NoteTexts is set to document when NoteTexts changed.
        /// true:change it in UI
        /// flase:load it from doc(page changed,SelectValue changed) or close doc (set empty)
        /// </summary>
        private bool bSet2Document;

        /// <summary>
        /// Init parameters when open,load or close project
        /// </summary>
        /// <param name="bOpen">true: create,open false: close</param>
        public void InitParameter( bool bOpen )
        {
            if(bOpen)
            {
                PageNoteEanbled = true;
                if (NotesList.Count > 0)
                {
                    SelectedValue = NotesList.ElementAt(0).NoteName;
                }
                bSet2Document = true;
            }
            else
            {
                PageNoteEanbled = false;
                bSet2Document = false;
            }
            _pageAnnotation = null;
            _model.ResetNameCounter();
            Raw_NoteTexts = string.Empty;
        }

        #endregion

        #region property

        public ObservableCollection<NodeViewModel> NotesList
        {
            get { return PageNoteModel.GetInstance().NoteList; }
        }

        private Dictionary<FlowDocument, string> notesCollection = new Dictionary<FlowDocument, string>();
        public Dictionary<FlowDocument, string> NotesCollection
        {
            get 
            {
                return notesCollection; 
            }  
        }

        //Selected note type.
        string selectVlaue;
        public string SelectedValue
        {
            get { return selectVlaue; }
            set
            {
                if (value != selectVlaue)
                {
                    selectVlaue = value;

                    if(_pageAnnotation!=null && !string.IsNullOrEmpty(value))
                    {
                        bSet2Document = false;
                        Raw_NoteTexts = _pageAnnotation.GetextValue(value);
                        bSet2Document = true;
                    }
                    else
                    {
                        Raw_NoteTexts = string.Empty;
                    }
                    FirePropertyChanged("SelectedValue");
                }
            }
        }

        private string noteTexts;
        public string NoteTexts
        {
            get { return noteTexts; }
            set
            {
                if(noteTexts!=value)
                {
                    if(CurrentUndoManager != null)
                    { 
                        PropertyChangeCommand cmd = new PropertyChangeCommand(this, "Raw_NoteTexts", noteTexts, value);
                        CurrentUndoManager.Push(cmd);
                    }
                    
                    Raw_NoteTexts = value;
                }
            }
        }

        public string Raw_NoteTexts
        {
            set
            {
                if (noteTexts != value)
                {
                    noteTexts = value;
                    FirePropertyChanged("NoteTexts");
                    if (selectVlaue != null && _pageAnnotation != null && bSet2Document)
                    {
                        _pageAnnotation.SetTextValue(selectVlaue, value);
                        _document.IsDirty = true;
                    }
                }
            }
        }

        private bool pageNoteEanbled;
        public bool PageNoteEanbled
        {
            get { return pageNoteEanbled; }
            set
            {
                if(pageNoteEanbled!=value)
                {
                    pageNoteEanbled = value;
                    FirePropertyChanged("PageNoteEanbled");
                }
            }
        }

        private FlowDocument flowDocument = new FlowDocument();
        public FlowDocument FlowDocument
        {
            //get { return flowDocument; }
            set
            {
                if (value != flowDocument)
                {
                   // flowDocument = value;
                   // FirePropertyChanged("FlowDocument");
                }
            }
        }
        #endregion
    }
}
