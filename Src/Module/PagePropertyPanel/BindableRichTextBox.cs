using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Naver.Compass.Module
{
    public class BindableRichTextBox : RichTextBox
    {
        public FlowDocument BindableDocument
        {
            get { return (FlowDocument)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("BindableDocument", typeof(FlowDocument), typeof(BindableRichTextBox), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTextPropertyChanged)));

        private static void OnTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BindableRichTextBox textBox = sender as BindableRichTextBox;
            if (textBox != null)
            {
                textBox._changeFromBinding = true;
                textBox.OnTextPropertyChanged(e);
            }
        }

        // 防止死锁，比如A变了通知B，B变了又通知A
        private bool _changeFromBinding = false;

        // 当BindableDocument属性变化时，通知Document属性
        protected virtual void OnTextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_changeFromBinding)
            {
                this.Document = e.NewValue as FlowDocument;
            }
        }

        // 当Document属性变化时，通知BindableDocument属性
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (!_changeFromBinding)
            {
                this.BindableDocument = this.Document;
            }
            // 放到外面
            _changeFromBinding = false;
        }
    }
}
