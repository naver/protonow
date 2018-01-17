using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Naver.Compass.Common
{
    public class InlineTextBlock : TextBlock
    {
        #region Content

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(InlineContent), typeof(InlineTextBlock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnContentChanged)));
        public InlineContent Content
        {
            get { return (InlineContent)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineTextBlock target = (InlineTextBlock)d;
            InlineContent oldContent = (InlineContent)e.OldValue;
            InlineContent newContent = target.Content;
            target.OnContentChanged(oldContent, newContent);
        }

        protected virtual void OnContentChanged(InlineContent oldContent, InlineContent newContent)
        {
            this.Inlines.Clear();
            if (newContent != null && newContent.Count > 0)
            {
                foreach (var inline in newContent)
                {
                    var run = new Run();
                    run.Text = inline.Text;
                    run.Foreground = inline.Foreground;
                    run.Background = inline.Background;
                    if (inline.FontSize > 0)
                    {
                        run.FontSize = inline.FontSize;
                    }

                    this.Inlines.Add(run);
                }
            }
        }

        #endregion


    }

    public class InlineInfo
    {
        public double FontSize { get; set; }
        public string Text { get; set; }
        public Brush Foreground { get; set; }
        public Brush Background { get; set; }

        public InlineInfo()
        {
            Foreground = new SolidColorBrush(Colors.Black);
        }
    }

    public class InlineContent : List<InlineInfo>
    {
    }
}
