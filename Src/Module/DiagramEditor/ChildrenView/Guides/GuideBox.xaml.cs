using Naver.Compass.Common.CommonBase;
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
    /// Interaction logic for GuideBox.xaml
    /// </summary>
    public partial class GuideBox : UserControl
    {
        public GuideBox()
        {
            InitializeComponent();
            this.DataContext = new GuideBoxViewModel();
        }

        #region Scale
        /// <summary>
        /// Model Dependency Property
        /// </summary>
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(GuideBox),
                new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleChanged)));

        /// <summary>
        /// Gets or sets the Model property.  This dependency property 
        /// indicates model attached to this view.
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        static void OnScaleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as GuideBox).OnScaleChanged();
        }

        void OnScaleChanged()
        {
            (this.DataContext as GuideBoxViewModel).Scale = Scale;
        }

        #endregion

        #region GuideType
        public static readonly DependencyProperty GuideTypeProperty =
            DependencyProperty.Register("GuideBox", typeof(GuideType), typeof(GuideBox),
         new FrameworkPropertyMetadata(GuideType.Local, new PropertyChangedCallback(OnGuideTypeChanged)));

        static void OnGuideTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as GuideBox).OnGuideTypeChanged();
        }
        void OnGuideTypeChanged()
        {
            (this.DataContext as GuideBoxViewModel).GuideType = GuideType;
        }

        /// <summary>
        /// Gets or sets the Model property.  This dependency property 
        /// indicates model attached to this view.
        /// </summary>
        public GuideType GuideType
        {
            get { return (GuideType)GetValue(GuideTypeProperty); }
            set { SetValue(GuideTypeProperty, value); }
        }
        #endregion

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
         DependencyProperty.Register("IsSelected", typeof(bool), typeof(GuideBox),
         new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));
        static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as GuideBox).OnIsSelectedChanged();
        }
        void OnIsSelectedChanged()
        {
            (this.DataContext as GuideBoxViewModel).IsSelected = IsSelected;
        }

    }
}
