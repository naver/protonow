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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Naver.Compass.Service.CustomLibrary;
using Microsoft.Win32;
using UndoCompositeCommand = Naver.Compass.InfoStructure.CompositeCommand;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        #region pirvate member
        private void UpdateAddedMaster2Canvas(Guid masterPageGuid)
        {
            List<IRegion> masters = _model.QueryMaster(masterPageGuid);
            if (masters == null || masters.Count < 1)
            {
                return;
            }

            foreach(IRegion master in masters)
            {
                var tar = items.FirstOrDefault(c => c.widgetGID == master.Guid);
                if (tar != null)
                {
                    continue;
                }
                WidgetViewModBase widgetVM = InsertWidget2Canvas(master,IsSelected);
                if (widgetVM != null)
                {
                    widgetVM.ParentPageVM = this;
                }                
            }


        }
        private void UpdateRemovedMasterFromCanvas(Guid masterPageGuid)
        {
            //list.Where(m => m.BookId == 1).ToList().ForEach(x => { list.Remove(x); });
            List<WidgetViewModBase> masters = items.Where<WidgetViewModBase>(x => x is MasterWidgetViewModel).ToList<WidgetViewModBase>(); ;
            foreach (MasterWidgetViewModel it in masters)
            {
                if (it == null)
                {
                    return;
                }

                if (it.EmbedePagetGUID == masterPageGuid)
                {
                    it.IsSelected = false;
                    Items.Remove(it);
                }
            }
        }
        private void RefreshZOrders()
        {
            foreach (WidgetViewModBase it in items)
            {                
                if(it.IsGroup==false)
                {
                    it.RefresgZOrder();
                }
            }
        }
        #endregion

        #region public function for widget mananger operation
        private void AddMasterEventCommandHandler(object obj)
        {
            IMasterPage MPage = obj as IMasterPage;
            if(MPage==null)
            {
                return;
            }
            if(MPage.ActiveConsumerPageGuidList.Contains(PageGID)==false)
            {
                return;
            }

            DeselectAll();
            UpdateAddedMaster2Canvas(MPage.Guid);
            RefreshZOrders();
        }      
        //this function just can be called by deleting a mater page.
        private void DeletMasterEventCommandHandler(object obj)
        {
            IMasterPage MPage = obj as IMasterPage;
            if(MPage==null)
            {
                return;
            }
            //if(MPage.ActiveConsumerPageGuidList.Contains(PageGID)==false)
            //{
            //    return;
            //}

            UpdateRemovedMasterFromCanvas(MPage.Guid);
            RefreshZOrders();

            //avoiding operation conflict, so clear the undo-stack
            this.UndoManager.Clear();
        }
        #endregion

        #region public function for master operation command handler
        private void ConvertToMasterCommandHandler(object parameter)
        {
            try
            {
                MasterNameWindow win = new MasterNameWindow();
                win.NewName = MasterListViewModel.GetNextDataName();
                win.Owner = Application.Current.MainWindow;
                win.ShowDialog();

                if (win.Result.HasValue && win.Result.Value)
                {
                    string masterName = win.NewName;

                    string objectName = string.Empty;
                    ISerializeWriter writer = GetSerializeWriter(ref objectName);
                    if (writer == null)
                        return;

                    var page = _document.CreateMasterPage(writer, masterName, CreateIconImage(false));
                    if (page != null)
                    {
                        //Add converted master to master list
                        _ListEventAggregator.GetEvent<AddConvertedMasterEvent>().Publish(page);
                        //keep widgets status when library editing mode
                        if (_document.DocumentType == DocumentType.Library)
                        {
                            return;
                        }

                        double minLeft = double.MaxValue;
                        double minTop = double.MaxValue;
                        foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
                        {
                            if (minLeft > wdgItem.Left)
                                minLeft = wdgItem.Left;
                            if (minTop > wdgItem.Top)
                                minTop = wdgItem.Top;
                        }

                        //remove selected widgets
                        UndoCompositeCommand cmds = new UndoCompositeCommand();
                        RemoveSelectedWidget(cmds);

                        //add converted master to current page. 
                        AddMasterItem(page.Guid, minLeft, minTop, cmds);

                        _undoManager.Push(cmds);

                        this.EditorCanvas.Focus();

                    }
                }
                else 
                {
                    MasterListViewModel.RollBackDataName();
                }

            }
            catch (Exception ex)
            {
                NLogger.Debug("Convert Master failed: " + ex.Message);
            }
        }
        private void MasterLock2LocationCommandHandler(object parameter)
        {
            MasterWidgetViewModel master = _selectionService.GetSelectedWidgets()[0] as MasterWidgetViewModel;
            if (master == null)
            {
                return;
            }


            //Change UI
            Naver.Compass.InfoStructure.CompositeCommand cmds = new Naver.Compass.InfoStructure.CompositeCommand();
            List<Guid> updateGroupList = new List<Guid>();
            CreatePropertyChangeUndoCommand(cmds, master, "IsLocked2MasterLocation", master.IsLocked2MasterLocation, !master.IsLocked2MasterLocation);
            master.IsLocked2MasterLocation = !master.IsLocked2MasterLocation;

            //Update X and Y
            if (master.IsLocked2MasterLocation == true)
            {
                if (master.MasterLockedLocationX != master.Left)
                {
                    CreatePropertyChangeUndoCommand(cmds, master, "Left", master.Left, master.MasterLockedLocationX);
                    master.ForceSetX(master.MasterLockedLocationX);
                }
                if (master.MasterLockedLocationY != master.Top)
                {
                    CreatePropertyChangeUndoCommand(cmds, master, "Top", master.Top, master.MasterLockedLocationY);
                    master.ForceSetY(master.MasterLockedLocationY);
                }
            }

            //Update Group
            if (master.ParentID != Guid.Empty)
            {
                updateGroupList.Add(master.ParentID);
                UpdateGroup(master.ParentID);
            }
            PushToUndoStack(cmds, updateGroupList);
        }
        private void MasterBreakAwayCommandHandler(object parameter)
        {
            MasterWidgetViewModel master = _selectionService.GetSelectedWidgets()[0] as MasterWidgetViewModel;
            if (master == null)
            {
                return;
            }

            Naver.Compass.InfoStructure.CompositeCommand cmds = new Naver.Compass.InfoStructure.CompositeCommand();
            var delCmd = new DeleteWidgetCommand(this, master);
            cmds.AddCommand(delCmd);
            Items.Remove(master);

            IObjectContainer container = _model.BreakMaster2Dom(master.widgetGID);
            AddObjects2Page2(container, _curAdaptiveViewGID, null, false, master.ZOrder, cmds);

            //TODO:Master Parent group operation

        }
        #endregion
    }
}
