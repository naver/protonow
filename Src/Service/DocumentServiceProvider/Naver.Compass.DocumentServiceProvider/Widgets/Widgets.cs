using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Widgets : XmlElementDictionary<Guid, Widget>, IWidgets
    {
        internal Widgets(Page parentPage)
            : base("Widgets")
        {
            // Widgets must always reside in a page
            Debug.Assert(parentPage != null);
            _parentPage = parentPage;
        }

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            XmlNodeList widgetList = element.ChildNodes;
            if (widgetList == null || widgetList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement widgetElement in widgetList)
            {
                try
                {
                    Widget widget = WidgetFactory.CreateWidget(_parentPage, widgetElement.Name) as Widget;
                    if (widget == null)
                    {
                        // Continue to load other widgets if something wrong happenned
                        continue;
                    }

                    widget.LoadDataFromXml(widgetElement);
                    Add(widget.Guid, widget);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public new IEnumerator<IWidget> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IWidget GetWidget(Guid widgetGuid)
        {
            return Get(widgetGuid);
        }

        public new IWidget this[Guid widgetGuid] 
        {
            get
            {
                return GetWidget(widgetGuid);
            }
        }

        private Page _parentPage;
    }
}
