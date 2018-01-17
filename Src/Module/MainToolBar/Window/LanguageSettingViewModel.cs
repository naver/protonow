using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls;
using System.Windows;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service;
using Naver.Compass.Common.Win32;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.Module
{
    public class LanguageSettingViewModel : ViewModelBase
    {
        LanguageSettingWindow lanView;
        public LanguageSettingViewModel(LanguageSettingWindow view)
        {
            lanView = view;
            this.ChangeLanCommand = new DelegateCommand<object>(ChangeLanExecute);
            SetLanguage();
        }


        public DelegateCommand<object> ChangeLanCommand { get; private set; }
        public void SetLanguage()
        {
            string language = GlobalData.Culture;
            if (language == "en-US")
                languageIndex = 1;
            else if (language == "zh-CN")
                languageIndex = 2;
            else if (language == "ja-JP")
                languageIndex = 3;
            else
                languageIndex = 0;
        }
        private void ChangeLanExecute(object obj)
        {
            int index = (int)obj;
            if (index == -1)
                return;
            string culture;
            if (index == 1)
                culture = "en-US";
            else if (index == 2)
                culture = "zh-CN";
            else if (index == 3)
                culture = "ja-JP";
            else
                culture = "ko-KR";

            if (culture != GlobalData.Culture)
            {
                GlobalData.Culture = culture;
                _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Publish(string.Empty);

                #region send message to other process
                Int32 m_Message = Win32MsgHelper.RegisterWindowMessage("CHANGE_CURRENT_LANGUAGE");
                try
                {
                    IShareMemoryService ShareMemSrv = ServiceLocator.Current.GetInstance<ShareMemorServiceProvider>();
                    ShareMemSrv.SetShareDate(culture);
                }
                catch
                {
                }
                finally
                {
                    Win32MsgHelper.PostMessage((IntPtr)Win32MsgHelper.HWND_BROADCAST, m_Message, IntPtr.Zero, IntPtr.Zero);
                }
                #endregion
            }

            lanView.Close();
        }

        private int languageIndex;
        public int LanguageIndex
        {
            get
            {
                return languageIndex;
            }
            set
            {
                if (value != languageIndex)
                {
                    languageIndex = value;
                    FirePropertyChanged("LanguageIndex");
                }
            }
        }
    }
}
