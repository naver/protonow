using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Naver.Compass.Common
{
    public class EnterTextbox : TextBox
    {
        public EnterTextbox()
            : base()
        {
            GotKeyboardFocus += new KeyboardFocusChangedEventHandler(EnterTextbox_GotKeyboardFocus);
            PreviewKeyDown += new KeyEventHandler(EnterTextBox_PreviewKeyDown);

            Foreground = new SolidColorBrush(Color.FromRgb(66, 66, 66));
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            UpdateSource();
            e.Handled = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            UpdateSource();
        }

        public bool IsEnterKeyMoveFocus
        {
            get { return (bool)GetValue(IsEnterKeyMoveFocusProperty); }
            set { SetValue(IsEnterKeyMoveFocusProperty, value); }
        }

        public static readonly DependencyProperty IsEnterKeyMoveFocusProperty =
            DependencyProperty.Register("IsEnterKeyMoveFocus", typeof(bool), typeof(EnterTextbox),
            new FrameworkPropertyMetadata(true, null));

        public bool IsSkipEnterKey { get; set; }


        private void EnterTextbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.OriginalSource as EnterTextbox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        private void EnterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !IsSkipEnterKey)
            {               
                e.Handled = true;
                if (IsEnterKeyMoveFocus == false)
                {
                    UpdateSource();
                    this.SelectAll();
                    return;
                }

                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                // Change keyboard focus.
                if (elementWithFocus != null)
                {
                    elementWithFocus.MoveFocus(request);
                }
                else
                {
                    UpdateSource();
                }
            }

        }

        private void UpdateSource()
        {
            BindingExpression be = GetBindingExpression(TextBox.TextProperty);
            if (be != null)
            {
                be.UpdateSource();
            }
        }

    }

    public class NumEnterTextbox : TextBox
    {

        public NumEnterTextbox()
            : base()
        {
            GotKeyboardFocus += new KeyboardFocusChangedEventHandler(EnterTextbox_GotKeyboardFocus);
            PreviewKeyDown += new KeyEventHandler(EnterTextBox_PreviewKeyDown);
            KeyUp += NumEnterTextbox_KeyUp;

            Foreground = new SolidColorBrush(Color.FromRgb(66, 66, 66));
            //PreviewLostKeyboardFocus += NumEnterTextbox_PreviewLostKeyboardFocus;
        }



        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            UpdateSource();
        }

        public bool IsEnterKeyMoveFocus
        {
            get { return (bool)GetValue(IsEnterKeyMoveFocusProperty); }
            set { SetValue(IsEnterKeyMoveFocusProperty, value); }
        }

        public static readonly DependencyProperty IsEnterKeyMoveFocusProperty =
            DependencyProperty.Register("IsEnterKeyMoveFocus", typeof(bool), typeof(NumEnterTextbox),
            new FrameworkPropertyMetadata(true, null));


        private void EnterTextbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                var textBox = e.OriginalSource as NumEnterTextbox;
                if (textBox != null)
                {
                    textBox.SelectAll();
                }
            }
        }

        private void EnterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateSource();
                e.Handled = true;
                if (IsEnterKeyMoveFocus == false)
                {
                    this.SelectAll();
                    return;
                }

                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                // Change keyboard focus.
                if (elementWithFocus != null)
                {
                    elementWithFocus.MoveFocus(request);
                }
            }
            else if (e.Key == Key.Up)
            {
                try
                {
                    TextBox textBox = e.OriginalSource as TextBox;
                    if (textBox != null)
                    {
                        double value = Convert.ToDouble(textBox.Text);
                        if (this._leftShiftDown || this._rightShiftDown)
                        {
                            value += 10;
                            if (value > MaxNum)
                            {
                                value = MaxNum;
                            }
                        }
                        else
                        {
                            if (value < MaxNum)
                            {
                                value += 1;
                            }
                        }

                        textBox.Text = Convert.ToString(value);
                        textBox.CaretIndex = textBox.Text.Length;
                    }
                }
                catch (Exception ex)
                {
                    
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                try
                {
                    TextBox textBox = e.OriginalSource as TextBox;
                    if (textBox != null)
                    {
                        double value = Convert.ToDouble(textBox.Text);
                        if (this._leftShiftDown || this._rightShiftDown)
                        {
                            value -= 10;
                            if (value < MiniNum)
                            {
                                value = MiniNum;
                            }
                        }
                        else
                        {
                            if (value > MiniNum)
                            {
                                value -= 1;
                            }
                        }
                        textBox.Text = Convert.ToString(value);
                        textBox.CaretIndex = textBox.Text.Length;
                    }
                }
                catch (Exception ex)
                {
                    
                }
                e.Handled = true;
            }
            else if (e.Key == Key.LeftShift)
            {
                _leftShiftDown = true;

            }
            else if (e.Key == Key.RightShift)
            {
                _rightShiftDown = true;
            }
        }

        private bool _leftShiftDown;
        private bool _rightShiftDown;
        void NumEnterTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                _leftShiftDown = false;
            }
            else if (e.Key == Key.RightShift)
            {
                _rightShiftDown = false;
            }
        }

        private void UpdateSource()
        {
            BindingExpression be = GetBindingExpression(TextBox.TextProperty);
            if (be != null)
            {
                be.UpdateSource();
            }
        }

        public double MaxNum
        {
            get { return (double)GetValue(MaxNumProperty); }
            set { SetValue(MaxNumProperty, value); }
        }
        public static readonly DependencyProperty MaxNumProperty =
            DependencyProperty.Register(
            "MaxNum",
            typeof(double),
            typeof(NumEnterTextbox),
            new PropertyMetadata(Double.MaxValue));

        public double MiniNum
        {
            get { return (double)GetValue(MiniNumProperty); }
            set { SetValue(MiniNumProperty, value); }
        }
        public static readonly DependencyProperty MiniNumProperty =
            DependencyProperty.Register(
            "MiniNum",
            typeof(double),
            typeof(NumEnterTextbox),
            new PropertyMetadata(Double.MinValue));
    }
}
