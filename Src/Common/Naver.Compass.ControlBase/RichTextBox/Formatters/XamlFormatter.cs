

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Naver.Compass.Common
{
  /// <summary>
  /// Formats the RichTextBox text as Xaml
  /// </summary>
  public class XamlFormatter : ITextFormatter
  {
      public List<CommonBase.WidgetText> GetListText(FlowDocument document)
      {
          return new List<CommonBase.WidgetText>();
      }
      public void SetListText(FlowDocument document, List<CommonBase.WidgetText> text)
      {

      }
    public string GetText( System.Windows.Documents.FlowDocument document )
    {
      TextRange tr = new TextRange( document.ContentStart, document.ContentEnd );
      using( MemoryStream ms = new MemoryStream() )
      {
        tr.Save( ms, DataFormats.Xaml );
          
        return ASCIIEncoding.UTF8.GetString(ms.ToArray());
      }
    }

    public void SetText( System.Windows.Documents.FlowDocument document, string text )
    {
      try
      {
        //if the text is null/empty clear the contents of the RTB. If you were to pass a null/empty string
        //to the TextRange.Load method an exception would occur.
        if( String.IsNullOrEmpty( text ) )
        {
          document.Blocks.Clear();
        }
        else
        {
          TextRange tr = new TextRange( document.ContentStart, document.ContentEnd );
          tr.ClearAllProperties();
          text = System.Text.RegularExpressions.Regex.Replace(text, "&#x(0?[0-8B-F]|1[0-9A-F]|7F);", "");
          using (MemoryStream ms = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(text)))
          {
            tr.Load( ms, DataFormats.Xaml );
          }
        }
      }
      catch
      {
        //throw new InvalidDataException( "Data provided is not in the correct Xaml format." );
          System.Diagnostics.Debug.WriteLine("Data provided is not in the correct Xaml format. Text is : " );
          System.Diagnostics.Debug.WriteLine(text);
      }
    }
  }
}
