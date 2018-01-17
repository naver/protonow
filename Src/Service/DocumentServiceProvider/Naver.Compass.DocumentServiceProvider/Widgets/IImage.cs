using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    public enum ImageType
    {
        BMP,
        GIF,
        JPG,
        PNG,
        ICO,
        SVG
    }

    public interface IImage : IStreamWidget
    {
        ImageType ImageType { get; set; }

        Stream ImageStream { get; set; }
    }
}
