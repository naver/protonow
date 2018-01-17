using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Naver.Compass.Module
{
    public interface ICommandSink
    {
        /// <summary>
        /// Returns true if the specified command can be executed by the command sink.
        /// </summary>
        /// <param name="command">
        /// The command whose execution status is being queried.
        /// </param>
        /// <param name="parameter">
        /// An optional command parameter.
        /// </param>
        /// <param name="handled">
        /// Set to true if there is no need to continue querying for an execution status.
        /// </param>
        bool CanExecuteCommand(ICommand command, object parameter, out bool handled);

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">
        /// The command being executed.
        /// </param>
        /// <param name="parameter">
        /// An optional command parameter.
        /// </param>
        /// <param name="handled">
        /// Set to true if the command has been executed and there is no need for others to respond.
        /// </param>
        void ExecuteCommand(ICommand command, object parameter, out bool handled);
    }
    public class CommandSink : ICommandSink
    {
        #region Data

        readonly Dictionary<ICommand, CommandCallbacks> _commandToCallbacksMap = new Dictionary<ICommand, CommandCallbacks>();

        #endregion // Data

        #region Command Registration

        public void RegisterCommand(ICommand command, Predicate<object> canExecute, Action<object> execute)
        {
            VerifyArgument(command, "command");
            VerifyArgument(canExecute, "canExecute");
            VerifyArgument(execute, "execute");

            _commandToCallbacksMap[command] = new CommandCallbacks(canExecute, execute);
        }

        public void UnregisterCommand(ICommand command)
        {
            VerifyArgument(command, "command");

            if (_commandToCallbacksMap.ContainsKey(command))
                _commandToCallbacksMap.Remove(command);
        }

        #endregion // Command Registration

        #region ICommandSink Members

        public virtual bool CanExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            VerifyArgument(command, "command");

            if (_commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                return _commandToCallbacksMap[command].CanExecute(parameter);
            }
            else
            {
                return (handled = false);
            }
        }

        public virtual void ExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            VerifyArgument(command, "command");

            if (_commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                _commandToCallbacksMap[command].Execute(parameter);
            }
            else
            {
                handled = false;
            }
        }

        #endregion // ICommandSink Members

        #region VerifyArgument

        static void VerifyArgument(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }

        #endregion // VerifyArgument

        #region CommandCallbacks [nested struct]

        private struct CommandCallbacks
        {
            public readonly Predicate<object> CanExecute;
            public readonly Action<object> Execute;

            public CommandCallbacks(Predicate<object> canExecute, Action<object> execute)
            {
                this.CanExecute = canExecute;
                this.Execute = execute;
            }
        }

        #endregion // CommandCallbacks [nested struct]
    }
    public class CommandSinkBinding : CommandBinding
    {
        #region CommandSink [instance property]

        ICommandSink _commandSink;

        public ICommandSink CommandSink
        {
            get { return _commandSink; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Cannot set CommandSink to null.");

                if (_commandSink != null)
                    throw new InvalidOperationException("Cannot set CommandSink more than once.");

                _commandSink = value;

                base.CanExecute += (s, e) =>
                {
                    bool handled;
                    e.CanExecute = _commandSink.CanExecuteCommand(e.Command, e.Parameter, out handled);
                    e.Handled = handled;
                };

                base.Executed += (s, e) =>
                {
                    bool handled;
                    _commandSink.ExecuteCommand(e.Command, e.Parameter, out handled);
                    e.Handled = handled;
                };
            }
        }

        #endregion // CommandSink [instance property]

        #region CommandSink [attached property]

        public static ICommandSink GetCommandSink(DependencyObject obj)
        {
            return (ICommandSink)obj.GetValue(CommandSinkProperty);
        }

        public static void SetCommandSink(DependencyObject obj, ICommandSink value)
        {
            obj.SetValue(CommandSinkProperty, value);
        }

        public static readonly DependencyProperty CommandSinkProperty =
            DependencyProperty.RegisterAttached(
            "CommandSink",
            typeof(ICommandSink),
            typeof(CommandSinkBinding),
            new UIPropertyMetadata(null, OnCommandSinkChanged));

        static void OnCommandSinkChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            ICommandSink commandSink = e.NewValue as ICommandSink;

            if (!ConfigureDelayedProcessing(depObj, commandSink))
                ProcessCommandSinkChanged(depObj, commandSink);
        }

        // This method is necessary when the CommandSink attached property is set on an element 
        // in a template, or any other situation in which the element's CommandBindings have not 
        // yet had a chance to be created and added to its CommandBindings collection.
        static bool ConfigureDelayedProcessing(DependencyObject depObj, ICommandSink commandSink)
        {
            bool isDelayed = false;

            CommonElement elem = new CommonElement(depObj);
            if (elem.IsValid && !elem.IsLoaded)
            {
                RoutedEventHandler handler = null;
                handler = delegate
                {
                    elem.Loaded -= handler;
                    ProcessCommandSinkChanged(depObj, commandSink);
                };
                elem.Loaded += handler;
                isDelayed = true;
            }

            return isDelayed;
        }

        static void ProcessCommandSinkChanged(DependencyObject depObj, ICommandSink commandSink)
        {
            CommandBindingCollection cmdBindings = GetCommandBindings(depObj);
            if (cmdBindings == null)
                throw new ArgumentException("The CommandSinkBinding.CommandSink attached property was set on an element that does not support CommandBindings.");

            foreach (CommandBinding cmdBinding in cmdBindings)
            {
                CommandSinkBinding csb = cmdBinding as CommandSinkBinding;
                if (csb != null && csb.CommandSink == null)
                    csb.CommandSink = commandSink;
            }
        }

        static CommandBindingCollection GetCommandBindings(DependencyObject depObj)
        {
            var elem = new CommonElement(depObj);
            return elem.IsValid ? elem.CommandBindings : null;
        }

        #endregion // CommandSink [attached property]

        #region CommonElement [nested class]

        /// <summary>
        /// This class makes it easier to write code that works 
        /// with the common members of both the FrameworkElement
        /// and FrameworkContentElement classes.
        /// </summary>
        private class CommonElement
        {
            readonly FrameworkElement _fe;
            readonly FrameworkContentElement _fce;

            public readonly bool IsValid;

            public CommonElement(DependencyObject depObj)
            {
                _fe = depObj as FrameworkElement;
                _fce = depObj as FrameworkContentElement;

                IsValid = _fe != null || _fce != null;
            }

            public CommandBindingCollection CommandBindings
            {
                get
                {
                    this.Verify();

                    if (_fe != null)
                        return _fe.CommandBindings;
                    else
                        return _fce.CommandBindings;
                }
            }

            public bool IsLoaded
            {
                get
                {
                    this.Verify();

                    if (_fe != null)
                        return _fe.IsLoaded;
                    else
                        return _fce.IsLoaded;
                }
            }

            public event RoutedEventHandler Loaded
            {
                add
                {
                    this.Verify();

                    if (_fe != null)
                        _fe.Loaded += value;
                    else
                        _fce.Loaded += value;
                }
                remove
                {
                    this.Verify();

                    if (_fe != null)
                        _fe.Loaded -= value;
                    else
                        _fce.Loaded -= value;
                }
            }

            void Verify()
            {
                if (!this.IsValid)
                    throw new InvalidOperationException("Cannot use an invalid CommmonElement.");
            }
        }

        #endregion 
    }
}