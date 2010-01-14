using System.Windows;

namespace Balder.Silverlight.MVVM
{
	public interface ICanExecuteCommand
	{
		event RoutedEventHandler Command;
	}
}