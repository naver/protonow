using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    class SetAdaptiveViewModel : ViewModelBase
    {
        #region Constructor
        public SetAdaptiveViewModel()
        {
            deleteList = new List<IAdaptiveView>();
            InheritFromCollection = new ObservableCollection<AdaptivVieweNode>();

            InitPresets();

            this.AddCommand = new DelegateCommand<object>(AddViewExecute);
            this.CloneCommand = new DelegateCommand<object>(CloneViewExecute, CanExecute);
            this.MoveUpCommand = new DelegateCommand<object>(MoveUpExecute, CanMoveUpExecute);
            this.MoveDownCommand = new DelegateCommand<object>(MoveDownExecute, CanMoveDownExecute);
            this.DeleteCommand = new DelegateCommand<object>(DeleteViewExecute, CanExecute);
            this.PresetChangedCommand = new DelegateCommand<object>(PresetChangedExecute);
            this.OKCommand = new DelegateCommand<object>(OKExecute);
            this.CancelCommand = new DelegateCommand<object>(CancelExecute);
            
            LoadAdaptiveView();
        }
        #endregion

        #region Command and Function
        public DelegateCommand<object> AddCommand { get; private set; }
        public DelegateCommand<object> CloneCommand { get; private set; }
        public DelegateCommand<object> MoveUpCommand { get; private set; }
        public DelegateCommand<object> MoveDownCommand { get; private set; }
        public DelegateCommand<object> DeleteCommand { get; private set; }
        public DelegateCommand<object> PresetChangedCommand { get; private set; }
        public DelegateCommand<object> OKCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }

        private void AddViewExecute(object obj)
        {

            AdaptivVieweNode node = new AdaptivVieweNode();
            node.Name = GetAdaptiveViewName();

            //all parent node is base now.
            node.ParentNode = RootNode;
            RootNode.Add(node);

            /*
            if (null == SelectValue)
            {
                node.ParentNode = RootNode;
                RootNode.Add(node);
            }
            else
            {
                node.ParentNode = SelectValue;
                SelectValue.Add(node);
            }
             * */
            SelectValue = node;
            LoadInherit();

            RootNode.Fire();
            UpdateUI();
        }

        private void CloneViewExecute(object obj)
        {
            AdaptivVieweNode node = selectValue.Clone();
            SelectValue = node;

            RootNode.Fire();
        }

        private void MoveUpExecute(object obj)
        {
             var idx = selectValue.IndexInParent;
             if (idx > 0)
             {
                 AdaptivVieweNode parent = selectValue.ParentNode;
                 parent.Children.Remove(selectValue);
                 parent.Children.Insert(--idx, selectValue);
                 selectValue.IsEdited = true;
             }
             RefreshCommands();
             RootNode.Fire();
        }
        private bool CanMoveUpExecute(object obj)
        {
            return (selectValue!=null) && (!selectValue.IsRoot) && (selectValue.IndexInParent != 0);
        }
        private void MoveDownExecute(object obj)
        {
            var idx = selectValue.IndexInParent;
            AdaptivVieweNode parent = selectValue.ParentNode;
            if (idx < parent.Children.Count - 1)
            {
                parent.Children.Remove(selectValue);
                parent.Children.Insert(++idx, selectValue);
                selectValue.IsEdited = true;
            }
            RefreshCommands();
            RootNode.Fire();
        }
        private bool CanMoveDownExecute(object obj)
        {
            if ((selectValue == null) || selectValue.IsRoot)
                return false;
            int index = selectValue.IndexInParent;
            int count = selectValue.ParentNode.Children.Count();
            return (index < count - 1);
        }

        private void DeleteViewExecute(object obj)
        {
            //add adaptive view to deletelist
            if(selectValue.AdaptiveView!=null)
            {
                deleteList.Add(selectValue.AdaptiveView);
            }
            selectValue.Delete();
            if (RootNode.Children.Count <= 0)
                SelectValue = null;
            else
                SelectValue = GetLastNode(RootNode.Children);
            RootNode.Fire();
            UpdateUI();
        }
        private bool CanExecute(object obj)
        {
            if (selectValue == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// load nodes that current selected node can inherit form
        /// </summary>
        private void LoadInherit()
        {
            InheritFromCollection.Clear();
            LoadInheritNodes(selectValue, RootNode);
        }
        private void LoadInheritNodes(AdaptivVieweNode beLoad, AdaptivVieweNode node)
        {
            if (beLoad != node)
            {
                InheritFromCollection.Add(node);
                foreach (var item in node.Children)
                {
                    LoadInheritNodes(beLoad, item);
                }
            }
        }
        private AdaptivVieweNode GetLastNode(ObservableCollection<AdaptivVieweNode> children)
        {
            AdaptivVieweNode lastnode = children.ElementAt(children.Count - 1);
            if (lastnode.Children.Count <= 0)
                return lastnode;
            else 
                return GetLastNode(lastnode.Children);
        }
        private void PresetChangedExecute(object obj)
        {
            if (obj == null || selectValue == null)
                return;
            PresetNode node = (PresetNode)obj;
            if (node != null)
            {
                SelectValue.Name = node.Name;
                SelectValue.Width = node.Width;
                SelectValue.Height = node.Height;
                SelectValue.Condition = AdaptiveViewCondition.LessOrEqual;

                FirePropertyChanged("PresetSelValue");
            }
           
        }

        private string NameWidthoutCompany(string name)
        {
            int index = name.IndexOf(" ");

            return name.Substring(index + 1, name.Length - (index + 1));
        }

        /// <summary>
        /// change selected parent
        /// </summary>
        /// <param name="parent">seleted parent</param>
        private void ChangeInherit(AdaptivVieweNode parent)
        {
            if (null == parent || parent == selectValue.ParentNode)
                return;
            
            SelectValue.ParentNode.Children.Remove(SelectValue);
            parent.Children.Add(SelectValue);
            SelectValue.ParentNode = parent;
            RootNode.Fire();
        }
        private void OKExecute(object obj)
        {
            Save();
            Window win = obj as Window;
            win.DialogResult = true;
            win.Close();
        }
        private void CancelExecute(object obj)
        {
            Window win = obj as Window;
            win.DialogResult = false;
            win.Close();
        }
        #endregion

        #region Property

        public AdaptivVieweNode RootNode { get; set; }

        private ObservableCollection<PresetNode> presets;
        public ObservableCollection<PresetNode> Presets
        {
            get
            {
                return presets;
            }
        }

        private AdaptivVieweNode selectValue;
        public AdaptivVieweNode SelectValue
        {
            get
            {
                return selectValue;
            }
            set
             {
                if(selectValue!=value)
                {
                    selectValue = value;
                    if(selectValue!=null)
                        ParentSelected = selectValue.ParentNode;
                    FirePropertyChanged("SelectValue");

                    LoadInherit();
                    UpdateUI();
                }
                RefreshCommands();
            }
        }

        private AdaptivVieweNode parentSelected;
        public AdaptivVieweNode ParentSelected
        {
            get { return parentSelected; }
            set
            {
                if (parentSelected != value)
                {
                    parentSelected = value;
                    ChangeInherit(value);
                    FirePropertyChanged("ParentSelected");
                }
            }
        }

        private bool isSettingEnabled;
        public bool IsSettingEnabled
        {
            get { return isSettingEnabled; }
            set
            {
                if(isSettingEnabled!=value)
                {
                    isSettingEnabled = value;
                    FirePropertyChanged("IsSettingEnabled");
                }
            }
        }

        ///adaptive views that current view can inherit from
        public ObservableCollection<AdaptivVieweNode> InheritFromCollection { get; set; }

        /// <summary>
        /// The slected preset in combobox is always null.
        /// </summary>
        public string PresetSelValue
        {
            get { return string.Empty; }
        }

        #endregion

        #region private member

        private void LoadAdaptiveView()
        {
            RootNode = new AdaptivVieweNode(AdaptiveViewSet.Base);
            RootNode.IsRoot = true;
            LoadAdaptiveFromDocument(RootNode, RootNode.AdaptiveView);
            if (RootNode.Children.Count < 1)
            {
                AddViewExecute(null);
            }
        }
        private void LoadAdaptiveFromDocument(AdaptivVieweNode parentNode, IAdaptiveView parentView)
        {
            foreach (IAdaptiveView view in parentView.ChildViews)
           {
               AdaptivVieweNode node = new AdaptivVieweNode(view);
               parentNode.Add(node);

               node.ParentNode = parentNode;
               LoadAdaptiveFromDocument(node, view);
           }
        }
        /// <summary>
        /// Save AdaptiveView to Document.
        /// </summary>
        private void Save()
        {
            //delete
            foreach (var item in deleteList)
            {
                AdaptiveViewSet.DeleteAdaptiveView(item);
            }

            //create or edit
            SaveAdaptive2Document(RootNode);

            _ListEventAggregator.GetEvent<LoadAdaptiveViewEvent>().Publish(AdaptiveLoadType.Edit);

            _document.IsDirty = true;
        }
        private void SaveAdaptive2Document(AdaptivVieweNode parent)
        {
            foreach (AdaptivVieweNode node in parent.Children)
            {
                IAdaptiveView view;
                if(node.AdaptiveView == null)//add node created
                {
                    view = AdaptiveViewSet.CreateAdaptiveView(node.Name, parent.AdaptiveView);
                }
                else //edit node
                {
                    if (!node.IsEdited)//nothing changed
                    {
                        SaveAdaptive2Document(node);
                        continue;
                    }
                    view = node.AdaptiveView;
                    view.Name = node.Name;
                    //AdaptiveViewSet.ChangeParent(view, node.ParentNode.AdaptiveView);
                }

                AdaptiveViewSet.MoveAdaptiveViewTo(view, node.IndexInParent);
                view.Width = node.Width;
                view.Height = node.Height;
                view.Condition = node.Condition;
                node.AdaptiveView = view;

                SaveAdaptive2Document(node);
            }
        }

        //get default name
        private string GetAdaptiveViewName()
        {
            string ViewName = "New View  ";
            int ViewCounter = 1;
            string viewName;
            do
            {
                viewName = String.Concat(ViewName, ViewCounter++);
            } while (CheckNameExist(viewName,RootNode.Children));

            return viewName;
        }

        private bool CheckNameExist(string name, ObservableCollection<AdaptivVieweNode> list)
        {
            foreach(var item in list)
            {
                if (item.Name == name||CheckNameExist(name, item.Children))
                    return true;
            }
             return false;
        }

        void UpdateUI()
        {
            if (SelectValue!=null)
                IsSettingEnabled = true;
            else
                IsSettingEnabled = false;
        }

        void RefreshCommands()
        {
            //CloneViewCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
        }

        void InitPresets()
        {
            presets = new ObservableCollection<PresetNode>();
            presets.Add(new PresetNode(CommonDefine.PCRetina, 2560, 1600));
            presets.Add(new PresetNode(CommonDefine.PC1280, 1280, 800));
            presets.Add(new PresetNode(CommonDefine.PC1024, 1024, 768));
            presets.Add(new PresetNode(CommonDefine.IPhone6Plus, 1242, 2208));
            presets.Add(new PresetNode(CommonDefine.IPhone6, 750, 1334));
            presets.Add(new PresetNode(CommonDefine.IPhone5, 640, 1136));
            presets.Add(new PresetNode(CommonDefine.GalaxyS6, 1440, 2560));
            presets.Add(new PresetNode(CommonDefine.GalaxyS5, 1080, 1920));
            presets.Add(new PresetNode(CommonDefine.GalaxyS3, 720, 1280));
            presets.Add(new PresetNode(CommonDefine.LGOptimusG3, 1440, 2560));
            presets.Add(new PresetNode(CommonDefine.GoogleNexus5, 1080, 1920));
            presets.Add(new PresetNode(CommonDefine.IPadPro, 2732, 2048));
            presets.Add(new PresetNode(CommonDefine.IpadMini2, 2048, 1536));
            presets.Add(new PresetNode(CommonDefine.IpadMini1, 1024, 768));
            presets.Add(new PresetNode(CommonDefine.GalaxyTabS2, 2048, 1536));
            presets.Add(new PresetNode(CommonDefine.GalaxyTabA, 1024, 768));
            presets.Add(new PresetNode(CommonDefine.GoogleNexus7, 1824, 1200));
            presets.Add(new PresetNode(CommonDefine.Watch42mm, 312, 390));
            presets.Add(new PresetNode(CommonDefine.Watch38mm, 272, 340));
        }
        //Adaptive view to be delete
        List<IAdaptiveView> deleteList ;

        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        IAdaptiveViewSet AdaptiveViewSet
        {
            get
            {
                return _document == null ? null : _document.AdaptiveViewSet;
            }
        }

        #endregion
    }

    class PresetNode
    {
        public PresetNode(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
        }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
