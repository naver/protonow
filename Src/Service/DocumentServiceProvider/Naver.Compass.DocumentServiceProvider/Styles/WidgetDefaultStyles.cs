using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class WidgetDefaultStyles : XmlElementDictionary<string, WidgetDefaultStyle>, IWidgetDefaultStyles
    {
        internal WidgetDefaultStyles(WidgetDefaultStyleSet set)
            : base("WidgetDefaultStyles") 
        {
            // WidgetDefaultStyles must exist with the WidgetDefaultStyleSet.
            Debug.Assert(set != null);
            _set = set;
        }

        #region XmlElementObject

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
                WidgetDefaultStyle widgetStyle = new WidgetDefaultStyle(_set, string.Empty);
                widgetStyle.LoadDataFromXml(widgetStyleElement);
                if (!String.IsNullOrEmpty(widgetStyle.Name))
                {
                    Add(widgetStyle.Name, widgetStyle);
                }
            }
        }

        #endregion

        public new IEnumerator<IWidgetDefaultStyle> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IWidgetDefaultStyle GetWidgetDefaultStyle(string name)
        {
            return Get(name);
        }

        public new IWidgetDefaultStyle this[string name]
        {
            get { return Get(name); }
        }

        private WidgetDefaultStyleSet _set;
    }
}
