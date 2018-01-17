using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Naver.Compass.InfoStructure
{
    public interface ICustomWidget
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string LbrType { get; set; }
        string Type { get; set; }
        string Icon { get; set; }
        string SvgIcon { get; set; }
        string LocalizedName { get; set; }
        string ToolTip { get; set; }
        string[] Tags { get; set; }
        string NClickCode { get; set; }


        void LocalizedTextChanged();
        WidgetModelType EnumType { get; }
        string LocalizedText { get; }
        InlineContent InlineContent { get; }
        void Search(string searchTxt);
        bool IsSvg { get; }
        ImageSource ImageSource { get; }
        bool IsFavourite { get; set; }
        bool IsVisible { get; set; }
        bool IsClickonFavourite { get; set; }
        void FavouriteExecute(object cmdParameter);
        void ClickTypeExecute(object cmdparameter);

        //DelegateCommand<object> FavouriteCommand { get;  set; }
        //DelegateCommand<object> ClickTypeCommand { get;  set; }
    }

    public enum WidgetModelType
    {
        image = 0,
        lable = 1,
        link = 2,
        rectangle = 3,
        roundedrectangle = 4,
        triangle = 5,
        circle = 6,
        diamand = 7,
        horizontalline = 8,
        verticalline = 9,
        listbox = 10,
        droplist = 11,
        htmlbutton = 12,
        checkbox = 13,
        radiobutton = 14,
        textarea = 15,
        textfield = 16,
        swipeviews = 17,
        hamburgermenu = 18,
        toastnotification = 19,
        svg = 20,
        custom = 21,
    }
}
