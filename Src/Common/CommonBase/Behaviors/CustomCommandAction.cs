using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Naver.Compass.Common.CommonBase
{
    public sealed class CustomCommandAction : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CustomCommandAction), null);

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(CustomCommandAction), null);

        public static readonly DependencyProperty PassEventArgsProperty = DependencyProperty.Register(
            "PassEventArgs", typeof(bool), typeof(CustomCommandAction), new PropertyMetadata(false));
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }

            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }

        public bool PassEventArgs
        {
            get
            {
                return (bool)this.GetValue(PassEventArgsProperty);
            }

            set
            {
                this.SetValue(PassEventArgsProperty, value);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject != null)
            {
                ICommand command = this.Command;
                if (command != null)
                {
                    if (!PassEventArgs)
                    {
                        if (command.CanExecute(this.CommandParameter))
                        {
                            command.Execute(this.CommandParameter);
                        }
                    }
                    else
                    {
                        var passParamters = new object[] { this.CommandParameter, parameter };
                        if (command.CanExecute(passParamters))
                        {
                            command.Execute(passParamters);
                        }
                    }
                }
            }
        }
    }

    public class SetterAction : TargetedTriggerAction<FrameworkElement>
    {
        public DependencyProperty Property { get; set; }
        public Object Value { get; set; }


        protected override void Invoke(object parameter)
        {
            if (SetterElement != null)
            {
                SetterElement.SetValue(Property, Value);
            }
            else
            {
                AssociatedObject.SetValue(Property, Value);
            }
        }

        #region SetterElement

        public static readonly DependencyProperty SetterElementProperty =
            DependencyProperty.Register("SetterElement", typeof(FrameworkElement), typeof(SetterAction),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None));

        public FrameworkElement SetterElement
        {
            get { return (FrameworkElement)GetValue(SetterElementProperty); }
            set { SetValue(SetterElementProperty, value); }
        }

        #endregion

        
    }
}
