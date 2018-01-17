using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows.Data;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.InfoStructure;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module
{
    public class AdaptivVieweNode : ViewModelBase
    {
        #region constructor
        public AdaptivVieweNode():this(null)
        {

        }
        public AdaptivVieweNode(IAdaptiveView view)
        {
            _condition = AdaptiveViewCondition.LessOrEqual;
            Children = new ObservableCollection<AdaptivVieweNode>();

            if(view!=null)
            {
                AdaptiveView = view;
                _name = view.Name;
                _condition = view.Condition;
                _description = view.Description;
                _width = view.Width;
                _height = view.Height;
                _isChecked = false;
            }
        }
        #endregion

        #region property and binding
        public IAdaptiveView AdaptiveView { get; set; }
        public Guid Guid
        {
            get { return AdaptiveView.Guid; }
        }
        public string Description
        {
            get
            {
                if (_name == @"Base")
                    return _name;

                string w = (_width == 0) ? "any" : _width.ToString();
                return string.Format("{0}({1})", _name, w);
            }
        }

        //Displayed in SetAdaptiveView
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    _isEdited = true;
                    FirePropertyChanged("Name");
                    FirePropertyChanged("Description");
                }
            }
        }
        //Displayed in editor view
        public string ButtonName
        {
            get
            {
                if (_name == @"Base")
                    return _name;

                string w = (_width == 0) ? "any" : _width.ToString();

                return string.Format("{0} - {1}", _name, w);
            }
        }
        public AdaptiveViewCondition Condition
        {
            get { return _condition; }
            set
            {
                if (_condition != value)
                {
                    _condition = value;
                    _isEdited = true;
                    FirePropertyChanged("Condition");
                }
            }
        }
        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    _isEdited = true;
                    FirePropertyChanged("Width");
                    FirePropertyChanged("Description");
                    _parentNode.SortChildren();
                }
            }
        }
        public int Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    _isEdited = true;
                    FirePropertyChanged("Height");
                    FirePropertyChanged("Description");
                }
            }
        }
        public double LeftSpacing
        {
            get { return _leftSpacing; }
            private set
            {
                if (_leftSpacing != value)
                {
                    _leftSpacing = value;
                    FirePropertyChanged("LeftSpacing");
                }
            }
        }
        public AdaptivVieweNode ParentNode
        {
            get { return _parentNode; }
            set
            {
                if (_parentNode != value)
                {
                    _parentNode = value;
                    //_isEdited = true;

                    //if (parentNode.IsRoot == true)
                    //    this.leftSpacing = parentNode.leftSpacing;
                    //else
                    //    this.leftSpacing = parentNode.leftSpacing +5;
                }
            }
        }
        public bool IsChecked
        {
            get 
            { 
                return _isChecked; 
            }
            set 
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    FirePropertyChanged("IsChecked");
                }
            }
            //set
            //{
            //    if (AdaptiveView.IsChecked != value)
            //    {
            //        if current node is already checked, click it again, don't uncheck it.
            //        if (value == false)
            //        {
            //            AdaptivVieweNode selNode;

            //            selNode = AdaptiveModel.GetInstance().AdaptiveViewsList.FirstOrDefault(a => a.IsChecked == true && a != this);

            //            if ( selNode == null)
            //                return;
            //        }
            //        AdaptiveView.IsChecked = value;
            //        _document.IsDirty = true;
            //        FirePropertyChanged("IsChecked");
            //    }
            //}
        }
        public int IndexInParent
        {
            get { return ParentNode.Children.IndexOf(this); }
        }
        public bool IsEdited
        {
            get
            {
                return (AdaptiveView != null) && _isEdited;
            }
            set
            {
                if (_isEdited != value)
                {
                    _isEdited = value;
                }
            }
        }
        public ObservableCollection<AdaptivVieweNode> Children { get; set; }
        #endregion

        #region function

        public void Fire()
        {
            FirePropertyChanged("Children");
        }

        public void Add(AdaptivVieweNode node)
        {
            Children.Add(node);
        }
        public AdaptivVieweNode Clone()
        {
            AdaptivVieweNode node = new AdaptivVieweNode();
            node.Name = this._name;
            node.Width = this._width;
            node.Height = this._height;
            node.LeftSpacing = this._leftSpacing;
            node.ParentNode = this._parentNode;
            ParentNode.Children.Add(node);
            return node;
        }

        public void Delete()
        {
            foreach (var item in Children)
            {
                ParentNode.Add(item);
                item.ParentNode = _parentNode;
            }
            ParentNode.Children.Remove(this);
        }

        public void SortChildren()
        {
            Children = new ObservableCollection<AdaptivVieweNode>(Children.OrderByDescending(a => a.Width));
            FirePropertyChanged("Children");  
        }

        #endregion

        #region private fields
        public bool IsRoot { get; set; }
        private string _description;
        private string _name;
        private AdaptiveViewCondition _condition;
        private int _width;
        private int _height;
        private AdaptivVieweNode _parentNode;
        private bool _isEdited;
        private bool _isChecked;
        private double _leftSpacing;
        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
        #endregion

    }
       

}

