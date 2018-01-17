using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;

namespace Naver.Compass.Module
{
    partial class PropertyPageViewModel 
    {
        private ObservableCollection<Property.PropertyViewModelBase> items;

        public ObservableCollection<Property.PropertyViewModelBase> Items
        {
            get
            {
                if (items == null)
                {
                    items = new ObservableCollection<Property.PropertyViewModelBase>();

                    //Add widget from DOM to UI
                    return items;
                }
                return items;
            }
        }

        //Update the property view after selection changed
        void UpdatePropertyView()
        {
            List<WidgetViewModBase> Widgets = _model.GetSelectionWidget();

            Property.ImagePropertyViewModel ImageVM=null;
            Property.LinkPropertyViewModel LinkVM = null;
            Property.TextPropertyViewModel LableVM = null;
            Property.ShapePropertyViewModel ShapeVM = null;
            Property.GroupPropertyViewModel GroupVM = null;
            Property.ListBoxPropertyViewModel ListboxVM = null;
            Property.DropListPropertyViewModel DroplistVM = null;
            Property.CheckBoxPropertyViewModel CheckboxVM = null;
            Property.RadioButtonPropertyViewModel RadioButtonVM = null;
            Property.TextAreaPropertyViewModel TextareaVM = null;
            Property.TextFieldPropertyViewModel TextFieldVM = null;
            Property.ButtonPropertyViewModel ButtonVM = null;
            Property.HamburgerPropertyViewModel HamburgerVM = null;
            Property.FlickPannelPropertyViewModel FlickpannelVM = null;
            Property.ToastPropertyViewModel ToastVM = null;
            Property.SVGPropertyViewModel SVGVM = null;
            Property.MasterPropertyViewModel MasterVM = null;
            Items.Clear();

            foreach (WidgetViewModBase data in Widgets)
            {                
                if ( data.IsGroup)
                {
                    if (GroupVM == null)
                    {
                        GroupVM = new Property.GroupPropertyViewModel();
                        Items.Add(GroupVM);
                    }

                    GroupVM.AddItems(data);

                }
                else if (data.WidgetType == WidgetType.Image)
                {
                    if (ImageVM == null)
                    {
                        ImageVM = new Property.ImagePropertyViewModel();
                        Items.Add(ImageVM);
                    }

                    ImageVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.HotSpot)
                {
                    if (LinkVM == null)
                    {
                        LinkVM = new Property.LinkPropertyViewModel();
                        Items.Add(LinkVM);
                    }

                    LinkVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.Shape && data.shapetype == ShapeType.Paragraph)
                {
                    if (LableVM == null)
                    {
                        LableVM = new Property.TextPropertyViewModel();
                        Items.Add(LableVM);
                    }

                    LableVM.AddItems(data);
                   
                }
              
                else if (data.WidgetType == WidgetType.ListBox)
                {

                    if (ListboxVM == null)
                    {
                        ListboxVM = new Property.ListBoxPropertyViewModel();
                        Items.Add(ListboxVM);
                    }

                    ListboxVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.DropList)
                {

                    if (DroplistVM == null)
                    {
                        DroplistVM = new Property.DropListPropertyViewModel();
                        Items.Add(DroplistVM);
                    }

                    DroplistVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.Checkbox)
                {
                    if (CheckboxVM == null)
                    {
                        CheckboxVM = new Property.CheckBoxPropertyViewModel();
                        Items.Add(CheckboxVM);
                    }

                    CheckboxVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.RadioButton)
                {
                    if (RadioButtonVM == null)
                    {
                        RadioButtonVM = new Property.RadioButtonPropertyViewModel();
                        Items.Add(RadioButtonVM);
                    }

                    RadioButtonVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.TextArea)
                {
                    if (TextareaVM == null)
                    {
                        TextareaVM = new Property.TextAreaPropertyViewModel();
                        Items.Add(TextareaVM);
                    }
                    TextareaVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.TextField)
                {
                    if (TextFieldVM == null)
                    {
                        TextFieldVM = new Property.TextFieldPropertyViewModel();
                        Items.Add(TextFieldVM);
                    }
                    TextFieldVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.Button)
                {
                    if (ButtonVM == null)
                    {
                        ButtonVM = new Property.ButtonPropertyViewModel();
                        Items.Add(ButtonVM);
                    }
                    ButtonVM.AddItems(data);
                }

                else if (data.WidgetType == WidgetType.Shape && 
                    (data.shapetype == ShapeType.Rectangle
                    || data.shapetype == ShapeType.RoundedRectangle
                    || data.shapetype == ShapeType.Ellipse
                    || data.shapetype == ShapeType.Diamond
                    || data.shapetype == ShapeType.Triangle))
                {
                    if (ShapeVM == null)
                    {
                        ShapeVM = new Property.ShapePropertyViewModel();
                        Items.Add(ShapeVM);
                    }

                    ShapeVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.HamburgerMenu)
                {
                    if (HamburgerVM == null)
                    {
                        HamburgerVM = new Property.HamburgerPropertyViewModel();
                        Items.Add(HamburgerVM);
                    }

                    HamburgerVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.DynamicPanel)
                {
                    if (FlickpannelVM == null)
                    {
                        FlickpannelVM = new Property.FlickPannelPropertyViewModel();
                        Items.Add(FlickpannelVM);
                    }

                    FlickpannelVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.Toast)
                {
                    if (ToastVM == null)
                    {
                        ToastVM = new Property.ToastPropertyViewModel();
                        items.Add(ToastVM);
                    }
                    ToastVM.AddItems(data);
                }
                else if (data.WidgetType == WidgetType.SVG)
                {
                    if(SVGVM == null)
                    {
                        SVGVM = new Property.SVGPropertyViewModel();
                        items.Add(SVGVM);
                    }
                    SVGVM.AddItems(data);
                }
                else if(data is MasterWidgetViewModel)
                {
                    if(MasterVM == null)
                    {
                        MasterVM = new Property.MasterPropertyViewModel();
                        items.Add(MasterVM);
                    }
                    MasterVM.AddItems(data);
                }
            }
            FirePropertyChanged("Items");
        }

        //Update widget property
        void UpdateWidgetProperty(string EventArg)
        {
            foreach (Property.PropertyViewModelBase data in Items)
            {
                data.OnPropertyChanged(EventArg);
            }
        }

        void UpdateCmdTarget(IInputElement target)
        {
            Property.PropertyViewModelBase.UpdateCmdTarget(target);
        }

    }
}
