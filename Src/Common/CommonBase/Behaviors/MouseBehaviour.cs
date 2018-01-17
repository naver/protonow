using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Naver.Compass.Common.CommonBase
{
    public class MouseBehaviour
    {
        #region MouseUp
        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached(
            "MouseUpCommand",
            typeof(ICommand),
            typeof(MouseBehaviour),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseUpCommandChanged)));

        private static void MouseUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.MouseUp += new MouseButtonEventHandler(element_MouseUp);
        }

        static void element_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;

            ICommand command = GetMouseUpCommand(element);

            command.Execute(e);
        }

        public static void SetMouseUpCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseUpCommandProperty, value);
        }

        public static ICommand GetMouseUpCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MouseUpCommandProperty);
        }
        #endregion

        #region MouseMoveCommand

        /// <summary>
        /// MouseMove Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.RegisterAttached("MouseMoveCommand", typeof(ICommand), typeof(MouseBehaviour),
                new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(OnMouseMoveCommandChanged)));

        /// <summary>
        /// Gets the MouseMove property. This dependency property 
        /// indicates ....
        /// </summary>
        public static ICommand GetMouseMoveCommand(UIElement d)
        {
            return (ICommand)d.GetValue(MouseMoveCommandProperty);
        }

        /// <summary>
        /// Sets the MouseMove property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetMouseMoveCommand(UIElement d, ICommand value)
        {
            d.SetValue(MouseMoveCommandProperty, value);
        }

        /// <summary>
        /// Handles changes to the MouseMove property.
        /// </summary>
        private static void OnMouseMoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            element.MouseMove += element_MouseMove;
        }

        static void element_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ICommand command = GetMouseMoveCommand(element);
            command.Execute(e);
        }

        #endregion

        #region MouseDown
        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached(
            "MouseDownCommand",
            typeof(ICommand),
            typeof(MouseBehaviour),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseDownCommandChanged)));

        private static void MouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.MouseDown += new MouseButtonEventHandler(element_MouseDown);
        }

        static void element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;

            ICommand command = GetMouseDownCommand(element);

            command.Execute(e);
        }

        public static void SetMouseDownCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseDownCommandProperty, value);
        }

        public static ICommand GetMouseDownCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MouseDownCommandProperty);
        }
        #endregion

        #region MouseDoubleClick
        public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.RegisterAttached(
            "MouseDoubleClickCommand",
            typeof(ICommand),
            typeof(MouseBehaviour),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseDoubleClickCommandChanged)));

        private static void MouseDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control element = (Control)d;

            element.MouseDoubleClick += new MouseButtonEventHandler(element_MouseDoubleClick);
        }

        static void element_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;

            ICommand command = GetMouseDoubleClickCommand(element);

            command.Execute(e);
        }

        public static void SetMouseDoubleClickCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseDoubleClickCommandProperty, value);
        }

        public static ICommand GetMouseDoubleClickCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MouseDoubleClickCommandProperty);
        }
        #endregion

        #region KeyDown
        public static readonly DependencyProperty KeyDownCommandProperty =
            DependencyProperty.RegisterAttached(
            "KeyDownCommand",
            typeof(ICommand),
            typeof(MouseBehaviour),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(KeyDownCommandChanged)));

        private static void KeyDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.KeyDown += new KeyEventHandler(element_KeyDown);
        }

        static void element_KeyDown(object sender, KeyEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;

            ICommand command = GetKeyDownCommand(element);

            command.Execute(e);
        }

        public static void SetKeyDownCommand(UIElement element, ICommand value)
        {
            element.SetValue(KeyDownCommandProperty, value);
        }

        public static ICommand GetKeyDownCommand(UIElement element)
        {
            return (ICommand)element.GetValue(KeyDownCommandProperty);
        }
        #endregion
    }
}
