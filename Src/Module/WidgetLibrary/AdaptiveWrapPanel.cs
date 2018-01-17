using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Naver.Compass.Module
{
    // Implements ItemsControl for ToolboxItems    
    public class AdaptiveWrapPanel : WrapPanel
    {
        // Defines the ItemHeight and ItemWidth properties of
        // the WrapPanel used for this Toolbox
        public AdaptiveWrapPanel()
        {
            SizeChanged += AdaptiveWrapPanel_SizeChanged2;
            this.Loaded += AdaptiveWrapPanel_Loaded;
        }

        void AdaptiveWrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            OnSearchTextChanged(null, null);
        }        
        
        #region Private Function
        private List<UIElement> GetVisibleChildren(UIElementCollection all)
        {
            List<UIElement> VisibleChildren = new List<UIElement>();
            foreach (UIElement it in all)
            {
                if(it.Visibility==Visibility.Visible)
                {
                    VisibleChildren.Add(it);
                }
            }

            return VisibleChildren;
        }
        #endregion

        #region Envent Handler
        void AdaptiveWrapPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight <= 0)
                return;            
            if (e.WidthChanged == false)
                return;
            if (sender != this)
                return;

            List<UIElement> VisibleChildren = GetVisibleChildren(this.Children);
            double width = e.NewSize.Width;  
            int minNum = Convert.ToInt32(width) / 80;

            double realszie, spare=0,nRealNum;
            if (minNum > 1)
            {
                int nNum = VisibleChildren.Count;
                if (nNum == 0)
                    return;

                nRealNum = Math.Min(minNum, nNum);
                realszie = Convert.ToInt32(width / nRealNum);
                spare =realszie * nRealNum-width; 
                if (minNum >= nNum)
                {
                    nRealNum = nNum;
                    realszie = Convert.ToInt32(width / nRealNum);
                    spare = realszie * nRealNum - width;
                    realszie = Math.Min(120, realszie);
                }
                
            }
            else
            {
                nRealNum = 1;
                spare = 0;
                realszie = Math.Max(80, width);
            }
            //Console.WriteLine("-------------"+minNum+"----"+realszie+"----"+width+"----"+spare);

            int i = 1;            
            foreach (UIElement it in VisibleChildren)
            {
                ToolboxItem item = it as ToolboxItem;
                if (i % nRealNum >=1 && i % nRealNum<=Math.Abs(spare))
                {
                    if (spare>0)
                        item.Width = realszie-1;
                    else if(spare<0)
                        item.Width = realszie + 1;
                    else
                        item.Width = realszie;
                }
                else
                {
                    item.Width = realszie;
                }
                //item.Height = item.Width;
                item.Height = 90;
                i++;
                
            }

            //InvalidateMeasure();
            //InvalidateArrange();
        }
        void AdaptiveWrapPanel_SizeChanged2(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight <= 0)
                return;
            if (e.WidthChanged == false)
                return;
            if (sender != this)
                return;

            List<UIElement> VisibleChildren = GetVisibleChildren(this.Children);
            double width = e.NewSize.Width;
            int minNum = Convert.ToInt32(width) / 90;

            double realszie, spare = 0, nRealNum;
            if (minNum > 0)
            {
                int nNum = VisibleChildren.Count;
                if (nNum == 0)
                    return;

                realszie = Convert.ToInt32(width / minNum);
                spare = realszie * minNum - width;
                nRealNum = minNum;
                //if (minNum >= nNum)
                //{
                //    nRealNum = nNum;
                //    realszie = Convert.ToInt32(width / nRealNum);
                //    spare = realszie * nRealNum - width;
                //    realszie = Math.Min(120, realszie);
                //}

            }
            else
            {
                nRealNum = 1;
                spare = 0;
                realszie = Math.Max(90, width);
            }
            //Console.WriteLine("-------------"+minNum+"----"+realszie+"----"+width+"----"+spare);

            int i = 1;
            foreach (UIElement it in VisibleChildren)
            {
                ToolboxItem item = it as ToolboxItem;
                if (i % nRealNum >= 1 && i % nRealNum <= Math.Abs(spare))
                {
                    if (spare > 0)
                        item.Width = realszie - 1;
                    else if (spare < 0)
                        item.Width = realszie + 1;
                    else
                        item.Width = realszie;
                }
                else
                {
                    item.Width = realszie;
                }
                //item.Height = item.Width;
                item.Height = 90;
                i++;

            }

            //InvalidateMeasure();
            //InvalidateArrange();
        }
        #endregion

        #region Dependency Property
        /// <summary>
        /// SearchText Dependency Property
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(AdaptiveWrapPanel),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnSearchTextChanged)));

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdaptiveWrapPanel target = (AdaptiveWrapPanel)d;
            string oldSearchText = (string)e.OldValue;
            string newSearchText = target.SearchText;
            target.OnSearchTextChanged(oldSearchText, newSearchText);
        }

        protected virtual void OnSearchTextChanged(string oldSearchText, string newSearchText)
        {
            if (this.ActualHeight <= 0)
                return;

            List<UIElement> VisibleChildren = GetVisibleChildren(this.Children);
            //if (VisibleChildren.Count < 1)
            //{
            //    return;
            //}

            double width = ActualWidth;
            int minNum = Convert.ToInt32(width) / 90;

            #region older auto-size arrange
            //double realszie, spare = 0, nRealNum;
            //if (minNum > 1)
            //{
            //    int nNum = VisibleChildren.Count;
            //    if (nNum == 0)
            //        return;

            //    nRealNum = Math.Min(minNum, nNum);
            //    realszie = Convert.ToInt32(width / nRealNum);
            //    spare = realszie * nRealNum - width;
            //    if (minNum >= nNum)
            //    {
            //        nRealNum = nNum;
            //        realszie = Convert.ToInt32(width / nRealNum);
            //        spare = realszie * nRealNum - width;
            //        realszie = Math.Min(120, realszie);
            //    }

            //}
            //else
            //{
            //    nRealNum = 1;
            //    spare = 0;
            //    realszie = Math.Max(80, width);
            //}

            //int i = 1;
            //foreach (UIElement it in VisibleChildren)
            //{
            //    ToolboxItem item = it as ToolboxItem;
            //    if (i % nRealNum >= 1 && i % nRealNum <= Math.Abs(spare))
            //    {
            //        if (spare > 0)
            //            item.Width = realszie - 1;
            //        else if (spare < 0)
            //            item.Width = realszie + 1;
            //        else
            //            item.Width = realszie;
            //    }
            //    else
            //    {
            //        item.Width = realszie;
            //    }
            //    //item.Height = item.Width;
            //    item.Height = 90;
            //    i++;

            //}
            #endregion
            
            #region latest auto-szie arrange
            double realszie, spare = 0, nRealNum;
            if (minNum > 0)
            {
                int nNum = VisibleChildren.Count;
                if (nNum == 0)
                    return;

                realszie = Convert.ToInt32(width / minNum);
                spare = realszie * minNum - width;
                nRealNum = minNum;
            }
            else
            {
                nRealNum = 1;
                spare = 0;
                realszie = Math.Max(90, width);
            }

            int i = 1;
            foreach (UIElement it in VisibleChildren)
            {
                ToolboxItem item = it as ToolboxItem;
                if (i % nRealNum >= 1 && i % nRealNum <= Math.Abs(spare))
                {
                    if (spare > 0)
                        item.Width = realszie - 1;
                    else if (spare < 0)
                        item.Width = realszie + 1;
                    else
                        item.Width = realszie;
                }
                else
                {
                    item.Width = realszie;
                }
                //item.Height = item.Width;
                item.Height = 90;
                i++;
            }
            #endregion

        }


        /// <summary>
        /// ItemChangedInfo Dependency Property
        /// </summary>
        public static readonly DependencyProperty ItemChangedInfoProperty =
            DependencyProperty.Register("ItemChangedInfo", typeof(object), typeof(AdaptiveWrapPanel),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnItemChangedInfoChanged)));

        /// <summary>
        /// Gets or sets the ItemChangedInfo property. This dependency property 
        /// indicates ....
        /// </summary>
        public object ItemChangedInfo
        {
            get { return (object)GetValue(ItemChangedInfoProperty); }
            set { SetValue(ItemChangedInfoProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ItemChangedInfo property.
        /// </summary>
        private static void OnItemChangedInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdaptiveWrapPanel target = (AdaptiveWrapPanel)d;
            object oldItemChangedInfo = (object)e.OldValue;
            object newItemChangedInfo = target.ItemChangedInfo;
            target.OnItemChangedInfoChanged(oldItemChangedInfo, newItemChangedInfo);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ItemChangedInfo property.
        /// </summary>
        protected virtual void OnItemChangedInfoChanged(object oldItemChangedInfo, object newItemChangedInfo)
        {
            Naver.Compass.Common.CommonBase.NLogger.Debug("OnItemChangedInfoChanged");
            OnSearchTextChanged(null, null);
        }

        #endregion
    }

}
