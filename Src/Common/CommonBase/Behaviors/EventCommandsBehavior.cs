using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Input;

namespace Naver.Compass.Common.CommonBase
{
    public class EventCommandsBehavior : Behavior<ContentControl>
    {
        #region Override Function
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDoubleClick += OnMouseDoubleClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.MouseDoubleClick -= OnMouseDoubleClick; 
        }
        #endregion

        #region Binding Command
       //Using a DependencyProperty to binding double-click event
        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        } 
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register(
                "DoubleClickCommand",
                typeof(ICommand),
                typeof(EventCommandsBehavior),
                new PropertyMetadata(null));
        #endregion

        #region EventHandler
        void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClickCommand != null)
            {
                DoubleClickCommand.Execute(e);
            }
        }
        #endregion

    }
}
