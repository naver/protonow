using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        private bool _isHamburgerEditorVisible;
        public bool IsHamburgerEditorVisible
        {
            get { return _isHamburgerEditorVisible; }
            set
            {
                if (_isHamburgerEditorVisible != value && CurrentUndoManager != null)
                {
                    PropertyMementos mementos = new PropertyMementos();
                    mementos.AddPropertyMemento(new PropertyMemento("Raw_IsHamburgerEditorVisible", _isHamburgerEditorVisible, _isHamburgerEditorVisible));
                    mementos.SetPropertyNewValue("Raw_IsHamburgerEditorVisible", value);
                    PropertyChangeCommand propCmd = new PropertyChangeCommand(this, mementos);
                    CurrentUndoManager.Push(propCmd);
                }
                Raw_IsHamburgerEditorVisible = value;
            }
        }
        public bool Raw_IsHamburgerEditorVisible
        {
            set
            {
                if (_isHamburgerEditorVisible != value)
                {
                    _isHamburgerEditorVisible = value;
                    FirePropertyChanged("IsHamburgerEditorVisible");
                }
            }
        }
        public void EditHanburgerPage()
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            var imgWdgs = wdgs.OfType<HamburgerMenuViewModel>();
            if (imgWdgs.Count() != 1)
            {
                return;
            }
            HamburgerMenuViewModel hamburger = imgWdgs.First();
            if (hamburger == null)
                return;

            if (!this.IsHamburgerEditorVisible)
            {
                this.IsHamburgerEditorVisible = true;
            }
            this.MenuPageEditorViewModel.InitHamburgerEditor(hamburger);
        }

        public void CancelEditHamburgerPage()
        {
            if (this.IsHamburgerEditorVisible)
            {
                this.IsHamburgerEditorVisible = false;
            }
        }
    }
}
