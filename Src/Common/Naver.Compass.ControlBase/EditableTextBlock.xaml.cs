using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Naver.Compass.Common
{
    /// <summary>
    /// Interaction logic for EditableTextBlock.xaml
    /// </summary>
    public partial class EditableTextBlock : UserControl
    {
        public EditableTextBlock()
        {
            InitializeComponent();
            Focusable = true;
            FocusVisualStyle = null;
            _timer = new Timer(400);
            _timer.AutoReset = false;
            _timer.Elapsed += _timer_Elapsed;
        }
        #region Properties

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new PropertyMetadata(""));

        public static readonly DependencyProperty IsEditableProperty =
       DependencyProperty.Register("IsEditable", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(true));

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }


        public static readonly DependencyProperty IsInEditModeProperty =
        DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false));

        public bool IsInEditMode
        {
            get
            {
                if (IsEditable)
                    return (bool)GetValue(IsInEditModeProperty);
                else
                    return false;
            }
            set
            {
                if (IsEditable)
                {
                    if (value)
                        _oldText = Text;
                    SetValue(IsInEditModeProperty, value);
                }
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(EditableTextBlock), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnSelectChanged)));

        static void OnSelectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as EditableTextBlock).OnSelectChanged();
        }
        void OnSelectChanged()
        {
            _nClickCount = 0;
        }

        #endregion Properties

        #region Event Handlers

        // Invoked when we enter edit mode.
        void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            textbox.Focus();
            textbox.SelectAll();
        }

        // Invoked when we exit edit mode.
        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsInEditMode = false;
            _timer.Stop();
            _nClickCount = 0;
        }

        // Invoked when the user edits the annotation.
        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox txtBox = (TextBox)sender;
                Text = txtBox.Text;

                IsInEditMode = false;
            }
            else if (e.Key == Key.Escape)
            {
                this.IsInEditMode = false;
                Text = _oldText; 
            }
            e.Handled = true;
        }


        //clike twice, set IsInEditMode = true;
        private void TextBlock_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                _timer.Stop();
            }
            else
            {
                _startMousePosition = e.GetPosition(this);
                _timer.Start();
            }
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            if (++_nClickCount > 1)
            {
                editorablTextBlock.Dispatcher.Invoke(new Action(delegate
                        {
                            var endMousePosition = Mouse.GetPosition(this);
                            var distance = Math.Sqrt(Math.Pow(this._startMousePosition.X - endMousePosition.X, 2)
                                        + Math.Pow(this._startMousePosition.Y - endMousePosition.Y, 2));
                            if (distance < 2d)
                            {
                                _nClickCount = 0;
                                editorablTextBlock.IsInEditMode = true;
                            }
                        }));
            }
        }

        #endregion Event Handlers

        int _nClickCount = 0;
        private Timer _timer;
        private Point _startMousePosition = default(Point);
        
        // We keep the old text when we go into editmode
        // in case the user aborts with the escape key
        private string _oldText;

    }
}
