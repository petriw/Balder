using System.ComponentModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
{
	public class BusinessObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private double _value;
		public double Value
		{
			get { return _value; }
			set
			{
				_value = value;
				OnPropertyChanged("Value");
			}
		}

		private void OnPropertyChanged(string property)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(property));

		}
	}
}
