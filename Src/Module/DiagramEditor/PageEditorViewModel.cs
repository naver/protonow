using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System.Drawing;


namespace Naver.Compass.Module
{
    public class PageEditorViewModel : EditPaneViewModelBase,
        ICommandSink, IPagePropertyData        
    { 
        #region Constructor
        public PageEditorViewModel(Guid pageGID)
        {
            //IconSource = ISC.ConvertFromInvariantString(@"pack://application:,,/Images/document.png") as ImageSource;
            _pageGID = pageGID;
            _model = new PageEditorModel(pageGID);

            _copyTime = 0;            
            Title = "page";
            ContentId = pageGID.ToString();

            //Register the Routed Command Handler,20140306
            _commandSink = new CommandSink();
            _commandSink.RegisterCommand(ApplicationCommands.Copy, param => CanRunCopyCommand,CopyCommanddHandler);
            _commandSink.RegisterCommand(ApplicationCommands.Paste, param => CanRunPasteCommand, PasteCommanddHandler);
            _commandSink.RegisterCommand(ApplicationCommands.Cut, param => CanRunCutCommand, CutCommanddHandler);
            _commandSink.RegisterCommand(EditingCommands.Delete, param => CanRunDeleteCommand, DeleteCommanddHandler);

            _commandSink.RegisterCommand(EditingCommands.AlignLeft, param => CanRunAlignLeftCommand, AlignLeftCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.AlignRight, param => CanRunAlignRightCommand, AlignRightCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.AlignCenter, param => CanRunAlignCenterCommand, AlignCenterCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.AlignJustify, param => CanRunAlignJustifyCommand, AlignJustifyCommandHandler);
            _commandSink.RegisterCommand(TextCommands.AlignTextTop, param => CanRunAlignTopCommand, AlignTopCommandHandler);
            _commandSink.RegisterCommand(TextCommands.AlignTextBottom, param => CanRunAlignBottomCommand, AlignBottomCommandHandler);
            _commandSink.RegisterCommand(TextCommands.AlignTextMiddle, param => CanRunAlignTextMiddleCommand, AlignTextMiddleCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Family, param => CanRunFontFamilyCommand, FontFamilyCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Size, param => CanRunFontSizeCommand, FontSizeCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.ToggleBold, param => CanRunFontBoldCommand, FontBoldCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.ToggleUnderline, param => CanRunFontUnderlineCommand, FontUnderlineCommandHandler);
            _commandSink.RegisterCommand(TextCommands.Strikethrough, param => CanRunFontStrikethroughCommand, FontStrikethroughCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Color, param => CanRunFontColorCommand, FontColorCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.ToggleItalic, param => CanRunFontItalicCommand, FontItalicCommandHandler);

            _commandSink.RegisterCommand(BorderCommands.BorderLineColor, param => CanRunBorderLineColorCommand, BorderLineColorCommandHandler);
            _commandSink.RegisterCommand(BorderCommands.BorderLinePattern, param => CanRunBorderLinePatternCommand, BorderLinePatternCommandHandler);
            _commandSink.RegisterCommand(BorderCommands.BorderLineThinck, param => CanRunBorderLineThinckCommand, BorderLineThinckCommandHandler);
            _commandSink.RegisterCommand(BorderCommands.BackGround, param => CanRunBackGroundCommand, ChangeBackGroundCommandHandler);   
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
        }
        #endregion Constructor

        #region Private member
        private PageEditorModel _model;
        Guid _pageGID;
        readonly CommandSink _commandSink;
        protected ISelectionService _selectionService;
        private int _copyTime;
        #endregion Private member 
    
        #region Model Data Binding Property
        //Is Data Changed
        public bool IsDirty
        {
            get { return _model.IsDirty; }
            set
            {
                if (_model.IsDirty != value)
                {
                    _model.IsDirty = value;
                    FirePropertyChanged("IsDirty");
                }
            }
        }

        //All Widgets in the Page;
        public ObservableCollection<WidgetViewModBase> items;
        public ObservableCollection<WidgetViewModBase> Items
        {
            get
            {  
                if (items == null)
                {
                    items = new ObservableCollection<WidgetViewModBase>(); 
                    
                    //Add widget from DOM to UI
                    foreach (IWidget it in _model.Widgets)
                    {
                        WidgetViewModBase vmItem;
                        switch (it.WidgetType)
                        {
                            case WidgetType.Button:
                                {
                                    vmItem = new ButtonWidgetViewModel(it);
                                    break;
                                }
                            case WidgetType.Image:
                                {
                                    vmItem = new ImageWidgetViewModel(it);
                                    break;
                                }

                            case WidgetType.FlowShape:
                                {
                                    IFlowShape shape = it as IFlowShape;
                                    if (shape.FlowShapeType == FlowShapeType.Diamond)
                                    {
                                        vmItem = new DiamondWidgetViewModel(it);
                                    }
                                    else if (shape.FlowShapeType == FlowShapeType.Ellipse)
                                    {
                                        vmItem = new CircleWidgetViewModel(it);
                                    }
                                    else if (shape.FlowShapeType == FlowShapeType.Rectangle)
                                    {
                                        vmItem = new RectangleWidgetViewModel(it);
                                    }
                                    else
                                    {
                                        vmItem = new ImageWidgetViewModel(it);
                                    }
                                    break;
                                }
                            default:
                                {
                                    vmItem = new ImageWidgetViewModel(it);
                                    break;
                                }
                        }//switch
                        items.Add(vmItem);

                    }//foreach
                    return items;
                }//if    
                return items;
            }
        }
        #endregion Model Data Binding Property
        
        #region Public Function
        public void Update()
        {
            UpdateTitle();
            UpdateContent();
        }
        public void UpdateTitle()
        {            
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {
                IPage pageItem = doc.Document.Pages.GetPage(_pageGID);
                if (pageItem != null)
                {
                    Title = pageItem.Name;
                }
            }               
            else
            {
                Title = _pageGID.ToString();
            }
        }
        public void UpdateContent()
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {

            }
        }
        #endregion Public Function

        #region Page Command
        //Close Command
        private DelegateCommand<object> _closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new DelegateCommand<object>(OnClose, CanClose);
                }
                return _closeCommand;
            }
        }
        private bool CanClose(object obj)
        {            
            return true;
        }
        private void OnClose(object obj)
        {
            _ListEventAggregator.GetEvent<ClosePageEvent>().Publish(_pageGID);
        }

        //Active Command
        private DelegateCommand<object> _activeCommand = null;
        public ICommand ActiveCommand
        {
            get
            {
                if (_activeCommand == null)
                {
                    _activeCommand = new DelegateCommand<object>(OnActived);
                }
                return _activeCommand;
            }
        }
        private void OnActived(object obj)
        {
            //_ListEventAggregator.GetEvent<ClosePageEvent>().Publish(_pageGID);
        }

        //Select all widgets Command
        public DelegateCommand<object> _selectAllCommand = null;
        public ICommand SelectAllCommand
        {
            get
            {
                if (_selectAllCommand == null)
                {
                    _selectAllCommand = new DelegateCommand<object>(OnSelectAll);
                }
                return _selectAllCommand;
            }
        }
        private void OnSelectAll(object obj)
        {
            foreach (WidgetViewModBase wdg in Items)
            {
                wdg.IsSelected = true;
            }            
        }
        #endregion Page Command

        #region Override function
        override protected void OnItemAdded(object obj)
        {
            DropInfo info = obj as DropInfo;
            if (info == null)
                return;

            string xamlString =info.e.Data.GetData("DESIGNER_ITEM") as string;
            if (!String.IsNullOrEmpty(xamlString))
            {
                FrameworkElement content = XamlReader.Load(XmlReader.Create(new StringReader(xamlString))) as FrameworkElement;
                if (content != null)
                {
                    if (xamlString.Contains("wRectangle.png"))
                    {
                        AddWidgetItem(WidgetType.FlowShape,
                            FlowShapeType.Rectangle, info.position.X, info.position.Y, 120, 60);
                    }
                    else if (xamlString.Contains("wDiamond.png"))
                    {
                        AddWidgetItem(WidgetType.FlowShape,
                            FlowShapeType.Diamond, info.position.X, info.position.Y, 120, 60);
                    }
                    else if (xamlString.Contains("wButton.png"))
                    {
                        AddWidgetItem(WidgetType.Button,
                            FlowShapeType.None, info.position.X, info.position.Y, 120, 60);
                    }
                    else if (xamlString.Contains("wCircle.png"))
                    {
                        AddWidgetItem(WidgetType.FlowShape,
                            FlowShapeType.Ellipse, info.position.X, info.position.Y, 120, 60);
                    }
                    else
                    {
                        //newItem.Content = content;
                        AddWidgetItem(WidgetType.Image,
                            FlowShapeType.None, info.position.X, info.position.Y, 120, 60);
                    }  
                }
            }
        }
        override protected void OnItemRemoved(object obj)
        {
            if (obj == null)
            {
                List<WidgetViewModBase> ToRemoveList = new List<WidgetViewModBase>();
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {
                        //Remove item from selection service
                        wdgItem.IsSelected = false;

                        //construct the  widget list to be removed
                        ToRemoveList.Add(wdgItem);
                    }
                }
                foreach (WidgetViewModBase it in ToRemoveList)
                {
                    Guid gid=it.widgetGID;
                    Items.Remove(it);
                    _model.RemoveWidgetFromDom(gid);
                }
            }
            else 
            {
                WidgetViewModBase wdgVM= obj as WidgetViewModBase;
                if(wdgVM==null)
                {
                    return;
                }

                                    
                Guid gid=wdgVM.widgetGID;
                Items.Remove(wdgVM);
                _model.RemoveWidgetFromDom(gid);
            }
        }
        override protected void OnPannelSelected(bool bIsSelected)
        {
            //Update the selection service data
            if (bIsSelected == true)
            {
                //Re-load the selected widget when page is re-selected
                List<IWidgetPropertyData> ToAddList = new List<IWidgetPropertyData>();
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {
                        ToAddList.Add(wdgItem);
                    }
                }

                //register the selected page
                _selectionService.RegisterPage(this,ToAddList);
            }
            else
            {
                _selectionService.RemovePage(this);
            }
            //Keep the selection UI
        }

        #endregion

        #region Private function
        private void AddWidgetItem(WidgetType type,FlowShapeType flowType,double x,double y,int w,int h)
        {
            IWidget widget = _model.AddWidgetItem2Dom(type, flowType, x, y, w, h);
            if (widget == null)
                return;
            Insert2Canvas(widget);          
        }
        private void Insert2Canvas(IWidget widget)
        {
            WidgetViewModBase vmItem;
            switch (widget.WidgetType)
            {
                case WidgetType.Button:
                    {
                        vmItem = new ButtonWidgetViewModel(widget);
                        break;
                    }
                case WidgetType.Image:
                    {
                        vmItem = new ImageWidgetViewModel(widget);
                        break;
                    }

                case WidgetType.FlowShape:
                    {
                        IFlowShape shape = widget as IFlowShape;
                        if (shape.FlowShapeType == FlowShapeType.Diamond)
                        {
                            vmItem = new DiamondWidgetViewModel(widget);
                        }
                        else if (shape.FlowShapeType == FlowShapeType.Ellipse)
                        {
                            vmItem = new CircleWidgetViewModel(widget);
                        }
                        else if (shape.FlowShapeType == FlowShapeType.Rectangle)
                        {
                            vmItem = new RectangleWidgetViewModel(widget);
                        }
                        else
                        {
                            vmItem = new ImageWidgetViewModel(widget);
                        }
                        break;
                    }
                default:
                    {
                        vmItem = new ImageWidgetViewModel(widget);
                        break;
                    }
            }
            vmItem.IsSelected = true;
            Items.Add(vmItem);
        }
        #endregion

        #region ICommandSink Members
        public bool CanExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            return _commandSink.CanExecuteCommand(command, parameter, out handled);
        }

        public void ExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            _commandSink.ExecuteCommand(command, parameter, out handled);
        }
        #endregion ICommandSink Members

        #region Global Routed Command Handler
        //Command Copy,20140306
        private void CopyCommanddHandler(object parameter)
        {
            //clear copy-data cache
            List<IWidget> ToCopyList = _selectionService.GetCloneCacheData();
            ToCopyList.Clear();
            _copyTime = 0;
           
            //implement copy operation
            if (_selectionService.WidgetNumber <= 0)
            {
                return;
            }
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                IWidget cloneItem =wdgItem.Clone();
                if (cloneItem != null)
                {
                    ToCopyList.Add(cloneItem);
                }
            }

        }
        public bool CanRunCopyCommand
        {
            //get
            //{
            //    if (_selectionService.WidgetNumber > 0)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }             
            //} 
            get{ return true; }            
        }

        //Command Paste,20140306
        private void PasteCommanddHandler(object parameter)
        {
            //clear copy-data cache
            List<IWidget> ToCopyList = _selectionService.GetCloneCacheData();
            if (ToCopyList.Count < 1)
            {
                return;
            }

            //Run Paste
            foreach (IWidget item in ToCopyList)
            {
                IWidget newItem = item.Clone() as IWidget;
                if(newItem !=null)
                {
                    //Add to Page(Dom)
                    _model.AddClonedItem2Dom(newItem);

                    //Render the UI
                    PasteWidgetItem(newItem);
                }                
            }
        }
        private void PasteWidgetItem(IWidget newItem)
        {
            _copyTime += 1;
            newItem.X += 20*_copyTime;
            newItem.Y += 20*_copyTime;
            Insert2Canvas(newItem);           
        }
        public bool CanRunPasteCommand
        {
            get
            {
                if (_selectionService.GetCloneCacheData().Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } 
        }

        //Command Cut,20140306
        private void CutCommanddHandler(object parameter)
        {
            //clear copy-data cache
            List<IWidget> ToCopyList = _selectionService.GetCloneCacheData();
            ToCopyList.Clear();
            _copyTime = 0;
           
            //implement copy operation
            if (_selectionService.WidgetNumber <= 0)
            {
                return;
            }

            List<IWidgetPropertyData> allSelects= _selectionService.GetSelectedWidgets();
            allSelects = allSelects.GetRange(0, allSelects.Count);
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                IWidget cloneItem =wdgItem.Clone();
                if (cloneItem != null)
                {
                    ToCopyList.Add(cloneItem);
                    OnItemRemoved(wdgItem);
                }
            }
        }
        public bool CanRunCutCommand
        {
            get
            {
                if (_selectionService.WidgetNumber > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            } 
        }

        //Command Delete,20140306
        private void DeleteCommanddHandler(object parameter)
        {
            OnItemRemoved(parameter);
        }
        public bool CanRunDeleteCommand
        {
            get 
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }                
                return false;
            }
        }

        #region Font Routed command Handler
        //
        #region Align Left Command Handler

        private void AlignLeftCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    if (Convert.ToBoolean(parameter))
                    {
                        wdgItem.vTextHorAligen = Alignment.Left;
                    }
                }
            }
          
        }
        public bool CanRunAlignLeftCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion Align Left Command Handler

        #region Align Right Command Handler

        private void AlignRightCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    if (Convert.ToBoolean(parameter))
                    {
                        wdgItem.vTextHorAligen = Alignment.Right;
                    }
                }
            }

        }
        public bool CanRunAlignRightCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion Align Left Command Handler

        #region Align Center Command Handler

        private void AlignCenterCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    if (Convert.ToBoolean(parameter))
                    {
                        wdgItem.vTextHorAligen = Alignment.Center;
                    }
                }
            }

        }
        public bool CanRunAlignCenterCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion 

        #region Align Justify Command Handler

        private void AlignJustifyCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    if (Convert.ToBoolean(parameter))
                    {
                        wdgItem.vTextHorAligen = Alignment.Left;
                    }
                }
            }

        }
        public bool CanRunAlignJustifyCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion 

        #region Align Text Top Command Handler

        private void AlignTopCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    if (Convert.ToBoolean(parameter))
                    {
                        wdgItem.vTextVerAligen =Alignment.Top;
                    }
                   
                }
            }

        }
        public bool CanRunAlignTopCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion 

        #region Align Text Bottom Command Handler

        private void AlignBottomCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    if (Convert.ToBoolean(parameter))
                    {
                        wdgItem.vTextVerAligen = Alignment.Bottom;
                    }
                }
            }

        }
        public bool CanRunAlignBottomCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion 

        #region Align Text Middle Command Handler

        private void AlignTextMiddleCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    if (Convert.ToBoolean(parameter))
                    {
                        wdgItem.vTextVerAligen = Alignment.Center;
                    }
                }
            }

        }
        public bool CanRunAlignTextMiddleCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion 

        #region Font Family Command Handler

        private void FontFamilyCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    wdgItem.vFontFamily = Convert.ToString(parameter);
                }
            }

        }
        public bool CanRunFontFamilyCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion 

        #region Font Size Command Handler

        private void FontSizeCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {

                    wdgItem.vFontSize = Convert.ToUInt16(parameter);
                }
            }

        }
        public bool CanRunFontSizeCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion 

        #region Bold style Command Handler
        private void FontBoldCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    wdgItem.vFontBold = Convert.ToBoolean(parameter);
                }
            }

        }
        public bool CanRunFontBoldCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region Underline style Command Handler

        private void FontUnderlineCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    Dictionary<int, bool> tStyles = new Dictionary<int,bool>();

                    tStyles.Add(0,Convert.ToBoolean(parameter));
                    wdgItem.uFontDecorations = tStyles;

                }
            }

        }
        public bool CanRunFontUnderlineCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Strikethrough style Command Handler

        private void FontStrikethroughCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    Dictionary<int, bool> tStyles = new Dictionary<int, bool>();

                    tStyles.Add(1, Convert.ToBoolean(parameter));
                    wdgItem.uFontDecorations = tStyles;
                }
            }

        }
        public bool CanRunFontStrikethroughCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Font color style Command Handler

        private void FontColorCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    wdgItem.vFontColor = ColorTranslator.FromHtml(Convert.ToString(parameter));
                }
            }

        }
        public bool CanRunFontColorCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Italic style Command Handler

        private void FontItalicCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true )
                {
                    wdgItem.vFontItalic =  Convert.ToBoolean(parameter);
                }
            }
        }
        public bool CanRunFontItalicCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion


        #region BorderLine color  Command Handler

        private void BorderLineColorCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    wdgItem.vBorderLineColor = ColorTranslator.FromHtml(Convert.ToString(parameter));
                }
            }
        }

        public bool CanRunBorderLineColorCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return wdgItem.IsSupportBorber;
                    }
                }
                return false;
            }
        }

        #endregion

        #region BorderLine Pattern  Command Handler

        private void BorderLinePatternCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    string nStrokeType = parameter.ToString();
                    if (nStrokeType.IndexOf("LineStyleDot.png") > -1)
                    {
                        wdgItem.vBorderlineStyle = LineStyle.Dot;
                    }
                    else if (nStrokeType.IndexOf("LineStyleDot2.png") > -1)
                    {
                        wdgItem.vBorderlineStyle = LineStyle.DashDot;
                    }
                    else if (nStrokeType.IndexOf("LineStyleDouble.png") > -1)
                    {
                        wdgItem.vBorderlineStyle = LineStyle.DashDotDot;
                    }
                    else if (nStrokeType.IndexOf("LineStyleSolid.png") > -1)
                    {
                        wdgItem.vBorderlineStyle = LineStyle.Solid;
                    }
                }
            }
        }
        public bool CanRunBorderLinePatternCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {
                        return wdgItem.IsSupportBorber;
                    }
                }
                return false;
            }
        }

        #endregion

        #region BorderLine Thinck  Command Handler

        private void BorderLineThinckCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    string nPar = parameter.ToString();
                    if (nPar.IndexOf('1') > -1)
                    {
                        wdgItem.vBorderLinethinck = 1;
                    }
                    else if (nPar.IndexOf('2') > -1)
                    {
                        wdgItem.vBorderLinethinck = 2;
                    }
                    else if (nPar.IndexOf('3') > -1)
                    {
                        wdgItem.vBorderLinethinck = 3;
                    }
                    else if (nPar.IndexOf('4') > -1)
                    {
                        wdgItem.vBorderLinethinck = 4;
                    }
                    else if (nPar.IndexOf('5') > -1)
                    {
                        wdgItem.vBorderLinethinck = 5;
                    }
                }
            }
        }

        public bool CanRunBorderLineThinckCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {
                        return wdgItem.IsSupportBorber;
                    }
                }
                return false;
            }
        }

        #endregion

        #endregion Font Routed command Handler

        #region BorderLine Thinck  Command Handler

        private void ChangeBackGroundCommandHandler(object parameter)
        {
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    wdgItem.vBackgroundColor = ColorTranslator.FromHtml(Convert.ToString(parameter));
                }
            }
        }

        public bool CanRunBackGroundCommand
        {
            get
            {
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {

                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #endregion Global Routed Command Handler
    }


}
