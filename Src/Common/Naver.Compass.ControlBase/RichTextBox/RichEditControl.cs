
using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using Naver.Compass.Common.CommonBase;
using System.Windows.Media;
using System.Diagnostics;
using Naver.Compass.Service.Document;
using System.IO;
using System.Text;
using System.Windows.Markup;

namespace Naver.Compass.Common
{
    public partial class ExRichTextBox : System.Windows.Controls.RichTextBox
    {
        #region Private Members

        private string BulletSymol = "•";
        #endregion //Private Members

        #region Constructors

        public ExRichTextBox()
        {
            Style style = new Style { TargetType = typeof(Paragraph) };
            Setter setter = new Setter();
            setter.Property = Paragraph.MarginProperty;
            setter.Value = new Thickness(0);
            style.Setters.Add(setter);
            Resources.Add(typeof(Paragraph), style);

            style = new Style { TargetType = typeof(List) };

            setter = new Setter();
            setter.Property = List.MarkerOffsetProperty;
            setter.Value = (double)1;
            style.Setters.Add(setter);

            setter = new Setter();
            setter.Property = List.PaddingProperty;
            setter.Value = new Thickness(6, 0, 6, 0);
            style.Setters.Add(setter);

            setter = new Setter();
            setter.Property = List.MarginProperty;
            setter.Value = new Thickness(0);
            style.Setters.Add(setter);

            Resources.Add(typeof(List), style);

            TargetUpdated += ExRichTextBox_TargetUpdated;
            Unloaded += ExRichTextBox_Unloaded;
            Loaded += ExRichTextBox_Loaded;

            ContextMenuOpening +=ExRichTextBox_ContextMenuOpening;
        }
       
        void ExRichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
            TextRange rang = new TextRange(Document.ContentStart, Document.ContentEnd);

            string sText = rang.Text.Replace("\r\n", "");

            if (sText.Length > 0)
            {
                SelectAll();
            }

            UndoLimit = 100;
            IsUndoEnabled = true;
            
            ScrollToHome();
        }

        void ExRichTextBox_Unloaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.BindingOperations.ClearAllBindings(this.Parent);
            System.Windows.Data.BindingOperations.ClearAllBindings(this);

            Document.Blocks.Clear();
            UndoLimit = -1;
            IsUndoEnabled = false;
        }

        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste));
            CommandBindings.Add(new CommandBinding(EditingCommands.IncreaseFontSize, OnFontSizeIncrement));
            CommandBindings.Add(new CommandBinding(EditingCommands.DecreaseFontSize, OnFontSizeIncrement));
            CommandBindings.Add(new CommandBinding(TextCommands.UpDownCaseHotKey, OnUpDownCase));

            CommandBindings.Add(new CommandBinding(EditingCommands.AlignCenter, OnPreviewEditCommand));
            CommandBindings.Add(new CommandBinding(EditingCommands.AlignJustify, OnPreviewEditCommand));
            CommandBindings.Add(new CommandBinding(EditingCommands.AlignLeft, OnPreviewEditCommand));
            CommandBindings.Add(new CommandBinding(EditingCommands.AlignRight, OnPreviewEditCommand));
            CommandBindings.Add(new CommandBinding(EditingCommands.ToggleBullets, OnPreviewEditCommand));
            CommandBindings.Add(new CommandBinding(EditingCommands.ToggleNumbering, OnPreviewEditCommand));

            CommandBindings.Add(new CommandBinding(EditingCommands.ToggleBold, OnEditCommand_Bold));
            CommandBindings.Add(new CommandBinding(EditingCommands.ToggleItalic, OnEditCommand_Italic));
            CommandBindings.Add(new CommandBinding(EditingCommands.ToggleUnderline, OnEditCommand_Underline));
            base.OnInitialized(e);
        }

        #endregion //Constructors

        #region Private function

        public void UpdateTextFromDocument()
        {
            Text = TextFormatter.GetText(Document);
        }

        private void UpdateDocumentFromText()
        {
            TextFormatter.SetText(Document, Text);
        }

        private void UpdateLineSpace()
        {
            int index = 0;
            double dLineHeight = 0.0;
            try
            {
                foreach (Block block in Document.Blocks)
                {
                    if (block is Paragraph)
                    {
                        Paragraph para = block as Paragraph;
                        index = 0;
                        dLineHeight = 0.0;
                        foreach (Run run in para.Inlines)
                        {
                            double oldHeight = run.FontSize * run.FontFamily.LineSpacing;
                            oldHeight += (oldHeight * 3/8) ;
                            dLineHeight += oldHeight;
                            index++;
                        }
                        para.LineHeight = Math.Round(dLineHeight / index);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


        }

        private void UpdateLineSpaceAndFixProperty()
        {
            int index = 0;
            double dLineHeight = 0.0;
            try
            {
                foreach (Block block in Document.Blocks)
                {
                    if (block is Paragraph)
                    {
                        Paragraph para = block as Paragraph;
                        index = 0;
                        dLineHeight = 0.0;
                        foreach (Run run in para.Inlines)
                        {
                            double oldHeight = run.FontSize * run.FontFamily.LineSpacing;
                            oldHeight += (oldHeight * 3 / 8);
                            dLineHeight += oldHeight;
                            index++;

                            run.FontFamily = run.FontFamily;
                            run.FontSize = run.FontSize;
                            run.Foreground = run.Foreground;
                            run.FontWeight = run.FontWeight;
                            run.FontStyle = run.FontStyle;
                           // run.TextDecorations = run.TextDecorations;
                        }
                        para.LineHeight = Math.Round(dLineHeight / index);
                        para.TextAlignment = para.TextAlignment;
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


        }

        private void FlashPropertyBySection(TextRange rang)
        {

            if (rang != null)
            {
                Object Obj = rang.GetPropertyValue(FontFamilyProperty);

                if (Obj != null && Obj != DependencyProperty.UnsetValue)
                {
                    SectionFontFamily = Obj.ToString();
                }
                else
                {
                    SectionFontFamily = "";
                }

                Obj = rang.GetPropertyValue(ForegroundProperty);
                if (Obj != null && Obj != DependencyProperty.UnsetValue)
                {
                    SectionFontcolor = ((SolidColorBrush)Obj).Color;
                }
                else
                {
                    SectionFontcolor = Color.FromRgb(0, 0, 1);
                }

                Obj = rang.GetPropertyValue(FontSizeProperty);
                if (Obj != null && Obj != DependencyProperty.UnsetValue)
                {
                    SectionFontsize = Convert.ToDouble(Obj);
                }
                else
                {
                    SectionFontsize = -1;
                }

                if (rang.GetPropertyValue(FontWeightProperty).ToString() == "Bold")
                {
                    SectionFontBold = true;
                }
                else
                {
                    SectionFontBold = false;
                }

                if (rang.GetPropertyValue(FontStyleProperty).ToString() == "Italic")
                {
                    SectionFontItalic = true;
                }
                else
                {
                    SectionFontItalic = false;
                }

                FlashfontDecoration(rang);

                Obj = rang.GetPropertyValue(Paragraph.TextAlignmentProperty);

                if (Obj != null && Obj != DependencyProperty.UnsetValue)
                {
                    FontAlignment = (TextAlignment)Obj;
                }

                if (FlashBulletStyle(rang))
                {
                    TextBulletStyle = TextMarkerStyle.Disc;
                }
                else
                {
                    TextBulletStyle = TextMarkerStyle.None;
                }
                
            }
        }

        private bool IsHaveBullet(Paragraph para)
        {
            if (para != null)
            {
                if (para.Inlines.Count > 0)
                {
                    Run run = para.Inlines.FirstInline as Run;
                    return (run.Text.IndexOf(BulletSymol) > -1);
                }
            }

            return false;
        }
        private bool FlashBulletStyle(TextRange rang)
        {
            TextPointer pStart = rang.Start;
            TextPointer pEnd = rang.End;
            bool bBullet = true;
            int compareValue = pStart.CompareTo(pEnd);

            bool bInit = false;
            if (compareValue <= 0)
            {
                Paragraph para = pStart.Paragraph;

                while (para != null && para.ContentStart.CompareTo(pEnd) < 0)
                {
                    bBullet &= IsHaveBullet(para);

                    para = para.NextBlock as Paragraph;

                    bInit = true;
                }
            }
            else if (compareValue > 0)
            {
                Paragraph para = pStart.Paragraph;

                while (para != null && para.ContentEnd.CompareTo(pEnd) > 0)
                {
                    bBullet &= IsHaveBullet(para);

                    para = para.PreviousBlock as Paragraph;

                    bInit = true;
                }
            }
            if (bInit)
            {
                return bBullet;
            }
            else
            {
                return false;
            }

        }
        private void UpdateProperty_AfterTextChange()
        {
            TextRange rang = new TextRange(Document.ContentStart, Document.ContentEnd);

            FlashPropertyBySection(rang);
        }

        private void UpdateProperty_AfterSelectChange()
        {
            FlashPropertyBySection(Selection);
        }
         private void SetfontDecoration(TextDecorationLocation type, bool setvalue)
        {
            if (Selection != null && Selection.Text.Length > 0)
            {
                TextRange rang = new TextRange(Selection.Start, Selection.End);
                SetfontDecoration(rang, type, setvalue);
                Selection.Select(rang.Start, rang.End);
             }
            else
            {
                try
                {
                    foreach (Block block in Document.Blocks)//vivid
                    {
                        if (block is Paragraph)
                        {
                            Paragraph para = block as Paragraph;
                            foreach (Run run in para.Inlines)
                            {
                                ChangeElementDecoration(run, type, setvalue);
                            }
                        }
                        else
                        {
                            NLogger.Error("SetfontDecoration->Invalid Block in Document.Blocks!");
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private void SetfontDecoration(TextRange range, TextDecorationLocation type, bool setvalue)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (range.CanSave(DataFormats.Xaml))
                {
                    range.Save(ms, DataFormats.Xaml, true);

                    //string teststring =  ASCIIEncoding.Default.GetString(ms.ToArray());
                    ms.Seek(0, SeekOrigin.Begin);

                    object obj = XamlReader.Load(ms);

                    if (obj is Span)
                    {
                        Span span = obj as Span;
                        foreach (Run run in span.Inlines)
                        {
                            ChangeElementDecoration(run, type, setvalue);
                        }

                        using (MemoryStream mso = new MemoryStream())
                        {
                            XamlWriter.Save(span, mso);
                            mso.Seek(0, SeekOrigin.Begin);

                            range.Load(mso, DataFormats.Xaml);
                        }
                    }
                    else if (obj is Section)
                    {

                        Section section = obj as Section;
                        foreach (Block block in section.Blocks)//vivid
                        {
                            if (block is Paragraph)
                            {
                                Paragraph para = block as Paragraph;
                                foreach (Run run in para.Inlines)
                                {
                                    ChangeElementDecoration(run, type, setvalue);
                                }
                            }
                            else
                            {
                                NLogger.Error("SetfontDecoration->Invalid Block in section.Blocks!");
                            }
                        }

                        using (MemoryStream mso = new MemoryStream())
                        {
                            XamlWriter.Save(section, mso);
                            mso.Seek(0, SeekOrigin.Begin);

                            range.Load(mso, DataFormats.Xaml);
                        }
                    }
                    else
                    {
                        NLogger.Error("SetfontDecoration select span or section is null\n" + Selection.Text.ToString());
                    }
                }
            }
        }

        private void FlashfontDecoration(TextRange range)
        {
            int iUnderline;
            int iStrikeThough;
            bool bUnderline = true;
            bool bstrike = true;

            bool bInit = false;
            using (MemoryStream ms = new MemoryStream())
            {
                if (range.CanSave(DataFormats.Xaml))
                {
                    range.Save(ms, DataFormats.Xaml, true);

                    ms.Seek(0, SeekOrigin.Begin);

                    object obj = XamlReader.Load(ms);

                    if (obj is Span)
                    {
                        iUnderline = 0;
                        iStrikeThough = 0;

                        Span span = obj as Span;
                        foreach (Run run in span.Inlines)
                        {
                            foreach (TextDecoration de in run.TextDecorations)
                            {
                                if (de.Location == TextDecorationLocation.Underline)
                                {
                                    iUnderline++;
                                }
                                if (de.Location == TextDecorationLocation.Strikethrough)
                                {
                                    iStrikeThough++;
                                }
                            }

                            bInit = true;
                        }

                        if (iUnderline != span.Inlines.Count && bUnderline == true)
                        {
                            bUnderline = false;
                        }

                        if (iStrikeThough != span.Inlines.Count && bstrike == true)
                        {
                            bstrike = false;
                        }

                       
                    }
                    else if (obj is Section)
                    {
                        iUnderline = 0;
                        iStrikeThough = 0;

                        Section section = obj as Section;
                        foreach (Block block in section.Blocks)//vivid
                        {
                            bInit = true;
                            if (block is Paragraph)
                            {
                                iUnderline = 0;
                                iStrikeThough = 0;

                                Paragraph para = block as Paragraph;
                                foreach (Run run in para.Inlines)
                                {
                                    foreach (TextDecoration de in run.TextDecorations)
                                    {
                                        if (de.Location == TextDecorationLocation.Underline)
                                        {
                                            iUnderline++;
                                        }
                                        if (de.Location == TextDecorationLocation.Strikethrough)
                                        {
                                            iStrikeThough++;
                                        }
                                    }
                                }

                                if (iUnderline != para.Inlines.Count && bUnderline)
                                {
                                    bUnderline = false;
                                }

                                if (iStrikeThough != para.Inlines.Count && bstrike)
                                {
                                    bstrike = false;
                                }

                                if (!bUnderline && !bstrike)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                NLogger.Error("block is not paragraph!");
                            }
                        }
                    }
                    else
                    {
                        NLogger.Error("FlashfontDecoration select span or section is null\n" + Selection.Text.ToString());
                    }

                }
            }

            if (bInit)
            {
                 SectionFontUnderline = bUnderline;
                 SectionFontStrikeThough = bstrike;
            }
            else
            {
                  SectionFontUnderline = false;
                  SectionFontStrikeThough = false;
            }
  
        }

        void ProcessFontSizeIncrement(bool bAdd, bool bSection = true)
        {

            try
            {
                if (bSection && Selection != null && Selection.Text.Length > 0)
                {
                    TextRange rang = new TextRange(Selection.Start,Selection.End);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        if (rang.CanSave(DataFormats.Xaml))
                        {
                            rang.Save(ms, DataFormats.Xaml, true);

                            //string teststring =  ASCIIEncoding.Default.GetString(ms.ToArray());
                            ms.Seek(0, SeekOrigin.Begin);

                            object obj = XamlReader.Load(ms);

                            if (obj is Span)
                            {
                                Span span = obj as Span;
                                foreach (Run run in span.Inlines)
                                {
                                    run.FontSize = FontSizeEnumerator.GetValue(run.FontSize, bAdd);
                                }

                                using (MemoryStream mso = new MemoryStream())
                                {
                                    XamlWriter.Save(span, mso);
                                    mso.Seek(0, SeekOrigin.Begin);

                                    rang.Load(mso, DataFormats.Xaml);
                                }
                            }
                            else if (obj is Section)
                            {

                                Section section = obj as Section;
                                foreach (Block block in section.Blocks)//vivid
                                {
                                    if (block is Paragraph)
                                    {
                                        Paragraph para = block as Paragraph;
                                        foreach (Run run in para.Inlines)
                                        {
                                            run.FontSize = FontSizeEnumerator.GetValue(run.FontSize, bAdd);
                                        }
                                    }
                                    else
                                    {
                                        NLogger.Error("ProcessFontSizeIncrement->Invalid Block in section.Blocks!");
                                    }

                                }

                                using (MemoryStream mso = new MemoryStream())
                                {
                                    XamlWriter.Save(section, mso);
                                    mso.Seek(0, SeekOrigin.Begin);

                                    rang.Load(mso, DataFormats.Xaml);
                                }

                            }
                            else
                            {
                                NLogger.Error("Increment select span or section is null\n" + Selection.Text.ToString());
                            }

                            Selection.Select(rang.Start, rang.End);
                        }
                        else
                        {
                            NLogger.Error("Increment selection can not save\n" + Selection.Text.ToString());
                        }
                    }
                }
                else
                {
                    if (Document != null)
                    {
                        

                        foreach (Block block in Document.Blocks)
                        {
                            if (block is Paragraph)
                            {
                                Paragraph para = block as Paragraph;
                                foreach (TextElement control in para.Inlines)
                                {
                                    control.FontSize = FontSizeEnumerator.GetValue(control.FontSize, bAdd);
                                }
                            }
                        }
                    }
                }

                FlashPropertyBySection(Selection);
            }
            catch (System.Exception ex)
            {
                NLogger.Error("Exception in ProcessFontSizeIncrement\n" + ex.Message);
            }
        }

        #endregion

        #region Properties

        #region TextFormatter

        public static readonly DependencyProperty TextFormatterProperty = DependencyProperty.Register("TextFormatter", typeof(ITextFormatter), typeof(ExRichTextBox), new FrameworkPropertyMetadata(new XamlFormatter()));
        public ITextFormatter TextFormatter
        {
            get
            {
                return (ITextFormatter)GetValue(TextFormatterProperty);
            }
            set
            {
                SetValue(TextFormatterProperty, value);
            }
        }

        #endregion //TextFormatter

        #endregion //Properties

        #region Methods

        void ExRichTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (ContextMenu != null)
            {
                ContextMenu.IsOpen = true;
            }
            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
           
            UpdateProperty_AfterSelectChange();
        }
       
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            UpdateTextFromDocument();

            UpdateProperty_AfterTextChange();

            base.OnLostFocus(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                e.Handled = true;

                return;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
           
            if (e.Key == Key.Enter)
            { 
                BeginChange();

                if (TextBulletStyle == TextMarkerStyle.Disc)
                {
                   // string sText = XamlWriter.Save(Document);
                    Selection.Text = BulletSymol+"  ";
                    
                    CaretPosition = Selection.End;

                    //sText = XamlWriter.Save(Document);
                } 

                EndChange();
            }
        }
        #endregion //Methods

        #region Command binding

        private TextRange GetOptionRange()
        {
            if (Selection != null && Selection.Text.Length > 0)
            {
            //    //int i4 = CaretPosition.DocumentStart.CompareTo(CaretPosition.DocumentEnd);
            //    System.Diagnostics.Debug.WriteLine("InsertionPosition-> " + CaretPosition.IsAtInsertionPosition.ToString());
            //    System.Diagnostics.Debug.WriteLine("IsAtLineStartPosition-> " + CaretPosition.IsAtLineStartPosition.ToString());

            //    Selection.Select(Selection.Start, Selection.End);//fix the bug of Mirosoft bug
                return new TextRange(Selection.Start, Selection.End);
            }
            else
            {
                return new TextRange(Document.ContentStart, Document.ContentEnd);
            }
        }

        private void OnPreviewEditCommand(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = false;
            return;
        }

        private void OnEditCommand_Bold(object sender, ExecutedRoutedEventArgs e)
        {
            OnFontBoldChange(!SectionFontBold);
            SectionFontBold = !SectionFontBold;
        }

        private void OnEditCommand_Italic(object sender, ExecutedRoutedEventArgs e)
        {
            OnFontItalicChange(!SectionFontItalic);
            SectionFontItalic = !SectionFontItalic;
        }
        private void OnEditCommand_Underline(object sender, ExecutedRoutedEventArgs e)
        {
            bool value = !SectionFontUnderline;
            OnFontUnderlineChange(value);
            SectionFontUnderline = value;
        }

        private void UpCaseText()
        {
            Block block = Document.Blocks.FirstBlock;
            while(block != null && block is Paragraph)
            {
                Paragraph para = block as Paragraph;

                Inline line = para.Inlines.FirstInline;
                while (line != null)
                {
                    Run run = line as Run;

                    run.Text = run.Text.ToUpper();

                    line = line.NextInline;
                }

                block = block.NextBlock;
            }
        }

        private void UpCaseFirstCharofText()
        {
            Block block = Document.Blocks.FirstBlock;
            while (block != null && block is Paragraph)
            {
                Paragraph para = block as Paragraph;

                if (block.PreviousBlock == null)
                {
                    Inline line = para.Inlines.FirstInline;
                    while (line != null)
                    {
                        Run run = line as Run;
                        if (line.PreviousInline == null)
                        {
                            string sFirst = run.Text.Substring(0, 1);
                            string sExceptFirst = run.Text.Substring(1);
                            sFirst = sFirst.ToUpper();
                            sExceptFirst = sExceptFirst.ToLower();

                            run.Text = (sFirst + sExceptFirst);
                        }
                        else
                        {
                            run.Text = run.Text.ToLower();
                        }

                        line = line.NextInline;
                    }
                }
                else
                {
                    Inline line = para.Inlines.FirstInline;
                    while (line != null)
                    {
                        Run run = line as Run;

                        run.Text = run.Text.ToLower();

                        line = line.NextInline;
                    }
                }

                block = block.NextBlock;
                
            }
        }

        private void LowCaseText()
        {
            Block block = Document.Blocks.FirstBlock;
            while (block != null && block is Paragraph)
            {
                Paragraph para = block as Paragraph;

                Inline line = para.Inlines.FirstInline;
                while (line != null)
                {
                    Run run = line as Run;

                    run.Text = run.Text.ToLower();

                    line = line.NextInline;
                }

                block = block.NextBlock;
            }
        }

        private void OnChangeCharactorCase()
        {

            TextRange rang = new TextRange(Document.ContentStart, Document.ContentEnd);
            string sText = rang.Text;
            StringStatus status = CommonFunction.GetTextStatus(sText);

            if (status == StringStatus.First)
            {
                UpCaseText();
            }
            else if (status == StringStatus.Middle || status == StringStatus.AllUplow)
            {
                LowCaseText();
            }
            else //if (status == StringStatus.FirstAndMiddle || status == StringStatus.AllSmall)
            {
                UpCaseFirstCharofText();
            }
        }
        private void OnUpDownCase(object sender, ExecutedRoutedEventArgs e)
        {
            OnChangeCharactorCase();
        }

        private void OnFontSizeIncrement(object sender, ExecutedRoutedEventArgs e)
        {
            BeginChange();
            ProcessFontSizeIncrement(e.Command == EditingCommands.IncreaseFontSize, true);

            UpdateLineSpace();
            EndChange();
        }

        private void ChangeElementFontSize(TextElement run, bool bAdd)
        {
            object rValue = run.GetValue(TextElement.FontSizeProperty);
            if (rValue != null && rValue != DependencyProperty.UnsetValue)
            {
                double oldValue = Convert.ToDouble(run.GetValue(TextElement.FontSizeProperty));

                run.SetValue(TextElement.FontSizeProperty, FontSizeEnumerator.GetValue(oldValue, bAdd));
            }
        }

        private void ChangeElementDecoration(Inline run, TextDecorationLocation type, bool bAdd)
        {
            if (type == TextDecorationLocation.Underline)
            {
                if (bAdd)
                {
                    bool isExit = false;

                    foreach (TextDecoration de in run.TextDecorations)
                    {
                        if (de.Location == TextDecorationLocation.Underline)
                        {
                            isExit = true;
                            break;
                        }
                    }

                    if (!isExit)
                    {
                        run.TextDecorations.Add(CommonFunction.GetUnderline()[0]);
                    }
                }
                else
                {
                    int index = -1;
                    foreach (TextDecoration de in run.TextDecorations)
                    {
                        index++;
                        if (de.Location == TextDecorationLocation.Underline)
                        {
                            run.TextDecorations.RemoveAt(index);
                            break;
                        }
                    }
                }
            }

            if (type == TextDecorationLocation.Strikethrough)
            {
                if (bAdd)
                {
                    bool isExit = false;

                    foreach (TextDecoration de in run.TextDecorations)
                    {
                        if (de.Location == TextDecorationLocation.Strikethrough)
                        {
                            isExit = true;
                            break;
                        }
                    }
                    if (!isExit)
                    {
                        run.TextDecorations.Add(CommonFunction.GetStrikeThough()[0]);
                    }
                }
                else
                {
                    int index = -1;
                    foreach (TextDecoration de in run.TextDecorations)
                    {
                        index++;
                        if (de.Location == TextDecorationLocation.Strikethrough)
                        {
                            run.TextDecorations.RemoveAt(index);
                            break;
                        }
                    }

                }
            }
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.Xaml))
                {
                    string sXamlText = Clipboard.GetText(TextDataFormat.Xaml);
                    sXamlText = System.Text.RegularExpressions.Regex.Replace(sXamlText, "&#x(0?[0-8B-F]|1[0-9A-F]|7F);", "");
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sXamlText)))
                    {
                        Selection.Load(ms, DataFormats.Xaml);
                    }
                    CaretPosition = Document.ContentEnd;

                    UpdateLineSpace();
                }
                else if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                {
                    string sText = Clipboard.GetText(TextDataFormat.UnicodeText);
                    sText = sText.Replace("\r\n", "");
                    Selection.Text = sText;

                    CaretPosition = Document.ContentEnd;

                    UpdateLineSpace();
                }
                //else if (Clipboard.ContainsText(TextDataFormat.Rtf))
                //{
                //    string sUnicodeText = Clipboard.GetText(TextDataFormat.Rtf);
                //    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sUnicodeText)))
                //    {
                //        Selection.Load(ms, DataFormats.Rtf);
                //    }
                //}

                UpdateProperty_AfterSelectChange();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                e.Handled = true;
                return;
            }
            e.Handled = true;
        }

        #endregion

    }
}
