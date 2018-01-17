using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using Naver.Compass.Common.Helper;

namespace MainToolBar.ViewModel
{
    partial class RibbonViewModel
    {
        private bool _IsShowToolbar;


        public DelegateCommand<object> HideToolbarCommand { get; private set; }


        private void InitToolbarViewData()
        {
            HideToolbarCommand = new DelegateCommand<object>(HideToolbarExcute);
            _IsShowToolbar = true;
        }

        private string ShortenPathname(string pathname)
        {
            string sTemp = pathname;
            string sReturn = pathname;

            //sReturn = sReturn.Substring(0, sReturn.LastIndexOf("\\"));

            int index = sReturn.LastIndexOf("\\");

            sReturn = sTemp.Substring(index + 1, sTemp.Length - index - 1);

            return sReturn;
        }

        public void HideToolbarExcute(object cmdParameter)
        {
            IsShowToolbar = (bool)cmdParameter;
        }

        public ObservableCollection<object> RecentFile
        {
            get
            {
                ObservableCollection<object> _recentFile = new ObservableCollection<object>();

                _recentFile.Clear();

                List<string> ListFile = _model.GetRecentFiles();

                foreach (string sPath in ListFile)
                {
                    RecentMenuItemData data = new RecentMenuItemData();
                    data.Path = sPath;
                    data.Name = ShortenPathname(sPath).Replace("_", "__");
                    data.iCommand = OpenRecentCommand;

                    _recentFile.Add(data);
                }

                if (ListFile.Count > 0)
                {
                    Separator se = new Separator();
                    se.Style = (Style)Application.Current.FindResource("MainSe");

                    _recentFile.Add(se);

                    RecentMenuItemData dataclean = new RecentMenuItemData();

                    dataclean.Name = GlobalData.FindResource("Menu_File_CleanRecent");
                    dataclean.iCommand = ClearRecentCommand;
                    _recentFile.Add(dataclean);
                }

                return _recentFile;
            }
        }


        /// <summary>
        /// When mouse over Grid and Guide menu,
        /// refresh checked state of grid and guide items 
        /// </summary>
        private void OpenGridGuideAction(object parameter)
        {
            FirePropertyChanged("IsShowGlobalGuide");
            FirePropertyChanged("IsShowGridCheck");
            FirePropertyChanged("IsSnapToGridCheck");
            FirePropertyChanged("IsShowPageGuide");
            FirePropertyChanged("IsSnapToGuide");
            FirePropertyChanged("IsLockGuides");
        }

        private void UpdateLanguageHandler(string str)
        {
            FirePropertyChanged("RecentFile");

            FirePropertyChanged("IncreaseTooltip");

            FirePropertyChanged("DecreaseTooltip");
        }


        public bool IsShowToolbar
        {
            get
            {
                return _IsShowToolbar;
            }

            set
            {
                if (_IsShowToolbar != value)
                {
                    _IsShowToolbar = value;
                    FirePropertyChanged("IsShowToolbar");
                    FirePropertyChanged("Visibility_Toolbar");
                }
            }
        }


        public bool IsShowGridCheck
        {
            get
            {
                return GlobalData.IsShowGrid;
            }
        }

        public bool IsSnapToGridCheck
        {
            get
            {
                return GlobalData.IsSnapToGrid;
            }
        }

        public bool IsShowGlobalGuide
        {
            get
            {
                return GlobalData.IsShowGlobalGuide;
            }
        }

        public bool IsShowPageGuide
        {
            get
            {
                return GlobalData.IsShowPageGuide;
            }
        }

        public bool IsSnapToGuide
        {
            get
            {
                return GlobalData.IsSnapToGuide;
            }
        }

        public bool IsLockGuides
        {
            get
            {
                return GlobalData.IsLockGuides;
            }
        }

        public bool IsPreviewDefault
        {
            get
            {
                return GlobalData.IsDefaultPreview;
            }
        }

        public bool IsPreviewCurPage
        {
            get
            {
                return !GlobalData.IsDefaultPreview;
            }
        }

    }
}
