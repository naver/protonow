using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    public interface IHtmlServiceProvider
    {
        IHtmlHashStreamManager ImagesStreamManager { get; }

        void Render(IDocument doc, string outputFolder = null,bool ByMD5=false);

        void RenderDifferInfo(List<IDocument> Docs, string outputFolder = null);

        string StartHtmlFile { get; }

        string StartPageName { get; }

        bool IsHtmlGenerating { get; set; }
    }
}
