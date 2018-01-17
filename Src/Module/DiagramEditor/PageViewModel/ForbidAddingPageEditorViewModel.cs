using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{

    /// <summary>
    /// Forbid adding rules:
    /// * Master cannot be added to Library pages(Page in Library), MasterPage, EmbeddedPage(Child Page).
    /// * EmbeddedPage(Interactive UI widgets: Toast, HamburgerMenu/DrawerMenu, DynamicPanel/SwipView ) cannot be added to EmbeddedPage(Child Page).
    /// </summary>
    public class ForbidAddingPageEditorViewModel : PageEditorViewModel
    {
        public ForbidAddingPageEditorViewModel()
        {
        }

        public ForbidAddingPageEditorViewModel(Guid pageGuid) :
            base(pageGuid)
        {
        }

        protected bool _isForbidSwipeviews = true;
        protected bool _isForbidDrawerMenu = true;
        protected bool _isForbidToast = true;
        protected bool _isForbidMaster = true;

        override protected void OnItemAdded(object obj)
        {
            DropInfo info = obj as DropInfo;
            if (info == null)
                return;
            if (CheckPageEmbeddedWidget(info.e.Data))
            {
                return;
            }
            base.OnItemAdded(obj);
        }
        override protected void AddItemFromDataobject(DataObject data)
        {
            if (CheckPageEmbeddedWidget(data))
            {
                return;
            }
            base.AddItemFromDataobject(data);
        }

        protected override bool CanPasteWidgets(ISerializeReader reader)
        {
            try
            {
                if (_isForbidMaster && reader.ContainsMaster)
                {
                    ForbidMasterAlert();
                    return false;
                }

                ReadOnlyCollection<WidgetType> typeList = reader.PeekWidgetTypeList();
                if (_isForbidDrawerMenu && typeList.Contains(WidgetType.HamburgerMenu))
                {
                    ForbidHamburgerAlert();
                    return false;
                }
                else if (_isForbidSwipeviews && typeList.Contains(WidgetType.DynamicPanel))
                {
                    ForbidFlickingAlert();
                    return false;
                }
                else if (_isForbidToast && typeList.Contains(WidgetType.Toast))
                {
                    ForbidToastAlert();
                    return false;
                }

            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                return false;
            }

            return true;
        }



        /// <summary>
        /// Check it current data contains IPageEmbeddedWidget
        /// </summary>
        private bool CheckPageEmbeddedWidget(IDataObject data)
        {
            if (data.GetDataPresent("DESIGNER_ITEM"))
            {
                string xamlString = data.GetData("DESIGNER_ITEM") as string;
                if (String.IsNullOrEmpty(xamlString))
                {
                    return true;
                }

                if (_isForbidDrawerMenu && xamlString == "lbw.hamburgermenu")
                {
                    ForbidHamburgerAlert();
                    return true;
                }
                else if (_isForbidSwipeviews && xamlString == "lbw.swipeviews")
                {
                    ForbidFlickingAlert();
                    return true;
                }
                else if (_isForbidToast && xamlString == "lbw.toastnotification")
                {
                    ForbidToastAlert();
                    return true;
                }
            }
            else if (data.GetDataPresent("CUSTOM_ITEM"))
            {
                ICustomObject customObject = GetCustomObjectFromData(data.GetData("CUSTOM_ITEM"));
                if (customObject != null)
                {
                    customObject.Open();

                    foreach (var item in customObject.Widgets)
                    {
                        if (_isForbidDrawerMenu && item is IHamburgerMenu)
                        {
                            ForbidHamburgerAlert();
                            customObject.Close();
                            return true;
                        }
                        else if (_isForbidToast && item is IToast)
                        {
                            ForbidToastAlert();
                            customObject.Close();
                            return true;
                        }
                        else if (_isForbidSwipeviews && item is IDynamicPanel)
                        {
                            ForbidFlickingAlert();
                            customObject.Close();
                            return true;
                        }
                    }
                    customObject.Close();
                }
            }
            else if (_isForbidMaster && data.GetDataPresent("MASTER_ITEM"))
            {
                ForbidMasterAlert();
                return true;
            }

            return false;
        }

        private void ForbidHamburgerAlert()
        {
            MessageBox.Show(GlobalData.FindResource("Msg_Forbidden_Drawer"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void ForbidFlickingAlert()
        {
            MessageBox.Show(GlobalData.FindResource("Msg_Forbidden_SwipeViews"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void ForbidToastAlert()
        {
            MessageBox.Show(GlobalData.FindResource("Msg_Forbidden_Toast"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void ForbidMasterAlert()
        {
            MessageBox.Show(GlobalData.FindResource("Msg_Forbidden_Master"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
