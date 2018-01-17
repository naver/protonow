using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Windows.Controls.Ribbon;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Naver.Compass.Service.Document;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Common
{
    /// <summary>
    /// Interaction logic for CompassColorPicker.xaml
    /// </summary>
    public partial class CompassColorPicker : System.Windows.Controls.UserControl, IPreviewCommandSource
    {
        public CompassColorPicker()
        {
            InitializeComponent();
            _currentColor = ColorSelected;
            _initialBorder = null;
            _orignColor = ColorSelected;
            //ColorPanelShow.Fill = new SolidColorBrush(_currentColor);
            Loaded += CompassColorPicker_Loaded;
        }

        void CompassColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            ColorInfoShow.Focus();
            ColorInfoShow.SelectionStart = ColorInfoShow.Text.Length;
            Popup pop = GetPopUpRoot();
            if (pop == null)
            {
                return;
            }

            //pop.Closed += Pop_Closed;
            pop.Opened -= Pop_Opened;
            pop.Opened += Pop_Opened;
            pop.PreviewLostKeyboardFocus -= pop_PreviewLostKeyboardFocus;
            pop.PreviewLostKeyboardFocus += pop_PreviewLostKeyboardFocus;

            DependencyPropertyDescriptor gradienteditorSelectColor = DependencyPropertyDescriptor.FromProperty(GradientEditor.SelectedColorProperty, typeof(GradientEditor));
            gradienteditorSelectColor.AddValueChanged(this.GradientEditor1, this.GradientedEditorSelectedColorChanged);
        }

        private void GradientedEditorSelectedColorChanged(object sender, EventArgs e)
        {
            var color32 = this.GradientEditor1.SelectedColor;
            var solidbrush = new SolidColorBrush(color32);
            ColorPanelShow.Fill = solidbrush;

            //Initial color-32 information
            string sARGB = ColorPanelShow.Fill.ToString();
            string sRGB = @"#" + sARGB.Substring(3, sARGB.Length - 3);
            ColorInfoShow.Text = sRGB;

            //Initialize the Alpha value
            OpacityText.Text = Convert.ToInt32(solidbrush.Color.A * 100 / 255).ToString();

            if (_initialBorder != null)
            {
                _initialBorder.Focusable = false;
            }
            Border target = EnumVisual(ColorPanel, color32);
            if (target == null)
            {
                target = EnumVisual(ColorPane2, color32);
            }
            _initialBorder = target;

            if (target != null)
            {
                //target.BorderThickness = new Thickness(2);
                //target.Tag = 0;
                target.Focusable = true;

            }
        }

        private void InvokeCommand(StyleColor color)
        {
            dynamic dataCollection = DataCollection;
            if (dataCollection is System.Collections.IDictionary && dataCollection.ContainsKey(DataCollectionKey))
            {
                var spData = dataCollection[DataCollectionKey];
                if (spData != null)
                {
                    if (DataCollectionKey == "Font Color")
                    {
                        spData.CommandParameter = color.ToBrush();
                    }
                    else
                    {
                        spData.CommandParameter = color;
                    }
                }
            }

            CommandHelpers.InvokeCommandSource(color, null, this, CommandOperation.Execute);
        }

        void pop_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (GetPopUpRoot(e.NewFocus as FrameworkElement) != null)
            {
                return;
            }

            if (_bIsShowDlgStatus == true)
            {
                return;
            }

            ///to do
            if (this.IsSolidType())
            {
                if (ColorPanelShow.Fill is SolidColorBrush)
                {
                    System.Windows.Media.Color panelColor = (ColorPanelShow.Fill as SolidColorBrush).Color;
                    var sc = new StyleColor(ColorFillType.Solid, System.Drawing.Color.FromArgb(panelColor.A, panelColor.R, panelColor.G, panelColor.B).ToArgb());
                    if (!_orignColor.Equals(sc))
                    {
                        _currentColor = sc;
                        InvokeCommand(_currentColor);
                    }
                }
            }
            else
            {
                var angle = 90d;
                double.TryParse(anglebox.Text, out angle);
                var rtnStyle = new StyleColor(ColorFillType.Gradient, 0);
                rtnStyle.Angle = angle;
                rtnStyle.Frames = this.GradientEditor1.CurrentStyle.Frames;
                if (!rtnStyle.Equals(_orignColor))
                {
                    InvokeCommand(rtnStyle);
                }
            }
        }

        void Pop_Opened(object sender, EventArgs e)
        {
            if (null != _initialBorder)
            {
                //_initialBorder.BorderThickness = new Thickness(0);
                //_initialBorder.Tag = 255;
                _initialBorder.Focusable = false;
            }


            Border target = EnumVisual(ColorPanel);
            if (target == null)
            {
                target = EnumVisual(ColorPane2);
            }
            _initialBorder = target;

            if (target != null)
            {
                //target.BorderThickness = new Thickness(2);
                //target.Tag = 0;
                target.Focusable = true;

            }
        }

        private Border EnumVisual(Visual myVisual, System.Windows.Media.Color c)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);
                if (childVisual != null)
                {
                    if (childVisual is Border)
                    {
                        System.Windows.Media.Color curColor = ((childVisual as Border).Background as SolidColorBrush).Color;
                        if (curColor.R == c.R
                            && curColor.G == c.G
                            && curColor.B == c.B)
                        {
                            return (childVisual as Border);
                        }
                    }
                    else if (childVisual is StackPanel)
                    {
                        Border Target = EnumVisual(childVisual, c);
                        if (Target != null)
                        {
                            return Target;
                        }
                    }
                }
            }
            return null;
        }

        private Border EnumVisual(Visual myVisual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);
                if (childVisual != null)
                {
                    if (childVisual is Border)
                    {
                        System.Windows.Media.Color curColor = ((childVisual as Border).Background as SolidColorBrush).Color;
                        if (curColor.R == _currentColor.ToColor().R
                            && curColor.G == _currentColor.ToColor().G
                            && curColor.B == _currentColor.ToColor().B)
                        {
                            return (childVisual as Border);
                        }
                    }
                    else if (childVisual is StackPanel)
                    {
                        Border Target = EnumVisual(childVisual);
                        if (Target != null)
                        {
                            return Target;
                        }
                    }
                }
            }
            return null;
        }
        //void Pop_Closed(object sender, EventArgs e)
        //{
        //    //if (_bIsShowDlgStatus == true)
        //    //{
        //    //    return;
        //    //}

        //    //System.Windows.Media.Color panelColor=(ColorPanelShow.Fill as SolidColorBrush).Color;
        //    //if (panelColor != _currentColor)
        //    //{
        //    //    _currentColor = panelColor;
        //    //    InvokeCommand(_currentColor);
        //    //}
        //}
        #region Private Member and Function
        Border _initialBorder;
        private bool _bIsShowDlgStatus = false;
        private StyleColor _currentColor, _orignColor;
        private Popup GetPopUpRoot()
        {
            DependencyObject UIParent = VisualTreeHelper.GetParent(this);
            while (UIParent != null)
            {
                FrameworkElement ele = UIParent as FrameworkElement;
                if (ele as Popup != null)
                {
                    Popup elem = ele as Popup;
                    return elem;
                }
                UIParent = VisualTreeHelper.GetParent(UIParent);
                if (UIParent == null)
                {
                    UIParent = ele.Parent;
                }
            }

            return null;
        }

        private Popup GetPopUpRoot(FrameworkElement target)
        {
            if (target == null)
                return null;

            DependencyObject UIParent = VisualTreeHelper.GetParent(target);
            while (UIParent != null)
            {
                FrameworkElement ele = UIParent as FrameworkElement;
                if (ele as Popup != null)
                {
                    Popup elem = ele as Popup;
                    return elem;
                }
                UIParent = VisualTreeHelper.GetParent(UIParent);
                if (UIParent == null)
                {
                    UIParent = ele.Parent;
                }
            }

            return null;
        }

        #endregion

        #region  waiting for remove
        public StyleColor ColorSelected1
        {
            get { return (StyleColor)GetValue(ColorSelected1Property); }
            set
            {
                SetValue(ColorSelected1Property, value);
            }
        }
        public static readonly DependencyProperty ColorSelected1Property =
          DependencyProperty.Register("ColorSelected1", typeof(StyleColor),
                                      typeof(CompassColorPicker),
                                      new FrameworkPropertyMetadata(default(StyleColor), new PropertyChangedCallback(OnColorChanged2)));

        static void OnColorChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CompassColorPicker).OnColorChanged2();
        }
        private void OnColorChanged2()
        {
            _currentColor = ColorSelected1;
            _orignColor = _currentColor;

            //Initial Color panel
            var solidbrush = default(SolidColorBrush);
            if (_currentColor.FillType == ColorFillType.Gradient && _currentColor.Frames != null && _currentColor.Frames.Count > 0)
            {
                solidbrush = _currentColor.Frames.First().Value.ToBrush() as SolidColorBrush;
                anglebox.Text = ((int)_currentColor.Angle).ToString();
            }
            else
            {
                solidbrush = _currentColor.ARGB.ToBrush() as SolidColorBrush;
            }

            ColorPanelShow.Fill = solidbrush;

            //Initial color-32 information
            string sARGB = ColorPanelShow.Fill.ToString();
            string sRGB = @"#" + sARGB.Substring(3, sARGB.Length - 3);
            ColorInfoShow.Text = sRGB;

            //Initialize the Alpha value
            OpacityText.Text = Convert.ToInt32(solidbrush.Color.A * 100 / 255).ToString();

            if (this.IsLoaded)
            {
                this.SetControlVisible(_currentColor.FillType);
            }
            else
            {
                RoutedEventHandler handler = default(RoutedEventHandler);
                handler = (sender, e) =>
                {
                    this.Loaded -= handler;
                    this.SetControlVisible(_currentColor.FillType);
                };

                this.Loaded += handler;
            }
        }
        #endregion


        #region Dependecy Propery

        public static readonly DependencyProperty DataCollectionProperty =
            DependencyProperty.Register("DataCollection", typeof(object), typeof(CompassColorPicker),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None));

        public object DataCollection
        {
            get { return (object)GetValue(DataCollectionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }


        public static readonly DependencyProperty DataCollectionKeyProperty =
            DependencyProperty.Register("DataCollectionKey", typeof(string), typeof(CompassColorPicker),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None));

        public string DataCollectionKey
        {
            get { return (string)GetValue(DataCollectionKeyProperty); }
            set { SetValue(DataCollectionKeyProperty, value); }
        }
        public StyleColor ColorSelected
        {
            get { return (StyleColor)GetValue(ColorSelectedProperty); }
            set
            {
                SetValue(ColorSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty ColorSelectedProperty =
          DependencyProperty.Register("ColorSelected", typeof(StyleColor),
                                      typeof(CompassColorPicker),
                                      new FrameworkPropertyMetadata(
                                          new StyleColor(ColorFillType.Solid, System.Drawing.Color.FromArgb(0x55, 0x55, 0x55).ToArgb()),
                                          new PropertyChangedCallback(OnColorChanged)));

        static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CompassColorPicker).OnColorChanged();
        }
        private void OnColorChanged()
        {
            _currentColor = ColorSelected;
            _orignColor = _currentColor;

            //Initial Color panel
            var solidbrush = default(SolidColorBrush);
            if (_currentColor.FillType == ColorFillType.Gradient && _currentColor.Frames != null && _currentColor.Frames.Count > 0)
            {
                solidbrush = _currentColor.Frames.First().Value.ToBrush() as SolidColorBrush;
            }
            else
            {
                solidbrush = _currentColor.ARGB.ToBrush() as SolidColorBrush;
            }

            //Initial color-32 information
            string sARGB = ColorPanelShow.Fill.ToString();
            string sRGB = @"#" + sARGB.Substring(3, sARGB.Length - 3);
            ColorInfoShow.Text = sRGB;

            //Initialize the Alpha value
            OpacityText.Text = Convert.ToInt32(solidbrush.Color.A * 100 / 255).ToString();
            OpacitySlider.UpdateLayout();
            //TargetColorRec.Fill = new SolidColorBrush(Color32);
            //TargetColorRec.UpdateLayout();
        }
        #region PreviewCommandParameter
        public object PreviewCommandParameter
        {
            get { return (object)GetValue(PreviewCommandParameterProperty); }
            set { SetValue(PreviewCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty PreviewCommandParameterProperty =
                    DependencyProperty.Register(
                            "PreviewCommandParameter",
                            typeof(object),
                            typeof(CompassColorPicker),
                            new FrameworkPropertyMetadata(null));
        #endregion

        #region Taraget Color




        #endregion

        #region IsGradientEnable

        public static readonly DependencyProperty IsGradientEnableProperty =
            DependencyProperty.Register("IsGradientEnable", typeof(bool), typeof(CompassColorPicker),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(OnIsGradientEnableChanged)));

        public bool IsGradientEnable
        {
            get { return (bool)GetValue(IsGradientEnableProperty); }
            set { SetValue(IsGradientEnableProperty, value); }
        }


        private static void OnIsGradientEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CompassColorPicker target = (CompassColorPicker)d;
            bool oldIsGradientEnable = (bool)e.OldValue;
            bool newIsGradientEnable = target.IsGradientEnable;
            target.OnIsGradientEnableChanged(oldIsGradientEnable, newIsGradientEnable);
        }

        protected virtual void OnIsGradientEnableChanged(bool oldIsGradientEnable, bool newIsGradientEnable)
        {
            if (!newIsGradientEnable)
            {
                if (this.IsLoaded)
                {
                    this.DisableGradient();
                }
                else
                {
                    RoutedEventHandler handler = default(RoutedEventHandler);
                    handler = (sender, e) =>
                    {
                        this.Loaded -= handler;
                        this.DisableGradient();
                    };

                    this.Loaded += handler;
                }
            }
            else
            {
                if (this.IsLoaded)
                {
                    this.EnableGradient();
                }
                else
                {
                    RoutedEventHandler handler = default(RoutedEventHandler);
                    handler = (sender, e) =>
                    {
                        this.Loaded -= handler;
                        this.EnableGradient();
                    };

                    this.Loaded += handler;
                }
            }
        }

        private void DisableGradient()
        {
            ComboboxType.IsEnabled = false;
        }

        private void EnableGradient()
        {
            ComboboxType.IsEnabled = true;
        }

        #endregion



        #region CommandTarget
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }
        public static readonly DependencyProperty CommandTargetProperty =
                    DependencyProperty.Register(
                            "CommandTarget",
                            typeof(IInputElement),
                            typeof(CompassColorPicker),
                            new FrameworkPropertyMetadata(null));
        #endregion

        #region CommandParameter
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        public static readonly DependencyProperty CommandParameterProperty =
                    DependencyProperty.Register(
                            "CommandParameter",
                            typeof(object),
                            typeof(CompassColorPicker),
                            new FrameworkPropertyMetadata(null));
        #endregion

        #region Command
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
                    DependencyProperty.Register(
                            "Command",
                            typeof(ICommand),
                            typeof(CompassColorPicker),
                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //RibbonTextBox textBox = (RibbonTextBox)d;
            //ICommand oldCommand = (ICommand)e.OldValue;
            //ICommand newCommand = (ICommand)e.NewValue;

            //if (oldCommand != null)
            //{
            //    textBox.UnhookCommand(oldCommand);
            //}
            //if (newCommand != null)
            //{
            //    textBox.HookCommand(newCommand);
            //}

            //RibbonHelper.OnCommandChanged(d, e);
        }
        #endregion

        #region Title

        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(CompassColorPicker),
        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Model property.  This dependency property 
        /// indicates model attached to this view.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        #endregion
        #endregion

        #region Toolbar Item Event handler
        private void SetColorButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            // PART_Popup.IsOpen = !PART_Popup.IsOpen;
            e.Handled = true;
        }
        #endregion



        #region Command Invoker
        private void ColorPane2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsSolidType())
            {
                RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
            }

            e.Handled = true;
        }

        private bool IsSolidType()
        {
            return ComboboxType.SelectedIndex == 0;
        }

        private void ColorExample_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (OpacitySlider == null)
                return;

            Border targetBox = e.OriginalSource as Border;
            if (targetBox == null)
                return;

            System.Windows.Media.Color Color16 = (targetBox.Background as SolidColorBrush).Color;
            Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(A, Color16.R, Color16.G, Color16.B);

            if (this.IsSolidType())
            {
                if (Color32.A == 0x00)
                {
                    Color32.A = 0xff;
                }

                ColorSelected = new StyleColor(ColorFillType.Solid, Color32.ToArgb());
                //PART_Popup.IsOpen = false;
                InvokeCommand(ColorSelected);
            }
            else
            {
                GradientEditor1.SelectedColor = Color32;

                /*InvokeCommand(GradientEditor1.CurrentStyle);
                //PART_Popup.IsOpen = false;
                RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));*/
            }
        }

        private void TransparentButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border targetBox = e.OriginalSource as Border;
            if (targetBox == null)
                return;

            System.Windows.Media.Color Color16 = (targetBox.Background as SolidColorBrush).Color;
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(0, Color16.R, Color16.G, Color16.B);

            if (this.IsSolidType())
            {
                ColorSelected = new StyleColor(ColorFillType.Solid, Color32.ToArgb());

                InvokeCommand(ColorSelected);
                RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
            }
            else
            {
                GradientEditor1.SelectedColor = Color32;
                //InvokeCommand(GradientEditor1.CurrentStyle);
                //RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
            }
        }
        private void ColorInfoShow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                System.Windows.Media.Color Color16 = (ColorPanelShow.Fill as SolidColorBrush).Color;
                System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(Color16.A, Color16.R, Color16.G, Color16.B);
                if (this.IsSolidType())
                {
                    ColorSelected = new StyleColor(ColorFillType.Solid, Color32.ToArgb());
                    //PART_Popup.IsOpen = false;
                    InvokeCommand(ColorSelected);
                    RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
                }
                else
                {
                    GradientEditor1.SelectedColor = Color32;
                    InvokeCommand(GradientEditor1.CurrentStyle);
                    RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
                }
            }
        }
        private void ColorPanelShow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            System.Windows.Media.Color Color16 = (ColorPanelShow.Fill as SolidColorBrush).Color;
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(Color16.A, Color16.R, Color16.G, Color16.B);
            if (this.IsSolidType())
            {
                ColorSelected = new StyleColor(ColorFillType.Solid, Color32.ToArgb());
                //PART_Popup.IsOpen = false;
                InvokeCommand(ColorSelected);
                RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
            }
            else
            {
                GradientEditor1.SelectedColor = Color32;
                StyleColor para = GradientEditor1.CurrentStyle;
                double.TryParse(anglebox.Text, out para.Angle);
                InvokeCommand(para);
                RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
            }
        }
        private void OpacityText_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (OpacitySlider == null)
                return;

            if (e.Key == Key.Enter)
            {
                try
                {
                    int value = Convert.ToInt32(OpacityText.Text);
                    OpacitySlider.Value = value;
                    Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);

                    System.Windows.Media.Color Color16 = (ColorPanelShow.Fill as SolidColorBrush).Color;
                    System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(A, Color16.R, Color16.G, Color16.B);
                    if (this.IsSolidType())
                    {
                        ColorSelected = new StyleColor(ColorFillType.Solid, Color32.ToArgb());
                        //PART_Popup.IsOpen = false;
                        InvokeCommand(ColorSelected);
                        RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
                    }
                    else
                    {
                        GradientEditor1.SelectedColor = Color32;
                        InvokeCommand(GradientEditor1.CurrentStyle);
                        RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    e.Handled = false;
                }
            }
        }
        #endregion

        #region Poppup Item Event handler
        private void StackPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border targetBox = e.OriginalSource as Border;
            if (targetBox == null)
            {
                if (this.IsSolidType())
                {
                    Byte CurAlpha = Convert.ToByte(OpacitySlider.Value * 255 / 100);
                    var c = _currentColor.ToColor();
                    c.A = CurAlpha;
                    ColorPanelShow.Fill = new SolidColorBrush(c);
                    //Initial color-32 information
                    string szARGB = ColorPanelShow.Fill.ToString();
                    string szRGB = @"#" + szARGB.Substring(3, szARGB.Length - 3);
                    ColorInfoShow.Text = szRGB;
                }
                else
                {
                    GradientEditor1.RecoverHoverColor();
                    var c = GradientEditor1.SelectedColor;
                    ColorPanelShow.Fill = new SolidColorBrush(c);
                    //Initial color-32 information
                    string szARGB = ColorPanelShow.Fill.ToString();
                    string szRGB = @"#" + szARGB.Substring(3, szARGB.Length - 3);
                    ColorInfoShow.Text = szRGB;
                }
                return;
            }
            //set color show panel
            System.Windows.Media.Color Color16 = (targetBox.Background as SolidColorBrush).Color;
            Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(A, Color16.R, Color16.G, Color16.B);
            ColorPanelShow.Fill = new SolidColorBrush(Color32);

            //set color-32 information
            string sARGB = targetBox.Background.ToString();
            string sRGB = @"#" + sARGB.Substring(3, sARGB.Length - 3);
            ColorInfoShow.Text = sRGB;

            if (!this.IsSolidType())
            {
                GradientEditor1.SetHoverColor(Color32);
            }
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);
            System.Windows.Media.Color Color16 = (ColorPanelShow.Fill as SolidColorBrush).Color;
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(A, Color16.R, Color16.G, Color16.B);
            ColorPanelShow.Fill = new SolidColorBrush(Color32);
            if (!this.IsSolidType())
            {
                GradientEditor1.SetAlpha(A);
            }
        }
        private void TransparentButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsSolidType())
            {
                Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);
                var c = _currentColor.ToColor();
                c.A = A;
                ColorPanelShow.Fill = new SolidColorBrush(c);
                //Initial color-32 information
                string sARGB = ColorPanelShow.Fill.ToString();
                string sRGB = @"#" + sARGB.Substring(3, sARGB.Length - 3);
                ColorInfoShow.Text = sRGB;
            }
            else
            {
                GradientEditor1.RecoverHoverColor();
                var c = GradientEditor1.SelectedColor;
                ColorPanelShow.Fill = new SolidColorBrush(c);
                //Initial color-32 information
                string szARGB = ColorPanelShow.Fill.ToString();
                string szRGB = @"#" + szARGB.Substring(3, szARGB.Length - 3);
                ColorInfoShow.Text = szRGB;
            }
        }
        private void ColorDlgButton_Click(object sender, RoutedEventArgs e)
        {
            // RaiseEvent(new RibbonDismissPopupEventArgs(RibbonDismissPopupMode.Always));
            //PART_Popup.StaysOpen = true;

            _bIsShowDlgStatus = true;
            Popup popRoot = GetPopUpRoot();
            popRoot.StaysOpen = true;
            popRoot.IsOpen = true;

            ///Load custom color
            var customcolor = default(int[]);
            try
            {
                //var dsRegkey = Registry.CurrentUser.OpenSubKey(@"Software\Design Studio");
                //if (dsRegkey != null)
                //{
                //    var customColorobj = dsRegkey.GetValue("CustomColor");
                //    if (customColorobj != null)
                //    {
                //        customcolor = customColorobj.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
                //    }
                //}
                var customColorStre = ConfigFileManager.GetValue("CustomColor");
                if (!string.IsNullOrEmpty(customColorStre))
                {
                    customcolor = customColorStre.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
                }
            }
            catch
            {
            }

            System.Windows.Forms.ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (customcolor != null && customcolor.Length > 0)
            {
                colorDialog.CustomColors = customcolor;
            }

            popRoot.StaysOpen = true;

            var cdialogrst = colorDialog.ShowDialog();
            ///save custom color
            if (colorDialog.CustomColors != null && colorDialog.CustomColors.Length > 0)
            {
                ConfigFileManager.SetValue("CustomColor", string.Join(",", colorDialog.CustomColors));
            }

            if (cdialogrst == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Media.Color Color32 = new System.Windows.Media.Color();
                Color32.A = colorDialog.Color.A;
                Color32.B = colorDialog.Color.B;
                Color32.G = colorDialog.Color.G;
                Color32.R = colorDialog.Color.R;
                ColorPanelShow.Fill = new SolidColorBrush(Color32);

                //set color-32 information
                string sARGB = ColorPanelShow.Fill.ToString();
                string sRGB = @"#" + sARGB.Substring(3, sARGB.Length - 3);
                ColorInfoShow.Text = sRGB;


                //Invoke the command
                _bIsShowDlgStatus = false;
                //ColorSelected = Color32;
                if (this.IsSolidType())
                {
                    ColorSelected = new StyleColor(ColorFillType.Solid, Color32.ToArgb());
                    InvokeCommand(ColorSelected);
                }
                else
                {
                    GradientEditor1.SelectedColor = Color32;
                    _bIsShowDlgStatus = false;
                    popRoot.IsOpen = true;
                }
            }
            else
            {
                _bIsShowDlgStatus = false;
                popRoot.IsOpen = true;
            }


        }
        private void OpacityText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                OpacitySlider.Value = Convert.ToInt32(OpacityText.Text);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            OpacitySlider.UpdateLayout();
        }
        private void OpacityText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            try
            {
                int value = Convert.ToInt32(e.Text);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                e.Handled = true;
            }
        }
        private void ColorInfoShow_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string colorName = ColorInfoShow.Text;
                if (colorName.StartsWith("#"))
                {
                    colorName = colorName.Replace("#", string.Empty);
                }
                else
                {
                    ColorInfoShow.Text = @"#" + ColorInfoShow.Text;
                }
                ColorInfoShow.SelectionStart = ColorInfoShow.Text.Length;

                int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);

                //Byte A = Convert.ToByte((v >> 24) & 255);
                Byte R = Convert.ToByte((v >> 16) & 255);
                Byte G = Convert.ToByte((v >> 8) & 255);
                Byte B = Convert.ToByte((v >> 0) & 255);
                if (OpacitySlider == null)
                    return;
                Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);

                //set color show panel
                System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(A, R, G, B);
                ColorPanelShow.Fill = new SolidColorBrush(Color32);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
        }
        private void ColorInfoShow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (e.Text.StartsWith("#"))
                    return;
                int v = int.Parse(e.Text, System.Globalization.NumberStyles.HexNumber);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                e.Handled = true;
            }
        }
        #endregion

        private void ComboboxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (this.IsSolidType())
                {
                    this.SetControlVisible(ColorFillType.Solid);
                }
                else
                {
                    this.SetControlVisible(ColorFillType.Gradient);
                    GradientEditor1.SwitchToGradient();
                }
            }
            else
            {
                RoutedEventHandler handler = default(RoutedEventHandler);
                handler = (a, b) =>
                {
                    this.Loaded -= handler;
                    ComboboxType_SelectionChanged(sender, e);
                };

                this.Loaded += handler;
            }
        }

        private void SetControlVisible(ColorFillType type)
        {
            if (type == ColorFillType.Solid)
            {
                ComboboxType.SelectedIndex = 0;
                GradientPanel.Visibility = System.Windows.Visibility.Collapsed;
                GradientPreview.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                ComboboxType.SelectedIndex = 1;
                GradientPanel.Visibility = System.Windows.Visibility.Visible;
                GradientPreview.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void AngleText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var value = 0;
            if (!int.TryParse(e.Text, out value))
            {
                e.Handled = true;
            }
        }
    }



    #region CommandHelpers Class
    internal static class CommandHelpers
    {
        /// <summary>
        ///     Carries out the specified action for the CommandSource's command as a RoutedCommand if possible,
        ///     otherwise as a regular Command.
        /// </summary>
        /// <param name="parameter">The parameter to the ICommandSource.</param>
        /// <param name="commandSource">The ICommandSource being executed.</param>
        internal static void InvokeCommandSource(object parameter, object previewParameter, ICommandSource commandSource, CommandOperation operation)
        {
            ICommand command = commandSource.Command;

            if (command != null)
            {
                RoutedCommand routed = command as RoutedCommand;

                if (routed != null)
                {
                    IInputElement target = commandSource.CommandTarget;

                    if (target == null)
                    {
                        target = commandSource as IInputElement;
                    }

                    if (routed.CanExecute(parameter, target))
                    {
                        //Debug.Assert(operation == CommandOperation.Execute, "We do not support Previewing RoutedCommands.");

                        switch (operation)
                        {
                            case CommandOperation.Execute:
                                routed.Execute(parameter, target);
                                break;
                        }
                    }
                }
                else if (command.CanExecute(parameter))
                {
                    IPreviewCommand previewCommand;
                    switch (operation)
                    {
                        case CommandOperation.Preview:
                            previewCommand = command as IPreviewCommand;
                            if (previewCommand != null)
                            {
                                previewCommand.Preview(previewParameter);
                            }
                            break;
                        case CommandOperation.CancelPreview:
                            previewCommand = command as IPreviewCommand;
                            if (previewCommand != null)
                            {
                                previewCommand.CancelPreview();
                            }
                            break;
                        case CommandOperation.Execute:
                            command.Execute(parameter);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Queries CanExecute status for the CommandSource's command as a RoutedCommand if possible,
        ///     otherwise as a regular Command.
        /// </summary>
        /// <param name="parameter">The parameter to the ICommandSource.</param>
        /// <param name="commandSource">The ICommandSource being executed.</param>
        internal static bool CanExecuteCommandSource(object parameter, ICommandSource commandSource)
        {
            ICommand command = commandSource.Command;

            if (command != null)
            {
                RoutedCommand routed = command as RoutedCommand;

                if (routed != null)
                {
                    IInputElement target = commandSource.CommandTarget;

                    if (target == null)
                    {
                        target = commandSource as IInputElement;
                    }

                    return routed.CanExecute(parameter, target);
                }
                else
                {
                    return command.CanExecute(parameter);
                }
            }

            return true;
        }
    }
    internal enum CommandOperation
    {
        Preview,
        CancelPreview,
        Execute
    }
    public interface IPreviewCommand : ICommand
    {
        /// <summary>
        ///   Defines the method that should be executed when the command is previewed.
        /// </summary>
        /// <param name="parameter">A parameter that may be used in executing the preview command. This parameter may be ignored by some implementations.</param>
        void Preview(object parameter);

        /// <summary>
        ///   Defines the method that should be executed to cancel previewing of the command.
        /// </summary>
        void CancelPreview();
    }
    public interface IPreviewCommandSource : ICommandSource
    {
        /// <summary>
        ///     The parameter that will be passed to the command when previewing the command.
        ///     The property may be implemented as read-write if desired.
        /// </summary>
        object PreviewCommandParameter
        {
            get;
        }
    }
    #endregion CommandHelpers Class

    #region Converter
    public class BrushAndAngleMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var angle = 0d;
            if (values == null
                || values.Length != 2
                || !(values[0] is System.Windows.Media.Brush)
                || !(values[1] is string)
                || !double.TryParse(values[1] as string, out angle))
            {
                return null;
            }

            var aRotateTransform = new System.Windows.Media.RotateTransform();
            aRotateTransform.CenterX = 0.5;
            aRotateTransform.CenterY = 0.5;
            aRotateTransform.Angle = angle;

            var brush = values[0] as System.Windows.Media.Brush;
            var bclone = brush.Clone();
            bclone.RelativeTransform = aRotateTransform;
            return bclone;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
