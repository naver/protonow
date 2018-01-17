

using System.Windows.Documents;
using System.Collections.Generic;

namespace Naver.Compass.Common
{
  /// <summary>
  /// Formats the RichTextBox text as plain text
  /// </summary>
  public class PlainTextFormatter : ITextFormatter
  {
    public string GetText( FlowDocument document )
    {
      return new TextRange( document.ContentStart, document.ContentEnd ).Text;
    }

    public void SetText( FlowDocument document, string text )
    {
      new TextRange( document.ContentStart, document.ContentEnd ).Text = text;
    }

    public List<CommonBase.WidgetText> GetListText(FlowDocument document)
    {
        return new List<CommonBase.WidgetText>();
    }
    public void SetListText(FlowDocument document, List<CommonBase.WidgetText> text)
    {
       
    }
  }
}
