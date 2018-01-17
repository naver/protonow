using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Data;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module
{
    public class PageNoteModel
    {
        PageNoteModel()
        {
            NoteList = new ObservableCollection<NodeViewModel>();
        }

        public static PageNoteModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PageNoteModel();
            }
            return _instance;
        }

        public void LoadNoteFields()
        {
            NoteList.Clear();
            foreach(IAnnotationField item in PageNoteFieldSet.AnnotationFields)
            {
                NoteList.Add(new NodeViewModel(item, false));
            }
        }

        public NodeViewModel AddNote()
        {
            IAnnotationField field = PageNoteFieldSet.CreateAnnotationField(GetNoteName(), AnnotationFieldType.Text);

            NodeViewModel node = new NodeViewModel(field);
            NoteList.Add(node);

            return node;
        }

        public void MoveUp(NodeViewModel node)
        {
            int index = NoteList.IndexOf(node);
            NoteList.Remove(node);
            NoteList.Insert(--index, node);
            PageNoteFieldSet.MoveAnnotationField(node.NoteName, -1);
        }

        public void MoveDown(NodeViewModel node)
        {
            int index = NoteList.IndexOf(node);
            NoteList.Remove(node);
            NoteList.Insert(++index, node);
            PageNoteFieldSet.MoveAnnotationField(node.NoteName, 1);
        }

        public void DeleteNode(NodeViewModel node)
        {
            NoteList.Remove(node);
            PageNoteFieldSet.DeleteAnnotationField(node.NoteName);
        }

        public int IndexOf(NodeViewModel node)
        {
            return NoteList.IndexOf(node);
        }

        public NodeViewModel ElementAt(int index)
        {
            if (NoteList.Count > 0)
                return NoteList.ElementAt(index);
            else
                return null;
        }

        public int Count 
        {
            get { return NoteList.Count; } 
        }

        public ObservableCollection<NodeViewModel> NoteList { get; set; }

        public IAnnotationFieldSet PageNoteFieldSet 
        {
            get 
            {
                return _document == null ? null : _document.PageAnnotationFieldSet;
            }
        }

        public void ResetNameCounter()
        {
            _noteCounter = 1;
        }

        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        private string DefaultNoteName
        {
            get { return String.Concat(_noteName, _noteCounter++); }
        }

        private string GetNoteName()
        {
            string noteName = DefaultNoteName;

            if (NoteList.Any(x => x.NoteName == noteName))
            {
                return GetNoteName();
            }

            return noteName;
        }

        private static PageNoteModel _instance;
        private string _noteName = "Note ";
        private int _noteCounter = 1;
    }
}
