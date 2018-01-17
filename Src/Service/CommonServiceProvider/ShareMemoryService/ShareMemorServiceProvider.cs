using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System.Windows.Media;
using System.Windows.Markup;
using Naver.Compass.Common.CommonBase;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Events;

namespace Naver.Compass.Service
{
    public class ShareMemorServiceProvider : IShareMemoryService
    {
        private ProcessSharedMemoryObj _sm=null;
        private bool _bIsInitialized = false;

        public void Initialize()
        {
            _sm = new ProcessSharedMemoryObj("ProtoNowLibraryFilePath@", 1024);
            if (true==_sm.Open())
            {
                _bIsInitialized = true;
            }

        }
        public string GettShareDatePath()
        {
            if (_bIsInitialized==false)
            {
                return string.Empty;
            }
            if (_sm == null)
            {
                return string.Empty;
            }

            return _sm.Data.ToString();
                
        }
        public void SetShareDate(string path)
        {
            if (_bIsInitialized == false)
            {
                return ;
            }
            if (_sm == null)
            {
                return;
            }
            _sm.Data = path;
        }

    }


}
