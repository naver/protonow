using MainToolBar.Common;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Module.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Naver.Compass.Module
{
    public class DocumentRecoveryViewModel : ViewModelBase
    {
        private DocumentRecoveryWindow documentRecoveryWindow;
        private readonly string RecoveryFileXmlPath;
        private List<RecoveryFile> _recoveryFiles;
        public List<RecoveryFile> RecoveryFiles
        {
            get { return this._recoveryFiles; }
            set
            {
                if (value != _recoveryFiles)
                {
                    _recoveryFiles = value;
                    FirePropertyChanged("RecoveryFiles");
                }
            }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                    FirePropertyChanged("SelectedIndex");
                }
            }
        }

        public DelegateCommand<object> RecoveryCommand { get; private set; }
        public DocumentRecoveryViewModel(DocumentRecoveryWindow documentRecoveryWindow)
        {
            this.documentRecoveryWindow = documentRecoveryWindow;
            //var programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create);
            //var autoSaveSetting = Path.Combine(programdata, @"Design Studio\AutoSaveSetting");
            //RecoveryFileXmlPath = Path.Combine(autoSaveSetting, "RecoveryFilesInfo.xml");
            //if (File.Exists(RecoveryFileXmlPath))
            //{
            //    try
            //    {
            //        var recoveryinfo = default(RecoveryInfo);
            //        using (var rdr = new StreamReader(RecoveryFileXmlPath))
            //        {
            //            var serializer = new XmlSerializer(typeof(RecoveryInfo));
            //            recoveryinfo = (RecoveryInfo)serializer.Deserialize(rdr);
            //            var recoveryfiles = recoveryinfo.RecoveryFiles.Where(x => File.Exists(x.GetFullPath())).ToList();
            //            recoveryfiles.Reverse();
            //            RecoveryFiles = recoveryfiles;
            //        }

            //        if (RecoveryFiles.Count != recoveryinfo.RecoveryFiles.Count)
            //        {
            //            recoveryinfo.RecoveryFiles = RecoveryFiles;
            //            using (var rdw = new StreamWriter(RecoveryFileXmlPath))
            //            {
            //                var serializer = new XmlSerializer(typeof(RecoveryInfo));
            //                serializer.Serialize(rdw, recoveryinfo);
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        RecoveryFiles = new List<RecoveryFile>();
            //    }
            //}
            //else
            //{
            //    RecoveryFiles = new List<RecoveryFile>();
            //}

            AutoSaveService.Instance.CheckRecoveryFileExist();
            if (AutoSaveService.Instance.RecoveryInfo != null && AutoSaveService.Instance.RecoveryInfo.RecoveryFiles != null)
            {
                var recoveryFiles = AutoSaveService.Instance.RecoveryInfo.RecoveryFiles.ToList();
                recoveryFiles.Reverse();
                RecoveryFiles = recoveryFiles;
            }

            this.RecoveryCommand = new DelegateCommand<object>(RecoveryCommandExecute);
        }

        private void RecoveryCommandExecute(object parameter)
        {
            var strParameter = parameter.ToString();

            //Recovery file
            if (strParameter == "1")
            {
                if (RecoveryFiles != null && RecoveryFiles.Count > 0 && SelectedIndex >= 0 && SelectedIndex <= RecoveryFiles.Count - 1)
                {
                    var torecoveryfile = RecoveryFiles[SelectedIndex];
                    if (torecoveryfile != null && !string.IsNullOrEmpty(torecoveryfile.GetFullPath()))
                    {
                        var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                        _ListEventAggregator.GetEvent<RecoveryFileEvent>().Publish(torecoveryfile);
                        this.documentRecoveryWindow.DialogResult = true;
                    }
                }
            }
            else
            {//Show Welcome screen window
                //_ListEventAggregator.GetEvent<OpenDialogEvent>().Publish(DialogType.WelcomeScreen);
                this.documentRecoveryWindow.DialogResult = false;
            }

            documentRecoveryWindow.Close();
        }
    }
}
