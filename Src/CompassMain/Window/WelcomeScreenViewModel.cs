using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Main
{
    public class WelcomeScreenViewModel : ViewModelBase
    {
        public WelcomeScreenViewModel(WelcomeScreen win)
        {
            _welcomeScreen = win;
            RecFileslist = new ObservableCollection<RecentFile>();
            InitRecentList();
            _ListEventAggregator.GetEvent<FlashRecentList>().Subscribe(FlashRecentFile);
            this.OpenRecentCommand = new DelegateCommand<string>(OpenFileHandler);
            this.NewCommand = new DelegateCommand<object>(NewExecute);
            this.OpenCommand = new DelegateCommand<object>(OpenExecute);
            this.CloseCommand = new DelegateCommand<object>(CloseExecute);
            this.OpenLinkCommand = new DelegateCommand<object>(OpenLinkExecute);
        }
        public ObservableCollection<RecentFile> RecFileslist { get; set; }
        public string CurrentVersion
        {
            get
            {
                // Display version format is x.x.x
                string productVersion = ConfigFileManager.CurrentVersion;
                if (String.IsNullOrEmpty(productVersion))
                {
                    return String.Empty;
                }
                else
                {
                    productVersion = productVersion.Substring(0, productVersion.LastIndexOf("."));
                }

                return productVersion;
            }
        }
        public DelegateCommand<object> OpenCommand { get; set; }
        public DelegateCommand<object> NewCommand { get; set; }
        public DelegateCommand<object> CloseCommand { get; set; }
        public DelegateCommand<object> OpenLinkCommand { get; set; }
        public DelegateCommand<string> OpenRecentCommand { get; private set; }
        public DelegateCommand<object> OpenSampleCommand { get; private set; }

        private WelcomeScreen _welcomeScreen;

        public void NewExecute(object cmdParameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Create);
        }
        public void OpenExecute(object cmdParameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Open);
        }
        private void CloseExecute(object cmdParameter)
        {
            _welcomeScreen.Close();
        }

        private void OpenLinkExecute(object cmdParameter)
        {
            switch(cmdParameter.ToString())
            {
                case "0":
                    System.Diagnostics.Process.Start(CommonDefine.UrlTableUI);
                    break;
                case "1":
                    System.Diagnostics.Process.Start(CommonDefine.UrlTemplateUI);
                    break;
                case "2":
                    System.Diagnostics.Process.Start(CommonDefine.UrlUIGuide);
                    break;
            }
        }
        private void InitRecentList()
        {
            RecFileslist.Clear();
            foreach (var item in ConfigFileManager.RecentFiles())
            {
                string name;
                string path = item;
                int index = item.LastIndexOf("\\");

                if (index > 0)
                    name = item.Substring(index + 1, item.Length - index - 1);
                else
                    name = item;
                RecFileslist.Add(new RecentFile() { FileName = name, FilePath = path });
            }
        }

        private void OpenFileHandler(string parameter)
        {
            if (!String.IsNullOrEmpty(parameter))
            {
                _ListEventAggregator.GetEvent<OpenFileEvent>().Publish(parameter);
                _welcomeScreen.Close();
            }
        }
        private void FlashRecentFile(object parameter)
        {
            InitRecentList();
        }

    }
}
