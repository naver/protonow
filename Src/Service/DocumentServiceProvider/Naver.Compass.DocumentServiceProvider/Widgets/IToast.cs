using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum ToastCloseSetting
    {
        ExposureTime,
        CloseButton,
        AreaTouch
    }

    public enum ToastDisplayPosition
    {
        UserSetting,
        Top
    }

    public interface IToast : IPageEmbeddedWidget
    {
        IToastPage ToastPage { get; }

        int ExposureTime { get; set; }

        ToastCloseSetting CloseSetting { get; set; }

        ToastDisplayPosition DisplayPosition { get; set; }
    }
}
