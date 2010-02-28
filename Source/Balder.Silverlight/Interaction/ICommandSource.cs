using System.Windows.Input;

namespace Balder.Silverlight.Interaction
{
	public interface ICommandSource
	{
		ICommand Command { get; set; }
		object CommandParameter { get; set; }
	}
}
