using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.Helper;
using System.IO;
using Naver.Compass.Common;

namespace Naver.Compass.Module
{
    // Represents a selectable item in the Toolbox/>.
    public class ToolboxItem : ContentControl
    {
        // caches the start point of the drag operation
        private Point? dragStartPoint = null;

        static ToolboxItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ToolboxItem), new FrameworkPropertyMetadata(typeof(ToolboxItem)));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            
            var model = this.DataContext as WidgetModel;
            if (model != null && model.IsInEditMode == false)
            {
                this.dragStartPoint = new Point?(e.GetPosition(this));
            }            
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            Focus();
            e.Handled = true;
            base.OnPreviewMouseRightButtonDown(e);
        }
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            var widgetModel = this.Content as WidgetModel;

            //if click on favaourite icon, donn't add widget.
            if (widgetModel == null || widgetModel.IsClickonFavourite) { return; }

            var dataObject = GetItemInfo(widgetModel);
            if (dataObject == null)
                return;

            if (dataObject != null)
            {
                IEventAggregator ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

                if (ListEventAggregator != null)
                {
                    ListEventAggregator.GetEvent<NewWidgetEvent>().Publish(dataObject);
                }
                e.Handled = true;

                SendNClick(widgetModel.NClickCode);
            }
        }

        /// <summary>
        /// Get NClick info and send it.
        /// </summary>
        /// <param name="panel">object to send nClick when click</param>
        private void SendNClick(string lbrNclickCode)
        {
            if (!string.IsNullOrEmpty(lbrNclickCode))
            {
                ServiceLocator.Current.GetInstance<INClickService>().SendNClick(lbrNclickCode);
                NLogger.Info("Send nclick: {0}", lbrNclickCode);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;

            if (this.dragStartPoint.HasValue == false)
            {
                return;
            }

            var widgetModel = this.Content as WidgetModel;
            if (widgetModel == null) { return; }

            DataObject dataObject = GetItemInfo(widgetModel);
            if (dataObject == null)
            {
                return;
            }

            try
            {
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);

                SendNClick(widgetModel.NClickCode);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                e.Handled = true;
            }

        }

        private DataObject GetItemInfo(WidgetModel widgetModel)
        {      
            if (widgetModel.LbrType.StartsWith("lbw"))
            {
                // XamlWriter.Save() has limitations in exactly what is serialized,
                // see SDK documentation; short term solution only;
                return new DataObject("DESIGNER_ITEM", widgetModel.LbrType);
            }
            else if (widgetModel.EnumType == WidgetModelType.svg)
            {
                string szURl = widgetModel.SvgIcon;

                try
                {
                    Uri uri = new Uri(@"pack://application:,,,/Naver.Compass.Module.WidgetLibrary;component/Resources/" + szURl, UriKind.RelativeOrAbsolute);

                    if (szURl.EndsWith(".svg"))
                    {
                        return new DataObject("SVG_ITEM", uri);
                    }

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
            else if (widgetModel.LbrType == "custom")
            {
                try
                {
                    var parantToolbox = Naver.Compass.Module.VisualTreeEx.GetAncestor<Toolbox>(this);
                    if (parantToolbox.DataContext is WidgetExpand)
                    {
                        WidgetExpand parentLibrary = null;
                        if ((this.DataContext as WidgetModel).IsFavourite)
                        {  //widget which is custom and is favourite
                            var gallery = Naver.Compass.Module.VisualTreeEx.GetAncestor<WidgetGallery>(this);
                            //get parent expand in UI tab.
                            var uiExpands = (gallery.DataContext as WidgetGalleryViewModel).UIWidgetLibraryTab.WidgetExpands;
                            foreach (var expand in uiExpands)
                            {
                                if (expand.WidgetModels.Contains(widgetModel))
                                {
                                    parentLibrary = expand;
                                }
                            }
                        }
                        else
                        {//common custom widget
                            parentLibrary = parantToolbox.DataContext as WidgetExpand;
                        }
                        if (parentLibrary == null)
                            return null;

                        if (!File.Exists(parentLibrary.Raw_FileName))
                        {
                            MessageBox.Show(GlobalData.FindResource("Error_LibFile_Not_Exit"));
                            return null;
                        }
                        return new DataObject(
                            "CUSTOM_ITEM",
                            new Tuple<Guid, string, Guid, string>(
                                parentLibrary.LibraryGID,
                                parentLibrary.Raw_FileName,                            
                                widgetModel.Id, 
                                parentLibrary.Header));
                    }
                }
                catch
                {
                    return null;
                }                
            }
            return null;
        }

    }


  
    // Wraps info of the dragged object into a class
    public class ItemDragObject
    {
        // Xaml string that represents the serialized content
        public String Xaml { get; set; }

        // Defines width and height of the DesignerItem
        // when this DragObject is dropped on the DesignerCanvas
        public Size? DesiredSize { get; set; }
    }
}
