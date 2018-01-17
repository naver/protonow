using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Naver.Compass.Module
{
    /// <summary>
    /// menuitem for contextmenu
    /// </summary>
    public class ContextMenuItem : MenuItem
    {
        public ContextMenuItem(string header)
            : this(header, null, null, string.Empty, string.Empty)
        {

        }

        public ContextMenuItem(string header, ICommand cmd)
            : this(header, cmd, null, string.Empty, string.Empty)
        {

        }

        public ContextMenuItem(string header, ICommand cmd, object cmdParameter)
            : this(header, cmd, cmdParameter, string.Empty, string.Empty)
        {

        }

        public ContextMenuItem(string header, string hotkey, ICommand cmd)
            : this(header, cmd, null, string.Empty, hotkey)
        {
        }

        public ContextMenuItem(string header, ICommand cmd, string imageResourceKey, string hotkey)
            : this(header, cmd, null, imageResourceKey, hotkey)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="header">MenuItem header text</param>
        /// <param name="cmd"></param>
        /// <param name="imagePath">get image resource from /Naver.Compass.Module.MainToolBar;component/Resource/Shared.xaml</param>
        public ContextMenuItem(string header, ICommand cmd, object cmdParameter, string imageResourceKey, string hotkey)
        {
            this.Header = header;
            this.Command = cmd;
            this.CommandParameter = cmdParameter;
            
           // this.Icon = new Image { Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)) };
            if(imageResourceKey!=string.Empty)
            {
                this.Icon = Application.Current.Resources[imageResourceKey];
            }
            if(hotkey!=string.Empty)
            {
                this.InputGestureText = hotkey;
            }
            this.Style = Application.Current.Resources["topLevel"] as Style;
        }
    }
}
