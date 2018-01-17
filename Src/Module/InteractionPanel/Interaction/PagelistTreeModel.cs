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
    class PagelistTreeModel
    {
        private static PagelistTreeModel _instance;
        public PageNode RootNode { get; private set; }

        PagelistTreeModel()
        {
            RootNode = new PageNode();
        }

        public static PagelistTreeModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PagelistTreeModel();
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

        public void SelectPage(Guid guid)
        {
            TraveralTree(RootNode, guid, true);
        }

        public void DeselectPage(Guid guid)
        {
            TraveralTree(RootNode, guid, false);
        }
        public void TraveralTree(PageNode rootNode,Guid guid,bool selected)
        {
            PageNode node = rootNode.Children.FirstOrDefault(x => x.Guid == guid);
            if (node != null)
            {
                node.IsSelected = selected;
                return;
            }
            foreach(var item in rootNode.Children)
            {
                TraveralTree(item, guid, selected);
            }
        }

        //Mark whether the page-list tree is opend in PageListPopup or in CaseEditor View.
        public bool IsInPopup { get; set; }
    }
}
