using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class LayoutSetting : XmlElementObject, ILayoutSetting
    {
        internal LayoutSetting(Document document)
            : base("LayoutSetting")
        {
            _document = document;
            _pageTree = new TreeNode(document, "PageTree");
            _masterPageTree = new TreeNode(document, "MasterPageTree");
        }
        
        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            XmlElement pageTreeElement = element["PageTree"];
            if (pageTreeElement != null)
            {
                _pageTree.LoadDataFromXml(pageTreeElement);
            }

            XmlElement masterPageTreeElement = element["MasterPageTree"];
            if (masterPageTreeElement != null)
            {
                _masterPageTree.LoadDataFromXml(masterPageTreeElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {

            XmlElement layoutElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(layoutElement);

            _pageTree.SaveDataToXml(xmlDoc, layoutElement);
            _masterPageTree.SaveDataToXml(xmlDoc, layoutElement);
        }

        #endregion

        public ITreeNode PageTree
        {
            get { return _pageTree; }
        }

        public ITreeNode MasterPageTree
        {
            get { return _masterPageTree; }
        }

        internal void Clear()
        {
            _pageTree.RemoveAllChildren();
        }

        private Document _document;
        private TreeNode _pageTree;
        private TreeNode _masterPageTree;
    }
}
