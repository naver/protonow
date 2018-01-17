using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Service
{
    public interface IShareMemoryService
    {
        void Initialize();
        string GettShareDatePath();
        void SetShareDate(string path);

    }
}
