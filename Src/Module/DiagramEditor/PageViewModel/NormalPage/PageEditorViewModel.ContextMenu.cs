using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Naver.Compass.Service;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.InfoStructure;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Windows.Markup;
using Naver.Compass.Service.CustomLibrary;
using UndoCompositeCommand = Naver.Compass.InfoStructure.CompositeCommand;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        #region Add menu Item
        private void OnPreSetContextMenu(object obj)
        {
            try
            {
                switch (_selectionService.WidgetNumber)
                {
                    case 0:
                        MenuFor0Widget();
                        break;
                    case 1:
                        MenuFor1Widget();
                        break;
                    case 2:
                        MenuFor2Widgets();
                        break;
                    case 3:
                        MenuFor3Widgets();
                        break;
                    default:
                        MenuFor3Widgets();
                        break;
                }
            }
            catch
            {

            }
            
        }
        #endregion

        #region select 0 widget

        /// <summary>
        /// Create menu when select nothing.
        /// </summary>
        private void MenuFor0Widget()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.IsOpen = true;
            ContextMenuItem item = new ContextMenuItem(GlobalData.FindResource("Menu_File_Paste"),
                PasteCommand, "Paste", CommonDefine.KeyPaste); 
            contextMenu.Items.Add(item);
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("ContextMenu_SelectAll"), CommonDefine.KeySelectAll, SelectAllCommand));

            AddExport2Menu(contextMenu);

            AddGridGuide2Menu(contextMenu);


            //code to extract menu template
            //var str = new StringBuilder();
            //using (var writer = new StringWriter(str))
            //    XamlWriter.Save(item.Template, writer);
            //Debug.Write(str);
        }
        #endregion

        #region select 1 widget
        /// <summary>
        /// Create menu when select one widget.
        /// </summary>
        private void MenuFor1Widget()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.IsOpen = true;
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs == null || wdgs.Count <= 0)
            {
                return;
            }
            WidgetViewModBase wdg = wdgs[0] as WidgetViewModBase;
            if (wdg == null)
            {
                return;
            }

            if (wdg.IsGroup==true)
            {
                //TODO:Add Group context Menu
                AddCopyPasete2Menu(contextMenu);
                AddLibrary2Menu(contextMenu);

                //Add master convert menu
                GroupViewModel group= wdg as GroupViewModel;
                var target=group.WidgetChildren.FirstOrDefault(a => a is MasterWidgetViewModel);
                if (target == null)
                {
                    AddConvertMaster2Menu(contextMenu);
                }

                AddUnplace2Menu(contextMenu);
                AddUnGroup2Menu(contextMenu);
                AddOrder2Menu(contextMenu);
            }
            else
            {
                IRegion selObject = ActivePage.WidgetsAndMasters[_selectionService.GetSelectedWidgetGUIDs()[0]];
                if (selObject == null)
                    return;
                if (selObject is IMaster)
                {
                    AddCopyPasete2Menu(contextMenu);
                    AddExport2Menu(contextMenu);
                    AddLibrary2Menu(contextMenu);
                    AddUnplace2Menu(contextMenu);

                    if (wdg.RealParentGroupGID==Guid.Empty)
                    {
                        AddMasterOperationMenu(contextMenu,(wdg as MasterWidgetViewModel).IsLocked2MasterLocation);
                    }
                    else
                    {
                        AddMasterOperationMenu(contextMenu,(wdg as MasterWidgetViewModel).IsLocked2MasterLocation,true);
                    }

                    AddOrder2Menu(contextMenu, false);
                }
                else if (selObject is IWidget)
                {
                    var widget = selObject as IWidget;
                    switch (widget.WidgetType)
                    {
                        case WidgetType.Image:
                            AddCopyPasete2Menu(contextMenu);
                            AddExport2Menu(contextMenu);
                            AddLibrary2Menu(contextMenu);
                            AddConvertMaster2Menu(contextMenu);
                            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("ContextMenu_ImportImage"), ImportImageCommand));
                            contextMenu.Items.Add(new Separator());
                            AddUnplace2Menu(contextMenu);
                            AddTooltip2Menu(contextMenu);
                            AddOrder2Menu(contextMenu);
                            break;
                        case WidgetType.RadioButton:
                        case WidgetType.Checkbox:
                        case WidgetType.TextField:
                        case WidgetType.TextArea:
                        case WidgetType.Button:
                        case WidgetType.Shape:
                            AddCopyPasete2Menu(contextMenu);
                            AddExport2Menu(contextMenu);
                            AddLibrary2Menu(contextMenu);
                            AddConvertMaster2Menu(contextMenu);
                            AddEditText2Menu(contextMenu);
                            contextMenu.Items.Add(new Separator());
                            AddUnplace2Menu(contextMenu);
                            AddTooltip2Menu(contextMenu);
                            AddOrder2Menu(contextMenu);
                            AddDefaultstyleToMenu(contextMenu);
                            break;
                        case WidgetType.HotSpot:
                        case WidgetType.SVG:
                            AddCopyPasete2Menu(contextMenu);
                            AddExport2Menu(contextMenu);
                            AddLibrary2Menu(contextMenu);
                            AddConvertMaster2Menu(contextMenu);
                            AddUnplace2Menu(contextMenu);
                            AddTooltip2Menu(contextMenu);
                            AddOrder2Menu(contextMenu);
                            break;
                        case WidgetType.Line:
                            AddCopyPasete2Menu(contextMenu);
                            AddExport2Menu(contextMenu);
                            AddLibrary2Menu(contextMenu);
                            AddConvertMaster2Menu(contextMenu, false);
                            if (AdaptiveViewsList.Count > 1)
                            {
                                contextMenu.Items.Add(new Separator());
                                AddUnplace2Menu(contextMenu, false);
                            }
                            //AddTooltip2Menu(contextMenu);
                            AddOrder2Menu(contextMenu);
                            AddDefaultstyleToMenu(contextMenu);
                            break;
                        case WidgetType.ListBox:
                        case WidgetType.DropList:
                            AddCopyPasete2Menu(contextMenu, true);
                            AddExport2Menu(contextMenu);
                            AddEditItems2Menu(contextMenu);
                            contextMenu.Items.Add(new Separator());
                            AddLibrary2Menu(contextMenu);
                            AddConvertMaster2Menu(contextMenu);
                            AddUnplace2Menu(contextMenu);
                            AddTooltip2Menu(contextMenu);
                            AddOrder2Menu(contextMenu);
                            AddDefaultstyleToMenu(contextMenu);
                            break;
                        case WidgetType.Toast:
                        case WidgetType.DynamicPanel:
                            AddCopyPasete2Menu(contextMenu);
                            AddExport2Menu(contextMenu);
                            AddLibrary2Menu(contextMenu);
                            AddConvertMaster2Menu(contextMenu);
                            AddUnplace2Menu(contextMenu);
                            AddTooltip2Menu(contextMenu);
                            AddOrder2Menu(contextMenu);
                            break;
                        case WidgetType.HamburgerMenu:
                            AddCopyPasete2Menu(contextMenu);
                            AddExport2Menu(contextMenu);
                            AddLibrary2Menu(contextMenu);
                            AddConvertMaster2Menu(contextMenu, false);
                            contextMenu.Items.Add(new Separator());
                            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("ContextMenu_ImportImage"), ImportImageCommand));
                            contextMenu.Items.Add(new Separator());
                            AddUnplace2Menu(contextMenu);
                            AddTooltip2Menu(contextMenu);
                            AddOrder2Menu(contextMenu);


                            break;
                    }
                }
            }
            
        }
        #endregion

        #region select 2 widgets
        /// <summary>
        /// Create menu when select two widgets.
        /// </summary>
        private void MenuFor2Widgets()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.IsOpen = true;
            AddCopyPasete2Menu(contextMenu);
            AddLibrary2Menu(contextMenu);
            AddConvertMaster2Menu(contextMenu, false);
            AddUnplace2Menu(contextMenu);
            AddGroup2Menu(contextMenu);
            if(CanRunUnGroupCommand)
            {
                AddUnGroup2Menu(contextMenu);
            }
            AddOrder2Menu(contextMenu);
            AddAlignM2Menu(contextMenu);
        }
        #endregion

        #region select more than 2 widgets
        /// <summary>
        /// Create menu when select three widgets or more.
        /// </summary>
        private void MenuFor3Widgets()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.IsOpen = true;
            AddCopyPasete2Menu(contextMenu);
            AddLibrary2Menu(contextMenu);
            AddConvertMaster2Menu(contextMenu, false);
            AddUnplace2Menu(contextMenu);
            AddGroup2Menu(contextMenu);
            if (CanRunUnGroupCommand)
            {
                AddUnGroup2Menu(contextMenu);
            }
            AddOrder2Menu(contextMenu);
            AddAlignM2Menu(contextMenu);
            AddDistribute2Menu(contextMenu);
        }
        #endregion

        #region Grid and Guide
        private void AddGridGuide2Menu(ContextMenu contextMenu)
        {
            ContextMenuItem grid = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid"));
            ContextMenuItem item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_ShowGrid"), CommonDefine.KeyShowGrid, ShowGridCommand);
            item.IsCheckable = true;
            item.IsChecked = GlobalData.IsShowGrid;
            grid.Items.Add(item);
            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_SnapToGrid"), CommonDefine.KeySnaptoGrid, SnaptoGridCommand);
            item.IsCheckable = true;
            item.IsChecked = GlobalData.IsSnapToGrid;
            grid.Items.Add(item);

            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_GridSet"), GridSettingCommand);
            grid.Items.Add(item);
            grid.Items.Add(new Separator());

            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_ShowGlobalGuid"), CommonDefine.KeyShowGlobalGuide, ShowGlobalGuideCommand);
            item.IsCheckable = true;
            item.IsChecked = GlobalData.IsShowGlobalGuide;
            grid.Items.Add(item);

            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_ShowPageGuide"), CommonDefine.KeyShowPageGuide, ShowPageGuideCommand);
            item.IsCheckable = true;
            item.IsChecked = GlobalData.IsShowPageGuide;
            grid.Items.Add(item);

            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_SnapToGuide"), CommonDefine.KeySnaptoGuide, SnaptoGuideCommand);
            item.IsCheckable = true;
            item.IsChecked = GlobalData.IsSnapToGuide;
            grid.Items.Add(item);

            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_LockGuide"), CommonDefine.KeyLockGuide, LockGuidesCommand);
            item.IsCheckable = true;
            item.IsChecked = GlobalData.IsLockGuides;
            grid.Items.Add(item);
            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_CreateGuide"), CreateGuidesCommand);
            grid.Items.Add(item);

            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_DelteAllGuide"), DeleteAllGuidesCommand);
            grid.Items.Add(item);

            item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_GuideSet"), GuideSetttingCommand);
            grid.Items.Add(item);

           // grid.Items.Add(new Separator());

            //item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_SnapToObject"), SnaptoObjectCommand);
            //item.IsCheckable = true;
            //item.IsChecked = false;
            //grid.Items.Add(item);
            //item = new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Grid_ObjectSnapSet"), ObjectSnapSettingCommand);
            //grid.Items.Add(item);

            contextMenu.Items.Add(grid);
        }
        #endregion

        #region copy paste
        private void AddCopyPasete2Menu(ContextMenu contextMenu, bool bSep = true)
        {
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_File_Cut"),
                CutCommand, "Cut", CommonDefine.KeyCut));
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_File_Copy"),
                CopyCommand, "Copy", CommonDefine.KeyCopy));// GetImage("Copy_16x16.png")
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_File_Paste"),
                PasteCommand, "Paste", CommonDefine.KeyPaste));
            if (bSep == true)
                contextMenu.Items.Add(new Separator());
        }
        #endregion

        #region Export to Image
        private void AddExport2Menu(ContextMenu contextMenu, bool bSep = true)
        {
            string menuHeader;
            if (0 == _selectionService.WidgetNumber)
            {
                menuHeader = GlobalData.FindResource("ContextMenu_ExportPage2Image");
            }
            else
            {
                menuHeader = GlobalData.FindResource("ContextMenu_ExportObject2Image");
            }
            ContextMenuItem item = new ContextMenuItem(menuHeader, ExportToImageCommand);

            contextMenu.Items.Add(item);

            if (bSep == true)
                contextMenu.Items.Add(new Separator());
        }
        #endregion

        #region Add/Export to Library

        private void AddLibrary2Menu(ContextMenu contextMenu)
        {
            if (_document == null)
            {
                return;
            }

            ContextMenuItem addToLibrary = new ContextMenuItem(GlobalData.FindResource("ContextMenu_AddToLibrary"));
            addToLibrary.Style = Application.Current.TryFindResource("librarymenu") as Style;
           
            ICustomLibraryService customLibraryService = ServiceLocator.Current.GetInstance<ICustomLibraryService>();
            
            foreach(ICustomLibrary item in customLibraryService.GetAllCustomLibraies())
            {
                //don't add current library to library list.
                if (_document.DocumentType == DocumentType.Library && item.LibraryGID == _document.Guid)
                    continue;

                string itemHeader = item.Header;
                if (item.TabType != null && item.Header.StartsWith(item.TabType))
                {
                    itemHeader = item.Header.Substring(item.TabType.Length);
                }

                addToLibrary.Items.Add(new ContextMenuItem(itemHeader, AddToLibraryCommand, item));
                addToLibrary.Tag = "limit";
            }
            addToLibrary.Items.Add(new ContextMenuItem(GlobalData.FindResource("ContextMenu_AddLibrary"),AddLibraryCommand));
            contextMenu.Items.Add(addToLibrary);

            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("ContextMenu_ExportToLibrary"), ExportToLibraryCommand));

            contextMenu.Items.Add(new Separator());
        }

        #endregion

        #region Add/Export to Library

        private void AddConvertMaster2Menu(ContextMenu contextMenu, bool bSep = true)
        {
            if (_document == null)
            {
                return;
            }


            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            bool bCanConvert2Master = true;
            foreach (WidgetViewModBase it in wdgs)
            {
                if (it is MasterWidgetViewModel)
                {
                    bCanConvert2Master = false;
                    break;
                }
                else if (it is GroupViewModel)
                {
                    GroupViewModel group = it as GroupViewModel;
                    var target = group.WidgetChildren.FirstOrDefault(a => a is MasterWidgetViewModel);
                    if (target != null)
                    {
                        bCanConvert2Master = false;
                        break;
                    }
                }
                else
                {
                    if (it.RealParentGroupGID!=Guid.Empty)
                    {
                        bCanConvert2Master = false;
                        break;
                    }
                }
            }

            if (bCanConvert2Master == false)
            {
                return;
            }


            ContextMenuItem convertToMaster = new ContextMenuItem(GlobalData.FindResource("Master_ContextMenu_ConvertToMaster"), ConvertToMasterCommand, "ConvertMaster", string.Empty);
            contextMenu.Items.Add(convertToMaster);

            if (bSep == true)
                contextMenu.Items.Add(new Separator());
        }

        #endregion

        #region group/ungroup
        private void AddGroup2Menu(ContextMenu contextMenu)
        {
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Group"), GroupCommand, "Group", CommonDefine.KeyGroup));
        }
        private void AddUnGroup2Menu(ContextMenu contextMenu)
        {
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Ungroup"), UnGroupCommand, "Ungroup", CommonDefine.KeyUnGroup));
        }
        #endregion

        #region edit text and tooltip
        private void AddEditText2Menu(ContextMenu contextMenu)
        {
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("ContextMenu_EditText"), EditCommand));
        }
        private void AddEditItems2Menu(ContextMenu contextMenu)
        {
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("ContextMenu_EditListItems"), EditListItemsCommamd));
        }
        private void AddTooltip2Menu(ContextMenu contextMenu)
        {
            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("ContextMenu_Tooltip"), ToolTipCommand));
        }

        private void AddUnplace2Menu(ContextMenu contextMenu, bool bSep = true)
        {
            //Add "unplace from view" only when there're adptive views.
            if (AdaptiveViewsList.Count < 1)
                return;

            contextMenu.Items.Add(new ContextMenuItem(GlobalData.FindResource("Adaptive_Unplace"), UnplaceFromViewCommand));
            if (bSep == true)
                contextMenu.Items.Add(new Separator());
        }

        private void AddMasterOperationMenu(ContextMenu contextMenu, bool bIsLocationLock=false,bool isGroupChild=false)
        {         
            //TODO:Master Break Away
            ContextMenuItem item2 = new ContextMenuItem(GlobalData.FindResource("Master_ContextMenu_BreakAway"), MasterBreakAwayCommand);
            if (isGroupChild == true)
            {
                item2.IsEnabled = false;
            }
            contextMenu.Items.Add(item2);

            //TODO:Master Lock to location
            ContextMenuItem item = new ContextMenuItem(GlobalData.FindResource("Master_ContextMenu_LocktoMasterLocation"), MasterLock2LocationCommand);
            item.IsCheckable = true;
            item.IsChecked = bIsLocationLock;
            contextMenu.Items.Add(item);
            
            //Add Separator;
            contextMenu.Items.Add(new Separator());
        }
        #endregion

        #region order
        private void AddOrder2Menu(ContextMenu contextMenu, bool bSep = true)
        {           
            if(bSep)
                contextMenu.Items.Add(new Separator());
            ContextMenuItem order = new ContextMenuItem(GlobalData.FindResource("ContextMenu_Order"));
            order.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_BringFront"),
                BringFrontCommand, "Bring_Front", CommonDefine.KeyBringFront));
            order.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_SendBack"),
                SendBackCommand, "Send_Back", CommonDefine.KeySendBack));
            order.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_BringForward"),
                BringForwardCommand, "Bring_Forward", CommonDefine.KeyBringForward));
            order.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_SendBackward"),
                SendBackwardCommand, "Send_Backward",CommonDefine.KeySendBackward));
            contextMenu.Items.Add(order);
        }
        #endregion

        #region align
        private void AddAlignM2Menu(ContextMenu contextMenu)
        {
            ContextMenuItem align = new ContextMenuItem(GlobalData.FindResource("ContextMenu_Align"));
            align.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Align_Left"),
                AlignLeftCommand, "Align_Left", CommonDefine.KeyAlignLeft));
            align.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Align_Center"),
                AlignCenterCommand, "Align_Center", CommonDefine.KeyAlignCenter));
            align.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Align_Right"),
                AlignRightCommand, "Align_Right", CommonDefine.KeyAlignRight));
            align.Items.Add(new Separator());
            align.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Align_Top"),
                AlignTopCommand, "Align_Top", CommonDefine.KeyAlignTop));
            align.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Align_Middle"),
                AlignMiddleCommand, "Align_Middle", CommonDefine.KeyAlignMiddle));
            align.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Align_Bottom"),
                AlignBottomCommand, "Align_Bottom", CommonDefine.KeyAlignBottom));
            contextMenu.Items.Add(align);
        }
        #endregion

        #region distribute
        private void AddDistribute2Menu(ContextMenu contextMenu)
        {
            ContextMenuItem distribute = new ContextMenuItem(GlobalData.FindResource("ContextMenu_Distribute"));
            distribute.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Distribute_Hor"),
                DistributeHorizCommand, "Distribute_Hor", CommonDefine.KeyDistributeHori));
            distribute.Items.Add(new ContextMenuItem(GlobalData.FindResource("Menu_Arrange_Distribute_Ver"),
                DistributeVertiCommand, "Distribute_Ver", CommonDefine.KeyDistributeVert));
            contextMenu.Items.Add(distribute);
        }
        #endregion

        #region Default style
        private void AddDefaultstyleToMenu(ContextMenu contextMenu)
        {
            contextMenu.Items.Add(new Separator());
            ContextMenuItem Defaultstyle = new ContextMenuItem(GlobalData.FindResource("ContextMenu_DefaultStyle"),
                SetDefaultStyleCommand);
            contextMenu.Items.Add(Defaultstyle);
         
        }
        #endregion

        #region get icon

        /// <summary>
        /// get image from MainToolbar
        /// </summary>
        /// <returns>image path</returns>
        private string GetImage(string imageName)
        {
            return string.Concat("pack://application:,,,/Naver.Compass.Module.MainToolBar;component/Images/", imageName);
        }
        #endregion

        #region event handler
        /// <summary>
        /// Selected page changed in Sitmap or EditorView
        /// For getting current selected widget.
        /// </summary>
        protected void SelectionPageChangeHandler(Guid pageGuid)
        {
            if (pageGuid == Guid.Empty)
                return;
            if (this.PageGID != pageGuid)
                return;

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {
                FireGrid();
                CheckDevices();
                LoadAdaptiveViewsHandler(AdaptiveLoadType.Load);
            }
            
        }
        #endregion

        #region Menu commands
        public DelegateCommand<object> CopyCommand { get; set; }
        public DelegateCommand<object> PasteCommand { get; set; }
        public DelegateCommand<object> CutCommand { get; set; }
        public DelegateCommand<object> ExportToImageCommand { get; set; }
        public DelegateCommand<object> GroupCommand { get; set; }
        public DelegateCommand<object> UnGroupCommand { get; set; }
        public DelegateCommand<object> BringFrontCommand { get; set; }
        public DelegateCommand<object> SendBackCommand { get; set; }
        public DelegateCommand<object> BringForwardCommand { get; set; }
        public DelegateCommand<object> SendBackwardCommand { get; set; }
        public DelegateCommand<object> AlignLeftCommand { get; set; }
        public DelegateCommand<object> AlignCenterCommand { get; set; }
        public DelegateCommand<object> AlignRightCommand { get; set; }
        public DelegateCommand<object> AlignTopCommand { get; set; }
        public DelegateCommand<object> AlignMiddleCommand { get; set; }
        public DelegateCommand<object> AlignBottomCommand { get; set; }
        public DelegateCommand<object> DistributeHorizCommand { get; set; }
        public DelegateCommand<object> DistributeVertiCommand { get; set; }
        public DelegateCommand<object> ImportImageCommand { get; set; }
        public DelegateCommand<object> EditCommand { get; set; }
        public DelegateCommand<object> ToolTipCommand { get; set; }
        public DelegateCommand<object> EditListItemsCommamd { get; set; }
        public DelegateCommand<object> ShowGridCommand { get; set; }
        public DelegateCommand<object> SnaptoGridCommand { get; set; }
        public DelegateCommand<object> GridSettingCommand { get; set; }
        public DelegateCommand<object> ShowGlobalGuideCommand { get; set; }
        public DelegateCommand<object> ShowPageGuideCommand { get; set; }
        public DelegateCommand<object> SnaptoGuideCommand { get; set; }
        public DelegateCommand<object> LockGuidesCommand { get; set; }
        public DelegateCommand<object> CreateGuidesCommand { get; set; }
        public DelegateCommand<object> DeleteAllGuidesCommand { get; set; }
        public DelegateCommand<object> GuideSetttingCommand { get; set; }
        public DelegateCommand<object> SnaptoObjectCommand { get; set; }
        public DelegateCommand<object> ObjectSnapSettingCommand { get; set; }
        public DelegateCommand<object> SetDefaultStyleCommand { get; set; }
        public DelegateCommand<object> UnplaceFromViewCommand { get; set; }

        public DelegateCommand<ICustomLibrary> AddToLibraryCommand { get; set; }
        public DelegateCommand<object> ExportToLibraryCommand { get; set; }
        public DelegateCommand<object> ConvertToMasterCommand { get; set; }
        public DelegateCommand<object> AddLibraryCommand { get; set; }
        public DelegateCommand<object> MasterBreakAwayCommand { get; set; }
        public DelegateCommand<object> MasterLock2LocationCommand { get; set; }

        private void ImportImageCommandHandler(object obj)
        {
            ImageWidgetViewModel img = _selectionService.GetSelectedWidgets()[0] as ImageWidgetViewModel;
            if (img == null)
                return;
            img.ImportImg();
            if (img.ParentID != Guid.Empty)
            {
                UpdateGroup(img.ParentID);
            }
        }

        private void EditTextCommandHandler(object obj)
        {
            WidgetViewModBase widgetVM = _selectionService.GetSelectedWidgets()[0] as WidgetViewModBase;
            widgetVM.CanEdit = true;
        }

        private void EditListItemsCommandHandler(object obj)
        {
            WidgetViewModBase widgetVM = _selectionService.GetSelectedWidgets()[0] as WidgetViewModBase;
            if (widgetVM is ListboxWidgetViewModel)
                (widgetVM as ListboxWidgetViewModel).EditListItems();
            else if (widgetVM is DroplistWidgetViewModel)
                (widgetVM as DroplistWidgetViewModel).EditListItems();
        }

        private void ToolTipCommandHandler(object obj)
        {
            ToolTipWindow win = new ToolTipWindow();
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

        public bool CanRunDefaultStyle
        {
            get
            {
                if (_selectionService.WidgetNumber == 1)
                {
                    List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

                    WidgetViewModBase date = allSelects[0] as WidgetViewModBase;

                    if (date != null && date.IsGroup == false)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        private void DefaultStyleCommandHandler(object obj)
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            if (allSelects.Count == 1)
            {
                WidgetViewModBase widgetVM = allSelects[0] as WidgetViewModBase;

                if (widgetVM != null)
                {
                    widgetVM.SetStyleAsDefaultStyle();
                }
            }
        }

        private void PlaceFromViewCommandHandler(object obj)
        {
            List<Guid> guids = (List<Guid>)obj;

            if (guids != null && guids.Count > 0)
            {
                PlaceWidgets2View(guids);

                PlaceWidgetCommand cmd = new PlaceWidgetCommand(this, guids);
                UndoManager.Push(cmd);
            }
           
        }

        private void UnplaceFromViewCommandHandler(object obj)
        {
            List<Guid> guids = _selectionService.GetSelectedWidgetGUIDs();
            UnplaceWidgetsFromView(guids);

            UnplaceWidgetCommand cmd = new UnplaceWidgetCommand(this, guids);
            UndoManager.Push(cmd);
        }

        private void AddToLibraryCommandHandler(ICustomLibrary library)
        {
            AddToLibrary(library);
        }

        private void ExportToLibraryCommandHandler(object parameter)
        {
            ExportToLibrary();
        }

        private void AddLibraryCommandHandler(object parameter)
        {
            CreateLibrary();            
        }



        private bool CanRunCopy(object obj)
        {
            return CanRunCopyCommand;
        }
        private bool CanRunCut(object obj)
        {
            return CanRunCutCommand;
        }
        private bool CanRunPaste(object obj)
        {
            return CanRunPasteCommand;
        }
        private bool CanRunGroup(object obj)
        {
            return CanRunGroupCommand;
        }
        private bool CanRunUnGroup(object obj)
        {
            return CanRunUnGroupCommand;
        }
        private bool CanRunBringFront(object obj)
        {
            return CanRunWidgetsBringFrontCommand;
        }
        private bool CanRunSendBack(object obj)
        {
            return CanRunWidgetsBringBottomCommand;
        }
        private bool CanRunBringForward(object obj)
        {
            return CanRunWidgetsBringForwardCommand;
        }
        private bool CanRunSendBackward(object obj)
        {
            return CanRunWidgetsBringBackwardCommand;
        }
        private bool CanRunAlignLeft(object obj)
        {
            return CanRunWidgetsAlignLeftCommand;
        }
        private bool CanRunAlignCenter(object obj)
        {
            return CanRunWidgetsAlignCenterCommand;
        }
        private bool CanRunAlignRight(object obj)
        {
            return CanRunWidgetsAlignRightCommand;
        }
        private bool CanRunAlignTop(object obj)
        {
            return CanRunWidgetsAlignTopCommand;
        }
        private bool CanRunAlignMiddle(object obj)
        {
            return CanRunWidgetsAlignMiddleCommand;
        }
        private bool CanRunAlignBottom(object obj)
        {
            return CanRunWidgetsAlignBottomCommand;
        }
        private bool CanRunDistributeHoriz(object obj)
        {
            return CanRunWidgetsDistributeHorizontallyCommand;
        }
        private bool CanRunDistributeVerti(object obj)
        {
            return CanRunWidgetsDistributeVerticallyCommand;
        }

        #endregion

        #region properties
        public ObservableCollection<ContextMenuItem> ContextMenuList { get; set; }
        #endregion
    }
}
