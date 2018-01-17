using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;

namespace Naver.Compass.Module
{
    static class WidgetFactory
    {
        //Create Widget VM According to XML data
        public static WidgetViewModBase CreateWidget(IRegion obj)
        {
            //string timeStamp = "_" + DateTime.Now.ToString("MMddHHmmssfff");
            WidgetViewModBase vmItem;
            if (obj is IMaster)
            {
                IMaster master = obj as IMaster;
                AutoSetWdgName(obj,"Master ",WidgetType.None);
                vmItem = new MasterWidgetViewModel(master);
                return vmItem;
            }

            IWidget it = obj as IWidget;
            switch (it.WidgetType)
            {
                case WidgetType.Button:
                    {
                        AutoSetWdgName(it, "Button ", WidgetType.Button);
                        vmItem = new ButtonWidgetViewModel(it);
                        break;
                    }
                case WidgetType.Image:
                    {
                        AutoSetWdgName(it, "Image ", WidgetType.Image);
                        vmItem = new ImageWidgetViewModel(it);
                        break;
                    }
                case WidgetType.SVG:
                    {
                        AutoSetWdgName(it, "Svg ", WidgetType.SVG);
                        vmItem = new SVGWidgetViewModel(it);
                        break;
                    }
                case WidgetType.HotSpot:
                    {
                        AutoSetWdgName(it, "Link ", WidgetType.HotSpot);
                        vmItem = new HotSpotWidgetViewModel(it);
                        break;
                    }
                case WidgetType.Line:
                    {
                        ILine line = it as ILine;
                        if (line.Orientation == Orientation.Vertical)
                        {
                            AutoSetWdgName(it, "VLine ", WidgetType.Line);
                            vmItem = new VLineWidgetViewModel(it);
                        }                            
                        else
                        {
                            AutoSetWdgName(it, "HLine ", WidgetType.Line);
                            vmItem = new HLineWidgetViewModel(it);
                        }
                        break;
                    }
                case WidgetType.Shape:
                    {
                        IShape shape = it as IShape;
                        if (shape.ShapeType == ShapeType.Diamond)
                        {
                            AutoSetWdgName(it, "Diamond ", WidgetType.Shape);
                            vmItem = new DiamondWidgetViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.Ellipse)
                        {
                            AutoSetWdgName(it, "Ellipse ", WidgetType.Shape);
                            vmItem = new CircleWidgetViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.Rectangle)
                        {
                            AutoSetWdgName(it, "Rectangle ", WidgetType.Shape);
                            vmItem = new RectangleWidgetViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.RoundedRectangle)
                        {
                            AutoSetWdgName(it, "Round ", WidgetType.Shape);
                            vmItem = new RoundedRecWidgetViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.Paragraph)
                        {
                            AutoSetWdgName(it, "Paragraph ", WidgetType.Shape);
                            vmItem = new LabelWidgetViewModel(it);
                        }
                        else if (shape.ShapeType == ShapeType.Triangle)
                        {
                            AutoSetWdgName(it, "Triangle ", WidgetType.Shape);
                            vmItem = new TriangleWidgetViewModel(it);
                        }
                        else
                        {
                            AutoSetWdgName(it, "Image ", WidgetType.Shape);
                            vmItem = new ImageWidgetViewModel(it);
                        }

                        break;
                    }
                case WidgetType.ListBox:
                    {
                        AutoSetWdgName(it, "ListBox ", WidgetType.ListBox);
                        vmItem = new ListboxWidgetViewModel(it);
                        break;
                    }

                case WidgetType.DropList:
                    {
                        AutoSetWdgName(it, "DropList ", WidgetType.DropList);
                        vmItem = new DroplistWidgetViewModel(it);
                        break;
                    }
                case WidgetType.Checkbox:
                    {
                        AutoSetWdgName(it, "Checkbox ", WidgetType.Checkbox);
                        vmItem = new CheckBoxWidgetViewModel(it);
                        break;
                    }
                case WidgetType.RadioButton:
                    {
                        AutoSetWdgName(it, "RadioButton ", WidgetType.RadioButton);
                        vmItem = new RadioButtonWidgetViewModel(it);
                        break;
                    }
                case WidgetType.TextArea:
                    {
                        AutoSetWdgName(it, "TextArea ", WidgetType.TextArea);
                        vmItem = new TextAreaWidgetViewModel(it);
                        break;
                    }
                case WidgetType.TextField:
                    {
                        AutoSetWdgName(it, "TextField ", WidgetType.TextField);
                        vmItem = new TextFieldWidgetViewModel(it);
                        break;
                    }
                case WidgetType.HamburgerMenu:
                    {
                        AutoSetWdgName(it, "Drawer ", WidgetType.HamburgerMenu);
                        vmItem = new HamburgerMenuViewModel(it);
                        break;
                    }
                case WidgetType.DynamicPanel:
                    {
                        AutoSetWdgName(it, "Flicking ", WidgetType.DynamicPanel);
                        vmItem = new DynamicPanelViewModel(it);
                        break;
                    }  
                case WidgetType.Toast:
                    {
                        AutoSetWdgName(it, "Toast ",WidgetType.Toast);
                        vmItem = new ToastViewModel(it);
                        break;
                    }
                case WidgetType.FlowShape: 
                    // To avoid crash when load old file as we don't support flow shape right now.
                    // This is temporary code and remove it once we support flow shape.
                    return null;
                case WidgetType.None: 
                    return null;
                default:
                    {
                        vmItem = null;
                        break;
                    }
            }//switch
            return vmItem;
        }    
  
        //TODO:
        public static WidgetViewModBase CreateGroup(IGroup it)
        {            
            AutoSetWdgName(it, "Group ", WidgetType.None);
            WidgetViewModBase vmItem = new GroupViewModel(it);                       
            return vmItem;
        }    

        private static void AutoSetWdgName(IRegion obj, string prex, WidgetType type)
        {
            if(string.IsNullOrEmpty(obj.Name)==false)
            {
                return;
            }

            if(string.IsNullOrEmpty(prex) ==true)
            {
                return;
            }

            IPage page = obj.ParentPage;
            if (page==null)
            {
                return;
            }


            //Master
            int pos = 1;
            string defaultname;
            if (obj is IMaster)
            {
                pos = page.Masters.Count ;
                defaultname = prex + pos;
                if (pos==1)
                {
                    obj.Name = defaultname;
                    return;
                }

                for(int i=pos;i<100; i++)
                {
                    defaultname = prex + i;
                    if(true==page.Masters.Any(x => x.Name == defaultname))
                    {
                        continue;
                    }
                    else
                    {
                        obj.Name = defaultname;
                        break;
                    }
                }
                return;                
            }

            if (obj is IGroup)
            {
                pos = page.Groups.Count;
                defaultname = prex + pos;
                if (pos==1)
                {
                    obj.Name = defaultname;
                    return;
                }

                for(int i=pos;i<100; i++)
                {
                    defaultname = prex + i;
                    if(true==page.Groups.Any(x => x.Name == defaultname))
                    {
                        continue;
                    }
                    else
                    {
                        obj.Name = defaultname;
                        break;
                    }
                }
                return;                
            }


            //Widgets
            IWidget it = obj as IWidget;
            List<IWidget> wdgs= page.Widgets.Where<IWidget>(x => x.WidgetType == type).ToList<IWidget>();
            switch (it.WidgetType)
            {
                case WidgetType.Shape:
                    {
                        IShape shape = it as IShape;
                        if (shape.ShapeType == ShapeType.Diamond)
                        {
                            wdgs = wdgs.Where<IWidget>(x => (x as IShape).ShapeType == ShapeType.Diamond).ToList<IWidget>();
                        }
                        else if (shape.ShapeType == ShapeType.Ellipse)
                        {
                            wdgs = wdgs.Where<IWidget>(x => (x as IShape).ShapeType == ShapeType.Ellipse).ToList<IWidget>();
                        }
                        else if (shape.ShapeType == ShapeType.Rectangle)
                        {
                            wdgs = wdgs.Where<IWidget>(x => (x as IShape).ShapeType == ShapeType.Rectangle).ToList<IWidget>();
                        }
                        else if (shape.ShapeType == ShapeType.RoundedRectangle)
                        {
                            wdgs = wdgs.Where<IWidget>(x => (x as IShape).ShapeType == ShapeType.RoundedRectangle).ToList<IWidget>();
                        }
                        else if (shape.ShapeType == ShapeType.Paragraph)
                        {
                            wdgs = wdgs.Where<IWidget>(x => (x as IShape).ShapeType == ShapeType.Paragraph).ToList<IWidget>();
                        }
                        else if (shape.ShapeType == ShapeType.Triangle)
                        {
                            wdgs = wdgs.Where<IWidget>(x => (x as IShape).ShapeType == ShapeType.Triangle).ToList<IWidget>();
                        }
                        break;                        
                    }
                case WidgetType.Line:
                    {
                        ILine line = it as ILine;
                        if (line.Orientation == Orientation.Vertical)
                        {
                            wdgs = wdgs.Where<IWidget>(x => (x as ILine).Orientation == Orientation.Vertical).ToList<IWidget>();
                        }
                        else
                        {
                            wdgs = wdgs.Where<IWidget>(x => (x as ILine).Orientation == Orientation.Horizontal).ToList<IWidget>();
                        }
                        break;
                    }
                case WidgetType.None:
                    {
                        return;
                    }
                default:
                    {
                        break;
                    }
            }//switch

            pos = wdgs.Count ;
            defaultname = prex + pos;
            if (pos == 1)
            {
                obj.Name = defaultname;
                return;
            }
            for (int i = pos; i < 10000; i++)
            {
                defaultname = prex + i;
                if (true == wdgs.Any(x => x.Name == defaultname))
                {
                    continue;
                }
                else
                {
                    obj.Name = defaultname;
                    break;
                }
            }
            return;

        }//function
    }
}
