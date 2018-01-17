using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_CreateAllTypeWidgets : TestCase
    {
        public Test_CreateAllTypeWidgets()
            : base("Test_CreateAllTypeWidgets")
        {
        }


        public override string Description
        {
            get
            {
                return "Test creating all type widgets:\n" +
                    "* Create a document.\n" +
                    "* Create page annotation field.\n" +
                    "* Create widget annotation field.\n" +
                    "* Create a device.\n" +
                    "* Create a new page.\n" +
                    "* Modify page annotation.\n" +
                    "* Create all widgets in the new page.\n" +
                    "* Modify widget annotation.\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
            // Create a new document.
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;

            // Create a page note field.
            document.PageAnnotationFieldSet.CreateAnnotationField("Default", AnnotationFieldType.Text);

            // Create some widget note fields.
            document.WidgetAnnotationFieldSet.CreateAnnotationField("Description", AnnotationFieldType.Text);
            document.WidgetAnnotationFieldSet.CreateAnnotationField("CreatedTime", AnnotationFieldType.Text);

            // Create a device.
            IDevice devide = document.DeviceSet.CreateDevice("iPhone 6 Plus");
            devide.Width = 1024;
            devide.Height = 768;
            devide.IsChecked = true;

            // Create a page.
            IDocumentPage page = document.CreatePage("Home");

            // Create the page node in page tree.
            ITreeNode node = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node.AttachedObject = page;

            // Must open the page before you read and modify it.
            page.Open(); 
 
            // Set page note.
            page.Annotation.SetTextValue("Default", "This Home page.");

            // Get the page view for base adaptive view.
            IPageView baseView = page.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create widgets on the base view in this page.
            IButton button = baseView.CreateWidget(WidgetType.Button) as IButton;
            button.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            
            // Size
            button.WidgetStyle.Height = 30;
            button.WidgetStyle.Width = 100;
            
            // Location
            button.WidgetStyle.X = 0;
            button.WidgetStyle.Y = 0;
            button.WidgetStyle.Z = 0;

            // Text things
            button.Name = "Button 1";
            button.Text = "Button";
            button.Tooltip = "Html button.";

            ICheckbox checkbox = baseView.CreateWidget(WidgetType.Checkbox) as ICheckbox;
            checkbox.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            checkbox.WidgetStyle.Height = 18;
            checkbox.WidgetStyle.Width = 120;
            checkbox.WidgetStyle.X = 150;
            checkbox.WidgetStyle.Y = 0;
            checkbox.WidgetStyle.Z = 1;
            checkbox.Name = "CheckBox 1";
            checkbox.Text = "CheckBox";
            checkbox.Tooltip = "Left align check box.";

            IDroplist dropList = baseView.CreateWidget(WidgetType.DropList) as IDroplist;
            dropList.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            dropList.WidgetStyle.Height = 22;
            dropList.WidgetStyle.Width = 200;
            dropList.WidgetStyle.X = 300;
            dropList.WidgetStyle.Y = 0;
            dropList.WidgetStyle.Z = 2;
            dropList.Name = "Droplist 1";
            dropList.Tooltip = "A droplist has 3 item and item 3 is seleted.";
            
            // Create list items.
            dropList.CreateItem("Droplist Item 1");
            dropList.CreateItem("Droplist Item 2");
            IListItem item3 = dropList.CreateItem("Droplist Item 3");
            item3.IsSelected = true;

            /*
            IFlowShape flowShap = baseView.CreateWidget(WidgetType.FlowShape) as IFlowShape; // Here flow shape type is none. 
            // You must set specific flow shape type, then the flowshape is a valid flowshape.
            flowShap.FlowShapeType = FlowShapeType.Database;
            flowShap.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            flowShap.Height = 80;
            flowShap.Width = 60;
            flowShap.X = 550;
            flowShap.Y = 0;
            flowShap.Z = 3;
            flowShap.Name = "FlowShape 1";
            flowShap.Tooltip = "A Database flow shape.";
            flowShap.SetRichText("Database"); // FlowShape support rich text.
            */
             
            IHotSpot hotSpot = baseView.CreateWidget(WidgetType.HotSpot) as IHotSpot;
            hotSpot.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            hotSpot.WidgetStyle.Height = 100;
            hotSpot.WidgetStyle.Width = 100;
            hotSpot.WidgetStyle.X = 0;
            hotSpot.WidgetStyle.Y = 100;
            hotSpot.WidgetStyle.Z = 4;
            hotSpot.Name = "HotSpot 1";
            hotSpot.Tooltip = "A hot sport link to Baidu";
            // Create a link action to open www.baidu.com in new window.
            IInteractionEvent clickEvent = hotSpot.Events[EventType.OnClick];
            IInteractionCase case1 = clickEvent.CreateCase("clickCase");
            IInteractionOpenAction openAction = case1.CreateAction(ActionType.OpenAction) as IInteractionOpenAction;
            openAction.LinkType = LinkType.LinkToUrl;
            openAction.ExternalUrl = @"www.baidu.com";
            openAction.OpenIn = ActionOpenIn.NewWindowOrTab;

            IImage image = baseView.CreateWidget(WidgetType.Image) as IImage;
            image.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            image.WidgetStyle.Height = 267;
            image.WidgetStyle.Width = 116;
            image.WidgetStyle.X = 150;
            image.WidgetStyle.Y = 100;
            image.WidgetStyle.Z = 5;
            image.Name = "4.png";
            image.Tooltip = "A png image has 116 x 267 in size";

            // It is a png image by default. Set image stream
            string imageFile = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "HangGame", "4.png");
            if(File.Exists(imageFile))
            {
                using(FileStream fileStream = new FileStream(imageFile, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream imageStream = new MemoryStream();
                    fileStream.CopyTo(imageStream);
                    image.ImageStream = imageStream;
                }
            }

            ILine line = baseView.CreateWidget(WidgetType.Line) as ILine;
            line.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            line.Orientation = Orientation.Vertical;
            line.WidgetStyle.Height = 200;
            line.WidgetStyle.Width = 10;
            line.WidgetStyle.X = 300;
            line.WidgetStyle.Y = 100;
            line.WidgetStyle.Z = 6;
            line.Name = "Line";
            line.Tooltip = "A Vertical line with";

            IListBox listBox = baseView.CreateWidget(WidgetType.ListBox) as IListBox;
            listBox.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            listBox.WidgetStyle.Height = 100;
            listBox.WidgetStyle.Width = 200;
            listBox.WidgetStyle.X = 350;
            listBox.WidgetStyle.Y = 100;
            listBox.WidgetStyle.Z = 7;
            listBox.Tooltip = "A multiple-selected listBox which has 5 itmes. Item 1 and item 4 is selcted.";

            listBox.AllowMultiple = true;
            IListItem item1 = listBox.CreateItem("ListBox Item 1");
            item1.IsSelected = true;
            listBox.CreateItem("ListBox Item 2");
            listBox.CreateItem("ListBox Item 3");
            IListItem item4 = listBox.CreateItem("ListBox Item 4");
            item4.IsSelected = true;
            listBox.CreateItem("ListBox Item 5");

            IRadioButton radioButton = baseView.CreateWidget(WidgetType.RadioButton) as IRadioButton;
            radioButton.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            radioButton.WidgetStyle.Height = 18;
            radioButton.WidgetStyle.Width = 120;
            radioButton.WidgetStyle.X = 0;
            radioButton.WidgetStyle.Y = 300;
            radioButton.WidgetStyle.Z = 8;

            radioButton.AlignButton = AlignButton.Right;
            radioButton.Text = "Radio Button";
            radioButton.Tooltip = "A right aligned radio button";

            IShape lable = baseView.CreateWidget(WidgetType.Shape) as IShape;
            lable.ShapeType = ShapeType.Paragraph;
            lable.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            lable.WidgetStyle.Height = 100;
            lable.WidgetStyle.Width = 200;
            lable.WidgetStyle.X = 150;
            lable.WidgetStyle.Y = 300;
            lable.WidgetStyle.Z = 9;
            lable.Name = "Label 1";
            lable.Tooltip = "A label.";
            lable.SetRichText("Label");
            lable.WidgetStyle.LineColor = new StyleColor(ColorFillType.Solid, -16777216);
            lable.WidgetStyle.LineWidth = 0; // No border
            lable.WidgetStyle.FillColor = new StyleColor(ColorFillType.Solid, 16777215); // Transparent
            lable.WidgetStyle.HorzAlign = Alignment.Left;
            lable.WidgetStyle.VertAlign = Alignment.Top;

            IShape roundedRectangle = baseView.CreateWidget(WidgetType.Shape) as IShape;
            roundedRectangle.ShapeType = ShapeType.RoundedRectangle;
            roundedRectangle.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            roundedRectangle.WidgetStyle.Height = 100;
            roundedRectangle.WidgetStyle.Width = 200;
            roundedRectangle.WidgetStyle.X = 400;
            roundedRectangle.WidgetStyle.Y = 300;
            roundedRectangle.WidgetStyle.Z = 10;
            roundedRectangle.Name = "RoundedRectangle 1";
            roundedRectangle.Tooltip = "A Rounded Rectangle.";
            roundedRectangle.SetRichText("RoundedRectangle");

            IShape triangle = baseView.CreateWidget(WidgetType.Shape) as IShape;
            triangle.ShapeType = ShapeType.Triangle;
            triangle.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            triangle.WidgetStyle.Height = 100;
            triangle.WidgetStyle.Width = 100;
            triangle.WidgetStyle.X = 650;
            triangle.WidgetStyle.Y = 300;
            triangle.WidgetStyle.Z = 11;
            triangle.Name = "Triangle 1";
            triangle.Tooltip = "A Triangle.";
            triangle.SetRichText("Triangle");

            ISvg svg = baseView.CreateWidget(WidgetType.SVG) as ISvg;
            svg.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            svg.WidgetStyle.Height = 117;
            svg.WidgetStyle.Width = 150;
            svg.WidgetStyle.X = 0;
            svg.WidgetStyle.Y = 450;
            svg.WidgetStyle.Z = 12;
            svg.Name = "airplane 03";
            svg.Tooltip = "A airplane svg";

            string svgFile = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "Svg", "airplane 03.svg");
            if (File.Exists(svgFile))
            {
                using (FileStream fileStream = new FileStream(svgFile, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream svgStream = new MemoryStream();
                    fileStream.CopyTo(svgStream);
                    svg.XmlStream = svgStream;
                }
            }

            ITextArea textArea = baseView.CreateWidget(WidgetType.TextArea) as ITextArea;
            textArea.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            textArea.WidgetStyle.Height = 100;
            textArea.WidgetStyle.Width = 200;
            textArea.WidgetStyle.X = 250;
            textArea.WidgetStyle.Y = 450;
            textArea.WidgetStyle.Z = 12;
            textArea.Name = "TextArea 1";
            textArea.Tooltip = "A hidden border text area with max length is 10.";
            textArea.HintText = "Password";
            textArea.MaxLength = 10;
            textArea.HideBorder = true;

            ITextField textField = baseView.CreateWidget(WidgetType.TextField) as ITextField;
            textField.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            textField.WidgetStyle.Height = 50;
            textField.WidgetStyle.Width = 100;
            textField.WidgetStyle.X = 500;
            textField.WidgetStyle.Y = 450;
            textField.WidgetStyle.Z = 13;
            textField.Name = "TextField 1";
            textField.Tooltip = "A TextField";
            textField.TextFieldType = TextFieldType.Email;
            textField.HintText = "emial";

            IHamburgerMenu hamburgerMenu = baseView.CreateWidget(WidgetType.HamburgerMenu) as IHamburgerMenu;
            hamburgerMenu.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            hamburgerMenu.WidgetStyle.Height = 280;
            hamburgerMenu.WidgetStyle.Width = 150;
            hamburgerMenu.WidgetStyle.X = 0;
            hamburgerMenu.WidgetStyle.Y = 700;
            hamburgerMenu.WidgetStyle.Z = 14;
            hamburgerMenu.Name = "HamburgerMenu 1";
            hamburgerMenu.Tooltip = "A hamburger menu.";

            // Menu botton
            hamburgerMenu.MenuButton.WidgetStyle.Height = 50;
            hamburgerMenu.MenuButton.WidgetStyle.Width = 50;
            hamburgerMenu.MenuButton.WidgetStyle.X = 0;
            hamburgerMenu.MenuButton.WidgetStyle.Y = 700;
            hamburgerMenu.MenuButton.WidgetStyle.Z = 0;

            // Menu page,  add a shape
            IPage menuPage = hamburgerMenu.MenuPage;
            menuPage.Open(); // Open page to edit.

            // Get the base view of menu page.
            IPageView menuBaseView = menuPage.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create widgts on the base view in the menu page.
            IShape diamond = menuBaseView.CreateWidget(WidgetType.Shape) as IShape;
            diamond.ShapeType = ShapeType.Diamond;
            diamond.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            diamond.WidgetStyle.Height = 100;
            diamond.WidgetStyle.Width = 100;
            diamond.Name = "Diamond 1";
            diamond.Tooltip = "A Diamond.";
            diamond.SetRichText("Diamond");
            menuPage.Close(); // Close Page to release resources.

            IToast toast = baseView.CreateWidget(WidgetType.Toast) as IToast;
            toast.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            toast.WidgetStyle.Height = 146;
            toast.WidgetStyle.Width = 298;
            toast.WidgetStyle.X = 200;
            toast.WidgetStyle.Y = 700;
            toast.WidgetStyle.Z = 15;
            toast.Name = "Toast 1";
            toast.Tooltip = "A ExposureTime toast.";
            toast.ExposureTime = 3;
            toast.DisplayPosition = ToastDisplayPosition.Top;
            toast.CloseSetting = ToastCloseSetting.CloseButton;

            IPage toastPage = toast.ToastPage;
            toastPage.Open(); // Open page to edit.

            // Get the base view of toast page.
            IPageView toastBaseView = toastPage.PageViews[document.AdaptiveViewSet.Base.Guid];

            IShape ellipse = toastBaseView.CreateWidget(WidgetType.Shape) as IShape;
            ellipse.ShapeType = ShapeType.Ellipse;
            ellipse.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            ellipse.WidgetStyle.Height = 100;
            ellipse.WidgetStyle.Width = 100;
            ellipse.Name = "Ellipse 1";
            ellipse.Tooltip = "A Ellipse.";
            ellipse.SetRichText("Ellipse");
            toastPage.Close(); // Close Page to release resources.

            IDynamicPanel dynamicPanel = baseView.CreateWidget(WidgetType.DynamicPanel) as IDynamicPanel;
            dynamicPanel.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            dynamicPanel.WidgetStyle.Height = 198;
            dynamicPanel.WidgetStyle.Width = 152;
            dynamicPanel.WidgetStyle.X = 500;
            dynamicPanel.WidgetStyle.Y = 700;
            dynamicPanel.WidgetStyle.Z = 16;
            dynamicPanel.IsAutomatic = true;

            // Set start panel page as the first created page.
            dynamicPanel.StartPanelStatePage = dynamicPanel.CreatePanelStatePage("Panel 1");
            dynamicPanel.CreatePanelStatePage("Panel 2");
            dynamicPanel.CreatePanelStatePage("Panel 3");

            int imageFileName = 1;
            foreach(IPage statePage in dynamicPanel.PanelStatePages)
            {
                statePage.Open(); // Open page to edit.
                
                // Get the base view of state page.
                IPageView stateBaseView = statePage.PageViews[document.AdaptiveViewSet.Base.Guid];

                IImage statePageImage = stateBaseView.CreateWidget(WidgetType.Image) as IImage;
                statePageImage.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
                statePageImage.WidgetStyle.Height = 198;
                statePageImage.WidgetStyle.Width = 152;
                string statePageImageFile = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "HangGame", imageFileName + ".png");
                if (File.Exists(statePageImageFile))
                {
                    using (FileStream fileStream = new FileStream(statePageImageFile, FileMode.Open, FileAccess.Read))
                    {
                        MemoryStream imageStream = new MemoryStream();
                        fileStream.CopyTo(imageStream);
                        statePageImage.ImageStream = imageStream;
                    }
                }
                statePage.Close(); // Close Page to release resources.

                imageFileName++;
            }

            // Close the page if you don't want to work on it.
            page.Close();

            // Save the document to a pn file.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // Close this document when you don't work on it anymore.
            Program.Service.Close();
        }
    }
}
