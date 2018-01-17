using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Naver.Compass.Common;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.WidgetLibrary
{
    public partial class SmallEditText : BaseEditControl
    {

        public SmallEditText()
        {
            InitializeComponent();

            Loaded += RichEditText_Loaded;
        }
        private void RichEditText_Loaded(object sender, RoutedEventArgs e)
        {
            Content = GetDisPlayControl();
        }

        private  RichBlocks _displayControl;
        private static Label _EditControl = null;

        #region Event Handlers

        void RichControl_LostFocus(object sender, RoutedEventArgs e)
        {
            IsInEditMode = false;
        }

        override protected void OnEdtModeChange(bool newValue)
        {
            RemoveLogicalChild(Content);
            Content = null;

            if (newValue)
            {
                Content = GetEditDateControl();

                ClearDisplayBinding();
                
            }
            else
            {
                Content = GetDisPlayControl();

                ClearEditBinding();
            }
        }

        private void RichTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is WidgetViewModelDate)
                {
                    ((WidgetViewModelDate)DataContext).viewInput = (UIElement)sender;
                }
            }
            catch (System.Exception ex)
            {
                NLogger.Warn("Init IInputelment error!\n" + ex.Message);
            }
        }

        private RichBlocks GetDisPlayControl()
        {
            if (_displayControl == null)
            {
                #region Init textBlock

                _displayControl = new RichBlocks();

                _displayControl.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                _displayControl.SetValue(UseLayoutRoundingProperty, true);
                _displayControl.SetValue(ClipToBoundsProperty, false);
                _displayControl.SetValue(BackgroundProperty, null);//new SolidColorBrush(Color.FromRgb(0,255,0))
                _displayControl.SetValue(RenderTransformOriginProperty, new Point(0.5, 0.5));

                #endregion
            }

            #region Init Binding

            _displayControl.AddHandler(RichBlocks.LoadedEvent, new RoutedEventHandler(RichTextBlock_Loaded));
            

            Binding bind = new Binding("vFontSize");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.DisplayFontsizeProperty, bind);

            bind = new Binding("vFontColor");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.DFontcolorProperty, bind);

            bind = new Binding("vFontFamily");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.DFontFamilyProperty, bind);

            bind = new Binding("vFontBold");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.DFontBoldProperty, bind);

            bind = new Binding("vFontItalic");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.DFontItalicProperty, bind);

            bind = new Binding("vFontUnderLine");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.DFontUnderlineProperty, bind);

            bind = new Binding("vFontStrickeThrough");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.DFontStrikeThoughProperty, bind);

            bind = new Binding("vTextHorAligen");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            bind.Converter = new DocmentAlignDataConverter();
            _displayControl.SetBinding(RichBlocks.DTextHorAligenProperty, bind);

            bind = new Binding("vTextVerAligen");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.Converter = new AlignDataConverter();
            _displayControl.SetBinding(RichBlocks.VerticalContentAlignmentProperty, bind);

            bind = new Binding("TextRotate");
            bind.Source = DataContext;
            bind.Mode = BindingMode.OneWay;
            bind.Converter = new RotateTransConverter();
            _displayControl.SetBinding(RenderTransformProperty, bind);

            bind = new Binding("vTextBulletStyle");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.TextBulletStyleProperty, bind);

            bind = new Binding("vTextContent");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            _displayControl.SetBinding(RichBlocks.RichStringProperty, bind);

            #endregion

            return _displayControl;
        }

        private Label GetEditDateControl()
        {
            #region RichTextBox Define

            if (_EditControl == null)
            {
                _EditControl = new Label();
                _EditControl.SetValue(Label.BackgroundProperty, null);
                _EditControl.SetValue(Label.ClipToBoundsProperty, false);
                _EditControl.SetValue(Label.UseLayoutRoundingProperty, true);
                _EditControl.SetValue(Label.PaddingProperty, new Thickness(0));
                _EditControl.SetValue(Label.MarginProperty, new Thickness(0, 5, 0,-1));
                _EditControl.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                _EditControl.SetValue(Label.RenderTransformOriginProperty, new Point(0.5, 0.5));

                ExRichTextBox temprich = new ExRichTextBox();
                temprich.SetValue(ExRichTextBox.PaddingProperty, new Thickness(0));
                temprich.SetValue(ExRichTextBox.MarginProperty, new Thickness(0));

                temprich.SetValue(ExRichTextBox.BorderThicknessProperty, new Thickness(0));
                temprich.SetValue(ExRichTextBox.BackgroundProperty, null);//new SolidColorBrush(Color.FromRgb(255,0,0))
                temprich.SetValue(ExRichTextBox.BorderBrushProperty, null);

                _EditControl.Content = temprich;
            }
            else if (_EditControl.Parent != null)
            {
                SmallEditText ParentUI = _EditControl.Parent as SmallEditText;
                if (ParentUI != null)
                {
                    ParentUI.RemoveContent(_EditControl);
                }
            }
            #endregion

            AddEditMenu((ExRichTextBox)_EditControl.Content);
            #region InitBinding and Handler

            Binding bind = new Binding("TextRotate");
            bind.Source = DataContext;
            bind.Mode = BindingMode.OneWay;
            bind.Converter = new RotateTransConverter();
            _EditControl.SetBinding(Label.RenderTransformProperty, bind);

            bind = new Binding("vTextVerAligen");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.Converter = new VerticalAlignDataConverter();
            _EditControl.SetBinding(Label.VerticalContentAlignmentProperty, bind);

            ExRichTextBox rich = (ExRichTextBox)_EditControl.Content;

            rich.AddHandler(LostFocusEvent, new RoutedEventHandler(RichControl_LostFocus));

            bind = new Binding("vFontSize");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            rich.SetBinding(ExRichTextBox.SectionFontsizeProperty, bind);

            bind = new Binding("vFontColor");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            rich.SetBinding(ExRichTextBox.SectionFontcolorProperty, bind);

            bind = new Binding("vFontFamily");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            rich.SetBinding(ExRichTextBox.SectionFontFamilyProperty, bind);

            bind = new Binding("vFontBold");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            rich.SetBinding(ExRichTextBox.SectionFontBoldProperty, bind);

            bind = new Binding("vFontItalic");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            rich.SetBinding(ExRichTextBox.SectionFontItalicProperty, bind);

            bind = new Binding("vFontUnderLine");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            rich.SetBinding(ExRichTextBox.SectionFontUnderlineProperty, bind);

            bind = new Binding("vFontStrickeThrough");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            rich.SetBinding(ExRichTextBox.SectionFontStrikeThoughProperty, bind);

            bind = new Binding("vTextHorAligen");
            bind.Source = DataContext;
            bind.Mode = BindingMode.TwoWay;
            bind.NotifyOnTargetUpdated = true;
            bind.Converter = new DocmentAlignDataConverter();
            rich.SetBinding(ExRichTextBox.FontAlignmentProperty, bind);

            bind = new Binding("vTextBulletStyle");
            bind.Source = DataContext;
            bind.NotifyOnTargetUpdated = true;
            bind.Mode = BindingMode.TwoWay;
            rich.SetBinding(ExRichTextBox.TextBulletStyleProperty, bind);

            bind = new Binding("vTextContent");
            bind.Source = DataContext;
            bind.NotifyOnTargetUpdated = true;
            bind.Mode = BindingMode.TwoWay;
            rich.SetBinding(ExRichTextBox.TextProperty, bind);

            #endregion

           return _EditControl;
        }

        private void AddEditMenu(ExRichTextBox control)
        {
            if (control.ContextMenu == null)
            {
                control.ContextMenu = new ContextMenu();
            }
            control.ContextMenu.Items.Clear();

            MenuItem Item = new MenuItem();
            Item.InputGestureText = "Ctrl+X";
            Item.Command = System.Windows.Input.ApplicationCommands.Cut;
            Item.CommandTarget = control;
            Item.Header = GlobalData.FindResource("Menu_File_Cut");
            Item.Style = Application.Current.Resources["topLevel"] as Style;
            control.ContextMenu.Items.Add(Item);

            Item = new MenuItem();
            Item.InputGestureText = "Ctrl+C";
            Item.Command = System.Windows.Input.ApplicationCommands.Copy;
            Item.CommandTarget = control;
            Item.Header = GlobalData.FindResource("Menu_File_Copy");
            Item.Style = Application.Current.Resources["topLevel"] as Style;
            control.ContextMenu.Items.Add(Item);

            Item = new MenuItem();
            Item.InputGestureText = "Ctrl+V";
            Item.Command = System.Windows.Input.ApplicationCommands.Paste;
            Item.CommandTarget = control;
            Item.Header = GlobalData.FindResource("Menu_File_Paste");
            Item.Style = Application.Current.Resources["topLevel"] as Style;
            control.ContextMenu.Items.Add(Item);

            control.ContextMenu.Items.Add(new Separator());

            Item = new MenuItem();
            Item.InputGestureText = "Ctrl+A";
            Item.Command = System.Windows.Input.ApplicationCommands.SelectAll;
            Item.CommandTarget = control;
            Item.Header = GlobalData.FindResource("Menu_File_SelectAll");
            Item.Style = Application.Current.Resources["topLevel"] as Style;
            control.ContextMenu.Items.Add(Item);

        }

        public void RemoveContent(Object child)
        {
            RemoveLogicalChild(child);
        }

        private void ClearEditBinding()
        {
            if (_EditControl != null)
            {
                //BindingOperations.ClearAllBindings((ExRichTextBox)_EditControl.Content);
                //BindingOperations.ClearAllBindings(_EditControl);

                ExRichTextBox rich = (ExRichTextBox)_EditControl.Content;

                rich.RemoveHandler(LostFocusEvent, new RoutedEventHandler(RichControl_LostFocus));
            }
        }
       
        private void ClearDisplayBinding()
        {
            if (_displayControl != null)
            {
                BindingOperations.ClearAllBindings(_displayControl);

                _displayControl.RemoveHandler(RichBlocks.LoadedEvent, new RoutedEventHandler(RichTextBlock_Loaded));
            }
        }

        #endregion Event Handlers

        #region Section binding properties

        #region TextRotate

        public int TextRotate
        {
            get
            {
                return (int)GetValue(TextRotateProperty);
            }
            set
            {
                SetValue(TextRotateProperty, value);
            }
        }

        public static readonly DependencyProperty TextRotateProperty =
           DependencyProperty.Register(
           "TextRotate",
           typeof(int),
           typeof(SmallEditText));

        #endregion

        #endregion



    }
}
