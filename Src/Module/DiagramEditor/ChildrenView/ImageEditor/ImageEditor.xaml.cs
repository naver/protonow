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

namespace Naver.Compass.Module
{
    /// <summary>
    /// ImageEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ImageEditor : UserControl
    {
        private ImageEditorViewModel _imageEditorViewmodel;
        public ImageEditor()
        {
            InitializeComponent();
            _imageEditorViewmodel = new ImageEditorViewModel(null);
            DataContext = _imageEditorViewmodel;

            Loaded += (sender, e) =>
            {
                var pageEditorViewModel = (Parent as FrameworkElement).DataContext as PageEditorViewModel;
                if (pageEditorViewModel != null)
                {
                    _imageEditorViewmodel.IniParentVM(pageEditorViewModel);
                    pageEditorViewModel.ImageEditorViewmodel = _imageEditorViewmodel;
                }
            };
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ImageEditor),
                new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleChanged)));

        /// <summary>
        /// Gets or sets the Scale property.  
        /// This dependency property indicates Scale attached to this view.
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        static void OnScaleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ImageEditor).OnScaleChanged();
        }

        void OnScaleChanged()
        {
            if (DataContext is ImageEditorViewModel)
            {
                (DataContext as ImageEditorViewModel).Scale = Scale;
            }
        }

        public new static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(ImageEditor),
                new FrameworkPropertyMetadata((bool)false));

        public new bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }


        #region ViewRectangle

        /// <summary>
        /// ViewRectangle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ViewRectangleProperty =
            DependencyProperty.Register("ViewRectangle", typeof(Rect), typeof(ImageEditor),
                new FrameworkPropertyMetadata(default(Rect), new PropertyChangedCallback(OnViewRectangleChanged)));

        /// <summary>
        /// Gets or sets the ViewRectangle property. This dependency property 
        /// indicates ....
        /// </summary>
        public Rect ViewRectangle
        {
            get { return (Rect)GetValue(ViewRectangleProperty); }
            set { SetValue(ViewRectangleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ViewRectangle property.
        /// </summary>
        private static void OnViewRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageEditor target = (ImageEditor)d;
            Rect oldViewRectangle = (Rect)e.OldValue;
            Rect newViewRectangle = target.ViewRectangle;
            target.OnViewRectangleChanged(oldViewRectangle, newViewRectangle);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ViewRectangle property.
        /// </summary>
        protected virtual void OnViewRectangleChanged(Rect oldViewRectangle, Rect newViewRectangle)
        {
            _imageEditorViewmodel.LayoutBtnGridMargin
                = new Thickness(newViewRectangle.Left + newViewRectangle.Width - 300 - 24,newViewRectangle.Top + 12, 0, 0);
        }

        #endregion

        #region NotifyFocus

        /// <summary>
        /// NotifyFocus Dependency Property
        /// </summary>
        public static readonly DependencyProperty NotifyFocusProperty =
            DependencyProperty.Register("NotifyFocus", typeof(object), typeof(ImageEditor),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNotifyFocusChanged)));

        public object NotifyFocus
        {
            get { return (object)GetValue(NotifyFocusProperty); }
            set { SetValue(NotifyFocusProperty, value); }
        }

        /// <summary>
        /// Handles changes to the NotifyFocus property.
        /// </summary>
        private static void OnNotifyFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageEditor target = (ImageEditor)d;
            object oldNotifyFocus = (object)e.OldValue;
            object newNotifyFocus = target.NotifyFocus;
            target.OnNotifyFocusChanged(oldNotifyFocus, newNotifyFocus);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the NotifyFocus property.
        /// </summary>
        protected virtual void OnNotifyFocusChanged(object oldNotifyFocus, object newNotifyFocus)
        {
            if (imageeditor_grid.IsLoaded)
            {
                imageeditor_grid.Focus();
            }
            else
            {
                RoutedEventHandler loadHandler = null;
                loadHandler = (sender, e) =>
                {
                    imageeditor_grid.Loaded -= loadHandler;
                    imageeditor_grid.Focus();
                };

                imageeditor_grid.Loaded += loadHandler;
            }
        }

        #endregion
    }
}
