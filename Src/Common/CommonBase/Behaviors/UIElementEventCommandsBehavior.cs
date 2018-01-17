using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace Naver.Compass.Common.CommonBase
{
    public class UIElementEventCommandsBehavior : Behavior<UIElement>
    {
        #region Override Function
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AddHandler(Keyboard.PreviewKeyDownEvent, new RoutedEventHandler(OnPreviewKeyDown), true);
            this.AssociatedObject.AddHandler(Keyboard.PreviewKeyUpEvent, new RoutedEventHandler(OnPreviewKeyUp), true);
            this.AssociatedObject.MouseLeftButtonDown += OnMouseClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RemoveHandler(Keyboard.PreviewKeyDownEvent, new RoutedEventHandler(OnPreviewKeyDown));
            this.AssociatedObject.RemoveHandler(Keyboard.PreviewKeyUpEvent, new RoutedEventHandler(OnPreviewKeyUp));
        }
        #endregion

        #region Binding Command

        public ICommand PreviewKeyDownCommand
        {
            get { return (ICommand)GetValue(PreviewKeyDownCommandProperty); }
            set { SetValue(PreviewKeyDownCommandProperty, value); }
        }

        public static readonly DependencyProperty PreviewKeyDownCommandProperty =
            DependencyProperty.Register(
                "PreviewKeyDownCommand",
                typeof(ICommand),
                typeof(UIElementEventCommandsBehavior),
                new PropertyMetadata(null));

        public ICommand PreviewKeyUpCommand
        {
            get { return (ICommand)GetValue(PreviewKeyUpCommandProperty); }
            set { SetValue(PreviewKeyUpCommandProperty, value); }
        }

        public static readonly DependencyProperty PreviewKeyUpCommandProperty =
            DependencyProperty.Register(
                "PreviewKeyUpCommand",
                typeof(ICommand),
                typeof(UIElementEventCommandsBehavior),
                new PropertyMetadata(null));

        //Using a DependencyProperty to binding double-click event
        public ICommand MouseDoubleClickCommand
        {
            get { return (ICommand)GetValue(MouseDoubleClickCommandProperty); }
            set { SetValue(MouseDoubleClickCommandProperty, value); }
        }
        public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.Register(
                "MouseDoubleClickCommand",
                typeof(ICommand),
                typeof(UIElementEventCommandsBehavior),
                new PropertyMetadata(null));
    
        //Using a DependencyProperty to binding click event
        public ICommand MouseClickCommand
        {
            get { return (ICommand)GetValue(MouseClickCommandProperty); }
            set { SetValue(MouseClickCommandProperty, value); }
        }
        public static readonly DependencyProperty MouseClickCommandProperty =
            DependencyProperty.Register(
                "MouseClickCommand",
                typeof(ICommand),
                typeof(UIElementEventCommandsBehavior),
                new PropertyMetadata(null));
        #endregion

        #region EventHandler

        void OnPreviewKeyDown(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType().Equals(typeof(TextBox))
                || e.OriginalSource is RichTextBox)
            {
                return;
            }

            if (PreviewKeyDownCommand != null)
            {
                PreviewKeyDownCommand.Execute(e);
            }
        }

        void OnPreviewKeyUp(object sender, RoutedEventArgs e)
        {
            if (PreviewKeyUpCommand != null)
            {
                PreviewKeyUpCommand.Execute(e);
            }
        }

        void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2 && MouseDoubleClickCommand != null)
            {
                MouseDoubleClickCommand.Execute(e);
            }
        }

        #endregion

    }
}
