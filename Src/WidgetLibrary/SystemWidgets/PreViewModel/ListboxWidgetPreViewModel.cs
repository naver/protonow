using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class ListboxWidgetPreViewModel : WidgetPreViewModeBase
    {
        public ListboxWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
            //_model = new WidgetModel(widget);
            ItemsList = new ObservableCollection<NodeViewModel>();
            LoadList();
        }

        #region Binding Propery
        public ObservableCollection<NodeViewModel> ItemsList { get; set; }
        public void LoadList()
        {
            ItemsList.Clear();
            foreach (IListItem item in (_model.WdgDom as IListBase).Items)
            {
                ItemsList.Add(new NodeViewModel(item.TextValue, item.IsSelected));
            }
        }
        #endregion

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            UpdateTextStyle();
            UpdateFontStyle();
            UpdateBackgroundStyle();
        }
        #endregion 
    }
}
