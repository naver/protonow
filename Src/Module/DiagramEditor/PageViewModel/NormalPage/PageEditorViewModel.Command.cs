using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;
using Naver.Compass.Common;
using Naver.Compass.Service;
using System.Windows.Resources;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {

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

        #region Helper

        protected void CreatePropertyChangeUndoCommand(CompositeCommand cmds, WidgetViewModBase wdgItem, string propertyName, object oldValue, object newValue, bool tryToSelect = true)
        {
            if (wdgItem.IsGroup)
            {
                // GroupViewModel will be created every time when creating a group based on the same IGroup,
                // So we can not save GroupViewModel in undo/redo stack
                GroupViewModel groupVM = wdgItem as GroupViewModel;
                var EligibleList = groupVM.GetEligibleWidgetsByProperty(propertyName);
                foreach (WidgetViewModBase childWdgItem in EligibleList)
                {
                    object childOldValue = childWdgItem.GetType().GetProperty(propertyName).GetValue(childWdgItem, null);
                    if (!childOldValue.Equals(newValue))
                    {
                        PropertyChangeCommand cmd = new PropertyChangeCommand(childWdgItem, propertyName, childOldValue, newValue);
                        cmds.AddCommand(cmd);
                    }
                }
            }
            else
            {
                if (!oldValue.Equals(newValue))
                {
                    PropertyChangeCommand cmd = new PropertyChangeCommand(wdgItem, propertyName, oldValue, newValue);
                    cmds.AddCommand(cmd);
                }
            }
        }

        protected void CreatePropertyDeltaChangeUndoCommand(CompositeCommand cmds, WidgetViewModBase wdgItem, string propertyName, double oldValue, double newValue)
        {
            if (wdgItem.IsGroup)
            {
                // GroupViewModel will be created every time when creating a group based on the same IGroup,
                // So we can not save GroupViewModel in undo/reo stack
                GroupViewModel groupVM = wdgItem as GroupViewModel;
                double delta = newValue - oldValue;
                if (delta != 0)
                {
                    foreach (WidgetViewModBase childWdgItem in groupVM.WidgetChildren)
                    {
                        object childOldValue = childWdgItem.GetType().GetProperty(propertyName).GetValue(childWdgItem, null);
                        object childNewValue = (double)childOldValue + delta;
                        PropertyChangeCommand cmd = new PropertyChangeCommand(childWdgItem, propertyName, childOldValue, childNewValue);
                        cmds.AddCommand(cmd);
                    }
                }
            }
            else
            {
                if (oldValue != newValue)
                {
                    PropertyChangeCommand cmd = new PropertyChangeCommand(wdgItem, propertyName, oldValue, newValue);
                    cmds.AddCommand(cmd);
                }
            }
        }

        protected void CreateWidthChangeUndoCommand(CompositeCommand cmds, WidgetViewModBase wdgItem, double oldValue, double newValue)
        {
            // See GroupViewModel ItemWidth property set method to understand below codes
            if (wdgItem.IsGroup)
            {
                // GroupViewModel will be created every time when creating a group based on the same IGroup,
                // So we can not save GroupViewModel in undo/reo stack
                GroupViewModel groupVM = wdgItem as GroupViewModel;

                double scale = newValue / oldValue;
                double groupLeft = groupVM.Left;
                foreach (WidgetViewModBase childWdgItem in groupVM.WidgetChildren)
                {
                    double delta = (childWdgItem.Left - groupLeft) * (scale - 1);

                    if (delta != 0)
                    {
                        double childNewValue = childWdgItem.Left + delta;
                        PropertyChangeCommand cmd = new PropertyChangeCommand(childWdgItem, "Left", childWdgItem.Left, childNewValue);
                        cmds.AddCommand(cmd);
                    }

                    if (scale != 1)
                    {
                        double childNewValue = childWdgItem.ItemWidth * scale;
                        PropertyChangeCommand cmd = new PropertyChangeCommand(childWdgItem, "ItemWidth", childWdgItem.ItemWidth, childNewValue);
                        cmds.AddCommand(cmd);
                    }
                }
            }
            else
            {
                if (oldValue != newValue)
                {
                    PropertyChangeCommand cmd = new PropertyChangeCommand(wdgItem, "ItemWidth", oldValue, newValue);
                    cmds.AddCommand(cmd);
                }
            }
        }

        protected void CreateHeightChangeUndoCommand(CompositeCommand cmds, WidgetViewModBase wdgItem, double oldValue, double newValue)
        {
            // See GroupViewModel ItemHeight property set method to understand below codes
            if (wdgItem.IsGroup)
            {
                // GroupViewModel will be created every time when creating a group based on the same IGroup,
                // So we can not save GroupViewModel in undo/reo stack
                GroupViewModel groupVM = wdgItem as GroupViewModel;

                double scale = newValue / oldValue;
                double grouptop = groupVM.Top;
                foreach (WidgetViewModBase childWdgItem in groupVM.WidgetChildren)
                {
                    double delta = (childWdgItem.Top - grouptop) * (scale - 1);

                    if (delta != 0)
                    {
                        double childNewValue = childWdgItem.Top + delta;
                        PropertyChangeCommand cmd = new PropertyChangeCommand(childWdgItem, "Top", childWdgItem.Top, childNewValue);
                        cmds.AddCommand(cmd);
                    }

                    if (scale != 1)
                    {
                        double childNewValue = childWdgItem.ItemHeight * scale;
                        PropertyChangeCommand cmd = new PropertyChangeCommand(childWdgItem, "ItemHeight", childWdgItem.ItemHeight, childNewValue);
                        cmds.AddCommand(cmd);
                    }
                }
            }
            else
            {
                if (oldValue != newValue)
                {
                    PropertyChangeCommand cmd = new PropertyChangeCommand(wdgItem, "ItemHeight", oldValue, newValue);
                    cmds.AddCommand(cmd);
                }
            }
        }

        protected void PushToUndoStack(CompositeCommand cmds, List<Guid> updateGroupList = null)
        {
            if (updateGroupList != null && updateGroupList.Count > 0)
            {
                UpdateGroupCommand cmd = new UpdateGroupCommand(this, updateGroupList);
                cmds.AddCommand(cmd);
            }
            
            if (cmds.Count > 0)
            {
                List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();
                cmds.AddCommand(new SelectCommand(this, allSelects));

                cmds.DeselectAllWidgetsFirst();
                _undoManager.Push(cmds);
            }
        }

        protected void widgetUpDownCase(CompositeCommand cmds, List<WidgetViewModBase> listwidget)
        {

            foreach (WidgetViewModBase WdgItem in listwidget)
            {
                if (WdgItem.IsGroup)
                {
                    widgetUpDownCase(cmds, ((GroupViewModel)WdgItem).WidgetChildren);
                }
                else if (WdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = WdgItem.vTextContent;

                    ((WidgetMultiTextViewModBase)WdgItem).OnUpDownCaseCommand();

                    CreatePropertyChangeUndoCommand(cmds, WdgItem, "vTextContent", oldValue, WdgItem.vTextContent);
                }
                //else
                //{
                //    double newValue = FontSizeEnumerator.GetValue(WdgItem.vFontSize, bAdd);

                //    CreatePropertyChangeUndoCommand(cmds, WdgItem, "vFontSize", WdgItem.vFontSize, newValue);

                //    WdgItem.vFontSize = newValue;

                //}
            }
        }

        protected void widgetListFontSizeChange(CompositeCommand cmds, List<WidgetViewModBase> listwidget, bool bAdd)
        {

            foreach (WidgetViewModBase WdgItem in listwidget)
            {
                if (WdgItem.IsGroup)
                {
                    widgetListFontSizeChange(cmds, ((GroupViewModel)WdgItem).WidgetChildren, bAdd);

                    ((GroupViewModel)WdgItem).RefreshProperty("vFontSize");
                }
                else  if (WdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = WdgItem.vTextContent;

                    ((WidgetMultiTextViewModBase)WdgItem).OnFontSizeInDeCrease(bAdd);

                    CreatePropertyChangeUndoCommand(cmds, WdgItem, "vTextContent", oldValue, WdgItem.vTextContent);
                }
                else
                {
                    double newValue = FontSizeEnumerator.GetValue(WdgItem.vFontSize, bAdd);

                    CreatePropertyChangeUndoCommand(cmds, WdgItem, "vFontSize", WdgItem.vFontSize, newValue);

                    WdgItem.vFontSize = newValue;

                }
            }
        }

        #endregion

        #region Global Routed Command Handler

        #region Operation Routed Command Handler
        private void OnPaintFormat(object para)
        {

            FormatPainterService service = para as FormatPainterService;


            PaintFormat format = service.GetPaintFormat();
            if (format != null)
            {
                List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

                if (allSelects.Count == 1)
                {
                    WidgetViewModBase wdg = allSelects[0] as WidgetViewModBase;
                    if (wdg != null)
                    {
                        CompositeCommand cmds = new CompositeCommand();

                        if (!(format.BackColor.FillType == ColorFillType.Gradient && !wdg.IsSupportGradientBackground))
                        {
                            CreatePropertyChangeUndoCommand(cmds, wdg, "vBackgroundColor", wdg.vBackgroundColor, format.BackColor);
                            wdg.vBackgroundColor = format.BackColor;
                        }


                        if (wdg.IsSupportBorber)
                        {
                            if (!(format.BorderColor.FillType == ColorFillType.Gradient && !wdg.IsSupportGradientBorderline))
                            {
                                CreatePropertyChangeUndoCommand(cmds, wdg, "vBorderLineColor", wdg.vBorderLineColor, format.BorderColor);
                                wdg.vBorderLineColor = format.BorderColor;
                            }

                            CreatePropertyChangeUndoCommand(cmds, wdg, "vBorderLinethinck", wdg.vBorderLinethinck, format.LineWidth);
                            wdg.vBorderLinethinck = format.LineWidth;

                            CreatePropertyChangeUndoCommand(cmds, wdg, "vBorderlineStyle", wdg.vBorderlineStyle, format.Linestyle);
                            wdg.vBorderlineStyle = format.Linestyle;
                        }

                        if (wdg.IsSupportText)
                        {
                            CreatePropertyChangeUndoCommand(cmds, wdg, "vFontFamily", wdg.vFontFamily, format.Fontfamily);
                            wdg.vFontFamily = format.Fontfamily;

                            CreatePropertyChangeUndoCommand(cmds, wdg, "vFontSize", wdg.vFontSize, format.FontSize);
                            wdg.vFontSize = format.FontSize;

                            CreatePropertyChangeUndoCommand(cmds, wdg, "vFontColor", wdg.vFontColor, format.Fontcolor);
                            wdg.vFontColor = format.Fontcolor;

                            CreatePropertyChangeUndoCommand(cmds, wdg, "vFontBold", wdg.vFontBold, format.IsBold);
                            wdg.vFontBold = format.IsBold;

                            CreatePropertyChangeUndoCommand(cmds, wdg, "vFontItalic", wdg.vFontItalic, format.IsItalic);
                            wdg.vFontItalic = format.IsItalic;

                            CreatePropertyChangeUndoCommand(cmds, wdg, "vFontUnderLine", wdg.vFontUnderLine, format.IsUnderline);
                            wdg.vFontUnderLine = format.IsUnderline;

                            CreatePropertyChangeUndoCommand(cmds, wdg, "vFontStrickeThrough", wdg.vFontStrickeThrough, format.IsStrikeThough);
                            wdg.vFontStrickeThrough = format.IsStrikeThough;
                        }

                        PushToUndoStack(cmds);
                    }
                }
            }
        }

        //Command Duplicated,20150424
        private void DuplicateCommanddHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            DuplicateWidgets(parameter);
        }
        public void DuplicateWidgets(object parameter)
        {
            //invalid operation
            if (_selectionService.WidgetNumber <= 0)
            {
                return;
            }

            //implement copy operation
            ISerializeWriter serializeWriter = _document.CreateSerializeWriter(_curAdaptiveViewGID);
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                if (wdgItem is GroupViewModel)
                {
                    IGroup group = (wdgItem as GroupViewModel).ExternalGroup;
                    if (group != null)
                    {
                        serializeWriter.AddGroup(group);
                    }
                }
                else if (wdgItem.WidgetModel != null)
                {
                    if (wdgItem.WidgetModel != null)
                    {
                        wdgItem.WidgetModel.SerializeObject(serializeWriter);
                    }
                }
            }


            if (serializeWriter.WidgetList.Count <= 0 &&
                serializeWriter.GroupList.Count<=0 && serializeWriter.MasterList.Count<=0)
            {
                return;
            }
            Stream stream = serializeWriter.WriteToStream();
            if (stream == null)
            {
                return;
            }

            //Implement paste operation
            stream.Position = 0;
            IObjectContainer container = _model.DeSerializeData2Dom(stream);
            
            //int backupCopyTime = _copyTime;
            //_copyTime = 1;
            //AddObjects2Page(container, _curAdaptiveViewGID,parameter,true);
            //_copyTime = backupCopyTime;
            if (parameter != null)
            {
                AddObjects2Page2(container, _curAdaptiveViewGID, parameter, false);
            }
            else
            {
                AddObjects2Page2(container, _curAdaptiveViewGID, new Point(20, 20), false);
            }

        }


        //Command Copy,20140306
        private void CopyCommanddHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            OnCopy2Clipboard(parameter);
        }
        private void OnCopy2Clipboard(object parameter)
        {
            //implement copy operation
            if (_selectionService.WidgetNumber <= 0)
            {
                return;
            }

            //implement copy operation
            List<Guid> historyItems = new List<Guid>();
            ISerializeWriter serializeWriter = _document.CreateSerializeWriter(_curAdaptiveViewGID);
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                historyItems.Add(wdgItem.widgetGID);
                if (wdgItem is GroupViewModel)
                {
                    IGroup group = (wdgItem as GroupViewModel).ExternalGroup;
                    if (group != null)
                    {
                        serializeWriter.AddGroup(group);
                    }
                }
                else if (wdgItem.WidgetModel != null)
                {
                    if (wdgItem.WidgetModel != null)
                    {
                        wdgItem.WidgetModel.SerializeObject(serializeWriter);
                    }
                }
            }

            //Serialize all Widgets and Groups
            try
            {
                Clipboard.Clear();
            }
            catch (System.Exception ex)
            {
                return;
            }

            if (serializeWriter.WidgetList.Count <= 0
                && serializeWriter.GroupList.Count <= 0 && serializeWriter.MasterList.Count<=0)
            {
                return;
            }



            try
            {
                Stream stream = serializeWriter.WriteToStream();
                if (stream == null)
                {
                    return;
                }

                //Clipboard operation
                var data = new DataObject();
                _copyGID = Guid.NewGuid();
                data.SetData(@"ProtoNowCopyID", _copyGID);
                data.SetData(@"ProtoNowAdaptiveID", _curAdaptiveViewGID);
                data.SetData(@"ProtoNowWidgets", stream);

                //Copy to Clipboard
                Clipboard.SetDataObject(data);
                //Clipboard.SetDataObject(data, true);
            }
            catch (System.Exception ex)
            {
                return;
            }

            _copyList.Clear();
            _copyTime = 0;
            _copyList.Add(historyItems);

        }
        public bool CanRunCopyCommand
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

        //Command Paste,20140306
        private void PasteCommanddHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }

            try
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                {
                    PasteText(parameter);
                }
                else if (Clipboard.ContainsImage())
                {
                    PasteImage(parameter);
                }
                else if (Clipboard.ContainsData(@"ProtoNowWidgets"))
                {
                    _selectionService.AllowWdgPropertyNotify(false);
                    PasteAllWidgets(parameter);
                    _selectionService.AllowWdgPropertyNotify(true);
                    _selectionService.UpdateSelectionNotify();
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            finally
            {
                _selectionService.AllowWdgPropertyNotify(true);
            }
        }
        public bool CanRunPasteCommand
        {
            get
            {
                try
                {
                    if (Clipboard.ContainsImage()
                        || Clipboard.ContainsText(TextDataFormat.Text)
                        || (Clipboard.ContainsData(@"ProtoNowWidgets")))
                    {
                        return true;
                    }
                    return false;
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        private void ExportCommandHandler(object parameter)
        {
            if (0 == _selectionService.WidgetNumber)
            {
                ExportPage2Image();
            }
            else
            {
                ExportObj2Image();
            }
        }
        private System.Windows.Point GetCenterOfCanvasOffset(double wX, double wY, double offsetx, double offsety)
        {
            System.Windows.Point rPoint = new System.Windows.Point();

            rPoint.X = _displayX + _displayWidth / 2 - wX / 2 + offsetx;
            rPoint.Y = _displayY + _displayHeight / 2 - wY / 2 + offsety;


            return rPoint;
        }
        private System.Windows.Point GetCenterOfCanvas(double wX, double wY)
        {
            System.Windows.Point rPoint = new System.Windows.Point();
            rPoint.X = _displayX + _displayWidth / 2 - wX / 2;
            rPoint.Y = _displayY + _displayHeight / 2 - wY / 2;
            return rPoint;
        }
        private void PasteText(object parameter)
        {
            try
            {
                string sText = Clipboard.GetText(TextDataFormat.UnicodeText);
                System.Windows.Point pt = GetCenterOfCanvas(200, 100);
                WidgetViewModBase wdg = AddWidgetItem(WidgetType.Shape, ShapeType.Paragraph, pt.X, pt.Y, 200, 100);
                LabelWidgetViewModel Labelwdg = wdg as LabelWidgetViewModel;

                if (Labelwdg != null)
                {
                    Labelwdg.Input_SimpleTextContent = sText;
                }
                IsDirty = true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
        }
        private void PasteImage(object parameter)
        {
            try
            {
                BitmapSource bitmap = Clipboard.GetImage();
                System.Windows.Point pt = GetCenterOfCanvas(bitmap.PixelWidth, bitmap.PixelHeight);
                WidgetViewModBase wdg = AddWidgetItem(WidgetType.Image, ShapeType.None, pt.X, pt.Y, 120, 60);
                ImageWidgetViewModel imgWdg = wdg as ImageWidgetViewModel;
                if (imgWdg != null)
                {
                    imgWdg.ImportImg(bitmap);
                    IsDirty = true;
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show(GlobalData.FindResource("Warn_Image_Too_Large"),
                                    GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PasteAllWidgets(object parameter)
        {
            Guid CopyAdaptiveID = Guid.Empty;
            //Get and arrange the Copy ID;
            object srcData = Clipboard.GetData(@"ProtoNowCopyID");
            if (srcData == null)
            {
                _copyGID = Guid.Empty;
                _copyTime = 0;
            }
            else
            {
                string sID, cID;
                try
                {
                    cID = Clipboard.GetData(@"ProtoNowCopyID").ToString();
                    sID = Clipboard.GetData(@"ProtoNowAdaptiveID").ToString();
                }
                catch (System.Exception ex)
                {
                    return;
                }

                if (string.IsNullOrEmpty(cID))
                {
                    return;
                }
                Guid CID = Guid.Parse(cID);

                if (string.IsNullOrEmpty(sID))
                {
                    CopyAdaptiveID = Guid.Empty;
                }
                else
                {
                    CopyAdaptiveID = Guid.Parse(sID);
                }

                if (CID == _copyGID)
                {
                    //_copyTime += 1;
                    int nNum=_copyList.Count();
                    if (nNum < 1)
                    {
                        _copyGID = CID;
                        _copyTime = 0;
                        _copyList.Clear();
                    }
                    else
                    {   bool bIsAnyDelete = false;
                        _copyTime = _copyList.Count();
                        for (int i = 0; i < nNum; i++)
                        {                           
                            List<Guid> hisItem = _copyList[i];
                            foreach (Guid gid in hisItem)
                            {
                                var node=items.FirstOrDefault(x => x.widgetGID == gid);
                                if(node!=null)
                                {
                                    bIsAnyDelete = false;
                                    break; 
                                }
                                bIsAnyDelete = true;
                            }

                            if(bIsAnyDelete==true)
                            {
                                _copyTime = i;
                                break;
                            }
                        }

                        if (bIsAnyDelete == true )
                        {
                            _copyList.RemoveRange(_copyTime, nNum-_copyTime);
                        }

                    }                              
                }
                else
                {
                    _copyGID = CID;
                    _copyTime = 0;
                    _copyList.Clear();
                }
            }

            //De-Serialize all Widgets and Groups from clipboard
            Stream SourceStream;
            try
            {
                SourceStream = Clipboard.GetData(@"ProtoNowWidgets") as Stream;
            }
            catch (System.Exception ex)
            {
                SourceStream = null;
            }

            if (SourceStream == null)
            {
                return;
            }

            ISerializeReader reader = _document.CreateSerializeReader(SourceStream);
            if (!CanPasteWidgets(reader))
            {
                return;
            }

            if (WantToPlaceWidgets(reader))
            {
                // User wants to place the widgets, so just return and don't paste any widgets.
                return;
            }
            
            SourceStream.Position = 0;
            IObjectContainer container = _model.DeSerializeData2Dom(SourceStream);
            PasteObjects2Page(container, CopyAdaptiveID, parameter);            
        }

        private void PasteObjects2Page(IObjectContainer container, Guid adaptiveGuid, Object posOffset)
        {
            List<WidgetViewModBase> AllVms = AddObjects2Page2(container, adaptiveGuid, posOffset, true);
            //record the copy/paste history for paste location setting
            if (AllVms != null)
            {
                RecordCopyHistory(AllVms);
            }
        }

        //record the copy/paste history
        private void RecordCopyHistory(List<WidgetViewModBase> AllVms)
        {
            List<Guid> historyItems = new List<Guid>();
            foreach (WidgetViewModBase it in AllVms)
            {
                if (it.RealParentGroupGID == Guid.Empty)
                {
                    historyItems.Add(it.WidgetID);
                }
            }
            _copyList.Add(historyItems);
        }

        protected virtual bool CanPasteWidgets(ISerializeReader reader)
        {
            return true;
        }

        protected bool WantToPlaceWidgets(ISerializeReader reader)
        {
            try
            {
                ReadOnlyCollection<Guid> widgetGuidList = reader.PeekWidgetGuidList();
                List<Guid> toPlaceWidgetGuids = new List<Guid>();
                foreach (Guid guid in widgetGuidList)
                {
                    if (_model.ActivePageView != null && _model.ActivePageView.ParentPage != null)
                    {
                        // If current page contains this widget, but it isn't placed on current page view,
                        // pop up a dialog and let user choose to place or paste it.
                        if (_model.ActivePageView.ParentPage.Widgets.Contains(guid)
                            && !_model.ActivePageView.Widgets.Contains(guid))
                        {
                            toPlaceWidgetGuids.Add(guid);
                        }
                    }
                }

                if (toPlaceWidgetGuids.Count > 0)
                {
                    // Pop up a dialog and let user choose to place or paste it.
                    PlacePasteWindow win = new PlacePasteWindow();
                    win.Owner = Application.Current.MainWindow;

                    bool? bRValue = win.ShowDialog(); //Close/Enter, the result is false, which means to Paste.
                    if ((bool)bRValue)
                    {
                        // User wants to paste a new one.
                        return false;
                    }
                    else
                    {
                        // User wants to place widgets
                        CompositeCommand cmds = new CompositeCommand();
                        PlaceWidgetCommand cmd = new PlaceWidgetCommand(this, toPlaceWidgetGuids);

                        PlaceWidgets2View(toPlaceWidgetGuids);

                        cmds.AddCommand(cmd);
                        UndoManager.Push(cmds);

                        return true;
                    }
                }

            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return false;
            }

            return false;
        }

        private WidgetViewModBase PasteWidgetItem(IWidget newItem, int Zbase, object positionOffset = null)
        {
            Point offset;
            if(positionOffset==null)
            {
                offset = new Point(20, 20);
            }
            else
            {
                offset = (Point)positionOffset;
            }
            
            IWidgetStyle _style = newItem.GetWidgetStyle(_curAdaptiveViewGID);
            if (_style == null)
            {
                _style = newItem.WidgetStyle;
            }

            if (newItem.WidgetType == WidgetType.HamburgerMenu)
            {
                IWidgetStyle _btnStyle = (newItem as IHamburgerMenu).MenuButton.GetWidgetStyle(_curAdaptiveViewGID);
                _btnStyle.X += offset.X * _copyTime;
                _btnStyle.Y += offset.Y * _copyTime;
            }
            else
            {
                _style.X += offset.X * _copyTime;
                _style.Y += offset.Y * _copyTime;
            }

            _style.Z += Zbase;
            return InsertWidget2Canvas(newItem);
        }
        private GroupViewModel PasteGroupItem(IGroup newGroup, int Zbase, object positionOffset = null)
        {
            return InsertGrou2pCanvas(newGroup, Zbase, positionOffset);
        }


        //Command Cut,20140306
        private void CutCommanddHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }

            //Initial Check
            if (_selectionService.WidgetNumber <= 0)
            {
                return;
            }

            //Implement copy operation
            OnCopy2Clipboard(null);
            _copyGID = Guid.Empty;


            //Implement Delete operation
            RemoveSelectedWidget();

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

        private void SelectAllCommanddHandler(object para)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            OnSelectAll(null);
        }
        public bool CanRunSelectAllCommand
        {
            get
            {
                return Items.Count > 0;
            }
        }

        //Command Delete,20140306
        private void DeleteCommanddHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
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

        //Command Group,20140402
        private void GroupCommanddHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }

            //OnItemRemoved(parameter);
            List<Guid> gids = _selectionService.GetSelectedWidgetGUIDs();
            if (gids.Count <= 1)
            {
                return;
            }
  
            //render the group
            IGroup group = _model.CreateGroup(gids);

            CompositeCommand cmds = new CompositeCommand();
            cmds.DeselectAllWidgetsFirst();

            //Create group
            GroupViewModel groupVM = CreateGroupRender(group, cmds);

            cmds.AddCommand(new CreateGroupCommand(this, groupVM));
            _undoManager.Push(cmds);

            //_ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            _ListEventAggregator.GetEvent<GroupChangedEvent>().Publish(true);

        }
        public bool CanRunGroupCommand
        {
            get
            {
                if (_selectionService.WidgetNumber <= 1)
                {
                    return false;
                }

                foreach (var item in _selectionService.GetSelectedWidgets())
                {
                    if ((item as WidgetViewModBase).ParentID != Guid.Empty)
                    {
                        return false;
                    }
                }
                return true;

            }
        }

        //Command UnGroup,20140402
        private void UnGroupCommanddHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            CompositeCommand cmds = new CompositeCommand();

            foreach (var item in _selectionService.GetSelectedWidgets())
            {
                if ((item as WidgetViewModBase).IsGroup == true)
                {
                    GroupViewModel groupVM = item as GroupViewModel;
                    UnGroup(groupVM);

                    UngroupCommand cmd = new UngroupCommand(this, groupVM);
                    cmds.AddCommand(cmd);
                }
            }

            PushToUndoStack(cmds);
            _ListEventAggregator.GetEvent<GroupChangedEvent>().Publish(false);
        }
        public bool CanRunUnGroupCommand
        {
            get
            {
                if (_selectionService.WidgetNumber <= 0)
                {
                    return false;
                }

                foreach (var item in _selectionService.GetSelectedWidgets())
                {
                    if ((item as WidgetViewModBase).IsGroup == false)
                        return false;
                }

                return true;
            }
        }

        //Command Undo,20140402
        private void UndoCommandHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            _undoManager.Undo();
        }
        private bool CanRunUndoCommand
        {
            get { return _undoManager.CanUndo; }
        }

        //Command Redo,20140402
        private void RedoCommandHandler(object parameter)
        {
            if (_busyIndicator.IsShow == true)
            {
                return;
            }
            _undoManager.Redo();
        }
        private bool CanRunRedoCommand
        {
            get { return _undoManager.CanRedo; }
        }
        #endregion

        #region Font Routed command Handler
        //
        #region Align Text Left Command Handler

        private void AlignTextLeftCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            var EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_TextHor);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextHorAligen", wdgItem.vTextHorAligen, Alignment.Left);

                wdgItem.vTextHorAligen = Alignment.Left;

            }

            PushToUndoStack(cmds);
        }
        public bool CanRunAlignTextLeftCommand
        {
            get
            {
               // return bSupportTextAlign(true);

                return IsSupportProperty(PropertyOption.Option_TextHor);
            }
        }

        #endregion Align Left Command Handler

        #region Align Text Right Command Handler

        private void AlignTextRightCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            var EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_TextHor);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {

                CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextHorAligen", wdgItem.vTextHorAligen, Alignment.Right);

                wdgItem.vTextHorAligen = Alignment.Right;

            }

            PushToUndoStack(cmds);
        }

        public bool CanRunAlignTextRightCommand
        {
            get
            {
               // return bSupportTextAlign(true);

                return IsSupportProperty(PropertyOption.Option_TextHor);
            }
        }

        #endregion Align Left Command Handler

        #region Align Text Center Command Handler

        private void AlignTextCenterCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            var EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_TextHor);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                //if (Convert.ToBoolean(parameter))
                //{
                CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextHorAligen", wdgItem.vTextHorAligen, Alignment.Center);

                wdgItem.vTextHorAligen = Alignment.Center;
                //}

            }

            PushToUndoStack(cmds);
        }
        public bool CanRunAlignTextCenterCommand
        {
            get
            {
                //return bSupportTextAlign(true);
                return IsSupportProperty(PropertyOption.Option_TextHor);
            }
        }

        #endregion

        #region Align Text Justify Command Handler

        private void AlignJustifyCommandHandler(object parameter)
        {
            //List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();
            //foreach (WidgetViewModBase wdgItem in allSelects)
            //{
            //    if (Convert.ToBoolean(parameter))
            //    {
            //        wdgItem.vTextHorAligen = Alignment.Left;
            //    }
            //}
        }
        public bool CanRunAlignJustifyCommand
        {
            get
            {
               return true;
            }
        }

        #endregion

        #region Align Text Top Command Handler

        private void AlignTopCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            var EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_TextVer);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextVerAligen", wdgItem.vTextVerAligen, Alignment.Top);

                wdgItem.vTextVerAligen = Alignment.Top;
            }

            PushToUndoStack(cmds);
        }

        public bool CanRunAlignTopCommand
        {
            get
            {
              //  return bSupportTextAlign(false);
                return IsSupportProperty(PropertyOption.Option_TextVer);
            }
        }

        #endregion

        #region Align Text Bottom Command Handler

        private void AlignBottomCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            var EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_TextVer);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {

                CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextVerAligen", wdgItem.vTextVerAligen, Alignment.Bottom);

                wdgItem.vTextVerAligen = Alignment.Bottom;

            }

            PushToUndoStack(cmds);
        }
        public bool CanRunAlignBottomCommand
        {
            get
            {
               // return bSupportTextAlign(false);
                return IsSupportProperty(PropertyOption.Option_TextVer);
            }
        }

        #endregion

        #region Align Text Middle Command Handler

        private void AlignTextMiddleCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            var EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_TextVer);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                //if (Convert.ToBoolean(parameter))
                //{
                CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextVerAligen", wdgItem.vTextVerAligen, Alignment.Center);

                wdgItem.vTextVerAligen = Alignment.Center;
                //}
            }

            PushToUndoStack(cmds);
        }
        public bool CanRunAlignTextMiddleCommand
        {
            get
            {
                //return bSupportTextAlign(false);
                return IsSupportProperty(PropertyOption.Option_TextVer);
            }
        }

        #endregion

        #region Font Family Command Handler

        private void FontFamilyCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            var EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                string newValue = Convert.ToString(parameter);

                if (!wdgItem.IsGroup && wdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = wdgItem.vTextContent;

                    wdgItem.vFontFamily = newValue;

                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextContent", oldValue, wdgItem.vTextContent);
                }
                else
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vFontFamily", wdgItem.vFontFamily, newValue);

                    wdgItem.vFontFamily = newValue;
                }
            }

            PushToUndoStack(cmds);
        }
        public bool CanRunFontFamilyCommand
        {
            get
            {
                //return bSupportText();
                return IsSupportProperty(PropertyOption.Option_Text);
            }
        }

        #endregion

        #region Font Size Command Handler

        private void FontSizeDecreaseCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
            if (select != null)
            {
                List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);

                List<WidgetViewModBase> templist = new List<WidgetViewModBase>();
                foreach (WidgetViewModBase data in EligibleWidgets)
                {
                    templist.Add(data);
                }

                widgetListFontSizeChange(cmds, templist, false);

                PushToUndoStack(cmds);
            }
        }

        private void FontSizeIncreaseCommandHandler(object parameter)
        {

            CompositeCommand cmds = new CompositeCommand();

            ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
            if (select != null)
            {
                List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);

                List<WidgetViewModBase> templist = new List<WidgetViewModBase>();
                foreach (WidgetViewModBase data in EligibleWidgets)
                {
                    templist.Add(data);
                }

                widgetListFontSizeChange(cmds, templist, true);

                PushToUndoStack(cmds);
            }
        }

        private void FontSizeCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                double newValue = Convert.ToDouble(parameter);

                if (!wdgItem.IsGroup && wdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = wdgItem.vTextContent;

                    wdgItem.vFontSize = newValue;

                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextContent", oldValue, wdgItem.vTextContent);
                }
                else
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vFontSize", wdgItem.vFontSize, newValue);

                    wdgItem.vFontSize = newValue;
                }

            }

            PushToUndoStack(cmds);
        }
        public bool CanRunFontSizeCommand
        {
            get
            {
               // return bSupportText();
                return IsSupportProperty(PropertyOption.Option_Text);
            }
        }

        #endregion

        #region Up/Down Case Command Handler
                private void TextUpDownCaseHandler(object parameter)
                {
                    CompositeCommand cmds = new CompositeCommand();

                    ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
                    if (select != null)
                    {
                        List<IWidgetPropertyData> allSelects = GetEligibleWidgets(PropertyOption.Option_Text);

                        List<WidgetViewModBase> templist = new List<WidgetViewModBase>();
                        foreach (WidgetViewModBase data in allSelects)
                        {
                            templist.Add(data);
                        }

                        widgetUpDownCase(cmds, templist);

                        PushToUndoStack(cmds);
                    }
                }

                public bool CanRunUpDownCase
                {
                    get
                    {
                        //return bSupportText();

                        return IsSupportProperty(PropertyOption.Option_Text);
                    }
                }
        #endregion

        #region Bold style Command Handler
        private void FontBoldCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);

            bool newValue = false;
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                newValue = newValue || (!wdgItem.vFontBold);
            }

            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                if (!wdgItem.IsGroup && wdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = wdgItem.vTextContent;

                    wdgItem.vFontBold = newValue;

                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextContent", oldValue, wdgItem.vTextContent);

                }
                else
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vFontBold", wdgItem.vFontBold, newValue);

                    wdgItem.vFontBold = newValue;
                }

            }

            PushToUndoStack(cmds);
        }
        public bool CanRunFontBoldCommand
        {
            get
            {
               // return bSupportText();
                return IsSupportProperty(PropertyOption.Option_Text);
            }
        }
        #endregion

        #region Underline style Command Handler

        private void FontUnderlineCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);
            bool newValue = false;
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                newValue = newValue || (!wdgItem.vFontUnderLine);
            }
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                if (!wdgItem.IsGroup && wdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = wdgItem.vTextContent;

                    wdgItem.vFontUnderLine = newValue;

                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextContent", oldValue, wdgItem.vTextContent);

                }
                else
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vFontUnderLine", wdgItem.vFontUnderLine, newValue);

                    wdgItem.vFontUnderLine = newValue;
                }

            }

            PushToUndoStack(cmds);
        }
        public bool CanRunFontUnderlineCommand
        {
            get
            {
                //return bSupportText();
                return IsSupportProperty(PropertyOption.Option_Text);
            }
        }

        #endregion

        #region StrikeThrough style Command Handler

        private void FontStrikeThroughCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);
            bool newValue = false;
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                newValue = newValue || (!wdgItem.vFontStrickeThrough);
            }
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                if (!wdgItem.IsGroup && wdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = wdgItem.vTextContent;

                    wdgItem.vFontStrickeThrough = newValue;

                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextContent", oldValue, wdgItem.vTextContent);

                }
                else
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vFontStrickeThrough", wdgItem.vFontStrickeThrough, newValue);

                    wdgItem.vFontStrickeThrough = newValue;
                }

            }

            PushToUndoStack(cmds);
        }
        public bool CanRunFontStrikeThroughCommand
        {
            get
            {
                //return bSupportText();
                return IsSupportProperty(PropertyOption.Option_Text);
            }
        }

        #endregion

        #region Bullet style Command Handler

        private void BulletStyleCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            TextMarkerStyle newValue = TextMarkerStyle.None;//parameter as TextMarkerStyle;

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Bullet);

            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                if (wdgItem.vTextBulletStyle == TextMarkerStyle.None)
                {
                    newValue = TextMarkerStyle.Disc;
                    break;
                }
            }

            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {

                //if (!wdgItem.IsGroup)
                //{
                //    string oldValue = wdgItem.vTextContent;

                //    wdgItem.vTextBulletStyle = newValue;

                //    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextContent", oldValue, wdgItem.vTextContent);
                //}
                //else
                //{
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextBulletStyle", wdgItem.vTextBulletStyle, newValue);

                    wdgItem.vTextBulletStyle = newValue;
                //}
            }

            PushToUndoStack(cmds);
        }
        public bool CanRunBulletStyleCommand
        {
            get
            {
               // return bSupportTextRotate();
                return IsSupportProperty(PropertyOption.Option_Bullet);
            }
        }

        #endregion

        #region Font color style Command Handler

        private void FontColorCommandHandler(object parameter)
        {
            Color newValue = default(Color);
            if (parameter is StyleColor)
            {
                var scolor = (StyleColor)parameter;
                newValue = scolor.ToColor();
            }
            else if (parameter is SolidColorBrush)
            {
                var scolor = parameter as SolidColorBrush;
                newValue = scolor.Color;
            }
            else
            {
                return;
            }

            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                if (!wdgItem.IsGroup && wdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = wdgItem.vTextContent;

                    wdgItem.vFontColor = newValue;

                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextContent", oldValue, wdgItem.vTextContent);

                }
                else
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vFontColor", wdgItem.vFontColor, newValue);

                    wdgItem.vFontColor = newValue;
                }

            }

            PushToUndoStack(cmds);
        }

        public bool CanRunFontColorCommand
        {
            get
            {
               // return bSupportText();
                return IsSupportProperty(PropertyOption.Option_Text);
            }
        }

        #endregion

        #region Italic style Command Handler

        private void FontItalicCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Text);

            bool newValue = false;
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                newValue = newValue || (!wdgItem.vFontItalic);
            }

            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                if (!wdgItem.IsGroup && wdgItem.WidgetType == WidgetType.Shape)
                {
                    string oldValue = wdgItem.vTextContent;

                    wdgItem.vFontItalic = newValue;

                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vTextContent", oldValue, wdgItem.vTextContent);
                }
                else
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vFontItalic", wdgItem.vFontItalic, newValue);

                    wdgItem.vFontItalic = newValue;
                }

            }

            PushToUndoStack(cmds);
        }
        public bool CanRunFontItalicCommand
        {
            get
            {
               // return bSupportText();
                return IsSupportProperty(PropertyOption.Option_Text);
            }
        }

        #endregion

        #region BorderLine color  Command Handler

        private void BorderLineColorCommandHandler(object parameter)
        {
            if (!(parameter is StyleColor))
            {
                return;
            }

            CompositeCommand cmds = new CompositeCommand();

            List<WidgetViewModBase> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Border).OfType<WidgetViewModBase>().ToList<WidgetViewModBase>();

            ProcessBorderlineColorChange(EligibleWidgets, (StyleColor)parameter, cmds);

            PushToUndoStack(cmds);
        }

        private void ProcessBorderlineColorChange(List<WidgetViewModBase> list,StyleColor newValue,CompositeCommand cmds)
        {
            foreach (WidgetViewModBase wdgItem in list)
            {
                if (newValue.FillType == ColorFillType.Solid ||
                   (newValue.FillType == ColorFillType.Gradient && wdgItem.IsSupportGradientBorderline))
                {
                    if (wdgItem.IsGroup)
                    {
                        ProcessBorderlineColorChange(((GroupViewModel)wdgItem).WidgetChildren, newValue, cmds);
                        ((GroupViewModel)wdgItem).RefreshProperty("vBorderLineColor");
                    }
                    else
                    {
                        if (wdgItem.vBorderLinethinck < 1)
                        {
                            CreatePropertyChangeUndoCommand(cmds, wdgItem, "vBorderLinethinck", wdgItem.vBorderLinethinck, 1);
                            wdgItem.vBorderLinethinck = 1;
                        }

                        CreatePropertyChangeUndoCommand(cmds, wdgItem, "vBorderLineColor", wdgItem.vBorderLineColor, newValue);

                        wdgItem.vBorderLineColor = newValue;
                    }
                }
            }
        }

        public bool CanRunBorderLineColorCommand
        {
            get
            {
                //return bSupportBorder();
                return IsSupportProperty(PropertyOption.Option_Border);
            }
        }

        #endregion

        #region Line arrow style Command Handler
        private void LineArrowStyleCommandHandler(object parameter)
        {
            if (parameter is LineArrowStyleDate)
            {
                CompositeCommand cmds = new CompositeCommand();

                LineArrowStyleDate nPar = parameter as LineArrowStyleDate;
                ArrowStyle newValue = ArrowStyle.Default;
                if (nPar != null)
                {
                    newValue = nPar.BLStyle;
                }

                List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_LineArrow);
                foreach (WidgetViewModBase wdgItem in EligibleWidgets)
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "LineArrowStyle", wdgItem.LineArrowStyle, newValue);

                    wdgItem.LineArrowStyle = newValue;
                }

                PushToUndoStack(cmds);
            }
        }

        public bool CanRunLineArrowStyleCommand
        {
            get
            {
                //return bSupportLineArrow();
                return IsSupportProperty(PropertyOption.Option_LineArrow);
            }
        }

        #endregion

        #region BorderLine Pattern  Command Handler

        private void BorderLinePatternCommandHandler(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            CompositeCommand cmds = new CompositeCommand();

            BorderLineStyleData para = parameter as BorderLineStyleData;
            LineStyle setValue = LineStyle.None;
            if (para != null)
            {
                setValue = (LineStyle)para.BLStyle;
            }
            else
            {
                return;
            }

            List<WidgetViewModBase> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Border).OfType<WidgetViewModBase>().ToList<WidgetViewModBase>();

            ProcessBorderlineStyle(EligibleWidgets, setValue, cmds);

            PushToUndoStack(cmds);
        }

        private void ProcessBorderlineStyle(List<WidgetViewModBase> list,LineStyle style,CompositeCommand cmds)
        {
             foreach (WidgetViewModBase wdgitem in list)
             {
                 if (wdgitem.IsGroup)
                 {
                     ProcessBorderlineStyle(((GroupViewModel)wdgitem).WidgetChildren, style, cmds);
                     ((GroupViewModel)wdgitem).RefreshProperty("vBorderlineStyle");
                 }
                 else 
                 {
                     if (wdgitem.vBorderLinethinck < 1.0 && style != LineStyle.None)
                     {
                         CreatePropertyChangeUndoCommand(cmds, wdgitem, "vBorderLinethinck", wdgitem.vBorderLinethinck, 1.0);
                         wdgitem.vBorderLinethinck = 1.0;
                     }

                     CreatePropertyChangeUndoCommand(cmds, wdgitem, "vBorderlineStyle", wdgitem.vBorderlineStyle, style);

                     wdgitem.vBorderlineStyle = style;
                 }
             }
        }
        public bool CanRunBorderLinePatternCommand
        {
            get
            {
                //return bSupportBorder();
                return IsSupportProperty(PropertyOption.Option_Border);
            }
        }

        #endregion

        #region BorderLine Thinck  Command Handler

        private void BorderLineThinckCommandHandler(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            CompositeCommand cmds = new CompositeCommand();

            BorderLineWidthData nPar = parameter as BorderLineWidthData;
            double setValue = 0;
            if (nPar != null)
            {
                setValue = nPar.Width;
            }

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.Option_Border);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                double oldValue = wdgItem.vBorderLinethinck;

                if (oldValue == setValue)
                {
                    continue;
                }

                CreatePropertyChangeUndoCommand(cmds, wdgItem, "vBorderLinethinck", oldValue, setValue);

                //must set value after create undo command.
                wdgItem.vBorderLinethinck = setValue;
            }

            PushToUndoStack(cmds);
        }

        public bool CanRunBorderLineThinckCommand
        {
            get
            {
                //return bSupportBorder();
                return IsSupportProperty(PropertyOption.Option_Border);
            }
        }

        #endregion

        #endregion Font Routed command Handler

        #region Background Command Handler

        private void ChangeBackGroundCommandHandler(object parameter)
        {
            if (!(parameter is StyleColor))
            {
                return;
            }

            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> EligibleWidgets = GetEligibleWidgets(PropertyOption.OPtion_BackColor);
            foreach (WidgetViewModBase wdgItem in EligibleWidgets)
            {
                var newValue = (StyleColor)parameter;
                if (newValue.FillType == ColorFillType.Solid ||
                    (newValue.FillType == ColorFillType.Gradient && wdgItem.IsSupportGradientBackground))
                {
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "vBackgroundColor", wdgItem.vBackgroundColor, newValue);

                    wdgItem.vBackgroundColor = newValue;
                }

            }

            PushToUndoStack(cmds);
        }

        public bool CanRunBackGroundCommand
        {
            get
            {
                //return bSupportBackColor();
                return IsSupportProperty(PropertyOption.OPtion_BackColor);
            }
        }

        #endregion

        #region Z index Command Handler
        //Widgets BringFront
        private void WidgetsBringFrontCommandHandler(object par)
        {
            if (_selectionService.WidgetNumber < 1)
            {
                return;
            }
            if (CanZOrderChange() == false)
            {
                return;
            }

            Guid topGroupID = _selectionService.GetSelectedWidgets()[0].ParentID;
            Guid realpGroupID = (_selectionService.GetSelectedWidgets()[0] as WidgetViewModBase).RealParentGroupGID;
            if (topGroupID == Guid.Empty)
            {
                BringFrontExternalObjects();
            }
            else
            {
                BringFrontChildrenObjects(topGroupID, realpGroupID);
            }   

        }
        private void BringFrontChildrenObjects(Guid groupID,Guid realGroupID)
        {
            //Create target list
            List<WidgetViewModBase> targetWidgets = new List<WidgetViewModBase>();
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                if (wdgItem.IsGroup == false)
                {
                    targetWidgets.Add(wdgItem);
                }
                else
                {
                    GroupViewModel group = wdgItem as GroupViewModel;
                    targetWidgets.AddRange(group.WidgetChildren);
                }
            }

            //Change the ZOrder
            CompositeCommand cmds = new CompositeCommand();
            bool bIsChanged = SetChildrenObject2Front(targetWidgets, cmds, groupID, realGroupID);

            //Send notify message to widget manager
            if (bIsChanged == true)
            {
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }   
        }
        private bool SetChildrenObject2Front(List<WidgetViewModBase> targetWidgets, CompositeCommand cmds, Guid groupID,Guid realGroupID)
        {
            if (targetWidgets.Count <= 0)
            {
                return false;
            }
            GroupViewModel ParentGroup = this.GetTargetGroup(groupID);
            
            if (ParentGroup == null)
            {
                return false;
            }
            if (ParentGroup.WidgetChildren.Count() < 2)
            {
                return false;
            }
            
            List<WidgetViewModBase> allWidget = ParentGroup.GetSpecificGroupAllChildren(realGroupID);
            targetWidgets = targetWidgets.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            allWidget = allWidget.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            int minZOrder = allWidget.Min(a => a.ZOrder);

            int i = 0;
            int j = 0;
            bool bIsOrderChanged = false;
            foreach (WidgetViewModBase item in allWidget)
            {
                if (targetWidgets.Contains(item))
                {
                    int newValue = allWidget.Count - targetWidgets.Count + (j++);
                    if (cmds != null)
                    {
                        CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue + minZOrder);
                    }
                    item.ZOrder = newValue + minZOrder;
                    bIsOrderChanged = true;
                }
                else
                {
                    int newValue = (i++);
                    if (cmds != null)
                    {
                        CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue + minZOrder);
                    }
                    item.ZOrder = newValue + minZOrder;
                    bIsOrderChanged = true;
                }
            }

            if (cmds != null)
            {
                if (bIsOrderChanged == false)
                {
                    return false;
                }
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
            }
            return true;
        }
        private void BringFrontExternalObjects()
        {
            //Create target list
            List<WidgetViewModBase> targetWidgets = new List<WidgetViewModBase>();
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                if (wdgItem.IsGroup == false)
                {
                    targetWidgets.Add(wdgItem);
                }
                else
                {
                    GroupViewModel group = wdgItem as GroupViewModel;
                    targetWidgets.AddRange(group.WidgetChildren);
                }
            }

            //Change the ZOrder
            CompositeCommand cmds = new CompositeCommand();
            bool bIsChanged=SetTargetObject2Front(targetWidgets, cmds);

            //Send notify message to widget manager
            if (bIsChanged==true)
            {
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }         
        }
        private bool SetTargetObject2Front(List<WidgetViewModBase> targetWidgets, CompositeCommand cmds)
        {
            if (targetWidgets.Count <= 0)
            {
                return false;
            }

            targetWidgets = targetWidgets.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            List<WidgetViewModBase> allWidget = Items.ToList<WidgetViewModBase>().Where(c => c.IsGroup == false).ToList<WidgetViewModBase>();
            allWidget = allWidget.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();            

            int i = 0;
            int j = 0;
            bool bIsOrderChanged = false;
            foreach (WidgetViewModBase item in allWidget)
            {
                if (targetWidgets.Contains(item))
                {
                    int newValue = allWidget.Count - targetWidgets.Count + (j++);
                    if (cmds != null)
                    {
                        CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue);                        
                    }
                    item.ZOrder = newValue;
                    bIsOrderChanged = true;
                }
                else
                {
                    int newValue = (i++);                                 
                    if (cmds != null)
                    {
                        CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue, false);
                    }
                    item.ZOrder = newValue;
                    bIsOrderChanged = true;
                }
            }

            if (cmds != null)
            {
                if (bIsOrderChanged == false)
                {
                    return false;
                }
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
            }
            return true;
        }
        public bool CanRunWidgetsBringFrontCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        //Widgets BringForward
        private void WidgetsBringForwardCommandHandler(object parameter)
        {            
            if(_selectionService.WidgetNumber < 1)
            {
                return;
            }
            if (CanZOrderChange() ==false)
            {
                return;
            }
            Guid itemID = _selectionService.GetSelectedWidgets()[0].ParentID;
            if (itemID == Guid.Empty)
            {
                BringForwardExternalObjects();
            }
            else
            {
                BringForwardChildrenObjects(itemID);
            }           
            
        }
        public bool CanRunWidgetsBringForwardCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }
        private void BringForwardChildrenObjects0(Guid groupID)
        {
            GroupViewModel ParentGroup = this.GetTargetGroup(groupID);
            if(ParentGroup==null)
            {
                return;
            }
            if(ParentGroup.WidgetChildren.Count()<2)
            {
                return;
            }

            List<WidgetViewModBase> AllChildren = ParentGroup.WidgetChildren;
            List<WidgetViewModBase> targetWidgets = ParentGroup.WidgetChildren.Where(c => c.IsSelected == true).ToList<WidgetViewModBase>();
            if (targetWidgets.Count() == AllChildren.Count())
            {
                return;
            }
            targetWidgets = targetWidgets.OrderByDescending(s => s.ZOrder).ToList<WidgetViewModBase>();
            int maxZOrder = AllChildren.Max(a => a.ZOrder);
            //int minZOrder = AllChildren.Min(a => a.ZOrder);

            // Make selected widgets Z-Order be increased by 1.
            bool bIsZOrderChanged = false;
            CompositeCommand cmds = new CompositeCommand();
            int nOrderMaxOperationLimit = maxZOrder;
            foreach (WidgetViewModBase targetItem in targetWidgets)
            {
                if (targetItem.ZOrder < nOrderMaxOperationLimit)
                {
                    CreatePropertyChangeUndoCommand(cmds, targetItem, "ZOrder", targetItem.ZOrder, targetItem.ZOrder + 1);
                    bIsZOrderChanged = true;
                    targetItem.ZOrder += 1;
                    nOrderMaxOperationLimit = targetItem.ZOrder - 1;
                }
                else
                {
                    nOrderMaxOperationLimit = targetItem.ZOrder - 1;
                }
            }

            // Decrease other widgets Z-Order if they have the same Z-Order with selected widgets updated Z-Order.
            foreach (WidgetViewModBase targetItem in targetWidgets)
            {
                foreach (WidgetViewModBase item in AllChildren)
                {
                    if (targetWidgets.Contains(item) == false)
                    {
                        if (item.ZOrder == targetItem.ZOrder)
                        {
                            CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, item.ZOrder - 1, false);
                            bIsZOrderChanged = true;
                            item.ZOrder -= 1;
                            break;
                        }
                    }
                }
            }

            if (bIsZOrderChanged==true)
            {
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }
         
        }
        private void BringForwardChildrenObjects(Guid groupID)
        {
            GroupViewModel ParentGroup = this.GetTargetGroup(groupID);
            if (ParentGroup == null)
            {
                return;
            }
            if (ParentGroup.WidgetChildren.Count() < 2)
            {
                return;
            }

            List<IWidgetPropertyData> AllSelects = _selectionService.GetSelectedWidgets();
            WidgetViewModBase target = AllSelects[0] as WidgetViewModBase;
            if (target == null)
            {
                return;
            }

            List<IZOrderTopChildObj> AllCurrentLevelObjs = ParentGroup.GetSpecificTopLevelZOrderChildren(target.RealParentGroupGID);
            if (AllCurrentLevelObjs == null || AllCurrentLevelObjs.Count()<2)
            {
                return;
            }

            AllCurrentLevelObjs = AllCurrentLevelObjs.OrderBy(s => s.ZOrdder).ToList<IZOrderTopChildObj>(); ;
            int nTotal = AllCurrentLevelObjs.Count();
            if (nTotal < 2)
            {
                return;
            }

            bool bIsZOrderChanged = false;
            CompositeCommand cmds = new CompositeCommand();
            for (int i = nTotal - 2; i >= 0; i--)
            {
                IZOrderTopChildObj second = AllCurrentLevelObjs[i + 1];
                IZOrderTopChildObj first = AllCurrentLevelObjs[i];
                if (first.IsSelected == false)
                {
                    //Undo second if it was changed
                    if (second.ZOrderDel!=0)
                    {
                        foreach (WidgetViewModBase it in second.GetChildren())
                        {
                            CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder - second.ZOrderDel, it.ZOrder, it.IsSelected);
                        }
                    }

                    continue;
                }
                else if(second.IsSelected == true)
                {
                    //Undo second anyway
                    foreach (WidgetViewModBase it in second.GetChildren())
                    {
                        CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder, it.ZOrder, it.IsSelected);
                    }
                    continue;
                }

                first.IncreaseZOrder(second.Count);
                second.DecreaseZOrder(first.Count);
                bIsZOrderChanged = true;

                //undo first
                List<WidgetViewModBase> children = first.GetChildren();
                if(children==null && children.Count()<1)
                {
                    return;
                }
                foreach(WidgetViewModBase it in children)
                {
                    CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder - first.ZOrderDel, it.ZOrder, it.IsSelected);
                }

                //switch position between both targets if necessary
                AllCurrentLevelObjs[i + 1]=first;
                AllCurrentLevelObjs[i] = second; ;
            }

            if (AllCurrentLevelObjs[0].ZOrderDel!=0)
            {
                foreach (WidgetViewModBase it in AllCurrentLevelObjs[0].GetChildren())
                {
                    CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder - AllCurrentLevelObjs[0].ZOrderDel, it.ZOrder, it.IsSelected);
                }
            }                     

            if (bIsZOrderChanged == true)
            {
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }

        }
        private void BringForwardExternalObjects()
        {
            if(_selectionService.WidgetNumber<1)
            {
                return;
            }

            //Get All External Objects including group
            List<WidgetViewModBase> AllObjs = Items.ToList<WidgetViewModBase>().Where(c => c.ParentID== Guid.Empty).ToList<WidgetViewModBase>();
            AllObjs = AllObjs.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            int nTotal = AllObjs.Count();
            if(nTotal<2)
            {
                return;
            }

            CompositeCommand cmds = new CompositeCommand();
            bool bIsZOrderChanged = false;
            for(int i=nTotal-2; i>=0; i--)
            {                
                WidgetViewModBase top = AllObjs[i + 1];
                WidgetViewModBase bottom = AllObjs[i];
                if(bottom.IsSelected==false)
                {
                    continue;
                }
                if(top.IsSelected==true)
                {
                    continue;
                }

                if(top.IsGroup==true)
                {
                    //Top object is a group
                    GroupViewModel topGroup = top as GroupViewModel;
                    int nTopNumber = topGroup.WidgetChildren.Count();
                    int nMaxZOrder = topGroup.ZOrder;
                    int nMinZOrder = nMaxZOrder - nTopNumber + 1;

                    if (bottom.IsGroup == true)
                    {                        
                        GroupViewModel bottomGroup = bottom as GroupViewModel;
                        int nBottomNumber = bottomGroup.WidgetChildren.Count();

                        //Change bottom's children ZOrder
                        foreach (WidgetViewModBase child in bottomGroup.WidgetChildren)
                        {
                            CreatePropertyChangeUndoCommand(cmds, child, "ZOrder", child.ZOrder, child.ZOrder + nTopNumber);
                            child.ZOrder=child.ZOrder + nTopNumber;
                            bIsZOrderChanged = true;
                        }

                        //Change top's children ZOrder
                        foreach (WidgetViewModBase child in topGroup.WidgetChildren)
                        {
                            CreatePropertyChangeUndoCommand(cmds, child, "ZOrder", child.ZOrder, child.ZOrder - nBottomNumber);
                            child.ZOrder = child.ZOrder - nBottomNumber;
                            bIsZOrderChanged = true;
                        }

                    }
                    else
                    {
                        //Change bottom's children ZOrder
                        CreatePropertyChangeUndoCommand(cmds, bottom, "ZOrder", bottom.ZOrder, bottom.ZOrder + nTopNumber);
                        bottom.ZOrder = bottom.ZOrder + nTopNumber;
                        bIsZOrderChanged = true;

                        //Change top's children ZOrder
                        foreach (WidgetViewModBase child in topGroup.WidgetChildren)
                        {
                            CreatePropertyChangeUndoCommand(cmds, child, "ZOrder", child.ZOrder, child.ZOrder - 1);
                            child.ZOrder = child.ZOrder - 1;
                            bIsZOrderChanged = true;
                        }

                    }
                }
                else
                {
                    //Top object is a common widget
                    int nTopNumber = 1;

                    if (bottom.IsGroup == true)
                    {
                        GroupViewModel bottomGroup = bottom as GroupViewModel;
                        int nBottomNumber = bottomGroup.WidgetChildren.Count();

                        //Change bottom's children ZOrder
                        foreach (WidgetViewModBase child in bottomGroup.WidgetChildren)
                        {
                            CreatePropertyChangeUndoCommand(cmds, child, "ZOrder", child.ZOrder, child.ZOrder + nTopNumber);
                            child.ZOrder = child.ZOrder + nTopNumber;
                            bIsZOrderChanged = true;
                        }

                        //Change top's children ZOrder
                        CreatePropertyChangeUndoCommand(cmds, top, "ZOrder", top.ZOrder, top.ZOrder - nBottomNumber);
                        top.ZOrder = top.ZOrder - nBottomNumber;
                        bIsZOrderChanged = true;
                    }
                    else
                    {
                        //Change bottom's children ZOrder
                        CreatePropertyChangeUndoCommand(cmds, bottom, "ZOrder", bottom.ZOrder, bottom.ZOrder + nTopNumber);
                        bottom.ZOrder = bottom.ZOrder + nTopNumber;
                        bIsZOrderChanged = true;

                        //Change top's children ZOrder
                        CreatePropertyChangeUndoCommand(cmds, top, "ZOrder", top.ZOrder, top.ZOrder - nTopNumber);
                        top.ZOrder = top.ZOrder - nTopNumber;
                        bIsZOrderChanged = true;
                    }
                }

                //Switch the top and bottom
                AllObjs[i + 1]=bottom;
                AllObjs[i]=top;
            }//for(int i=nTotal-2; i>=0; i--)


            //Push Command to Undo/Redo stack
            if (bIsZOrderChanged == true)
            {
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }    

        }

        
        //Widgets BringBackward
        private void WidgetsBringBackwardCommandHandler(object parameter)
        {
            if (_selectionService.WidgetNumber < 1)
            {
                return;
            }
            if (CanZOrderChange() == false)
            {
                return;
            }
            Guid itemID = _selectionService.GetSelectedWidgets()[0].ParentID;
            if (itemID == Guid.Empty)
            {
                BringBackwardExternalObjects();
            }
            else
            {
                BringBackwardChildrenObjects(itemID);
            }    
        }
        private void BringBackwardChildrenObjects0(Guid groupID)
        {
            GroupViewModel ParentGroup = this.GetTargetGroup(groupID);
            if (ParentGroup == null)
            {
                return;
            }
            if (ParentGroup.WidgetChildren.Count() < 2)
            {
                return;
            }

            List<WidgetViewModBase> AllChildren = ParentGroup.WidgetChildren;
            List<WidgetViewModBase> targetWidgets = ParentGroup.WidgetChildren.Where(c => c.IsSelected == true).ToList<WidgetViewModBase>();
            if (targetWidgets.Count() == AllChildren.Count())
            {
                return;
            }

            targetWidgets = targetWidgets.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            int minZOrder = AllChildren.Min(a => a.ZOrder);
            //int minZOrder = AllChildren.Min(a => a.ZOrder);

            // Make selected widgets Z-Order be increased by 1.
            bool bIsZOrderChanged = false;
            CompositeCommand cmds = new CompositeCommand();
            int nOrderMinOperationLimit = minZOrder;
            foreach (WidgetViewModBase targetItem in targetWidgets)
            {
                if (targetItem.ZOrder > nOrderMinOperationLimit)
                {
                    CreatePropertyChangeUndoCommand(cmds, targetItem, "ZOrder", targetItem.ZOrder, targetItem.ZOrder - 1);
                    bIsZOrderChanged = true;
                    targetItem.ZOrder -= 1;
                    nOrderMinOperationLimit = targetItem.ZOrder + 1;
                }
                else
                {
                    nOrderMinOperationLimit = targetItem.ZOrder + 1;
                }
            }

            // Decrease other widgets Z-Order if they have the same Z-Order with selected widgets updated Z-Order.
            foreach (WidgetViewModBase targetItem in targetWidgets)
            {
                foreach (WidgetViewModBase item in AllChildren)
                {
                    if (targetWidgets.Contains(item) == false)
                    {
                        if (item.ZOrder == targetItem.ZOrder)
                        {
                            CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, item.ZOrder + 1, false);
                            bIsZOrderChanged = true;
                            item.ZOrder += 1;
                            break;
                        }
                    }
                }
            }

            if (bIsZOrderChanged == true)
            {
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }
        }
        private void BringBackwardChildrenObjects(Guid groupID)
        {
            GroupViewModel ParentGroup = this.GetTargetGroup(groupID);
            if (ParentGroup == null)
            {
                return;
            }
            if (ParentGroup.WidgetChildren.Count() < 2)
            {
                return;
            }

            List<IWidgetPropertyData> AllSelects = _selectionService.GetSelectedWidgets();
            WidgetViewModBase target = AllSelects[0] as WidgetViewModBase;
            if (target == null)
            {
                return;
            }

            List<IZOrderTopChildObj> AllCurrentLevelObjs = ParentGroup.GetSpecificTopLevelZOrderChildren(target.RealParentGroupGID);
            if (AllCurrentLevelObjs == null || AllCurrentLevelObjs.Count() < 2)
            {
                return;
            }

            AllCurrentLevelObjs = AllCurrentLevelObjs.OrderByDescending(s => s.ZOrdder).ToList<IZOrderTopChildObj>(); ;
            int nTotal = AllCurrentLevelObjs.Count();
            if (nTotal < 2)
            {
                return;
            }

            bool bIsZOrderChanged = false;
            CompositeCommand cmds = new CompositeCommand();
            for (int i = nTotal - 2; i >= 0; i--)
            {
                IZOrderTopChildObj second = AllCurrentLevelObjs[i + 1];
                IZOrderTopChildObj first = AllCurrentLevelObjs[i];
                if (first.IsSelected == false)
                {
                    //Undo second if it was changed
                    if (second.ZOrderDel != 0)
                    {
                        foreach (WidgetViewModBase it in second.GetChildren())
                        {
                            CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder - second.ZOrderDel, it.ZOrder, it.IsSelected);
                        }
                    }

                    continue;
                }
                else if (second.IsSelected == true)
                {
                    //Undo second anyway
                    foreach (WidgetViewModBase it in second.GetChildren())
                    {
                        CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder, it.ZOrder, it.IsSelected);
                    }
                    continue;
                }

                first.DecreaseZOrder(second.Count);
                second.IncreaseZOrder(first.Count);                
                bIsZOrderChanged = true;

                //undo first
                List<WidgetViewModBase> children = first.GetChildren();
                if (children == null && children.Count() < 1)
                {
                    return;
                }
                foreach (WidgetViewModBase it in children)
                {
                    CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder - first.ZOrderDel, it.ZOrder, it.IsSelected);
                }

                //switch position between both targets if necessary
                AllCurrentLevelObjs[i + 1] = first;
                AllCurrentLevelObjs[i] = second; ;
            }

            if (AllCurrentLevelObjs[0].ZOrderDel != 0)
            {
                foreach (WidgetViewModBase it in AllCurrentLevelObjs[0].GetChildren())
                {
                    CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder - AllCurrentLevelObjs[0].ZOrderDel, it.ZOrder, it.IsSelected);
                }
            }

            if (bIsZOrderChanged == true)
            {
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }
        }
        private void BringBackwardExternalObjects()
        {
            if (_selectionService.WidgetNumber < 1)
            {
                return;
            }

            //Get All External Objects including group
            List<WidgetViewModBase> AllObjs = Items.ToList<WidgetViewModBase>().Where(c => c.ParentID == Guid.Empty).ToList<WidgetViewModBase>();
            AllObjs = AllObjs.OrderByDescending(s => s.ZOrder).ToList<WidgetViewModBase>();
            int nTotal = AllObjs.Count();
            if (nTotal < 2)
            {
                return;
            }

            CompositeCommand cmds = new CompositeCommand();
            bool bIsZOrderChanged = false;
            for (int i = nTotal - 2; i >= 0; i--)
            {
                WidgetViewModBase top = AllObjs[i + 1];
                WidgetViewModBase bottom = AllObjs[i];
                if (bottom.IsSelected == false)
                {
                    continue;
                }
                if (top.IsSelected == true)
                {
                    continue;
                }

                if (top.IsGroup == true)
                {
                    //Top object is a group
                    GroupViewModel topGroup = top as GroupViewModel;
                    int nTopNumber = topGroup.WidgetChildren.Count();
                    int nMaxZOrder = topGroup.ZOrder;
                    int nMinZOrder = nMaxZOrder - nTopNumber + 1;

                    if (bottom.IsGroup == true)
                    {
                        GroupViewModel bottomGroup = bottom as GroupViewModel;
                        int nBottomNumber = bottomGroup.WidgetChildren.Count();

                        //Change bottom's children ZOrder
                        foreach (WidgetViewModBase child in bottomGroup.WidgetChildren)
                        {
                            CreatePropertyChangeUndoCommand(cmds, child, "ZOrder", child.ZOrder, child.ZOrder - nTopNumber);
                            child.ZOrder = child.ZOrder - nTopNumber;
                            bIsZOrderChanged = true;
                        }

                        //Change top's children ZOrder
                        foreach (WidgetViewModBase child in topGroup.WidgetChildren)
                        {
                            CreatePropertyChangeUndoCommand(cmds, child, "ZOrder", child.ZOrder, child.ZOrder + nBottomNumber);
                            child.ZOrder = child.ZOrder + nBottomNumber;
                            bIsZOrderChanged = true;
                        }

                    }
                    else
                    {
                        //Change bottom's children ZOrder
                        CreatePropertyChangeUndoCommand(cmds, bottom, "ZOrder", bottom.ZOrder, bottom.ZOrder - nTopNumber);
                        bottom.ZOrder = bottom.ZOrder - nTopNumber;
                        bIsZOrderChanged = true;

                        //Change top's children ZOrder
                        foreach (WidgetViewModBase child in topGroup.WidgetChildren)
                        {
                            CreatePropertyChangeUndoCommand(cmds, child, "ZOrder", child.ZOrder, child.ZOrder + 1);
                            child.ZOrder = child.ZOrder + 1;
                            bIsZOrderChanged = true;
                        }

                    }
                }
                else
                {
                    //Top object is a common widget
                    int nTopNumber = 1;

                    if (bottom.IsGroup == true)
                    {
                        GroupViewModel bottomGroup = bottom as GroupViewModel;
                        int nBottomNumber = bottomGroup.WidgetChildren.Count();

                        //Change bottom's children ZOrder
                        foreach (WidgetViewModBase child in bottomGroup.WidgetChildren)
                        {
                            CreatePropertyChangeUndoCommand(cmds, child, "ZOrder", child.ZOrder, child.ZOrder - nTopNumber);
                            child.ZOrder = child.ZOrder - nTopNumber;
                            bIsZOrderChanged = true;
                        }

                        //Change top's children ZOrder
                        CreatePropertyChangeUndoCommand(cmds, top, "ZOrder", top.ZOrder, top.ZOrder + nBottomNumber);
                        top.ZOrder = top.ZOrder + nBottomNumber;
                        bIsZOrderChanged = true;
                    }
                    else
                    {
                        //Change bottom's children ZOrder
                        CreatePropertyChangeUndoCommand(cmds, bottom, "ZOrder", bottom.ZOrder, bottom.ZOrder - nTopNumber);
                        bottom.ZOrder = bottom.ZOrder - nTopNumber;
                        bIsZOrderChanged = true;

                        //Change top's children ZOrder
                        CreatePropertyChangeUndoCommand(cmds, top, "ZOrder", top.ZOrder, top.ZOrder + nTopNumber);
                        top.ZOrder = top.ZOrder + nTopNumber;
                        bIsZOrderChanged = true;
                    }
                }

                //Switch the top and bottom
                AllObjs[i + 1] = bottom;
                AllObjs[i] = top;
            }//for(int i=nTotal-2; i>=0; i--)


            //Push Command to Undo/Redo stack
            if (bIsZOrderChanged == true)
            {
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }  
        
        }
        //This is discarded code, delete it later
        private void WidgetsBringBackwardCommandHandler2(object parameter)
        {
            List<WidgetViewModBase> targetWidgets = new List<WidgetViewModBase>();
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                if (wdgItem.IsGroup == false)
                {
                    targetWidgets.Add(wdgItem);
                }
                else
                {
                    GroupViewModel group = wdgItem as GroupViewModel;
                    targetWidgets.AddRange(group.WidgetChildren);
                }
            }

            if (targetWidgets.Count <= 0)
            {
                return;
            }

            List<WidgetViewModBase> allWidget = 
                Items.ToList<WidgetViewModBase>().Where(c => c.IsGroup == false).ToList<WidgetViewModBase>();

            CompositeCommand cmds = new CompositeCommand();

            // Make selected widgets Z-Order be decreased by 1.
            foreach (WidgetViewModBase targetItem in targetWidgets)
            {
                if (targetItem.ZOrder > 0)
                {
                    CreatePropertyChangeUndoCommand(cmds, targetItem, "ZOrder", targetItem.ZOrder, targetItem.ZOrder - 1);
                    targetItem.ZOrder -= 1;
                }
            }

            targetWidgets = targetWidgets.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();

            // Increase other widgets Z-Order if they have the same Z-Order with selected widgets updated Z-Order.
            foreach (WidgetViewModBase targetItem in targetWidgets)
            {
                foreach (WidgetViewModBase item in allWidget)
                {
                    if (targetWidgets.Contains(item) == false)
                    {
                        if (item.ZOrder == targetItem.ZOrder)
                        {
                            CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, item.ZOrder + 1, false);
                            item.ZOrder += 1;
                            break;
                        }
                    }
                }
            }

            PushToUndoStack(cmds);
            _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
        }
        public bool CanRunWidgetsBringBackwardCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }


        //Widgets BringBottom
        private void WidgetsBringBottomCommandHandler(object parameter)
        {
            if (_selectionService.WidgetNumber < 1)
            {
                return;
            }
            if (CanZOrderChange() == false)
            {
                return;
            }
            Guid itemID = _selectionService.GetSelectedWidgets()[0].ParentID;
            Guid realpGroupID = (_selectionService.GetSelectedWidgets()[0] as WidgetViewModBase).RealParentGroupGID;
            if (itemID == Guid.Empty)
            {
                BringBottomExternalObjects();
            }
            else
            {
                BringBottomChildrenObjects(itemID, realpGroupID);
            }   
        }
        private void BringBottomChildrenObjects(Guid groupID, Guid realGroupID)
        {
            //Create target list
            List<WidgetViewModBase> targetWidgets = new List<WidgetViewModBase>();
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                if (wdgItem.IsGroup == false)
                {
                    targetWidgets.Add(wdgItem);
                }
                else
                {
                    GroupViewModel group = wdgItem as GroupViewModel;
                    targetWidgets.AddRange(group.WidgetChildren);
                }
            }

            //Change the ZOrder
            CompositeCommand cmds = new CompositeCommand();
            bool bIsChanged = SetChildrenObject2Bottom(targetWidgets, cmds, groupID, realGroupID);

            //Send notify message to widget manager
            if (bIsChanged == true)
            {
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }   
        
        }
        private bool SetChildrenObject2Bottom(List<WidgetViewModBase> targetWidgets, CompositeCommand cmds, Guid groupID, Guid realGroupID)
        {
            if (targetWidgets.Count <= 0)
            {
                return false;
            }
            GroupViewModel ParentGroup = this.GetTargetGroup(groupID);

            if (ParentGroup == null)
            {
                return false;
            }
            if (ParentGroup.WidgetChildren.Count() < 2)
            {
                return false;
            }

            //
            List<WidgetViewModBase> allWidget = ParentGroup.GetSpecificGroupAllChildren(realGroupID);
            targetWidgets = targetWidgets.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            allWidget = allWidget.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            int minZOrder = allWidget.Min(a => a.ZOrder);

            
            int i = 0;
            int j = 0;
            bool bIsOrderChanged = false;
            foreach (WidgetViewModBase item in allWidget)
            {
                if (targetWidgets.Contains(item))
                {
                    int newValue = j++;
                    if (cmds != null)
                    {
                        CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue + minZOrder);
                    }
                    item.ZOrder = newValue + minZOrder; 
                    bIsOrderChanged = true;
                }
                else
                {
                    int newValue = targetWidgets.Count + (i++);
                    if (cmds != null)
                    {
                        CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue + minZOrder);
                    }
                    bIsOrderChanged = true;
                    item.ZOrder = newValue + minZOrder;
                }
            }


            if (cmds != null)
            {
                if (bIsOrderChanged == false)
                {
                    return false;
                }
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
            }
            return true;
        }
        private void BringBottomExternalObjects()
        {
            //Create target list
            List<WidgetViewModBase> targetWidgets = new List<WidgetViewModBase>();
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                if (wdgItem.IsGroup == false)
                {
                    targetWidgets.Add(wdgItem);
                }
                else
                {
                    GroupViewModel group = wdgItem as GroupViewModel;
                    targetWidgets.AddRange(group.WidgetChildren);
                }
            }

            //Change the ZOrder
            CompositeCommand cmds = new CompositeCommand();
            bool bIsChanged = SetTargetObject2Bottom(targetWidgets, cmds);

            //Send notify message to widget manager
            if (bIsChanged == true)
            {
                _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
            }         
        }
        private bool SetTargetObject2Bottom(List<WidgetViewModBase> targetWidgets, CompositeCommand cmds)
        {
            if (targetWidgets.Count <= 0)
            {
                return false;
            }

            targetWidgets = targetWidgets.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            List<WidgetViewModBase> allWidget = Items.ToList<WidgetViewModBase>().Where(c => c.IsGroup == false).ToList<WidgetViewModBase>();
            allWidget = allWidget.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            
            //Change ZOrder
            int i = 0;
            int j = 0;
            bool bIsOrderChanged = false;
            foreach (WidgetViewModBase item in allWidget)
            {
                if (targetWidgets.Contains(item))
                {
                    int newValue = j++;
                    if (cmds != null)
                    {
                        CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue, false);
                    }
                    item.ZOrder = newValue;
                    bIsOrderChanged = true;
                }
                else
                {
                    int newValue = targetWidgets.Count + (i++);
                    if (cmds != null)
                    {
                        CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue, false);
                    }
                    bIsOrderChanged = true;
                    item.ZOrder = newValue;
                }
            }

            if (cmds != null)
            {
                if (bIsOrderChanged == false)
                {
                    return false;
                }
                NotifyEventCommand notifyCmd = new NotifyEventCommand(CompositeEventType.ZorderChange);
                cmds.AddCommand(notifyCmd);
                PushToUndoStack(cmds);
            }
            return true;
        }
        private void WidgetsBringBottomCommandHandler2(object parameter)
        {
            List<WidgetViewModBase> targetWidgets = new List<WidgetViewModBase>();
            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                if (wdgItem.IsGroup == false)
                {
                    targetWidgets.Add(wdgItem);
                }
                else
                {
                    GroupViewModel group = wdgItem as GroupViewModel;
                    targetWidgets.AddRange(group.WidgetChildren);
                }
            }

            if (targetWidgets.Count <= 0)
            {
                return;
            }
            targetWidgets = targetWidgets.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();

            List<WidgetViewModBase> allWidget = Items.ToList<WidgetViewModBase>().Where(c => c.IsGroup == false).ToList<WidgetViewModBase>();
            allWidget = allWidget.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();

            CompositeCommand cmds = new CompositeCommand();

            int i = 0;
            int j = 0;
            foreach (WidgetViewModBase item in allWidget)
            {
                if (targetWidgets.Contains(item))
                {
                    int newValue = j++;
                    CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue);

                    item.ZOrder = newValue;
                }
                else
                {
                    int newValue = targetWidgets.Count + (i++);
                    CreatePropertyChangeUndoCommand(cmds, item, "ZOrder", item.ZOrder, newValue, false);

                    item.ZOrder = newValue;
                }
            }

            PushToUndoStack(cmds);
            _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
        }
        public bool CanRunWidgetsBringBottomCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private bool CanZOrderChange()
        {
            if (GetSelectWidgetsNum() < 1)
            {
                return false;
            }


            List<IWidgetPropertyData> AllSelects = _selectionService.GetSelectedWidgets();
            WidgetViewModBase first = AllSelects[0] as WidgetViewModBase;
            foreach (WidgetViewModBase it in AllSelects)
            {
                if (it.RealParentGroupGID!=first.RealParentGroupGID)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region  Align Widgets Command Handler
        //AlignLeft
        private void WidgetsAlignLeftCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            double iLeft = GetTargetRect(allSelects).Left;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                Rect rect = wdgItem.GetBoundingRectangle();
                double offset = rect.Left - iLeft;
                if (offset != 0)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, wdgItem, "Left", wdgItem.Raw_Left, wdgItem.Raw_Left - offset);
                    wdgItem.Raw_Left = wdgItem.Left - offset;

                    if (wdgItem.ParentID != Guid.Empty)
                    {
                        parentGID = wdgItem.ParentID;
                        updateGroupList.Add(wdgItem.ParentID);
                    }

                    // Update this group when undo/redo
                    if (wdgItem.IsGroup)
                    {
                        updateGroupList.Add(wdgItem.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        public bool CanRunWidgetsAlignLeftCommand
        {
            get
            {
                if (GetUnlockedWidgetsNum() > 1)
                {
                    return true;
                }

                return false;
            }
        }

        //AlignRight
        private void WidgetsAlignRightCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            double iRight = GetTargetRect(allSelects).Right;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                Rect rect = wdgItem.GetBoundingRectangle();
                double offset = rect.Right - iRight;
                if (offset != 0)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, wdgItem, "Left", wdgItem.Raw_Left, wdgItem.Raw_Left - offset);
                    wdgItem.Raw_Left = wdgItem.Left - offset;

                    if (wdgItem.ParentID != Guid.Empty)
                    {
                        parentGID = wdgItem.ParentID;
                        updateGroupList.Add(wdgItem.ParentID);
                    }

                    // Update this group when undo/redo
                    if (wdgItem.IsGroup)
                    {
                        updateGroupList.Add(wdgItem.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        public bool CanRunWidgetsAlignRightCommand
        {
            get
            {
                if (GetUnlockedWidgetsNum() > 1)
                    return true;
                return false;
            }
        }

        //AlignCenter
        private void WidgetsAlignCenterCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            Rect targeRect = GetTargetRect(allSelects);
            double dCenter = (targeRect.Left + targeRect.Right) / 2;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                Rect rect = wdgItem.GetBoundingRectangle();
                double offset = (rect.Left + rect.Right) / 2 - dCenter;
                if (offset != 0)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, wdgItem, "Left", wdgItem.Raw_Left, wdgItem.Raw_Left - offset);
                    wdgItem.Raw_Left = wdgItem.Left - offset;

                    if (wdgItem.ParentID != Guid.Empty)
                    {
                        parentGID = wdgItem.ParentID;
                        updateGroupList.Add(wdgItem.ParentID);
                    }

                    // Update this group when undo/redo
                    if (wdgItem.IsGroup)
                    {
                        updateGroupList.Add(wdgItem.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        public bool CanRunWidgetsAlignCenterCommand
        {
            get
            {
                if (GetUnlockedWidgetsNum() > 1)
                {
                    return true;
                }

                return false;
            }
        }

        //Align Top
        private void WidgetsAlignTopCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            double dTop= GetTargetRect(allSelects).Top;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                Rect rect = wdgItem.GetBoundingRectangle();
                double offset = rect.Top - dTop;
                if (offset != 0)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, wdgItem, "Top", wdgItem.Raw_Top, wdgItem.Raw_Top - offset);
                    wdgItem.Raw_Top = wdgItem.Top - offset;

                    if (wdgItem.ParentID != Guid.Empty)
                    {
                        parentGID = wdgItem.ParentID;
                        updateGroupList.Add(wdgItem.ParentID);
                    }

                    // Update this group when undo/redo
                    if (wdgItem.IsGroup)
                    {
                        updateGroupList.Add(wdgItem.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        public bool CanRunWidgetsAlignTopCommand
        {
            get
            {
                if (GetUnlockedWidgetsNum() > 1)
                {
                    return true;
                }
                return false;
            }
        }

        //Align Middle
        private void WidgetsAlignMiddleCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            Rect targetRect = GetTargetRect(allSelects);
            double dMiddle = (targetRect.Top + targetRect.Bottom) / 2;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                Rect rect = wdgItem.GetBoundingRectangle();
                double offset = (rect.Top + rect.Bottom) / 2 - dMiddle;
                if (offset != 0)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, wdgItem, "Top", wdgItem.Raw_Top, wdgItem.Raw_Top - offset);
                    wdgItem.Raw_Top=wdgItem.Top - offset;

                    if (wdgItem.ParentID != Guid.Empty)
                    {
                        parentGID = wdgItem.ParentID;
                        updateGroupList.Add(wdgItem.ParentID);
                    }

                    // Update this group when undo/redo
                    if (wdgItem.IsGroup)
                    {
                        updateGroupList.Add(wdgItem.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        public bool CanRunWidgetsAlignMiddleCommand
        {
            get
            {
                if (GetUnlockedWidgetsNum() > 1)
                {
                    return true;
                }
                return false;
            }
        }

        //Align Bottom
        private void WidgetsAlignBottomCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            double dBottom = GetTargetRect(allSelects).Bottom;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                Rect rect = wdgItem.GetBoundingRectangle();
                double offset = rect.Bottom - dBottom;
                if (offset != 0)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, wdgItem, "Top", wdgItem.Raw_Top, wdgItem.Raw_Top - offset);
                    wdgItem.Raw_Top = wdgItem.Top - offset;

                    if (wdgItem.ParentID != Guid.Empty)
                    {
                        parentGID = wdgItem.ParentID;
                        updateGroupList.Add(wdgItem.ParentID);
                    }

                    // Update this group when undo/redo
                    if (wdgItem.IsGroup)
                    {
                        updateGroupList.Add(wdgItem.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        public bool CanRunWidgetsAlignBottomCommand
        {
            get
            {
                if (GetUnlockedWidgetsNum() > 1)
                {
                    return true;
                }
                return false;
            }
        }

        //Widgets Distribute Horizontally
        private void WidgetsDistributeHorizontallyCommandHandler(object parameter)
        {
            double dMinLeft = double.MaxValue;
            double dMaxRight = double.MinValue;
            double dCountWidth = 0;

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                Rect rec = wdgItem.GetBoundingRectangle();
                if (rec.Left < dMinLeft)
                {
                    dMinLeft = rec.Left;
                }

                if (rec.Right > dMaxRight)
                {
                    dMaxRight = rec.Right;
                }

                dCountWidth += rec.Width;
            }

            int dInterval = Convert.ToInt32(((dMaxRight - dMinLeft) - dCountWidth) / (allSelects.Count - 1));

            allSelects.Sort(CompareByLeft);

            WidgetViewModBase CurDate;
            WidgetViewModBase PreDate;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;

            for (int i = 1; i < allSelects.Count - 1; i++)
            {
                CurDate = allSelects[i] as WidgetViewModBase;
                PreDate = allSelects[i - 1] as WidgetViewModBase;
                if (CurDate != null && PreDate != null)
                {
                    Rect CurRec = CurDate.GetBoundingRectangle();
                    Rect PrcRec = PreDate.GetBoundingRectangle();
                    double newValue = (PrcRec.Right + dInterval);
                    double offset = newValue - CurRec.Left;
                    CreatePropertyDeltaChangeUndoCommand(cmds, CurDate, "Left", CurDate.Raw_Left, CurDate.Raw_Left + offset);
                    CurDate.Raw_Left = CurDate.Left + offset;

                    if (CurDate.ParentID != Guid.Empty)
                    {
                        parentGID = CurDate.ParentID;
                        updateGroupList.Add(CurDate.ParentID);
                    }

                    // Update this group when undo/redo
                    if (CurDate.IsGroup)
                    {
                        updateGroupList.Add(CurDate.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        public bool CanRunWidgetsDistributeHorizontallyCommand
        {
            get
            {
                if (GetUnlockedWidgetsNum() > 2)
                {
                    return true;
                }
                return false;
            }
        }

        //Widgets Distribute Vertically
        private void WidgetsDistributeVerticallyCommandHandler(object parameter)
        {
            double dMinTop = double.MaxValue;
            double dMaxBottom = double.MinValue;
            double dCountHeight = 0;

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                Rect rec = wdgItem.GetBoundingRectangle();
                if (rec.Top < dMinTop)
                {
                    dMinTop = rec.Top;
                }

                if (rec.Bottom > dMaxBottom)
                {
                    dMaxBottom = rec.Bottom;
                }

                dCountHeight += rec.Height;
            }
            double dInterval = Convert.ToInt32(((dMaxBottom - dMinTop) - dCountHeight) / (allSelects.Count - 1));

            allSelects.Sort(CompareByTop);

            WidgetViewModBase CurDate;
            WidgetViewModBase PreDate;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            for (int i = 1; i < allSelects.Count - 1; i++)
            {
                CurDate = allSelects[i] as WidgetViewModBase;
                PreDate = allSelects[i - 1] as WidgetViewModBase;
                if (CurDate != null && PreDate != null)
                {
                    Rect CurRec = CurDate.GetBoundingRectangle();
                    Rect PrcRec = PreDate.GetBoundingRectangle();
                    double newValue = (PrcRec.Bottom + dInterval);
                    double offset = newValue - CurRec.Top;
                    CreatePropertyDeltaChangeUndoCommand(cmds, CurDate, "Top", CurDate.Raw_Top, CurDate.Raw_Top + offset);
                    CurDate.Raw_Top = CurDate.Top + offset;

                    if (CurDate.ParentID != Guid.Empty)
                    {
                        parentGID = CurDate.ParentID;
                        updateGroupList.Add(CurDate.ParentID);
                    }

                    // Update this group when undo/redo
                    if (CurDate.IsGroup)
                    {
                        updateGroupList.Add(CurDate.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        public bool CanRunWidgetsDistributeVerticallyCommand
        {
            get
            {
                if (GetUnlockedWidgetsNum() > 2)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Widget Propery Routed Command Handler
        List<Guid> updateGroupList = new List<Guid>();
        private void AdjustWidgetSize(CompositeCommand cmds, List<WidgetViewModBase> list, bool bHeight, bool bInCrease)
        {
            List<Guid> updateGroupList = new List<Guid>();
            foreach (WidgetViewModBase data in list)
            {
                if (data.IsGroup)
                {
                    AdjustWidgetSize(cmds, ((GroupViewModel)data).WidgetChildren, bHeight, bInCrease);
                }
                else
                {
                    if (bHeight)
                    {
                        double oldCenter = data.Top + (data.ItemHeight / 2);
                        double adjustValue = data.ItemHeight/12;
                        double newValue = (bInCrease) ? (data.ItemHeight + adjustValue) : (data.ItemHeight - adjustValue);

                        newValue = (newValue < 10) ? 10 : newValue;

                        PropertyChangeCommand cmd = new PropertyChangeCommand(data, "ItemHeight", data.ItemHeight, newValue);
                        cmds.AddCommand(cmd);
                        data.ItemHeight = newValue;

                        double newTop = oldCenter - (data.ItemHeight / 2);
                      //  newTop = (newTop < 1) ? 1 : newTop;
                        CreateWidthChangeUndoCommand(cmds, data, data.Top, newTop);
                        data.Top = newTop;
                    }
                    else
                    {
                        double oldCenter = data.Left + (data.ItemWidth / 2);
                        double adjustValue = data.ItemWidth / 12;
                        double newValue = (bInCrease) ? (data.ItemWidth + adjustValue) : (data.ItemWidth - adjustValue);

                        newValue = (newValue < 10) ? 10 : newValue;
                        CreateWidthChangeUndoCommand(cmds, data, data.ItemWidth, newValue);
                        data.ItemWidth = newValue;

                        double newLeft = oldCenter - (data.ItemWidth / 2);
                        //  newLeft = (newLeft < 1) ? 1 : newLeft;
                        CreateWidthChangeUndoCommand(cmds, data, data.Left, newLeft);
                        data.Left = newLeft;
                    }
                   
                }
            }

            foreach (Guid guid in updateGroupList)
            {
                UpdateGroup(guid);
            }
        }

        private void WidgetsIncreaseWidthCommandHandler(object parameter)
        {
            bool bUndoStactk = true;
            if (parameter is bool)
            {
                bUndoStactk = Convert.ToBoolean(parameter);
            }

            CompositeCommand cmds = new CompositeCommand();

            List<Guid> updateGroupList = new List<Guid>();
            ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
            if (select != null)
            {
                List<IWidgetPropertyData> allSelects = select.GetSelectedWidgets();

                foreach (WidgetViewModBase data in allSelects)
                {
                    double oldCenter = data.Left + (data.ItemWidth / 2);
                    double adjustValue = data.ItemWidth / 12;
                    double newValue = data.ItemWidth + adjustValue;

                    newValue = (newValue < 10) ? 10 : newValue;

                    CreateWidthChangeUndoCommand(cmds, data, data.ItemWidth, newValue);
                    data.ItemWidth = newValue;

                    double newLeft = oldCenter - (data.ItemWidth / 2);
                    //  newTop = (newTop < 1) ? 1 : newTop;
                    CreatePropertyDeltaChangeUndoCommand(cmds, data, "Left", data.Left, newLeft);
                    data.Left = newLeft;

                    if (data.ParentID != Guid.Empty)
                    {
                        updateGroupList.Add(data.ParentID);
                    }

                    if (data.IsGroup)
                    {
                        updateGroupList.Add(data.WidgetID);
                    }
                }

               
                foreach (Guid id in updateGroupList)
                {
                    UpdateGroup(id);
                }

                if (bUndoStactk)
                {
                    PushToUndoStack(cmds);
                }
            }
        }

        private void WidgetsIncreaseHeightCommandHandler(object parameter)
        {

            bool bUndoStactk = true;
            if (parameter is bool)
            {
                bUndoStactk = Convert.ToBoolean(parameter);
            }

            CompositeCommand cmds = new CompositeCommand();

            List<Guid> updateGroupList = new List<Guid>();
            ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
            if (select != null)
            {
                List<IWidgetPropertyData> allSelects = select.GetSelectedWidgets();

                foreach (WidgetViewModBase data in allSelects)
                {

                    double oldCenter = data.Top + (data.ItemHeight / 2);
                    double adjustValue = data.ItemHeight / 12;
                    double newValue = data.ItemHeight + adjustValue;

                    newValue = (newValue < 10) ? 10 : newValue;

                    CreateHeightChangeUndoCommand(cmds, data, data.ItemHeight, newValue);
                    data.ItemHeight = newValue;

                    double newTop = oldCenter - (data.ItemHeight / 2);
                    //  newTop = (newTop < 1) ? 1 : newTop;
                    CreatePropertyDeltaChangeUndoCommand(cmds, data,"Top", data.Top, newTop);
                    data.Top = newTop;

                    if (data.ParentID != Guid.Empty)
                    {
                        updateGroupList.Add(data.ParentID);
                    }

                    if (data.IsGroup)
                    {
                        updateGroupList.Add(data.WidgetID);
                    }
                }

               
                if (bUndoStactk)
                {
                    PushToUndoStack(cmds);
                }

                foreach (Guid id in updateGroupList)
                {
                    UpdateGroup(id);
               }
            }
        }

        private void WidgetsDecreaseWidthCommandHandler(object parameter)
        {
            bool bUndoStactk = true;
            if (parameter is bool)
            {
                bUndoStactk = Convert.ToBoolean(parameter);
            }

            CompositeCommand cmds = new CompositeCommand();

            List<Guid> updateGroupList = new List<Guid>();
            ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
            if (select != null)
            {
                List<IWidgetPropertyData> allSelects = select.GetSelectedWidgets();

                foreach (WidgetViewModBase data in allSelects)
                {

                    double oldCenter = data.Left + (data.ItemWidth / 2);
                    double adjustValue = data.ItemWidth / 12;
                    double newValue = data.ItemWidth - adjustValue;

                    newValue = (newValue < 10) ? 10 : newValue;

                    CreateWidthChangeUndoCommand(cmds, data, data.ItemWidth, newValue);
                    data.ItemWidth = newValue;

                    double newLeft = oldCenter - (data.ItemWidth / 2);
                    //  newTop = (newTop < 1) ? 1 : newTop;
                    CreatePropertyDeltaChangeUndoCommand(cmds, data, "Left", data.Left, newLeft);
                    data.Left = newLeft;

                    if (data.ParentID != Guid.Empty)
                    {
                        updateGroupList.Add(data.ParentID);
                    }

                    if (data.IsGroup)
                    {
                        updateGroupList.Add(data.WidgetID);
                    }
                }

                //AdjustWidgetSize(cmds, templist, true, true);

                foreach (Guid id in updateGroupList)
                {
                    UpdateGroup(id);
                }

                if (bUndoStactk)
                {
                    PushToUndoStack(cmds);
                }
            }
        }

        private void WidgetsDecreaseHeightCommandHandler(object parameter)
        {
            bool bUndoStactk = true;
            if (parameter is bool)
            {
                bUndoStactk = Convert.ToBoolean(parameter);
            }

            CompositeCommand cmds = new CompositeCommand();

            List<Guid> updateGroupList = new List<Guid>();
            ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
            if (select != null)
            {
                List<IWidgetPropertyData> allSelects = select.GetSelectedWidgets();

                foreach (WidgetViewModBase data in allSelects)
                {

                    double oldCenter = data.Top + (data.ItemHeight / 2);
                    double adjustValue = data.ItemHeight / 12;
                    double newValue = data.ItemHeight - adjustValue;

                    newValue = (newValue < 10) ? 10 : newValue;

                    CreateHeightChangeUndoCommand(cmds, data, data.ItemHeight, newValue);
                    data.ItemHeight = newValue;

                    double newTop = oldCenter - (data.ItemHeight / 2);
                    //  newTop = (newTop < 1) ? 1 : newTop;
                    CreatePropertyDeltaChangeUndoCommand(cmds, data, "Top", data.Top, newTop);
                    data.Top = newTop;

                    if (data.ParentID != Guid.Empty)
                    {
                        updateGroupList.Add(data.ParentID);
                    }

                    if (data.IsGroup)
                    {
                        updateGroupList.Add(data.WidgetID);
                    }
                }

                //AdjustWidgetSize(cmds, templist, true, true);

                foreach (Guid id in updateGroupList)
                {
                    UpdateGroup(id);
                }

                if (bUndoStactk)
                {
                    PushToUndoStack(cmds);
                }
            }
        }

        public bool CanRunWidgetSizeAdjustCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsLeftChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;

            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.Left)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, item, "Left", item.Left, value);

                    item.Left = value;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }
        public bool CanRunWidgetsLeftChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsTopChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.Top)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, item, "Top", item.Top, value);

                    item.Top = value;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }
        public bool CanRunWidgetsTopChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsWidthChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.ItemWidth)
                {
                    CreateWidthChangeUndoCommand(cmds, item, item.ItemWidth, value);

                    // keep width/height the same ratio 
                    if(GlobalData.IsLockRatio)
                    {
                        double newHeight = item.ItemHeight * (value / item.ItemWidth);
                        CreateHeightChangeUndoCommand(cmds, item, item.ItemHeight, newHeight);
                        item.ItemHeight = newHeight;
                    }

                    item.ItemWidth = value;

                    //Dynamic panel: reload children.
                    if(item is DynamicPanelViewModel)
                    {
                        _ListEventAggregator.GetEvent<RefreshWidgetChildPageEvent>().Publish(item.widgetGID);
                        RefreshDynamicPanelCommand cmd = new RefreshDynamicPanelCommand(item.widgetGID);
                        cmds.AddCommand(cmd);
                    }

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }
        public bool CanRunWidgetsWidthChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsHeightChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (item is RadioButtonWidgetViewModel
                    || item is CheckBoxWidgetViewModel)
                {
                    continue;
                }

                if (value != item.ItemHeight)
                {
                    CreateHeightChangeUndoCommand(cmds, item, item.ItemHeight, value);

                    // keep width/height the same ratio 
                    if (GlobalData.IsLockRatio)
                    {
                        double newWidth = item.ItemWidth * (value / item.ItemHeight);
                        CreateWidthChangeUndoCommand(cmds, item, item.ItemWidth, newWidth);
                        item.ItemWidth = newWidth;
                    }

                    item.ItemHeight = value;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }
        public bool CanRunWidgetsHeightChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsNameChangeCommandHandler(object parameter)
        {
            string value = parameter as string;
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();

            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.Name)
                {
                    PropertyChangeCommand cmd = new PropertyChangeCommand(item, "Name", item.Name, value);
                    cmds.AddCommand(cmd);
                    item.Name = value;
                }
            }

            PushToUndoStack(cmds);
        }
        public bool CanRunWidgetsNameChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsRotateChangeCommandHandler(object parameter)
        {
            int value = (int)parameter;
            List<IWidgetPropertyData> wdgs = GetEligibleWidgets(PropertyOption.Option_WidgetRotate);
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.RotateAngle)
                {
                    CreatePropertyChangeUndoCommand(cmds, item, "RotateAngle", item.RotateAngle, value);
                    item.RotateAngle = value;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (parentGID != Guid.Empty)
                    {
                        UpdateGroup(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }
            PushToUndoStack(cmds, updateGroupList);
        }
        public bool CanRunWidgetsRotateChangeCommand
        {
            get
            {
                return IsSupportProperty(PropertyOption.Option_WidgetRotate);
            }
        }

        private void WidgetsCornerRadiusChangeCommandHandler(object parameter)
        {
            int value = (int)parameter;
            List<IWidgetPropertyData> wdgs = GetEligibleWidgets(PropertyOption.Option_CornerRadius);
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            foreach (WidgetViewModBase item in wdgs)
            {
                double maxLimit = Math.Min(item.ItemWidth, item.ItemHeight) / 2;
                int newValue=(int)Math.Min(value, maxLimit);

                if (value != item.CornerRadius)
                {
                    CreatePropertyChangeUndoCommand(cmds, item, "CornerRadius", item.CornerRadius, newValue);
                    item.CornerRadius = newValue;
                }
            }
            PushToUndoStack(cmds);
        }

        public bool CanRunWidgetsCornerRadiusChangeCommand
        {
            get
            {
                return IsSupportProperty(PropertyOption.Option_CornerRadius);
            }
        }

        private void WidgetsTextRotateChangeCommandHandler(object parameter)
        {
            int value = (int)parameter;
            List<IWidgetPropertyData> wdgs = GetEligibleWidgets(PropertyOption.Option_TextRotate);
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.TextRotate && item.IsSupportText == true)
                {
                    CreatePropertyChangeUndoCommand(cmds, item, "TextRotate", item.TextRotate, value);
                    item.TextRotate = value;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGrooupList.Add(parentGID);
                    }
                    if (parentGID != Guid.Empty)
                    {
                        UpdateGroup(parentGID);
                    }
                }
            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        public bool CanRunWidgetsTextRotateChangeCommand
        {
            get
            {
                return IsSupportProperty(PropertyOption.Option_TextRotate);
            }
        }

        private void WidgetsOpacityChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();

            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.Opacity)
                {
                    CreatePropertyChangeUndoCommand(cmds, item, "Opacity", item.Opacity, value);

                    item.Opacity = value;
                }
            }

            PushToUndoStack(cmds);
        }
        public bool CanRunWidgetsOpacityChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsHideChangeCommandHandler(object parameter)
        {
            bool? value = (bool?)parameter;

            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;
            bool? ResValue = value;
            if (value.HasValue == false)
            {
                ResValue = false;
            }

            CompositeCommand cmds = new CompositeCommand();

            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.IsHidden)
                {
                    bool newValue = (bool)ResValue;
                    CreatePropertyChangeUndoCommand(cmds, item, "IsHidden", item.IsHidden, newValue);

                    item.IsHidden = newValue;
                }
            }

            PushToUndoStack(cmds);
        }
        public bool CanRunWidgetsHideChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }
        private void WidgetsIsFixedChangeCommandHandler(object parameter)
        {
            bool? value = (bool?)parameter;

            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;
            bool? ResValue = value;
            if (value.HasValue == false)
            {
                ResValue = false;
            }

            CompositeCommand cmds = new CompositeCommand();

            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.IsFixed)
                {
                    bool newValue = (bool)ResValue;
                    CreatePropertyChangeUndoCommand(cmds, item, "IsFixed", item.IsFixed, newValue);

                    item.IsFixed = newValue;
                }
            }

            PushToUndoStack(cmds);
        }

        public bool CanRunWidgetsIsFixedChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsTooltipChangeCommandHandler(object parameter)
        {
            string value = parameter as string;

            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();

            foreach (WidgetViewModBase item in wdgs)
            {
                if (value != item.Tooltip)
                {
                    if (item.IsGroup == false)
                    {
                        PropertyChangeCommand cmd = new PropertyChangeCommand(item, "Tooltip", item.Tooltip, value);
                        cmds.AddCommand(cmd);
                    }

                    item.Tooltip = value;
                }
            }

            PushToUndoStack(cmds);
        }
        public bool CanRunWidgetsTooltipChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }
        private void WidgetsImportImageChangeCommandHandler(object parameter)
        {
            this.ImageEditorViewmodel.CancleEdit();
            bool bIsImport = (null != parameter);
            if (bIsImport)
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                var imgWdgs = wdgs.OfType<ImageWidgetViewModel>();
                if (imgWdgs.Count() != 1)
                    return;
                ImageWidgetViewModel img = imgWdgs.First() as ImageWidgetViewModel;
                if (img == null)
                    return;
                img.ImportImg();
                if (img.ParentID != Guid.Empty)
                {
                    UpdateGroup(img.ParentID);
                }

            }
            else
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                var imgWdgs = wdgs.OfType<ImageWidgetViewModel>();
                if (imgWdgs.Count() != 1)
                    return;
                ImageWidgetViewModel img = imgWdgs.First() as ImageWidgetViewModel;
                if (img == null)
                    return;
                img.ClearImg();
                if (img.ParentID != Guid.Empty)
                {
                    UpdateGroup(img.ParentID);
                }
            }
        }
        public bool CanRunWidgetsImportImageChangeCommand
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                var imgWdgs = wdgs.OfType<ImageWidgetViewModel>();
                if (imgWdgs.Count() == 1)
                {
                    if (imgWdgs.First() != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void WidgetsSliceCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            var imgWdgs = wdgs.OfType<ImageWidgetViewModel>();
            if (imgWdgs.Count() != 1)
            {
                return;
            }
            ImageWidgetViewModel img = imgWdgs.First();
            if (img.ParentID != Guid.Empty)
            {
                // TODO: At present, the item in group cannot be deleted. Update this when this feature is implemented
                return;
            }
            if (img == null || img.ImgStream == null)
                return;
            if (!this.IsImageEditorVisible)
            {
                this.IsImageEditorVisible = true;
            }

            this.ImageEditorViewmodel.SetEditType(ImageEditType.Slice, img);
        }
        public bool CanRunWidgetsSliceCommand
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                var imgWdgs = wdgs.OfType<ImageWidgetViewModel>();
                if (imgWdgs.Count() == 1)
                {
                    if (imgWdgs.First() != null && imgWdgs.First().ImgStream != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void WidgetsCropCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            var imgWdgs = wdgs.OfType<ImageWidgetViewModel>();
            if (imgWdgs.Count() != 1)
            {
                return;
            }
            ImageWidgetViewModel img = imgWdgs.First();
            if (img.ParentID != Guid.Empty)
            {
                // TODO: At present, the item in group cannot be deleted. Update this when this feature is implemented
                return;
            }
            if (img == null || img.ImgStream == null)
                return;
            if (!this.IsImageEditorVisible)
            {
                this.IsImageEditorVisible = true;
            }

            this.ImageEditorViewmodel.SetEditType(ImageEditType.Crop, img);
        }
        public bool CanRunWidgetsCropCommand
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                var imgWdgs = wdgs.OfType<ImageWidgetViewModel>();
                if (imgWdgs.Count() == 1)
                {
                    if (imgWdgs.First() != null && imgWdgs.First().ImgStream != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }



        private void WidgetsEnableChangeCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            bool value = (bool)par[0];
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("IsDisabled");
                bool oldValue = Convert.ToBoolean(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "IsDisabled", oldValue, value);
                propertyInfo.SetValue(item, value, null);

                if (item.ParentID != Guid.Empty)
                {
                    parentGID = item.ParentID;
                    updateGrooupList.Add(parentGID);
                }
                if (parentGID != Guid.Empty)
                {
                    UpdateGroup(parentGID);
                }

            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        public bool CanRunWidgetsEnableChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsShowSelectChangeCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            bool value = (bool)par[0];
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("IsShowSelect");
                bool oldValue = Convert.ToBoolean(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "IsShowSelect", oldValue, value);
                propertyInfo.SetValue(item, value, null);

                if (item.ParentID != Guid.Empty)
                {
                    parentGID = item.ParentID;
                    updateGrooupList.Add(parentGID);
                }
                if (parentGID != Guid.Empty)
                {
                    UpdateGroup(parentGID);
                }

            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        public bool CanRunWidgetsShowSelectChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsButtonAlignCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            bool value = (bool)par[0];
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("IsBtnAlignLeft");
                bool oldValue = Convert.ToBoolean(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "IsBtnAlignLeft", oldValue, value);
                propertyInfo.SetValue(item, value, null);

                if (item.ParentID != Guid.Empty)
                {
                    parentGID = item.ParentID;
                    updateGrooupList.Add(parentGID);
                }
                if (parentGID != Guid.Empty)
                {
                    UpdateGroup(parentGID);
                }

            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        public bool CanRunWidgetsButtonAlignChangeCommand
        {
            get
            {
                return (GetSelectWidgetsNum() > 0);
            }
        }

        private void WidgetsRadioGroupSetCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            string value = (string)par[0];
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("RadioGroup");
                string oldValue = Convert.ToString(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "RadioGroup", oldValue, value);
                propertyInfo.SetValue(item, value, null);

                if (item.ParentID != Guid.Empty)
                {
                    parentGID = item.ParentID;
                    updateGrooupList.Add(parentGID);
                }
                if (parentGID != Guid.Empty)
                {
                    UpdateGroup(parentGID);
                }

            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        private void WidgetsHideBorderCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            bool value = (bool)par[0];
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("IsHideBorder");
                bool oldValue = Convert.ToBoolean(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "IsHideBorder", oldValue, value);
                propertyInfo.SetValue(item, value, null);
            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        private void WidgetsReadOnlyCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            bool value = (bool)par[0];
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("IsReadOnly");
                bool oldValue = Convert.ToBoolean(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "IsReadOnly", oldValue, value);
                propertyInfo.SetValue(item, value, null);
            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        private void WidgetsHintTextCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            string value = par[0] as string;
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("HintText");
                string oldValue = Convert.ToString(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "HintText", oldValue, value);
                propertyInfo.SetValue(item, value, null);
            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        private void WidgetsMaxLengthCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            int value = (int)par[0];
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("MaxTextLength");
                int oldValue = Convert.ToInt32(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "MaxTextLength", oldValue, value);
                propertyInfo.SetValue(item, value, null);
            }
            PushToUndoStack(cmds, updateGrooupList);
        }
        private void WidgetsTextFieldTypeCommandHandler(object parameter)
        {
            List<object> par = parameter as List<object>;
            TextFieldType value = (TextFieldType)par[0];
            List<WidgetViewModBase> wdgs = par[1] as List<WidgetViewModBase>;
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGrooupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                PropertyInfo propertyInfo = item.GetType().GetProperty("TextFieldType");
                TextFieldType oldValue = (TextFieldType)(propertyInfo.GetValue(item, null));
                CreatePropertyChangeUndoCommand(cmds, item, "TextFieldType", oldValue, value);
                propertyInfo.SetValue(item, value, null);
            }
            PushToUndoStack(cmds, updateGrooupList);
        }

        #endregion

        #region hamburger menu Command Handler
        private void MenuPageLeftChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;

            IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
            if (wdgs.Count() == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (HamburgerMenuViewModel item in wdgs)
            {
                if (value != item.MenuPageLeft)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, item, "MenuPageLeft", item.MenuPageLeft, value);

                    item.MenuPageLeft = value;
                    this.MenuPageEditorViewModel.UpdateHanburgerEditor();

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        private void MenuPageTopChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;

            IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
            if (wdgs.Count() == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (HamburgerMenuViewModel item in wdgs)
            {
                if (value != item.MenuPageTop)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, item, "MenuPageTop", item.MenuPageTop, value);

                    item.MenuPageTop = value;
                    this.MenuPageEditorViewModel.UpdateHanburgerEditor();

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        private void MenuPageWidthChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;

            IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
            if (wdgs.Count() == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (HamburgerMenuViewModel item in wdgs)
            {
                if (value != item.MenuPageWidth)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, item, "MenuPageWidth", item.MenuPageWidth, value);

                    item.MenuPageWidth = value;
                    this.MenuPageEditorViewModel.UpdateHanburgerEditor();

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        private void MenuPageHeightChangeCommandHandler(object parameter)
        {
            double value = (double)parameter;
            IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
            if (wdgs.Count() == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (HamburgerMenuViewModel item in wdgs)
            {
                if (value != item.MenuPageHeight)
                {
                    CreatePropertyDeltaChangeUndoCommand(cmds, item, "MenuPageHeight", item.MenuPageHeight, value);

                    item.MenuPageHeight = value;
                    this.MenuPageEditorViewModel.UpdateHanburgerEditor();

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        private void MenuPageHideChangeCommandHandler(object parameter)
        {
            bool? value = (bool?)parameter;

            IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
            if (wdgs.Count() == 0)
                return;
            bool? ResValue = value;
            if (value.HasValue == false)
            {
                ResValue = false;
            }

            CompositeCommand cmds = new CompositeCommand();

            foreach (HamburgerMenuViewModel item in wdgs)
            {
                if (value != item.IsMenuPageHidden)
                {
                    bool newValue = (bool)ResValue;
                    CreatePropertyChangeUndoCommand(cmds, item, "IsMenuPageHidden", item.IsMenuPageHidden, newValue);

                    item.IsMenuPageHidden = newValue;
                }
            }

            PushToUndoStack(cmds);
        }
        #endregion

        #region Toolbar Location Command Handler
        private void WidgetsAllTopChangeCommandHandler(object parameter)
        {
            double offset = (double)parameter;
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (offset != 0)
                {
                    double newValue = offset + item.Top;

                    CreatePropertyDeltaChangeUndoCommand(cmds, item, "Top", item.Raw_Top, newValue);

                    item.Raw_Top = newValue;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            PushToUndoStack(cmds, updateGroupList);
        }
        private void WidgetsAllLeftChangeCommandHandler(object parameter)
        {
            double offset = (double)parameter;
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (offset != 0)
                {
                    double newValue = offset + item.Left;

                    CreatePropertyDeltaChangeUndoCommand(cmds, item, "Left", item.Raw_Left, newValue);

                    item.Raw_Left = newValue;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            PushToUndoStack(cmds, updateGroupList);
        }

        private void WidgetsLockCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (item.IsLocked == false)
                {
                    CreatePropertyChangeUndoCommand(cmds, item, "IsLocked", false, true);

                    item.IsLocked = true;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    // Update this group when undo/redo
                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }
        public bool CanRunWidgetsLockCommand
        {
            get
            {
                if (GetSelectWidgetsNum() <= 0)
                {
                    return false;
                }
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (item.IsLocked == false)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void WidgetsUnlockCommandHandler(object parameter)
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return;

            CompositeCommand cmds = new CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();

            Guid parentGID = Guid.Empty;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (item.IsLocked == true)
                {
                    if (item is MasterWidgetViewModel)
                    {
                        if ((item as MasterWidgetViewModel).IsLocked2MasterLocation == true)
                        {
                            continue;
                        }
                    }
                    CreatePropertyChangeUndoCommand(cmds, item, "IsLocked", true, false);

                    item.IsLocked = false;

                    if (item.ParentID != Guid.Empty)
                    {
                        parentGID = item.ParentID;
                        updateGroupList.Add(parentGID);
                    }

                    // Update this group when undo/redo
                    if (item.IsGroup)
                    {
                        updateGroupList.Add(item.WidgetID);
                    }
                }
            }

            if (parentGID != Guid.Empty)
            {
                UpdateGroup(parentGID);
            }

            PushToUndoStack(cmds, updateGroupList);
        }
        public bool CanRunWidgetsUnlockCommand
        {
            get
            {
                if (GetSelectWidgetsNum() <= 0)
                {
                    return false;
                }
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (item.IsLocked == true)
                    {
                        if(item is MasterWidgetViewModel)
                        {
                            if((item as MasterWidgetViewModel).IsLocked2MasterLocation==false)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }                        
                    }
                }
                return false;
            }
        }
        #endregion

        #region Grid and Guide Comman Handler

        public bool CanRunGridGuideCommand
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return (doc.Document != null);
            }
        }

        public bool CanRunCreateGuideCommand
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                IPagePropertyData page = SelectionService.GetCurrentPage();
                return (doc.Document != null && page != null);
            }
        }
        private void ShowGridCommandHandler(object obj)
        {
            IsGridVisible = !GlobalData.IsShowGrid;
        }
        private void SnaptoGridCommandHandler(object obj)
        {
            GlobalData.IsSnapToGrid = !GlobalData.IsSnapToGrid;
        }

        private void GridSettingCommandHandler(object obj)
        {
            OpenGridGuideSettingWindow(SettingType.Grid);
        }
        private void ShowGlobalGuidesCommandHandler(object obj)
        {
            IsGlobalGuideVisible = !IsGlobalGuideVisible;
        }
        private void ShowPageGuidesCommandHandler(object obj)
        {
            IsPageGuideVisible = !IsPageGuideVisible;
        }
        private void SnapToGuidesCommandHandler(object obj)
        {
            GlobalData.IsSnapToGuide = !GlobalData.IsSnapToGuide;
        }
        private void LockGuidesCommandHandler(object obj)
        {
            GlobalData.IsLockGuides = !GlobalData.IsLockGuides;
            _ListEventAggregator.GetEvent<GlobalLockGuides>().Publish(null);
        }
        private void CreateGuidesCommandHandler(object obj)
        {
            CreateGuidesWindow win = new CreateGuidesWindow(_pageGID);
            win.Owner = Application.Current.MainWindow;
            bool? bRValue = win.ShowDialog();
            if ((bool)bRValue)
            {
                _ListEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);
            }
        }
        private void DeleteAllGuidesCommandHandler(object obj)
        {
            _ListEventAggregator.GetEvent<DeleteGuideEvent>().Publish(new GuideInfo(GuideType.Global, null));
        }
        private void GuidesSettingCommandHandler(object obj)
        {
            OpenGridGuideSettingWindow(SettingType.Guides);
        }
        private void SnapToObjectCommandHandler(object obj)
        {

        }
        private void ObjectSnapSettingCommandHandler(object obj)
        {
            OpenGridGuideSettingWindow(SettingType.OjectSnap);
        }

        private void OpenGridGuideSettingWindow(SettingType type)
        {
            GridGuideSetting win = new GridGuideSetting(type);
            win.Owner = Application.Current.MainWindow;
            bool? bRValue = win.ShowDialog();
            if ((bool)bRValue)
            {
                _ListEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);
            }
        }

        #endregion

        #region Format paint

        private void SetCursorOnWidget(bool val)
        {
           // IsFormatCursor = val;
            try
            {
                if (val)
                {
                    //StreamResourceInfo info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Naver.Compass.Module.DiagramEditor;component/Image/Format.cur", UriKind.RelativeOrAbsolute));

                    CanvasCursor = CommonFunction.GetFormatCur() ;
                }
                else
                {
                    CanvasCursor = null;
                }
            }
            catch (System.Exception ex)
            {

            }
            foreach (WidgetViewModBase wdg in Items)
            {
                wdg.IsBrushModel = val;
            }
        }

        private void FormatPaintCommandHandler(object parameter)
        {
            IFormatPainterService _Format = ServiceLocator.Current.GetInstance<FormatPainterService>();

            if (IsSelected)
            {
                List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();
                if (allSelects.Count == 1 && !_Format.GetMode())
                {
                    _Format.SetPaintFormat(allSelects[0] as WidgetViewModelDate);
                    _Format.SetMode(true);
                }
            }

            SetCursorOnWidget(true);
        }

        private void CancelFormatPaintCommandHandler(object parameter)
        {
            if (parameter != null)
            {
                IFormatPainterService _Format = ServiceLocator.Current.GetInstance<FormatPainterService>();
                if (_Format.GetMode() && IsSelected)
                {
                    if ((bool)parameter)
                    {
                        OnPaintFormat(_Format);
                    }
                    _Format.SetMode(false);
                }
                SetCursorOnWidget(false);
            }
        }

        private void MouseOverInteractionObjectHandler(IUniqueObject targetObject)
        {
            //Mouse leave
            if (targetObject == null)
            {
                InteractionObjectHeight = 0;
                InteractionObjectWidth = 0;
            }
            else
            {// Mouse over

                WidgetViewModBase widget = Items.FirstOrDefault(x => x.WidgetID == targetObject.Guid);
                if (widget == null && targetObject is IGroup)
                {
                    //Child group
                    GroupViewModel group = new GroupViewModel(targetObject as IGroup);
                    foreach (WidgetViewModBase wdg in Items)
                    {
                        if (true == group.IsChild(wdg.widgetGID, false))
                        {
                            group.AddChild(wdg);
                        }
                    }
                    widget = group;
                }

                if (widget != null)
                {
                    InteractionObjectPosition = new Thickness(widget.Left, widget.Top, 0, 0);
                    InteractionObjectWidth = widget.ItemWidth;
                    InteractionObjectHeight = widget.ItemHeight;
                    InteractionObjectRotateAngle = widget.RotateAngle;
                }
            }
        }

        #endregion

        #region Flick pannel commandler handler

        public bool CanRunFlickCommand
        {
            get
            {
                return true;
            }
        }

        private void FlickShowArrowCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            bool newValue = (bool)parameter;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is DynamicPanelViewModel)
                {
                    DynamicPanelViewModel DPItem = wdgItem as DynamicPanelViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "IsShowArrow", DPItem.IsShowArrow, newValue);

                    DPItem.IsShowArrow = newValue;
                }
            }

            PushToUndoStack(cmds);
        }

        private void FlickCirculerCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            bool newValue = (bool)parameter;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is DynamicPanelViewModel)
                {
                    DynamicPanelViewModel DPItem = wdgItem as DynamicPanelViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "IsCirculer", DPItem.IsCirculer, newValue);

                    DPItem.IsCirculer = newValue;
                }
            }

            PushToUndoStack(cmds);
        }

        private void FlickAutomaticCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            bool newValue = (bool)parameter;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is DynamicPanelViewModel)
                {
                    DynamicPanelViewModel DPItem = wdgItem as DynamicPanelViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "IsAutomatic", DPItem.IsAutomatic, newValue);

                    DPItem.IsAutomatic = newValue;
                }
            }

            PushToUndoStack(cmds);

        }

        private void FlickStartPageCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            System.Guid newValue = (System.Guid)parameter;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is DynamicPanelViewModel)
                {
                    DynamicPanelViewModel DPItem = wdgItem as DynamicPanelViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "StartPage", DPItem.StartPage, newValue);

                    DPItem.StartPage = newValue;
                }
            }

            PushToUndoStack(cmds);
        }

        private void FlickNavigationCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            NavigationType newValue = (NavigationType)parameter;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is DynamicPanelViewModel)
                {
                    DynamicPanelViewModel DPItem = wdgItem as DynamicPanelViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "ShowType", DPItem.ShowType, newValue);

                    DPItem.ShowType = newValue;
                }
            }

            PushToUndoStack(cmds);
        }

        private void FlickViewModeCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            DynamicPanelViewMode newValue = (DynamicPanelViewMode)parameter;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is DynamicPanelViewModel)
                {
                    DynamicPanelViewModel DPItem = wdgItem as DynamicPanelViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "ViewMode", DPItem.ViewMode, newValue);

                    DPItem.ViewMode = newValue;
                }
            }

            PushToUndoStack(cmds);
        }

        private void FlickPanelWidthCommandHander(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            int newValue = (int)parameter;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is DynamicPanelViewModel)
                {
                    DynamicPanelViewModel DPItem = wdgItem as DynamicPanelViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "PanelWidth", DPItem.PanelWidth, newValue);

                    DPItem.PanelWidth = newValue;
                }
            }

            PushToUndoStack(cmds);
        }

        private void FlickLineWidthCommandHander(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            double newValue = (double)parameter;
            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is DynamicPanelViewModel)
                {
                    DynamicPanelViewModel DPItem = wdgItem as DynamicPanelViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "LineWidth", DPItem.LineWidth, newValue);

                    DPItem.LineWidth = newValue;
                }
            }

            PushToUndoStack(cmds);
        }
        #endregion

        #region Hamburger command handler

        public bool CanRunHamburgerCommand
        {
            get
            {
                return true;
            }
        }

        private void HamburgerHideStyleCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            HiddenOn Newstyle = HiddenOn.Left;

            if (!((bool)parameter))
            {
                Newstyle = HiddenOn.Right;
            }

            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is HamburgerMenuViewModel)
                {
                    HamburgerMenuViewModel HMItem = wdgItem as HamburgerMenuViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "HiddenOn", HMItem.HiddenOn, Newstyle);

                    HMItem.HiddenOn = Newstyle;
                }
            }

            PushToUndoStack(cmds);
        }
        #endregion


        #region Toast Command Handler

        public bool CanRunToastCommand
        {
            get
            {
                return true;
            }
        }

        private void ToastExposureTimeCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            int exposureTime = (int)parameter;

            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is ToastViewModel)
                {
                    ToastViewModel toastItem = wdgItem as ToastViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "ExposureTime", toastItem.ExposureTime, exposureTime);

                    toastItem.ExposureTime = exposureTime;
                }
            }

            PushToUndoStack(cmds);
        }

        private void ToastCloseSettingCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            ToastCloseSetting closeSettting = (ToastCloseSetting)parameter;

            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is ToastViewModel)
                {
                    ToastViewModel toastItem = wdgItem as ToastViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "CloseSetting", toastItem.CloseSetting, closeSettting);

                    toastItem.CloseSetting = closeSettting;
                }
            }

            PushToUndoStack(cmds);
        }

        private void ToastDisplayPositionCommandHandler(object parameter)
        {
            CompositeCommand cmds = new CompositeCommand();

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            ToastDisplayPosition displayPosition = (ToastDisplayPosition)parameter;

            foreach (WidgetViewModBase wdgItem in allSelects)
            {
                if (wdgItem is ToastViewModel)
                {
                    ToastViewModel toastItem = wdgItem as ToastViewModel;
                    CreatePropertyChangeUndoCommand(cmds, wdgItem, "DisplayPosition", toastItem.DisplayPosition, displayPosition);

                    if (displayPosition == ToastDisplayPosition.Top)
                    {
                        CreatePropertyChangeUndoCommand(cmds, wdgItem, "Top", toastItem.Top, 0);
                        toastItem.Top = 0;
                    }
                    
                    toastItem.DisplayPosition = displayPosition;
                }
            }

            PushToUndoStack(cmds);
        }

        #endregion
        #endregion Global Routed Command Handler
    }
}
