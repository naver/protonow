using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    static public class ReadOnlyWidgetFactory
    {
        //Create Widget VM According to XML data
        public static WidgetPreViewModeBase CreateWidget(IRegion obj, bool bIsNail)
        {
            WidgetPreViewModeBase vmItem;
            if (obj is IMaster)
            {
                IMaster master = obj as IMaster;
                vmItem = new MasterWidgetPreViewModel(master);
                return vmItem;
            }


            IWidget it = obj as IWidget;
            switch (it.WidgetType)
            {
                case WidgetType.Button:
                    {
                        vmItem = new ButtonWidgetPreViewModel(it);
                        break;
                    }
                case WidgetType.Image:
                    {
                        vmItem = new ImageWidgetPreViewModel(it);
                        (vmItem as ImageWidgetPreViewModel).IsNail = bIsNail;
                        break;
                    }
                case WidgetType.SVG:
                    {
                        vmItem = new SVGWidgetPreViewModel(it);
                        break;
                    }
                case WidgetType.HotSpot:
                    {
                        vmItem = new HotSpotWidgetPreViewModel(it);
                        break;
                    }
                case WidgetType.Line:
                    {
                        if ((it as ILine).Orientation == Orientation.Horizontal)
                        {
                            vmItem = new HLineWidgetPreViewModel(it);
                        }
                        else if ((it as ILine).Orientation == Orientation.Vertical)
                        {
                            vmItem = new VLineWidgetPreViewModel(it);
                        }
                        else
                        {
                            throw(new Exception("This line is illegal, please check the code"));
                        }
                        break;
                    }
                case WidgetType.Shape:
                    {
                        IShape shape = it as IShape;
                        if (shape.ShapeType == ShapeType.Diamond)
                        {
                            vmItem = new DiamondWidgetPreViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.Ellipse)
                        {
                            vmItem = new CircleWidgetPreViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.RoundedRectangle)
                        {
                            vmItem = new RoundedRecWidgetPreViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.Rectangle)
                        {
                            vmItem = new RectangleWidgetPreViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.Paragraph)
                        {
                            vmItem = new LabelWidgetPreViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.Triangle)
                        {
                            vmItem = new TriangleWidgetPreViewModel(it);
                        }
                        else
                        {
                            vmItem = new ImageWidgetPreViewModel(it);
                        }
                        break;
                    }

                case WidgetType.ListBox:
                    {
                        vmItem = new ListboxWidgetPreViewModel(it);
                        break;
                    }

                case WidgetType.DropList:
                    {
                        vmItem = new DroplistWidgetPreViewModel(it);
                        break;
                    }
                case WidgetType.RadioButton:
                    {
                        vmItem = new RadioButtonWidgetPreViewModel(it);
                        break;
                    }
                case WidgetType.Checkbox:
                    {
                        vmItem = new CheckBoxWidgetPreViewModel(it);
                        break;
                    }
                case WidgetType.TextArea:
                    {
                        vmItem = new TextAreaWidgetPreViewModel(it);
                        break;
                    }
                case WidgetType.TextField:
                    {
                        vmItem = new TextFieldWidgetPreViewModel(it);
                        break;
                    }
                case WidgetType.HamburgerMenu:
                    {
                        vmItem = new HamburgerMenutPreViewModel(it);
                        break;
                    }

                case WidgetType.DynamicPanel:
                    {
                        vmItem = new DynamicPanelPreViewModel(it);
                        break;
                    }

                case WidgetType.Toast:
                    {
                        vmItem = new ToastPreViewModel(it);
                        break;
                    }

                case WidgetType.FlowShape:
                    // To avoid crash when load old file as we don't support flow shape right now.
                    // This is temporary code and remove it once we support flow shape.
                    return null;

                default:
                    {
                        vmItem = new ImageWidgetPreViewModel(it);
                        break;
                    }
            }//switch
            return vmItem;
        }    
        //TODO:
    }
}
