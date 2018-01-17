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
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Interactivity;
using Naver.Compass.Common.Shadow;

namespace Naver.Compass.Common
{
    public abstract class CustomWindow : Window
    {
        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand,
                new ExecutedRoutedEventHandler((s, args) => Microsoft.Windows.Shell.SystemCommands.CloseWindow((Window)this))));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand,
                new ExecutedRoutedEventHandler((s, args) => Microsoft.Windows.Shell.SystemCommands.MaximizeWindow((Window)this))));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand,
                new ExecutedRoutedEventHandler((s, args) => Microsoft.Windows.Shell.SystemCommands.MinimizeWindow((Window)this))));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand,
                new ExecutedRoutedEventHandler((s, args) => Microsoft.Windows.Shell.SystemCommands.RestoreWindow((Window)this))));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.ShowSystemMenuCommand,ExecuteSystemMenuCommand));
            //Debug.Assert(this.Owner != null);
            base.OnInitialized(e);
        }

        private void ExecuteSystemMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = sender as Window;
          
            if (window != null)
            {
                Point pointRe = Mouse.GetPosition(window);
                Point PointSc = window.PointToScreen(pointRe);

                Microsoft.Windows.Shell.SystemCommands.ShowSystemMenu(window, PointSc);
            }
        }

        #region MetroChromeBehavior 

        /// <summary>
        /// 
        /// </summary>
        public MetroChromeBehavior MetroChromeBehavior
        {
            get { return (MetroChromeBehavior)this.GetValue(MetroChromeBehaviorProperty); }
            set { this.SetValue(MetroChromeBehaviorProperty, value); }
        }
        public static readonly DependencyProperty MetroChromeBehaviorProperty =
            DependencyProperty.Register("MetroChromeBehavior", typeof(MetroChromeBehavior), typeof(CustomWindow), new UIPropertyMetadata(null, MetroChromeBehaviorChangedCallback));

        private static void MetroChromeBehaviorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (CustomWindow)d;
            var oldBehavior = (MetroChromeBehavior)e.OldValue;
            var newBehavior = (MetroChromeBehavior)e.NewValue;

            if (Equals(oldBehavior, newBehavior)) return;

            var behaviors = Interaction.GetBehaviors(instance);

            if (oldBehavior != null) behaviors.Remove(oldBehavior);
            if (newBehavior != null) behaviors.Add((Behavior)newBehavior.Clone());
        }

        #endregion

    }

    public class BaseWindow : CustomWindow
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
            base.OnKeyDown(e);
        }
    }
    public class MainBase : CustomWindow
    {
        public MainBase()
        {

            
        }
    }
}
