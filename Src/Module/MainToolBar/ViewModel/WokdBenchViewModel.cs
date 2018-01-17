using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Xceed.Wpf.AvalonDock.Themes;

using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using MainToolBar.Common;

namespace MainToolBar.ViewModel
{
    class WokdBenchViewModel : NotificationObject
    {

        private IEventAggregator eventAggregation;
        private SubscriptionToken subscriptionToken;

        public WokdBenchViewModel()
        {
            //eventAggregation = eventAggregate;

            //ThemesEvent fundAddedEvent = eventAggregation.GetEvent<ThemesEvent>();

            //if (subscriptionToken != null)
            //{
            //    fundAddedEvent.Unsubscribe(subscriptionToken);
            //}

            //subscriptionToken = fundAddedEvent.Subscribe(ThemesChangedEventHandler, ThreadOption.UIThread);

  
        }

        public void ThemesChangedEventHandler(string strTheme)
        {
            switch(strTheme)
            {
                case "Light":
                    DockThemes = new ExpressionLightTheme();
                    break;
                case "Dark":
                    DockThemes = new ExpressionDarkTheme();
                    break;

            }
        }

        private Theme _dockThemes;
        public Theme DockThemes
        {
            get
            {
                //_dockThemes = new GenericTheme();
                return _dockThemes;
            }

            set
            {
                if (_dockThemes != value)
                {
                
                    OnPropertyChanged(new PropertyChangedEventArgs("DockThemes"));
                }
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
