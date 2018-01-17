using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Windows;
using Naver.Compass.Common.Helper;
using System.Windows.Media;

namespace Naver.Compass.Module
{
    public class NodeViewModel : ViewModelBase
    {
        public NodeViewModel(IAnnotationField field, bool editable = true)
        {
            _field = field;
            _preName = _field.Name;
            _isNodeEditable = editable;

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _document = doc.Document; 
        }

        public string NoteName
        {
            get
            {
                return _field.Name;
            }

            set
            {
                if (!String.IsNullOrEmpty(value) && _field.Name != value)
                {
                    _field.Name = value;
                    _document.IsDirty = true;

                    FirePropertyChanged("NoteName");
                }
            }
        }

        //binding to UI :be changed when click empty area on listbox 
        public bool IsNodeEditable
        {
            get { return _isNodeEditable; }
            set
            {
                if (_isNodeEditable != value)
                {
                    // Change to readonly.
                    if (value == false)
                    {
                        if(IsNameCollision(_field.Name))
                        {
                            MessageBox.Show(GlobalData.FindResource("PageNoteDialog_NameAlert"), 
                                GlobalData.FindResource("PageNoteDialog_Warning"),
                                MessageBoxButton.OK);
                            NoteName = _preName;
                            return;
                        }
                        else
                        {
                            _preName = NoteName;
                        }
                    }

                    _isNodeEditable = value;
                    FirePropertyChanged("IsNodeEditable");
                }
            }
        }

        private bool IsNameCollision(string name)
        {
            NodeViewModel node = PageNoteModel.GetInstance().NoteList.FirstOrDefault(x => x.NoteName == name);
            if (node != null && node != this)
            {
                return true;
            }
            return false;
        }

        private IDocument _document;
        private IAnnotationField _field;
        private bool _isNodeEditable;

        // This is a workaround for checking name collision. Remove this when we can set text after lost focus.
        private string _preName;
    }

}
