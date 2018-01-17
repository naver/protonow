using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Naver.Compass.WidgetLibrary
{
    /// <summary>
    /// Interaction logic for SimpleEditText.xaml
    /// </summary>
    public partial class SimpleEditText : BaseEditControl
    {
        public SimpleEditText()
        {
            InitializeComponent();
        }

        #region Dependency Property

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public static readonly DependencyProperty TextWrappingProperty =
           DependencyProperty.Register(
           "TextWrapping",
           typeof(TextWrapping),
           typeof(SimpleEditText),
           new PropertyMetadata(TextWrapping.Wrap));

        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        public static readonly DependencyProperty AcceptsReturnProperty =
           DependencyProperty.Register(
           "AcceptsReturn",
           typeof(bool),
           typeof(SimpleEditText),
           new PropertyMetadata(false));

        public bool EnableLabel
        {
            get { return (bool)GetValue(EnableLabelProperty); }
            set
            {
                SetValue(EnableLabelProperty, value);
            }
        }

        public static readonly DependencyProperty EnableLabelProperty =
           DependencyProperty.Register(
           "EnableLabel",
           typeof(bool),
           typeof(SimpleEditText),
           new PropertyMetadata(true));

        //public bool IsInEditMode
        //{
        //    get
        //    {
        //        return (bool)GetValue(IsInEditModeProperty);
        //    }
        //    set
        //    {
        //        SetValue(IsInEditModeProperty, value);
        //    }
        //}
        //public static readonly DependencyProperty IsInEditModeProperty =
        //    DependencyProperty.Register(
        //    "IsInEditMode",
        //    typeof(bool),
        //    typeof(SimpleEditText),
        //    new PropertyMetadata(false));


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
           typeof(SimpleEditText),
           new PropertyMetadata(0));

        public double LabelMaxLen
        {
            get
            {
                return (double)GetValue(LabelMaxLenProperty);
            }
            set
            {
                SetValue(LabelMaxLenProperty, value);
            }
        }

        public static readonly DependencyProperty LabelMaxLenProperty =
           DependencyProperty.Register(
           "LabelMaxLen",
           typeof(double),
           typeof(SimpleEditText),
           new PropertyMetadata((double)0));

        #endregion

        #region Event Handlers

        // Invoked when we exit edit mode.
        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsInEditMode = false;
        }

        override protected void OnEdtModeChange(bool newValue)
        {
            if (EditControl != null && newValue == true)
            {
                EditControl.Focus();

                if (EditControl.Text.Length > 0)
                {
                    EditControl.SelectAll();
                }
            }
        }

        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !AcceptsReturn)
            {
                TextBox txtBox = (TextBox)sender;

                DependencyObject ancestor = txtBox.Parent;
                while (ancestor != null)
                {
                    var element = ancestor as UIElement;
                    if (element != null && element.Focusable)
                    {
                        element.Focus();
                        break;
                    }

                    ancestor = VisualTreeHelper.GetParent(ancestor);
                }

                
                e.Handled = true;
            }
        }
        #endregion Event Handlers
    }


}
