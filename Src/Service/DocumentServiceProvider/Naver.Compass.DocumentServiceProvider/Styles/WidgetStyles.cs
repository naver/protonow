using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class WidgetStyles : XmlElementDictionary<Guid, WidgetStyle>, IWidgetStyles
    {
        internal WidgetStyles(Widget ownerWidget)
            : base("WidgetStyles")
        {
            _ownerWidget = ownerWidget;
        }

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlNodeList widgetStyleList = element.ChildNodes;
            if (widgetStyleList == null || widgetStyleList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement widgetStyleElement in widgetStyleList)
            {
                WidgetStyle widgetStyle = new WidgetStyle(_ownerWidget, Guid.Empty);
                widgetStyle.LoadDataFromXml(widgetStyleElement);
                if (widgetStyle.ViewGuid != Guid.Empty)
                {
                    Add(widgetStyle.ViewGuid, widgetStyle);
                }
            }
        }

        public new IEnumerator<IWidgetStyle> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IWidgetStyle GetWidgetStyle(Guid viewGuid)
        {
            return Get(viewGuid);
        }

        public new IWidgetStyle this[Guid viewGuid]
        {
            get { return GetWidgetStyle(viewGuid); }
        }

        private Widget _ownerWidget;
    }
}
