using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Naver.Compass.Service.Document;
using System.Reflection;

namespace Naver.Compass.Service.Html
{
    internal abstract class JsStreamWidget : JsWidget
    {
        public JsStreamWidget(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            SaveStreamToFile();
        }

        private void SaveStreamToFile()
        {
            IStreamWidget _streamWidget = _widget as IStreamWidget;
            Stream stream = _streamWidget.DataStream;
            if (stream != null)
            {
                _hash = _service.ImagesStreamManager.SetConsumerStream(_streamWidget.Guid, Guid.Empty, stream, StreamType);
            }
            else
            {
                // Use default resource
                if (!String.IsNullOrEmpty(DefaultResourceName))
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    using (stream = assembly.GetManifestResourceStream(DefaultResourceName))
                    {
                        _hash = _service.ImagesStreamManager.SetConsumerStream(_streamWidget.Guid, Guid.Empty, stream, DefaultStreamType);
                    }
                }
            }
        }

        protected abstract string StreamType
        {
            get;
        }

        protected abstract string DefaultResourceName
        {
            get;
        }

        protected abstract string DefaultStreamType
        {
            get;
        }

        protected string _hash;
    }
}
