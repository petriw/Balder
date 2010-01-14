using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Balder.Silverlight.MVVM
{
	public class CommandManager
	{
		public static readonly CommandManager Instance = new CommandManager();

		
		private Dictionary<UIElement, CommandSubscription> _subscriptions;

		private CommandManager()
		{
			_subscriptions = new Dictionary<UIElement, CommandSubscription>();
		}

		private CommandSubscription GetOrCreateSubscription(UIElement element)
		{
			CommandSubscription subscription = null;
			if( !_subscriptions.ContainsKey(element) )
			{
				subscription = new CommandSubscription(element);
				_subscriptions[element] = subscription;
			} else
			{
				subscription = _subscriptions[element];
			}
			return subscription;
		}


		public void SetCommandForUIElement(UIElement element, ICommand command)
		{
			var subscription = GetOrCreateSubscription(element);
			subscription.Command = command;
			HookupExecuteEvent(subscription);
			HookUpCanExecuteEvent(subscription);
			SetCanExecute(subscription);
		}

		public void SetCommandParameterForUIElement(UIElement element, object parameter)
		{
			var subscription = GetOrCreateSubscription(element);
			subscription.CommandParameter = parameter;
			SetCanExecute(subscription);
		}

		private void SetCanExecute(CommandSubscription subscription)
		{
			subscription.UIElement.Dispatcher.BeginInvoke(
				() =>
					{
						if (subscription.UIElement is Control)
						{
							((Control) subscription.UIElement).IsEnabled = subscription.Command.CanExecute(subscription.CommandParameter);
						}
						else if (subscription.UIElement is ICanBeEnabled)
						{
							((ICanBeEnabled) subscription.UIElement).IsEnabled =
								subscription.Command.CanExecute(subscription.CommandParameter);
						}
					});
		}

		private void HookUpCanExecuteEvent(CommandSubscription subscription)
		{
			subscription.Command.CanExecuteChanged +=
				(s, e) => SetCanExecute(subscription);
		}



		private void HookupExecuteEvent(CommandSubscription subscription)
		{
			if( subscription.UIElement is ButtonBase )
			{
				HookupExecuteEventForButtonBase(subscription);	
			} else if( subscription.UIElement is TextBox )
			{
				HookupExecuteEventForTextBox(subscription);
			} else if( subscription.UIElement is ICanExecuteCommand )
			{
				HookupExecuteEventForRoutedCommand(subscription);
			} else
			{
				HookupExceuteEventForLeftMouseButtonUp(subscription);
			}
		}


		private void HookupExecuteEventForButtonBase(CommandSubscription subscription)
		{
			var button = subscription.UIElement as ButtonBase;
			button.Click += (s,e) => subscription.Execute();
		}

		private void HookupExecuteEventForTextBox(CommandSubscription subscription)
		{
			var textBox = subscription.UIElement as TextBox;
			textBox.TextChanged += (s, e) => subscription.Execute();
		}

		private void HookupExceuteEventForLeftMouseButtonUp(CommandSubscription subscription)
		{
			subscription.UIElement.MouseLeftButtonDown += (s, e) => subscription.Execute();
			
		}

		private void HookupExecuteEventForRoutedCommand(CommandSubscription subscription)
		{
			var routedCommand = subscription.UIElement as ICanExecuteCommand;
			routedCommand.Command += (s, e) => subscription.Execute();
		}
	}
}