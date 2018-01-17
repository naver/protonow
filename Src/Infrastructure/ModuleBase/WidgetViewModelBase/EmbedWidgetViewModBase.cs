using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.InfoStructure
{
    public class EmbedWidgetViewModBase : WidgetViewModBase
    {
        public EmbedWidgetViewModBase()
        {
        }

        virtual public Guid EmbedePagetGUID { get; set; }
    }
}
