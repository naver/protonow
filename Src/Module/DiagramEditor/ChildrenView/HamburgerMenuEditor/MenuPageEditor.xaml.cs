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
    /// Interaction logic for MenuPageEditor.xaml
    /// </summary>
    public partial class MenuPageEditor : UserControl
    {
        private MenuPageEditorViewModel menuEditorViewmodel;
        public MenuPageEditor()
        {
            InitializeComponent();
            menuEditorViewmodel = new MenuPageEditorViewModel(null);
            this.DataContext = menuEditorViewmodel;

            this.Loaded += (sender, e) =>
            {
                var pageEditorViewModel = (this.Parent as FrameworkElement).DataContext as PageEditorViewModel;
                if (pageEditorViewModel != null)
                {
                    menuEditorViewmodel.IniParentVM(pageEditorViewModel);
                    pageEditorViewModel.MenuPageEditorViewModel = menuEditorViewmodel;
                }
            };
        }

        /// <summary>
        /// IsVisible Dependency Property
        /// </summary>
        public new static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(MenuPageEditor),
                new FrameworkPropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the IsVisible property. This dependency property 
        /// indicates ....
        /// </summary>
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
            DependencyProperty.Register("ViewRectangle", typeof(Rect), typeof(MenuPageEditor),
                new FrameworkPropertyMetadata(default(Rect)));

        /// <summary>
        /// Gets or sets the ViewRectangle property. This dependency property 
        /// indicates ....
        /// </summary>
        public Rect ViewRectangle
        {
            get { return (Rect)GetValue(ViewRectangleProperty); }
            set { SetValue(ViewRectangleProperty, value); }
        }

        #endregion

        #region NotifyFocus

        /// <summary>
        /// NotifyFocus Dependency Property
        /// </summary>
        public static readonly DependencyProperty NotifyFocusProperty =
            DependencyProperty.Register("NotifyFocus", typeof(object), typeof(MenuPageEditor),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnNotifyFocusChanged)));

        /// <summary>
        /// Gets or sets the NotifyFocus property. This dependency property 
        /// indicates ....
        /// </summary>
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
            MenuPageEditor target = (MenuPageEditor)d;
            object oldNotifyFocus = (object)e.OldValue;
            object newNotifyFocus = target.NotifyFocus;
            target.OnNotifyFocusChanged(oldNotifyFocus, newNotifyFocus);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the NotifyFocus property.
        /// </summary>
        protected virtual void OnNotifyFocusChanged(object oldNotifyFocus, object newNotifyFocus)
        {
            menulpage_grid.Focus();
        }

        #endregion
    }
}
