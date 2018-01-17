using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Forms;
using System.IO;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Windows;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using System.Diagnostics;

namespace Naver.Compass.Module
{
    public class GenerateHTMLViewModel : ViewModelBase
    {
        #region Constructor
        public GenerateHTMLViewModel()
        {
            this.BrowseCommand = new DelegateCommand<object>(BrowseExecute);
            this.UseDefaultCommand = new DelegateCommand<object>(UseDefaultExecute);
            this.GenerateCommand = new DelegateCommand<object>(GenerateExecute);
            this.CloseCommand = new DelegateCommand<object>(CloseExecute);

            defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Compass\Untitled";

            if (String.IsNullOrEmpty(OutPutFolder))
            {
                //set default path
                GeneratePath = defaultPath;
                OutPutFolder = defaultPath;
                CreateDefaultFolder();
            }
            else
            {
                GeneratePath = OutPutFolder;
            }
        }
        #endregion

        #region Command and property
        public DelegateCommand<object> BrowseCommand { get; private set; }
        public DelegateCommand<object> UseDefaultCommand { get; private set; }
        public DelegateCommand<object> GenerateCommand { get; private set; }
        public DelegateCommand<object> CloseCommand { get; private set; }

        private void BrowseExecute(object cmdParameter)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = GlobalData.FindResource("Toolbar_HtmlDlg_SlectPath");
            dlg.SelectedPath = OutPutFolder;
            dlg.ShowNewFolderButton = true;
            DialogResult ret = dlg.ShowDialog();
            if(ret == DialogResult.OK)
            {
                GeneratePath = dlg.SelectedPath;
                OutPutFolder = GeneratePath;
            }

        }
        private void UseDefaultExecute(object cmdParameter)
        {
            GeneratePath = defaultPath;
            OutPutFolder = defaultPath;
            CreateDefaultFolder();
        }
        private void GenerateExecute(object cmdParameter)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document.DocumentSettings.LayoutSetting.PageTree.ChildNodesCount <= 0)
            {
                System.Windows.MessageBox.Show(GlobalData.FindResource("Toolbar_Generate_AddPageAlert"));
                return;
            }

            if (false == CheckDestinationPath())
                return;

            //send message to generate html
            PreviewParameter para = new PreviewParameter();
            para.SavePath = GeneratePath;
            para.IsBrowerOpen = true;

            doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.GenerateMobileFiles = true;

            _ListEventAggregator.GetEvent<GenerateHTMLEvent>().Publish(para);

            CloseWindow(cmdParameter);
        }
        private void CloseExecute(object cmdParameter)
        {
            if (Directory.Exists(GeneratePath) == false)
            {
               OutPutFolder = defaultPath;
            }

            CloseWindow(cmdParameter);
        }

        private string generatePath;
        public string GeneratePath
        {
            get { return generatePath; }
            set
            {
                if (generatePath != value)
                {
                    generatePath = value;
                    FirePropertyChanged("GeneratePath");
                }
            }
        }
        public string OutPutFolder
        {
            get
            {
               if(HtmlGenerator!=null)
               {
                   return HtmlGenerator.OutputFolder;
               }
                else
               {
                   return defaultPath;
               }
            }

            set
            {
                if(HtmlGenerator.OutputFolder !=value)
                {
                    HtmlGenerator.OutputFolder = value;
                    ServiceLocator.Current.GetInstance<IDocumentService>().Document.IsDirty = true;
                }
            }
        }

        public IHtmlGeneratorConfiguration HtmlGenerator
        {
            get
            {
                try
                {
                    IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                    return doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration;
                }
                catch(Exception e)
                {
                    System.Windows.MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }
        #endregion

        #region private member

        private string defaultPath;
        private void CreateDefaultFolder()
        {
            if (Directory.Exists(defaultPath) == false)
            {
                Directory.CreateDirectory(defaultPath);
            }
        }

        /// <summary>
        /// check if the path is valid
        /// </summary>
        /// <returns>true if valid, else false </returns>
        private bool CheckDestinationPath()
        {
            if (Directory.Exists(GeneratePath) == false)
            {
                MessageBoxResult ret = System.Windows.MessageBox.Show(GlobalData.FindResource("Toolbar_HtmlDlg_CreatePathAlert"), GlobalData.FindResource("Common_Warning"), MessageBoxButton.YesNo);

                if (ret == System.Windows.MessageBoxResult.No)
                    return false;
                try
                {
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(GeneratePath);

                    GeneratePath = directoryInfo.FullName;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("The process failed: {0}", e.ToString());
                    System.Windows.MessageBox.Show(GlobalData.FindResource("Toolbar_HtmlDlg_PathInvalidAlert"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            OutPutFolder = GeneratePath;
            return true;
        }

        private void CloseWindow(object Parameter)
        {
            Window win = Parameter as Window;
            win.Close();    
        }

        #endregion
    }
}
