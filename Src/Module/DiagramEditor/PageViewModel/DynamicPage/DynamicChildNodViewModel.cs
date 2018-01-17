using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows.Data;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.Module
{
    public class DynamicChildNodViewModel : ViewModelBase
    {
        public DynamicChildNodViewModel(IPage childPage)
        {
            _page = childPage;
            GID = childPage.Guid;
            Name = childPage.Name;
            isChecked = false;
            _canDelete = false;
        }
        #region Private Functions and Properties
        private IPage _page;
        #endregion

        #region Public Binding Propery
        private Guid id;
        public Guid GID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    FirePropertyChanged("GID");
                }
            }
        }
        public IPage Page
        {
            get { return _page; }  
        }

        public string Name
        {
            get { return _page.Name; }
            set
            {
                if (_page.Name != value)
                {
                    _page.Name = value;

                    IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                    if (doc != null)
                    { 
                        doc.Document.IsDirty = true;
                    }                    
                    FirePropertyChanged("Name");
                }
            }
        }        

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if(isChecked!=value)
                {
                    isChecked = value;
                    FirePropertyChanged("IsChecked");
                }
            }

        }

        private int _showNumber;
        public int ShowNumber
        {
            get { return _showNumber; }
            set
            {
                if (_showNumber != value)
                {
                    _showNumber = value;
                    FirePropertyChanged("ShowNumber");
                }
            }
        }

        private int _showType;
        public int ShowType
        {
            get { return _showType; }
            set
            {
                if (_showType != value)
                {
                    _showType = value;
                    FirePropertyChanged("ShowType");
                }
            }
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get { return _canDelete; }
            set
            {
                if (_canDelete != value)
                {
                    _canDelete = value;
                    FirePropertyChanged("CanDelete");
                }
            }

        }
        #endregion

        #region Popup Binding Propery
        bool isEditboxFocus;
        public bool IsEditboxFocus
        {
            get { return isEditboxFocus; }
            set
            {
                if (isEditboxFocus != value)
                {
                    isEditboxFocus = value;
                    FirePropertyChanged("IsEditboxFocus");
                }
            }
        }
        private bool _isNodeEditable;
        public bool IsNodeEditable
        {
            get { return _isNodeEditable; }
            set
            {
                if (_isNodeEditable != value)
                {
                    _isNodeEditable = value;
                    FirePropertyChanged("IsNodeEditable");
                }
            }
        }
        private bool IsNameCollision(string name)
        {
            return false;
        }
        #endregion 
    }
}

