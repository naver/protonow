using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IViewport
    {
        bool IncludeViewportTag { get; set; }
        string Name { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }

    public enum ExportType
    {
        ImageFile,
        Data
    }

    public interface IHtmlGeneratorConfiguration : IGeneratorConfiguration
    {
        // Gerneral
        string OutputFolder { get; set; }

        // Mobile/Device
        IViewport Viewport { get; }
        bool GenerateMobileFiles { get; set; }

        ExportType ExportType { get; set; }

        Guid StartPage { get; set; }

        Guid CurrentPage { get; set; }
    }
}
