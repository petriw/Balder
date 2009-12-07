namespace Balder.Core.Execution
{
	public interface ICanNotifyChanges
	{
		void Notify(string propertyName, object oldValue, object newValue);
	}
}
