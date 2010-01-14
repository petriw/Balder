using System;
using System.Windows;
using System.Windows.Input;

namespace Balder.Silverlight.MVVM
{
	public class CommandSubscription
	{
		internal CommandSubscription(UIElement element)
		{
			UIElement = element;
		}

		public UIElement UIElement { get; private set; }
		public ICommand Command { get; internal set; }
		public object CommandParameter { get; internal set; }

		public bool IsValid
		{
			get
			{
				return (null != Command);
			}
		}


		public void Execute()
		{
			if( !IsValid )
			{
				throw new ArgumentException("Command is not valid");
			}

			if( Command.CanExecute(CommandParameter))
			{
				Command.Execute(CommandParameter);
			}
		}
	}
}