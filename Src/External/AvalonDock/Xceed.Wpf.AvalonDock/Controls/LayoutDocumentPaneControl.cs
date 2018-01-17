/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Input;

namespace Xceed.Wpf.AvalonDock.Controls
{
    public class LayoutDocumentPaneControl : TabControl, ILayoutControl//, ILogicalChildrenContainer
    {
        static LayoutDocumentPaneControl()
        {
            FocusableProperty.OverrideMetadata(typeof(LayoutDocumentPaneControl), new FrameworkPropertyMetadata(false));
        }


        internal LayoutDocumentPaneControl(LayoutDocumentPane model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            _model = model;
            SetBinding(ItemsSourceProperty, new Binding("Model.Children") { Source = this });
            SetBinding(FlowDirectionProperty, new Binding("Model.Root.Manager.FlowDirection") { Source = this });

            this.LayoutUpdated += new EventHandler(OnLayoutUpdated);
        }

        void OnLayoutUpdated(object sender, EventArgs e)
        {
            var modelWithAtcualSize = _model as ILayoutPositionableElementWithActualSize;
            modelWithAtcualSize.ActualWidth = ActualWidth;
            modelWithAtcualSize.ActualHeight = ActualHeight;
        }

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            System.Diagnostics.Trace.WriteLine( string.Format( "OnGotKeyboardFocus({0}, {1})", e.Source, e.NewFocus ) );


            //if (_model.SelectedContent != null)
            //    _model.SelectedContent.IsActive = true;

        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (_model.SelectedContent != null)
                _model.SelectedContent.IsActive = true;
        }

        List<object> _logicalChildren = new List<object>();

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                return _logicalChildren.GetEnumerator();
            }
        }

        LayoutDocumentPane _model;

        public ILayoutElement Model
        {
            get { return _model; }
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!e.Handled && _model.SelectedContent != null)
                _model.SelectedContent.IsActive = true;
        }

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (!e.Handled && _model.SelectedContent != null)
                _model.SelectedContent.IsActive = true;

        }


        #region HideCommand

        /// <summary>
        /// HideCommand Dependency Property
        /// </summary>
        public static readonly DependencyProperty HideCommandProperty =
            DependencyProperty.Register("HideCommand", typeof(ICommand), typeof(LayoutDocumentPaneControl),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnHideCommandChanged),
                    new CoerceValueCallback(CoerceHideCommandValue)));

        /// <summary>
        /// Gets or sets the HideCommand property.  This dependency property 
        /// indicates the command to execute when an anchorable is hidden.
        /// </summary>
        public ICommand HideCommand
        {
            get { return (ICommand)GetValue(HideCommandProperty); }
            set { SetValue(HideCommandProperty, value); }
        }

        /// <summary>
        /// Handles changes to the HideCommand property.
        /// </summary>
        private static void OnHideCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutDocumentPaneControl)d).OnHideCommandChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the HideCommand property.
        /// </summary>
        protected virtual void OnHideCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces the HideCommand value.
        /// </summary>
        private static object CoerceHideCommandValue(DependencyObject d, object value)
        {
            return value;
        }


        private bool CanExecuteHideCommand(object parameter)
        {
            return true;
        }

        private void ExecuteHideCommand(object parameter)
        {
           // MessageBox.Show("sss");
        }

        #endregion
    }
}
