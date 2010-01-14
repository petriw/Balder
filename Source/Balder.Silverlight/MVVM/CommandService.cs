using System.Windows;
using System.Windows.Input;

namespace Balder.Silverlight.MVVM
{
	public static class CommandService
	{
		#region Command Property
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached(
				"Command",
				typeof(ICommand),
				typeof(CommandService),
				new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));

		public static void OnCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			SetCommand(obj, (ICommand)e.NewValue);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
			CommandManager.Instance.SetCommandForUIElement(obj as UIElement,value);
		}

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}
		#endregion

		#region CommandParameter Property
		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.RegisterAttached(
				"CommandParameter",
				typeof(object),
				typeof(CommandService),
				new PropertyMetadata(new PropertyChangedCallback(OnCommandParameterChanged)));

		public static void OnCommandParameterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			SetCommandParameter(obj, (object)e.NewValue);
		}

		public static void SetCommandParameter(DependencyObject obj, object value)
		{
			obj.SetValue(CommandParameterProperty, value);
			CommandManager.Instance.SetCommandParameterForUIElement(obj as UIElement, value);
		}

		public static object GetCommandParameter(DependencyObject obj)
		{
			return (object)obj.GetValue(CommandParameterProperty);
		}
		#endregion
	}
}