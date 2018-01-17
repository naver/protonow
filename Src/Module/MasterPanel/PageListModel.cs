using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Load page list
    /// </summary>
    class PageListModel
    {
        private static PageListModel _instance;
        public PageNode RootNode { get; private set; }

        PageListModel()
        {
            RootNode = new PageNode();
        }

        public static PageListModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PageListModel();
            }
            return _instance;
        }

        public void LoadPageTree()
        {
            try
            {
                RootNode.Children.Clear();
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc.Document == null)
                    return;
                RootNode.TreeNodeObject = doc.Document.DocumentSettings.LayoutSetting.PageTree;
                LoadNodeViewModelFromTreeNodeObject(RootNode, RootNode.TreeNodeObject);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Get pagetree-node failed: {0}", e.ToString());
            }
           
        }
        void LoadNodeViewModelFromTreeNodeObject(PageNode nodeVM, ITreeNode treeNodeObject)
        {
            if (nodeVM == null || treeNodeObject == null)
            {
                return;
            }
            foreach (ITreeNode treeNode in treeNodeObject.ChildNodes)
            {
                PageNode node = new PageNode(treeNode);
                nodeVM.Children.Add(node);

                LoadNodeViewModelFromTreeNodeObject(node, treeNode);
            }
        }
    }
}
