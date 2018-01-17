using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public interface ILayoutSetting
    {
        ITreeNode PageTree { get; }

        ITreeNode MasterPageTree { get; }
    }
}
