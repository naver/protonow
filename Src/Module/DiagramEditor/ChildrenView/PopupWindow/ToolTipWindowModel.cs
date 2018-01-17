using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    public class ToolTipWindowModel:ViewModelBase
    {
        public ToolTipWindowModel()
        {
            OKCommand = new DelegateCommand<object>(OKExecute);
            CancelCommand = new DelegateCommand<object>(CancelExecute);
            EnterCommand = new DelegateCommand<object>(EnterExecute);
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();

            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();


            string tooltip = (wdgs[0] as WidgetViewModelDate).Tooltip;
            ToolTipText = tooltip;
        }

        public DelegateCommand<object> OKCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }
        public DelegateCommand<object> EnterCommand { get; set; }

        private string toolTipText;
        public string ToolTipText
        {
            get
            {
                return toolTipText;
            }
            set
            {
                if(toolTipText!=value)
                {
                    toolTipText = value;
                    FirePropertyChanged("ToolTipText");
                }
            }
        }

        private void OKExecute(object obj)
        {
            _ListEventAggregator.GetEvent<EditTooltipEvent>().Publish(ToolTipText);

            Window win = obj as Window;
            win.Close();
        }
        private void CancelExecute(object obj)
        {
            Window win = obj as Window;
            win.Close();
        }
        private void EnterExecute(object obj)
        {
            _ListEventAggregator.GetEvent<EditTooltipEvent>().Publish(ToolTipText);

            Window win = obj as Window;
            win.Close();
        }

        private ISelectionService _selectionService;
    }
}
