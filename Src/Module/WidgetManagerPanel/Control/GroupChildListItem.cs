using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.ComponentModel;

namespace Naver.Compass.Module
{
    class GroupChildListItem : GroupListItem
    {
        public GroupChildListItem(IGroup data)
            : base(data)
        {
        }

        override public bool DisplayHeadIconFlag
        {
            get
            {
                return false;
            }
        }
    }
}
