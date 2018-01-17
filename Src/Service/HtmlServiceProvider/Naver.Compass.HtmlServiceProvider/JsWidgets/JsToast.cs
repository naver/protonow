using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsToast : JsWidget
    {
        public JsToast(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            IToast toast = _widget as IToast;
            bool bTouchClose = false;
            int exposureTime = 0;
            switch (toast.CloseSetting)
            {
                case ToastCloseSetting.ExposureTime:
                    exposureTime =  toast.ExposureTime * 1000; // ExposureTime is in milliseconds in js.
                    bTouchClose = false;
                    break;
                case ToastCloseSetting.CloseButton:
                    exposureTime = 0;
                    bTouchClose = false;
                    break;
                case ToastCloseSetting.AreaTouch:
                    exposureTime = 0;
                    bTouchClose = true;
                    break;
                default:
                    break;
            }

            builder.AppendFormat("\"exposureTime\":{0},", exposureTime);
            builder.AppendFormat("\"bTouchClose\":{0},", bTouchClose.ToString().ToLower());

            bool isClosedPage = false;
            if (!toast.ToastPage.IsOpened)
            {
                isClosedPage = true;
                toast.ToastPage.Open();
            }

            if(IsSetMD5==true)
            {
                builder.AppendFormat("\"Content\":\"{0}\",", toast.ToastPage.MD5);
            }

            builder.Append("\"toastWidgets\":[");
            foreach (IWidget widget in toast.ToastPage.Widgets)
            {
                JsWidget jsWidget = JsWidgetFactory.CreateJsWidget(_service, widget, IsSetMD5);
                builder.Append(jsWidget.ToString());
                builder.Append(",");
            }

            if (isClosedPage)
            {
                toast.ToastPage.Close();
            }

            JsHelper.RemoveLastComma(builder);
            builder.Append("],");
        }
    }
}
