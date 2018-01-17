using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;
using System.Windows.Media;

namespace Naver.Compass.Module.Property
{
    class HamburgerPropertyViewModel : PropertyViewModelBase
    {

        #region Override base function

        override protected void OnItemsAdd()
        {
            base.OnItemsAdd();
            FirePropertyChanged("ImgSource");
        }

        override public void OnPropertyChanged(string args)
        {
            base.OnPropertyChanged(args);
            switch (args)
            {
                case "ImgSource":
                    {
                        FirePropertyChanged("ImgSource");
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Binding property

        public ImageSource ImgSource
        {
            get
            {
                if (_VMItems.Count == 1)
                {
                    ImageWidgetViewModel ImageData = _VMItems[0] as ImageWidgetViewModel;
                    if (ImageData != null)
                    {
                        return ImageData.ImgSource;
                    }
                }

                return null;
            }
        }

        public bool IsLeft
        {
            get
            {
                foreach (HamburgerMenuViewModel wdg in _VMItems)
                {
                    if (wdg != null)
                    {
                        if (wdg.HiddenOn == HiddenOn.Left)
                        {
                            return true;
                        }
                        else if (wdg.HiddenOn == HiddenOn.Right)
                        {
                            return false;
                        }

                    }
                }

                return true;
            }
            set
            {
                OnLeftRightChange((bool)value);

                FirePropertyChanged("IsLeft");
            }
        }

        private void OnLeftRightChange(bool isLeft)
        {
            HanburgerCommands.HideStyle.Execute(isLeft, _CmdTarget);
        }

        #endregion
    }


}
