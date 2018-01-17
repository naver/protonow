
using System.Windows.Documents;
using System.Collections.Generic;


namespace Naver.Compass.Common
{
  public interface ITextFormatter
  {
    List<CommonBase.WidgetText> GetListText(FlowDocument document);
    void SetListText(FlowDocument document, List<CommonBase.WidgetText> text);

     string GetText(System.Windows.Documents.FlowDocument document);
     void SetText(System.Windows.Documents.FlowDocument document, string text);
  }
}
