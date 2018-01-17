using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainToolBar.ViewModel
{
    public class GalleryItemData : ControlData
    {
    }

    public class GalleryuriData : ControlData
    {
        public GalleryuriData(string uriString, UriKind uriKind,object ExternInfo)
        {
            _uri = new Uri(uriString,uriKind);
            _externInfo = ExternInfo;

        }

        private Uri _uri;
        private Object _externInfo;
    }

}
