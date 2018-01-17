using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module
{
    public class PageNoteFieldsViewModel:ViewModelBase
    {
        public PageNoteFieldsViewModel()
        {
            this.AddNoteCommand = new DelegateCommand<object>(AddNoteExecute, CanExecuteAddNote);
            this.MoveUpCommand = new DelegateCommand<object>(MoveUpExecute, CanExecuteMoveUp);
            this.MoveDownCommand = new DelegateCommand<object>(MoveDownExecute, CanExecuteMoveDown);
            this.DeleteNodeCommand = new DelegateCommand<object>(DeleteNodeExecute, CanExecuteDeleteNode);
            this.CloseWindowCommand = new DelegateCommand<Window>(CloseWindowExecute);
            //NoteList = new ObservableCollection<Node>();

            _model = PageNoteModel.GetInstance();

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _document =  doc.Document;

        }

        #region Commands and functions.
        public DelegateCommand<object> AddNoteCommand { get; private set; }
        public DelegateCommand<object> MoveUpCommand { get; private set; }
        public DelegateCommand<object> MoveDownCommand { get; private set; }
        public DelegateCommand<object> DeleteNodeCommand { get; private set; }
        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        
        public void AddNoteExecute(object obj)
        {
            NodeViewModel node = _model.AddNote();
            SelectValue = node;
            _document.IsDirty = true;
        }
        public bool CanExecuteAddNote(object obj)
        {
            return true;
        }
        public void MoveUpExecute(object obj)
        {
            NodeViewModel node = selectValue;
            _model.MoveUp(node);
            SelectValue = node;
            _document.IsDirty = true;
        }
        public bool CanExecuteMoveUp(object obj)
        {
            return _model.IndexOf(selectValue) > 0 && selectValue != null;
        }
        public void MoveDownExecute(object obj)
        {
            NodeViewModel node = selectValue;
            _model.MoveDown(node);
            SelectValue = node;
            _document.IsDirty = true;
        }
        public bool CanExecuteMoveDown(object obj)
        {
            return _model.IndexOf(selectValue) < _model.Count - 1 && selectValue != null;
        }
        public void DeleteNodeExecute(object obj)
        {
            //MessageBoxResult res = MessageBox.Show(GlobalData.FindResource("PageNoteDialog_Alert"),
            //    GlobalData.FindResource("PageNoteDialog_Warning"), MessageBoxButton.YesNo);
            //if (res == MessageBoxResult.No)
            //    return;
            int index = _model.IndexOf(selectValue);
            _model.DeleteNode(selectValue);
            if (index > 0)
            {
                SelectValue = _model.ElementAt(--index);
            }
            else
            {
                SelectValue = _model.ElementAt(0);
            }
            _document.IsDirty = true;
        }
        public bool CanExecuteDeleteNode(object obj)
        {
            return selectValue != null && NoteList.Count > 1;
        }

        public void CloseWindowExecute(Window win)
        {
            if(win!=null)
            {
                _ListEventAggregator.GetEvent<UpdateNoteEvent>().Publish("PageNote");
                win.Close();
               
            }
        }
        #endregion

        #region property
        public ObservableCollection<NodeViewModel> NoteList
       {
           get { return _model.NoteList; }
           set
           {
               if (_model.NoteList != value)
               {
                   _model.NoteList = value;
               }
               
           }
       }

       private NodeViewModel selectValue;
        public NodeViewModel SelectValue
        {
            get
            {
                return selectValue;
            }
            set
            {
                if (selectValue != value)
                {
                    selectValue = value;
                    FirePropertyChanged("SelectValue");
                    RefreshCommands();
                }
            }
        }
        #endregion

        #region private member
        private PageNoteModel _model;
        private IDocument _document;

        
        private void RefreshCommands()
        {
            AddNoteCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
            DeleteNodeCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }

}
