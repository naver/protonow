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
    public class CustomEventArgs<T> : EventArgs
    {
        public T Item1 { get; set; }
        public CustomEventArgs(T item1)
        {
            Item1 = item1;
        }
    }

    /// <summary>
    /// Interaction logic for EditableInlineTextblock.xaml
    /// </summary>
    public partial class EditableInlineTextblock : UserControl
    {
        public event EventHandler<CustomEventArgs<string>> TextChanged;
        #region Constructor

        public EditableInlineTextblock()
        {
            InitializeComponent();
            base.Focusable = true;
            base.FocusVisualStyle = null;
            ClickTimer = new Timer(400);
            ClickTimer.AutoReset = false;
            ClickTimer.Elapsed += ClickTimer_Elapsed;
        }



        #endregion Constructor

        #region Member Variables

        // We keep the old text when we go into editmode
        // in case the user aborts with the escape key
        private string oldText;

        #endregion Member Variables

        #region Properties

        public static readonly DependencyProperty InContentProperty =
            DependencyProperty.Register("InContent", typeof(InlineContent), typeof(EditableInlineTextblock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnInContentChanged)));

        public InlineContent InContent
        {
            get { return (InlineContent)GetValue(InContentProperty); }
            set { SetValue(InContentProperty, value); }
        }

        private static void OnInContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditableInlineTextblock target = (EditableInlineTextblock)d;
            InlineContent oldInContent = (InlineContent)e.OldValue;
            InlineContent newInContent = target.InContent;
            target.OnInContentChanged(oldInContent, newInContent);
        }

        protected virtual void OnInContentChanged(InlineContent oldInContent, InlineContent newInContent)
        {
            if (newInContent == null || newInContent.Count == 0)
            {
                Text = string.Empty;
            }
            else
            {
                Text = string.Join(string.Empty, newInContent.Select(inline => inline.Text));
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(EditableInlineTextblock),
            new PropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditableInlineTextblock target = (EditableInlineTextblock)d;
            string oldText = (string)e.OldValue;
            string newText = target.Text;
            target.OnTextContentChanged(oldText, newText);
        }

        private void OnTextContentChanged(string oldText, string newText)
        {
            if (string.IsNullOrEmpty(newText) && !string.IsNullOrEmpty(oldText))
            {
                Text = oldText;
                return;
            }

            var matchText = string.Empty;
            if (InContent == null || InContent.Count == 0)
            {
                matchText = string.Empty;
            }
            else
            {
                matchText = string.Join(string.Empty, InContent.Select(inline => inline.Text));
            }

            if (matchText != newText)
            {
                if (string.IsNullOrEmpty(newText))
                {
                    InContent.Clear();
                }
                else
                {
                    InContent.Clear();
                    var inline = new InlineContent();
                    InContent.Add(new InlineInfo { Text = newText });
                }

                if (TextChanged != null)
                {
                    TextChanged(this, new CustomEventArgs<string>(newText));
                }
            }
        }

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(EditableInlineTextblock),
            new PropertyMetadata(true));

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
                    if (value) oldText = Text;
                    SetValue(IsInEditModeProperty, value);
                }
            }
        }
        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(EditableInlineTextblock),
            new PropertyMetadata(false));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(EditableInlineTextblock),
             new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnSelectChanged)));

        static void OnSelectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as EditableInlineTextblock).OnSelectChanged();
        }
        void OnSelectChanged()
        {
            nClickCount = 0;
        }

        #endregion Properties

        #region Event Handlers

        // Invoked when we enter edit mode.
        void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Give the TextBox input focus
            txt.Focus();

            txt.SelectAll();
        }

        // Invoked when we exit edit mode.
        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsInEditMode = false;
            ClickTimer.Stop();
            nClickCount = 0;
        }

        // Invoked when the user edits the annotation.
        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox txtBox = (TextBox)sender;
                Text = txtBox.Text;

                this.IsInEditMode = false;

                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.IsInEditMode = false;
                Text = oldText;

                e.Handled = true;
            }
        }

        private Point _startMousePosition = default(Point);

        //clike twice, set IsInEditMode = true;
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                ClickTimer.Stop();
            }
            else
            {
                this._startMousePosition = e.GetPosition(this);
                ClickTimer.Start();
            }
        }

        void ClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ClickTimer.Stop();

            if (++nClickCount > 1)
            {
                this.mainControl.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            var endMousePosition = Mouse.GetPosition(this);
                            var distance = Math.Sqrt(Math.Pow(this._startMousePosition.X - endMousePosition.X, 2)
                                        + Math.Pow(this._startMousePosition.Y - endMousePosition.Y, 2));
                            if (distance < 2d)
                            {
                                this.nClickCount = 0;
                                this.mainControl.IsInEditMode = true;
                            }
                        }
                    )
                );
            }

        }

        #endregion Event Handlers

        #region privete variable
        int nClickCount = 0;
        private Timer ClickTimer;
        #endregion
    }
}
