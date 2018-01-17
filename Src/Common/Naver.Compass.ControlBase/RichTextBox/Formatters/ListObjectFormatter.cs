using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Common
{
    public class ListObjectFormatter : ITextFormatter
    {
        public string GetText(System.Windows.Documents.FlowDocument document)
        {
            return @"";
        }
        public void SetText(System.Windows.Documents.FlowDocument document, string text)
        {

        }
       public List<CommonBase.WidgetText> GetListText(FlowDocument document)
        {
           List<CommonBase.WidgetText> list = new List<CommonBase.WidgetText>();
           if (document != null)
           {
               foreach (Block block in document.Blocks)
               {
                   if (block is Paragraph)
                   {
                       Paragraph para = block as Paragraph;
                       foreach (TextElement control in para.Inlines)
                       {
                           if (control is Run)
                           {
                               Run run = control as Run;
                               CommonBase.WidgetText text = new CommonBase.WidgetText();
                               if (run.FontWeight == FontWeights.Bold)
                               {
                                   text.Bold = true;
                               }

                               if (run.FontStyle == FontStyles.Italic)
                               {
                                   text.Italic = true;
                               }

                               text.Underline = false;
                               text.StrikeThough = false;
                               foreach (TextDecoration dec in run.TextDecorations)
                               {
                                   if (dec.Location == TextDecorationLocation.Underline)
                                   {
                                       text.Underline = true;
                                   }
                                   else if (dec.Location == TextDecorationLocation.Strikethrough)
                                   {
                                       text.StrikeThough = true;
                                   }

                               }
                               
                               text.FontSize = run.FontSize;
                               text.FontFamily = run.FontFamily.ToString();
                               text.Color = CommonFunction.CovertColorToInt(((SolidColorBrush)run.Foreground).Color);
                               text.sText = run.Text;

                               list.Add(text);
                           }
                       }
                   }
               }
           }

           return list;
        }
       public void SetListText(FlowDocument document, List<CommonBase.WidgetText> text)
        {
            Paragraph para = new Paragraph();
            document.Blocks.Clear();
           foreach (CommonBase.WidgetText line in text)
           {
               Run run = new Run();
               run.Text = line.sText;
               run.FontFamily = new System.Windows.Media.FontFamily(line.FontFamily);
               run.FontSize = line.FontSize;
               run.Foreground = new SolidColorBrush(CommonFunction.CovertIntToColor(line.Color));
               
               if (line.Bold)
               {
                   run.FontWeight = FontWeights.Bold;
               }

               if (line.Italic)
               {
                   run.FontStyle = FontStyles.Italic;
               }

               TextDecorationCollection collect = new TextDecorationCollection();
               if (line.Underline)
               {
                   collect.Add(CommonFunction.GetUnderline());
               }

               if (line.StrikeThough)
               {
                   collect.Add(CommonFunction.GetStrikeThough());
               }

               run.TextDecorations = collect;

               para.Inlines.Add(run);
           }

           document.Blocks.Add(para);
        }
    }
}
