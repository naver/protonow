using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class TreeNode : XmlElementObject, ITreeNode
    {
        public TreeNode(Document document)
            : this(document, null, TreeNodeType.None, 0)
        {
        }

        public TreeNode(Document document, string tagName)
            : this(document, tagName, null, TreeNodeType.None, 0)
        {
        }

        public TreeNode(Document document, TreeNode parentNode, TreeNodeType nodeType)
            : this(document, "Node", parentNode, nodeType, parentNode == null ? 0 : parentNode.ChildNodesCount)
        {
        }

        public TreeNode(Document document, TreeNode parentNode, TreeNodeType nodeType, int index)
            : this(document, "Node", parentNode, nodeType, index)
        {
        }

        public TreeNode(Document document, string tagName, TreeNode parentNode, TreeNodeType nodeType, int index)
            : base(tagName)
        {
            _document = document;
            _parentNode = parentNode;
            _nodeType = nodeType;
            if(_parentNode != null)
            {
                _parentNode.InsertChild(this, index);
            }
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            XmlElement typeElement = element["Type"];
            if(Enum.TryParse<TreeNodeType>(typeElement.InnerText, out _nodeType) == false)
            {
                throw new Exception("Invalid tree node type!");
            }

            XmlElement nameElement = element["Name"];
            Name = nameElement.InnerText;

            LoadBoolFromChildElementInnerText("IsExpanded", element, ref _isExpanded);

            XmlElement guidElement = element["Guid"];
            if (guidElement != null)
            {
                try
                {
                    Guid guid = new Guid(guidElement.InnerText);
                    if (_nodeType == TreeNodeType.Page)
                    {
                        _attachedObject = _document.Pages[guid];
                    }
                    else if (_nodeType == TreeNodeType.MasterPage)
                    {
                        _attachedObject = _document.MasterPages[guid];
                    }
                    else if (_nodeType == TreeNodeType.Folder)
                    {
                        _folderGuid = guid;
                    }
                }
                catch
                {
                }
            }

            XmlElement childNodesElement = element["ChildNotes"];
            if (childNodesElement != null)
            {
                XmlNodeList nodeList = childNodesElement.ChildNodes;
                if (nodeList != null || nodeList.Count > 0)
                {
                    foreach (XmlElement nodeElement in nodeList)
                    {
                        TreeNode node = new TreeNode(_document);
                        node.LoadDataFromXml(nodeElement);

                        if ((node.NodeType == TreeNodeType.Page || node.NodeType == TreeNodeType.MasterPage) 
                            && node.AttachedObject == null)
                        {
                            // If this is a page or master page, but we cannot find the page object,
                            continue;
                        }


                        InsertChild(node, ChildNodesCount);
                    }
                }
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement nodeElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(nodeElement);

            SaveStringToChildElement("Name", Name, xmlDoc, nodeElement);
            SaveStringToChildElement("Type", _nodeType.ToString(), xmlDoc, nodeElement);
            SaveStringToChildElement("IsExpanded", _isExpanded.ToString(), xmlDoc, nodeElement);
            if(Guid != Guid.Empty)
            {
                SaveStringToChildElement("Guid", Guid.ToString(), xmlDoc, nodeElement);
            }

            XmlElement childNodesElement = xmlDoc.CreateElement("ChildNotes");
            nodeElement.AppendChild(childNodesElement);

            foreach(TreeNode node in _childNodes)
            {
                node.SaveDataToXml(xmlDoc, childNodesElement);
            }
        }

        #endregion

        #region ITreeNode Methods

        public ITreeNode AddChild(TreeNodeType nodeType)
        {
            return new TreeNode(_document, this, nodeType);
        }

        public ITreeNode InsertChild(TreeNodeType nodeType, int index)
        {
            return new TreeNode(_document, this, nodeType, index);
        }

        public void InsertChild(ITreeNode node, int index)
        {
            node.ParentNode = this;
            _childNodes.Insert(index, node);
        }

        public ITreeNode InsertSiblingAfter(TreeNodeType nodeType)
        {
            if(_parentNode == null)
            {
                throw new Exception("Cannot insert sibling of root node!");
            }

            return _parentNode.InsertChild(nodeType, IndexInParent + 1);
        }

        public ITreeNode InsertSiblingBefore(TreeNodeType nodeType)
        {
            if (_parentNode == null)
            {
                throw new Exception("Cannot insert sibling of root node!");
            }

            return _parentNode.InsertChild(nodeType, IndexInParent);
        }

        public void RemoveChild(int index)
        {
            _childNodes.RemoveAt(index);
        }

        public void RemoveChild(ITreeNode node)
        {
            _childNodes.Remove(node);
        }

        public void RemoveAllChildren()
        {
            _childNodes.Clear();
        }

        public void RemoveMe()
        {
            if (_parentNode != null)
            {
                _parentNode.RemoveChild(this);
            }
        }

        public void Move(ITreeNode newParentNode, int index)
        {
            if(_parentNode == null)
            {
                throw new Exception("Can not move the root node!");
            }

            if (newParentNode == null)
            {
                throw new Exception("Can not move node to the root node!");
            }

            int indexInOldParent = IndexInParent;
            TreeNode oldParent = _parentNode;
            oldParent.RemoveChild(this);
            
            _parentNode = newParentNode as TreeNode;
            _parentNode.InsertChild(this, index);
        }

        public int IndexOf(ITreeNode child)
        {
            return _childNodes.IndexOf(child);
        }

        public string Name 
        {
            get
            {
                if (_nodeType == TreeNodeType.Folder)
                {
                    return _folderName;
                }
                else if (_attachedObject != null && (_nodeType == TreeNodeType.Page || _nodeType == TreeNodeType.MasterPage))
                {
                    return _attachedObject.Name;
                }

                return String.Empty;
            }

            set
            {
                if (_nodeType == TreeNodeType.Folder)
                {
                    _folderName = value;
                }
                else if (_attachedObject != null && (_nodeType == TreeNodeType.Page || _nodeType == TreeNodeType.MasterPage))
                {
                    _attachedObject.Name = value;
                }
            }
        }

        public Guid Guid 
        {
            get
            {
                if (_nodeType == TreeNodeType.Folder)
                {
                    return _folderGuid;
                }
                else if (_attachedObject != null && (_nodeType == TreeNodeType.Page || _nodeType == TreeNodeType.MasterPage))
                {
                    return _attachedObject.Guid;
                }

                return Guid.Empty;
            }
        }

        public TreeNodeType NodeType
        {
            get
            {
                return _nodeType;
            }
        }

        public IDocumentPage AttachedObject
        {
            get
            {
                return _attachedObject;
            }
            set
            {
                _attachedObject = value;
            }
        }

        public ITreeNode ParentNode
        {
            get
            {
                return _parentNode;
            }
            set
            {
                _parentNode = value as TreeNode;
            }
        }

        public List<ITreeNode> ChildNodes
        {
            get
            {
                return _childNodes.ToList();
            }
        }

        public bool IsRootNode
        {
            get
            {
                return _parentNode == null;
            }
        }

        public int ChildNodesCount 
        {
            get
            {
                return _childNodes.Count;
            }
        }

        public int IndexInParent
        {
            get
            {
                if (_parentNode != null)
                {
                    return _parentNode.IndexOf(this);
                }
                return -1;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
            }
        }

        #endregion

        private List<ITreeNode> _childNodes = new List<ITreeNode>();
        private TreeNode _parentNode;
        private TreeNodeType _nodeType;
        private IDocumentPage _attachedObject;
        private string _folderName;
        private Guid _folderGuid = Guid.NewGuid();
        private Document _document;
        private bool _isExpanded; 
    }
}
