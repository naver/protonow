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
using System.Diagnostics;

namespace Naver.Compass.Common
{
    /// <summary>
    /// Interaction logic for CompassColorPicker.xaml
    /// </summary>
    public partial class CompassColorPicker2 : System.Windows.Controls.UserControl, IPreviewCommandSource
    {
        public CompassColorPicker2()
        {
            InitializeComponent();
            _bzIsDropDownOpen = false;
            _currentColor = ColorSelected;
            ColorPanelShow.Fill = new SolidColorBrush(_currentColor);
        }

        #region Private Member and Function
        private bool _bzIsDropDownOpen;
        private System.Windows.Media.Color _currentColor;
        #endregion

        #region Dependecy Propery
        #region Taraget Color
        public System.Windows.Media.Color ColorSelected
        {
            get { return (System.Windows.Media.Color)GetValue(ColorSelectedProperty); }
            set 
            { 
                SetValue(ColorSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty ColorSelectedProperty =
          DependencyProperty.Register("ColorSelected", typeof(System.Windows.Media.Color),
                                      typeof(CompassColorPicker2),
                                      new FrameworkPropertyMetadata(System.Windows.Media.Color.FromArgb(255, 133, 133, 133), new PropertyChangedCallback(OnColorChanged)));

        static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CompassColorPicker2).OnColorChanged();
        }
        private void OnColorChanged()
        {
            _currentColor = (System.Windows.Media.Color)ColorSelected;


            //Initial Color panel
            System.Windows.Media.Color Color16 = _currentColor;
            Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(A, Color16.R, Color16.G, Color16.B);
            ColorPanelShow.Fill = new SolidColorBrush(Color32);

            //Initial color-32 information
            string sARGB = ColorPanelShow.Fill.ToString();
            string sRGB = @"#" + sARGB.Substring(3, sARGB.Length - 3);
            ColorInfoShow.Text = sRGB;

            //Initialize the Alpha value
            OpacityText.Text = Convert.ToInt32(A * 100 / 255).ToString();
            OpacitySlider.UpdateLayout();
            TargetColorRec.Fill = new SolidColorBrush(Color32);
            TargetColorRec.UpdateLayout();
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
                            typeof(CompassColorPicker2),
                            new FrameworkPropertyMetadata(null));
        #endregion
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
                            typeof(CompassColorPicker2),
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
                            typeof(CompassColorPicker2),
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
                            typeof(CompassColorPicker2),
                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CompassColorPicker2 textBox = (CompassColorPicker2)d;
            ICommand oldCommand = (ICommand)e.OldValue;
            ICommand newCommand = (ICommand)e.NewValue;

            if (oldCommand != null)
            {
                textBox.UnhookCommand(oldCommand);
            }
            if (newCommand != null)
            {
                textBox.HookCommand(newCommand);
            }

            //RibbonHelper.OnCommandChanged(d, e);
        }       
 

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++
        private EventHandler _canExecuteChangedHandler;
        private void HookCommand(ICommand command)
        {
            _canExecuteChangedHandler = new EventHandler(OnCanExecuteChanged);
            command.CanExecuteChanged += _canExecuteChangedHandler;
            UpdateCanExecute();
        }

        private void UnhookCommand(ICommand command)
        {
            if (_canExecuteChangedHandler != null)
            {
                command.CanExecuteChanged -= _canExecuteChangedHandler;
                _canExecuteChangedHandler = null;
            }
            UpdateCanExecute();
        }
        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            UpdateCanExecute();
        }

        private void UpdateCanExecute()
        {
            if (Command != null)
            {
                CanExecute = CommandHelpers.CanExecuteCommandSource(CommandParameter, this);
            }
            else
            {
                CanExecute = true;
            }
        }
        bool bIsEnable=false;
        private bool CanExecute
        {
            get
            {
                return bIsEnable;
            }
            set
            {
                bIsEnable = value;
                this.IsEnabled = value;
                this.UpdateLayout();
            }
        }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++

        #endregion
        #endregion

        #region Toolbar Item Event handler
        private void SetColorButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            PART_Popup.IsOpen = !PART_Popup.IsOpen;
            e.Handled = true;
        }
        #endregion

        #region Poppup Item Event handler

        #region Command Invoker
        private void ColorExample_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (OpacitySlider == null)
                return;
            Border targetBox = e.OriginalSource as Border;
            if (targetBox == null)
                return;

            System.Windows.Media.Color Color16 = (targetBox.Background as SolidColorBrush).Color;
            Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(A, Color16.R, Color16.G, Color16.B);
            ColorSelected = Color32;

            CommandHelpers.InvokeCommandSource(ColorSelected, null, this, CommandOperation.Execute);
            PART_Popup.IsOpen = false;
        }
        private void TransparentButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border targetBox = e.OriginalSource as Border;
            if (targetBox == null)
                return;

            System.Windows.Media.Color Color16 = (targetBox.Background as SolidColorBrush).Color;
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(0, Color16.R, Color16.G, Color16.B);
            ColorSelected = Color32;

            CommandHelpers.InvokeCommandSource(ColorSelected, null, this, CommandOperation.Execute);
            PART_Popup.IsOpen = false;
        }
        private void ColorInfoShow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                System.Windows.Media.Color Color16 = (ColorPanelShow.Fill as SolidColorBrush).Color;
                System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(Color16.A, Color16.R, Color16.G, Color16.B);
                ColorSelected = Color32;

                CommandHelpers.InvokeCommandSource(ColorSelected, null, this, CommandOperation.Execute);
                PART_Popup.IsOpen = false;
            }
        }
        private void ColorPanelShow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Media.Color Color16 = (ColorPanelShow.Fill as SolidColorBrush).Color;
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(Color16.A, Color16.R, Color16.G, Color16.B);
            ColorSelected = Color32;

            CommandHelpers.InvokeCommandSource(ColorSelected, null, this, CommandOperation.Execute);
            PART_Popup.IsOpen = false;
        }
        #endregion


        private void StackPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (OpacitySlider == null)
                return;
            Border targetBox = e.OriginalSource as Border;

            if (targetBox == null)
            {
                Byte CurAlpha = Convert.ToByte(OpacitySlider.Value * 255 / 100);
                _currentColor.A = CurAlpha;
                ColorPanelShow.Fill = new SolidColorBrush(_currentColor);
                //Initial color-32 information
                string szARGB = ColorPanelShow.Fill.ToString();
                string szRGB = @"#" + szARGB.Substring(3, szARGB.Length - 3);
                ColorInfoShow.Text = szRGB;
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
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OpacitySlider == null)
                return;

            Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);
            System.Windows.Media.Color Color16 = (ColorPanelShow.Fill as SolidColorBrush).Color;
            System.Windows.Media.Color Color32 = System.Windows.Media.Color.FromArgb(A, Color16.R, Color16.G, Color16.B);
            ColorPanelShow.Fill = new SolidColorBrush(Color32);
        }
        private void TransparentButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (OpacitySlider == null)
                return;

            Byte A = Convert.ToByte(OpacitySlider.Value * 255 / 100);
            _currentColor.A = A;
            ColorPanelShow.Fill = new SolidColorBrush(_currentColor);
            //Initial color-32 information
            string sARGB = ColorPanelShow.Fill.ToString();
            string sRGB = @"#" + sARGB.Substring(3, sARGB.Length - 3);
            ColorInfoShow.Text = sRGB;
            return;
        }
        private void ColorDlgButton_Click(object sender, RoutedEventArgs e)
        {
            PART_Popup.StaysOpen = true;
            System.Windows.Forms.ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            }
            PART_Popup.StaysOpen = false;
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
        #endregion
    }



    #region CommandHelpers Class
    //internal static class CommandHelpers
    //{
    //    /// <summary>
    //    ///     Carries out the specified action for the CommandSource's command as a RoutedCommand if possible,
    //    ///     otherwise as a regular Command.
    //    /// </summary>
    //    /// <param name="parameter">The parameter to the ICommandSource.</param>
    //    /// <param name="commandSource">The ICommandSource being executed.</param>
    //    internal static void InvokeCommandSource(object parameter, object previewParameter, ICommandSource commandSource, CommandOperation operation)
    //    {
    //        ICommand command = commandSource.Command;

    //        if (command != null)
    //        {
    //            RoutedCommand routed = command as RoutedCommand;

    //            if (routed != null)
    //            {
    //                IInputElement target = commandSource.CommandTarget;

    //                if (target == null)
    //                {
    //                    target = commandSource as IInputElement;
    //                }

    //                if (routed.CanExecute(parameter, target))
    //                {
    //                    //Debug.Assert(operation == CommandOperation.Execute, "We do not support Previewing RoutedCommands.");

    //                    switch (operation)
    //                    {
    //                        case CommandOperation.Execute:
    //                            routed.Execute(parameter, target);
    //                            break;
    //                    }
    //                }
    //            }
    //            else if (command.CanExecute(parameter))
    //            {
    //                IPreviewCommand previewCommand;
    //                switch (operation)
    //                {
    //                    case CommandOperation.Preview:
    //                        previewCommand = command as IPreviewCommand;
    //                        if (previewCommand != null)
    //                        {
    //                            previewCommand.Preview(previewParameter);
    //                        }
    //                        break;
    //                    case CommandOperation.CancelPreview:
    //                        previewCommand = command as IPreviewCommand;
    //                        if (previewCommand != null)
    //                        {
    //                            previewCommand.CancelPreview();
    //                        }
    //                        break;
    //                    case CommandOperation.Execute:
    //                        command.Execute(parameter);
    //                        break;
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    ///     Queries CanExecute status for the CommandSource's command as a RoutedCommand if possible,
    //    ///     otherwise as a regular Command.
    //    /// </summary>
    //    /// <param name="parameter">The parameter to the ICommandSource.</param>
    //    /// <param name="commandSource">The ICommandSource being executed.</param>
    //    internal static bool CanExecuteCommandSource(object parameter, ICommandSource commandSource)
    //    {
    //        ICommand command = commandSource.Command;

    //        if (command != null)
    //        {
    //            RoutedCommand routed = command as RoutedCommand;

    //            if (routed != null)
    //            {
    //                IInputElement target = commandSource.CommandTarget;

    //                if (target == null)
    //                {
    //                    target = commandSource as IInputElement;
    //                }

    //                return routed.CanExecute(parameter, target);
    //            }
    //            else
    //            {
    //                return command.CanExecute(parameter);
    //            }
    //        }

    //        return true;
    //    }
    //}
    //internal enum CommandOperation
    //{
    //    Preview,
    //    CancelPreview,
    //    Execute
    //}
    //public interface IPreviewCommand : ICommand
    //{
    //    /// <summary>
    //    ///   Defines the method that should be executed when the command is previewed.
    //    /// </summary>
    //    /// <param name="parameter">A parameter that may be used in executing the preview command. This parameter may be ignored by some implementations.</param>
    //    void Preview(object parameter);

    //    /// <summary>
    //    ///   Defines the method that should be executed to cancel previewing of the command.
    //    /// </summary>
    //    void CancelPreview();
    //}
    //public interface IPreviewCommandSource : ICommandSource
    //{
    //    /// <summary>
    //    ///     The parameter that will be passed to the command when previewing the command.
    //    ///     The property may be implemented as read-write if desired.
    //    /// </summary>
    //    object PreviewCommandParameter
    //    {
    //        get;
    //    }
    //}
    #endregion CommandHelpers Class
}
