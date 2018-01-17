using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.Helper;
using System.Windows.Controls;
using System.Windows.Documents;
using Naver.Compass.Service;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        private void InitializePage(Guid pageGID)
        {
            //IconSource = ISC.ConvertFromInvariantString(@"pack://application:,,/Images/document.png") as ImageSource;  
            _copyTime = 0;
            _copyGID = Guid.Empty;
            Title = "page";
            _pageGID = pageGID;            
            ContentId = pageGID.ToString();
            _model = new PageEditorModel(pageGID);
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler);
        }

        protected void InitializeCommonData()
        {
            InitCommandSink();
            InitEvetnManger();
            InitAdaptiveView();
            InitContextMenu();
        }

        #region Init Adaptive View
        protected void InitAdaptiveView()
        {
            // axure mode 
            this.OpenAdaptiveCommand = new DelegateCommand<object>(OpenAdaptveExecute);
            this.CheckAdaptiveCommand = new DelegateCommand<object>(CheckAdaptiveExecute);
            PagePropOffOnCommand = new DelegateCommand<object>(PagePropOffOnExecute);
            _ListEventAggregator.GetEvent<OpenPanesEvent>().Subscribe(OpenPanesEventHandler);
            //_ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler);

            _ListEventAggregator.GetEvent<LoadAdaptiveViewEvent>().Subscribe(LoadAdaptiveViewsHandler);
            _ListEventAggregator.GetEvent<OpenDialogEvent>().Subscribe(OpenAdaptiveViewSettingHandler);
            _adaptiveModel = new AdaptiveModel();

            //Current AdaptiveView GID Initial
            _curAdaptiveViewGID = Guid.Empty;
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {
                _curAdaptiveViewGID = doc.Document.AdaptiveViewSet.Base.Guid;    
            }

            //LoadAdaptiveViewsHandler(string.Empty);
            InitDeviceView();

        }
        #endregion

        #region Init Context Menu
        protected void InitContextMenu()
        {
            //_ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler);

            CutCommand = new DelegateCommand<object>(CutCommanddHandler, CanRunCut);
            CopyCommand = new DelegateCommand<object>(CopyCommanddHandler, CanRunCopy);
            PasteCommand = new DelegateCommand<object>(PasteCommanddHandler, CanRunPaste);
            ExportToImageCommand = new DelegateCommand<object>(ExportCommandHandler);
            GroupCommand = new DelegateCommand<object>(GroupCommanddHandler, CanRunGroup);
            UnGroupCommand = new DelegateCommand<object>(UnGroupCommanddHandler, CanRunUnGroup);
            BringFrontCommand = new DelegateCommand<object>(WidgetsBringFrontCommandHandler, CanRunBringFront);
            SendBackCommand = new DelegateCommand<object>(WidgetsBringBottomCommandHandler, CanRunSendBack);
            BringForwardCommand = new DelegateCommand<object>(WidgetsBringForwardCommandHandler, CanRunBringForward);
            SendBackwardCommand = new DelegateCommand<object>(WidgetsBringBackwardCommandHandler, CanRunSendBackward);
            AlignLeftCommand = new DelegateCommand<object>(WidgetsAlignLeftCommandHandler, CanRunAlignLeft);
            AlignCenterCommand = new DelegateCommand<object>(WidgetsAlignCenterCommandHandler, CanRunAlignCenter);
            AlignRightCommand = new DelegateCommand<object>(WidgetsAlignRightCommandHandler, CanRunAlignRight);
            AlignTopCommand = new DelegateCommand<object>(WidgetsAlignTopCommandHandler, CanRunAlignTop);
            AlignMiddleCommand = new DelegateCommand<object>(WidgetsAlignMiddleCommandHandler, CanRunAlignMiddle);
            AlignBottomCommand = new DelegateCommand<object>(WidgetsAlignBottomCommandHandler, CanRunAlignBottom);
            DistributeHorizCommand = new DelegateCommand<object>(WidgetsDistributeHorizontallyCommandHandler, CanRunDistributeHoriz);
            DistributeVertiCommand = new DelegateCommand<object>(WidgetsDistributeVerticallyCommandHandler, CanRunDistributeVerti);
            ImportImageCommand = new DelegateCommand<object>(ImportImageCommandHandler);
            EditCommand = new DelegateCommand<object>(EditTextCommandHandler);
            ToolTipCommand = new DelegateCommand<object>(ToolTipCommandHandler);
            ShowGridCommand = new DelegateCommand<object>(ShowGridCommandHandler);
            SnaptoGridCommand = new DelegateCommand<object>(SnaptoGridCommandHandler);
            GridSettingCommand = new DelegateCommand<object>(GridSettingCommandHandler);
            ShowGlobalGuideCommand = new DelegateCommand<object>(ShowGlobalGuidesCommandHandler);
            ShowPageGuideCommand = new DelegateCommand<object>(ShowPageGuidesCommandHandler);
            SnaptoGuideCommand = new DelegateCommand<object>(SnapToGuidesCommandHandler);
            LockGuidesCommand = new DelegateCommand<object>(LockGuidesCommandHandler);
            CreateGuidesCommand = new DelegateCommand<object>(CreateGuidesCommandHandler);
            DeleteAllGuidesCommand = new DelegateCommand<object>(DeleteAllGuidesCommandHandler);
            GuideSetttingCommand = new DelegateCommand<object>(GuidesSettingCommandHandler);
            SnaptoObjectCommand = new DelegateCommand<object>(SnapToObjectCommandHandler);
            ObjectSnapSettingCommand = new DelegateCommand<object>(ObjectSnapSettingCommandHandler);
            EditListItemsCommamd = new DelegateCommand<object>(EditListItemsCommandHandler);
            SetDefaultStyleCommand = new DelegateCommand<object>(DefaultStyleCommandHandler);
            UnplaceFromViewCommand = new DelegateCommand<object>(UnplaceFromViewCommandHandler);
            AddToLibraryCommand = new DelegateCommand<ICustomLibrary>(AddToLibraryCommandHandler);
            ExportToLibraryCommand = new DelegateCommand<object>(ExportToLibraryCommandHandler);
            ConvertToMasterCommand = new DelegateCommand<object>(ConvertToMasterCommandHandler);
            AddLibraryCommand = new DelegateCommand<object>(AddLibraryCommandHandler);

            MasterBreakAwayCommand = new DelegateCommand<object>(MasterBreakAwayCommandHandler);
            MasterLock2LocationCommand = new DelegateCommand<object>(MasterLock2LocationCommandHandler);

        }
        #endregion

        #region Init CommandSink register
        protected void InitCommandSink()
        {
            //Register the Routed Command Handler,Widgets Command
            _commandSink = new CommandSink();
            _commandSink.RegisterCommand(ApplicationCommands.Copy, param => CanRunCopyCommand, CopyCommanddHandler);
            _commandSink.RegisterCommand(ApplicationCommands.Paste, param => CanRunPasteCommand, PasteCommanddHandler);
            _commandSink.RegisterCommand(ApplicationCommands.Cut, param => CanRunCutCommand, CutCommanddHandler);
            _commandSink.RegisterCommand(ApplicationCommands.Undo, param => CanRunUndoCommand, UndoCommandHandler);
            _commandSink.RegisterCommand(ApplicationCommands.Redo, param => CanRunRedoCommand, RedoCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.Delete, param => CanRunDeleteCommand, DeleteCommanddHandler);
            _commandSink.RegisterCommand(ApplicationCommands.SelectAll, param => CanRunSelectAllCommand, SelectAllCommanddHandler);
            _commandSink.RegisterCommand(WidgetsCommands.GroupWidgets, param => CanRunGroupCommand, GroupCommanddHandler);
            _commandSink.RegisterCommand(WidgetsCommands.UngroupWidgets, param => CanRunUnGroupCommand, UnGroupCommanddHandler);
            _commandSink.RegisterCommand(WidgetsCommands.DuplicateWidgets, param => CanRunCopyCommand, DuplicateCommanddHandler);

            //Register the Routed Command Handler,Font Command
            _commandSink.RegisterCommand(TextCommands.AlignTextLeft, param => CanRunAlignTextLeftCommand, AlignTextLeftCommandHandler);
            _commandSink.RegisterCommand(TextCommands.AlignTextRight, param => CanRunAlignTextRightCommand, AlignTextRightCommandHandler);
            _commandSink.RegisterCommand(TextCommands.AlignTextCenter, param => CanRunAlignTextCenterCommand, AlignTextCenterCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.AlignJustify, param => CanRunAlignJustifyCommand, AlignJustifyCommandHandler);
            _commandSink.RegisterCommand(TextCommands.AlignTextTop, param => CanRunAlignTopCommand, AlignTopCommandHandler);
            _commandSink.RegisterCommand(TextCommands.AlignTextBottom, param => CanRunAlignBottomCommand, AlignBottomCommandHandler);
            _commandSink.RegisterCommand(TextCommands.AlignTextMiddle, param => CanRunAlignTextMiddleCommand, AlignTextMiddleCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Family, param => CanRunFontFamilyCommand, FontFamilyCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Size, param => CanRunFontSizeCommand, FontSizeCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.IncreaseFontSize, param => CanRunFontSizeCommand, FontSizeIncreaseCommandHandler);
            _commandSink.RegisterCommand(EditingCommands.DecreaseFontSize, param => CanRunFontSizeCommand, FontSizeDecreaseCommandHandler);
            _commandSink.RegisterCommand(TextCommands.UpDownCaseHotKey, param => CanRunUpDownCase, TextUpDownCaseHandler);
            _commandSink.RegisterCommand(FontCommands.Bold, param => CanRunFontBoldCommand, FontBoldCommandHandler);
            _commandSink.RegisterCommand(TextCommands.Strikethrough, param => CanRunFontStrikeThroughCommand, FontStrikeThroughCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Underline, param => CanRunFontUnderlineCommand, FontUnderlineCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Bullet, param => CanRunBulletStyleCommand, BulletStyleCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Color, param => CanRunFontColorCommand, FontColorCommandHandler);
            _commandSink.RegisterCommand(FontCommands.Italic, param => CanRunFontItalicCommand, FontItalicCommandHandler);

            //Register the Routed Command Handler,Border Command
            _commandSink.RegisterCommand(BorderCommands.BorderLineColor, param => CanRunBorderLineColorCommand, BorderLineColorCommandHandler);
            _commandSink.RegisterCommand(BorderCommands.LineArrowStyle, param => CanRunLineArrowStyleCommand, LineArrowStyleCommandHandler);
            _commandSink.RegisterCommand(BorderCommands.BorderLinePattern, param => CanRunBorderLinePatternCommand, BorderLinePatternCommandHandler);
            _commandSink.RegisterCommand(BorderCommands.BorderLineThinck, param => CanRunBorderLineThinckCommand, BorderLineThinckCommandHandler);
            _commandSink.RegisterCommand(BorderCommands.BackGround, param => CanRunBackGroundCommand, ChangeBackGroundCommandHandler);

            _commandSink.RegisterCommand(WidgetsCommands.WidgetsBringFront, param => CanRunWidgetsBringFrontCommand, WidgetsBringFrontCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsBringForward, param => CanRunWidgetsBringForwardCommand, WidgetsBringForwardCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsBringBackward, param => CanRunWidgetsBringBackwardCommand, WidgetsBringBackwardCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsBringBottom, param => CanRunWidgetsBringBottomCommand, WidgetsBringBottomCommandHandler);

            _commandSink.RegisterCommand(WidgetsCommands.WidgetsAlignLeft, param => CanRunWidgetsAlignLeftCommand, WidgetsAlignLeftCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsAlignRight, param => CanRunWidgetsAlignRightCommand, WidgetsAlignRightCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsAlignCenter, param => CanRunWidgetsAlignCenterCommand, WidgetsAlignCenterCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsAlignTop, param => CanRunWidgetsAlignTopCommand, WidgetsAlignTopCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsAlignMiddle, param => CanRunWidgetsAlignMiddleCommand, WidgetsAlignMiddleCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsAlignBottom, param => CanRunWidgetsAlignBottomCommand, WidgetsAlignBottomCommandHandler);

            _commandSink.RegisterCommand(WidgetsCommands.WidgetsIncreaseWidth, param => CanRunWidgetSizeAdjustCommand, WidgetsIncreaseWidthCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsIncreaseHeight, param => CanRunWidgetSizeAdjustCommand, WidgetsIncreaseHeightCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsDecreaseWidth, param => CanRunWidgetSizeAdjustCommand, WidgetsDecreaseWidthCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsDecreaseHeight, param => CanRunWidgetSizeAdjustCommand, WidgetsDecreaseHeightCommandHandler);

            _commandSink.RegisterCommand(WidgetsCommands.WidgetsDistributeHorizontally, param => CanRunWidgetsDistributeHorizontallyCommand, WidgetsDistributeHorizontallyCommandHandler);
            _commandSink.RegisterCommand(WidgetsCommands.WidgetsDistributeVertically, param => CanRunWidgetsDistributeVerticallyCommand, WidgetsDistributeVerticallyCommandHandler);

            //Register the Routed Command Handler,Widget Propery Command
            _commandSink.RegisterCommand(WidgetPropertyCommands.Left, param => CanRunWidgetsLeftChangeCommand, WidgetsLeftChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Top, param => CanRunWidgetsTopChangeCommand, WidgetsTopChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Width, param => CanRunWidgetsWidthChangeCommand, WidgetsWidthChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Height, param => CanRunWidgetsHeightChangeCommand, WidgetsHeightChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Name, param => CanRunWidgetsNameChangeCommand, WidgetsNameChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Rotate, param => CanRunWidgetsRotateChangeCommand, WidgetsRotateChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.CornerRadius, param => CanRunWidgetsCornerRadiusChangeCommand, WidgetsCornerRadiusChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.TextRotate, param => CanRunWidgetsTextRotateChangeCommand, WidgetsTextRotateChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Opacity, param => CanRunWidgetsOpacityChangeCommand, WidgetsOpacityChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Hide, param => CanRunWidgetsHideChangeCommand, WidgetsHideChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.IsFixed, param => CanRunWidgetsIsFixedChangeCommand, WidgetsIsFixedChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Tooltip, param => CanRunWidgetsTooltipChangeCommand, WidgetsTooltipChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.ImportImage, param => CanRunWidgetsImportImageChangeCommand, WidgetsImportImageChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Slice, param => CanRunWidgetsSliceCommand, WidgetsSliceCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Crop, param => CanRunWidgetsCropCommand, WidgetsCropCommandHandler);

            //Hamburger menu command handler
            _commandSink.RegisterCommand(WidgetPropertyCommands.MenuPageLeft, param => CanRunWidgetsLeftChangeCommand, MenuPageLeftChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.MenuPageTop, param => CanRunWidgetsTopChangeCommand, MenuPageTopChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.MenuPageWidth, param => CanRunWidgetsWidthChangeCommand, MenuPageWidthChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.MenuPageHeight, param => CanRunWidgetsHeightChangeCommand, MenuPageHeightChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.MenuPageHide, param => CanRunWidgetsHideChangeCommand, MenuPageHideChangeCommandHandler);


            _commandSink.RegisterCommand(WidgetPropertyCommands.Enable, param => CanRunWidgetsEnableChangeCommand, WidgetsEnableChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.ShowSelect, param => CanRunWidgetsShowSelectChangeCommand, WidgetsShowSelectChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.ButtonAlign, param => CanRunWidgetsButtonAlignChangeCommand, WidgetsButtonAlignCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.RadioGroup, param => CanRunWidgetsButtonAlignChangeCommand, WidgetsRadioGroupSetCommandHandler);

            _commandSink.RegisterCommand(WidgetPropertyCommands.HideBorder, param => CanRunWidgetsEnableChangeCommand, WidgetsHideBorderCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.ReadOnly, param => CanRunWidgetsEnableChangeCommand, WidgetsReadOnlyCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.HintText, param => CanRunWidgetsEnableChangeCommand, WidgetsHintTextCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.MaxLength, param => CanRunWidgetsEnableChangeCommand, WidgetsMaxLengthCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.TextFieldType, param => CanRunWidgetsEnableChangeCommand, WidgetsTextFieldTypeCommandHandler);

            //Register the Toolbar Location Command
            _commandSink.RegisterCommand(WidgetPropertyCommands.AllLeft, param => CanRunWidgetsLeftChangeCommand, WidgetsAllLeftChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.AllTop, param => CanRunWidgetsTopChangeCommand, WidgetsAllTopChangeCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Lock, param => CanRunWidgetsLockCommand, WidgetsLockCommandHandler);
            _commandSink.RegisterCommand(WidgetPropertyCommands.Unlock, param => CanRunWidgetsUnlockCommand, WidgetsUnlockCommandHandler);

            //Register Grid and Guide Command
            _commandSink.RegisterCommand(GridGuideCommands.ShowGrid, param => CanRunGridGuideCommand, ShowGridCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.SnapToGrid, param => CanRunGridGuideCommand, SnaptoGridCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.GridSetting, param => CanRunGridGuideCommand, GridSettingCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.ShowGlobalGuides, param => CanRunGridGuideCommand, ShowGlobalGuidesCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.ShowPageGuides, param => CanRunGridGuideCommand, ShowPageGuidesCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.SnapToGuides, param => CanRunGridGuideCommand, SnapToGuidesCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.LockGuides, param => CanRunGridGuideCommand, LockGuidesCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.CreateGuides, param => CanRunCreateGuideCommand, CreateGuidesCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.DeleteAllGuides, param => CanRunCreateGuideCommand, DeleteAllGuidesCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.GuideSetting, param => CanRunGridGuideCommand, GuidesSettingCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.SnapToObject, param => CanRunGridGuideCommand, SnapToObjectCommandHandler);
            _commandSink.RegisterCommand(GridGuideCommands.ObjectSnapSetting, param => CanRunGridGuideCommand, ObjectSnapSettingCommandHandler);
            _commandSink.RegisterCommand(AppCommands.DefaultStyle, param => CanRunDefaultStyle, DefaultStyleCommandHandler);

            //Flick pannel command handler
            _commandSink.RegisterCommand(FlickCommands.ShowArrow, param => CanRunFlickCommand, FlickShowArrowCommandHandler);
            _commandSink.RegisterCommand(FlickCommands.Circuler, param => CanRunFlickCommand, FlickCirculerCommandHandler);
            _commandSink.RegisterCommand(FlickCommands.Automatic, param => CanRunFlickCommand, FlickAutomaticCommandHandler);
            _commandSink.RegisterCommand(FlickCommands.StartPage, param => CanRunFlickCommand, FlickStartPageCommandHandler);
            _commandSink.RegisterCommand(FlickCommands.Navigation, param => CanRunFlickCommand, FlickNavigationCommandHandler);
            _commandSink.RegisterCommand(FlickCommands.ViewMode, param => CanRunFlickCommand, FlickViewModeCommandHandler);
            _commandSink.RegisterCommand(FlickCommands.PanelWidth, param => CanRunFlickCommand, FlickPanelWidthCommandHander);
            _commandSink.RegisterCommand(FlickCommands.LineWidth, param => CanRunFlickCommand, FlickLineWidthCommandHander);

            _commandSink.RegisterCommand(HanburgerCommands.HideStyle, param => CanRunHamburgerCommand, HamburgerHideStyleCommandHandler);
            _commandSink.RegisterCommand(ToastCommands.ExposureTime, param => CanRunToastCommand, ToastExposureTimeCommandHandler);
            _commandSink.RegisterCommand(ToastCommands.CloseSetting, param => CanRunToastCommand, ToastCloseSettingCommandHandler);
            _commandSink.RegisterCommand(ToastCommands.DisplayPosition, param => CanRunToastCommand, ToastDisplayPositionCommandHandler);
        }
        #endregion

        #region Init CommandSink register
        protected void InitEvetnManger()
        {
            //Selection Service	    
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();

            //Progress bar
            _busyIndicator = ServiceLocator.Current.GetInstance<BusyIndicatorContext>();

            _ListEventAggregator.GetEvent<EditTooltipEvent>().Subscribe(WidgetsTooltipChangeCommandHandler);
            _ListEventAggregator.GetEvent<UpdateGridGuide>().Subscribe(UpdateGridGuideHandler);

            _ListEventAggregator.GetEvent<FormatPaintEvent>().Subscribe(FormatPaintCommandHandler);
            _ListEventAggregator.GetEvent<CancelFormatPaintEvent>().Subscribe(CancelFormatPaintCommandHandler);
            _ListEventAggregator.GetEvent<MouseOverInteractionObject>().Subscribe(MouseOverInteractionObjectHandler);

            _ListEventAggregator.GetEvent<AddMasterEvent>().Subscribe(AddMasterEventCommandHandler);
            _ListEventAggregator.GetEvent<DeleteMasterPageEvent>().Subscribe(DeletMasterEventCommandHandler);            
        }
        #endregion

    }
}
