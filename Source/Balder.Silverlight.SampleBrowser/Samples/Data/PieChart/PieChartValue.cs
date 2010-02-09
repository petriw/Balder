using System.ComponentModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
{
	public class PieChartValue : INotifyPropertyChanged
	{
		private double _startAngle;
		public double StartAngle
		{
			get { return _startAngle; }
			set
			{
				_startAngle = value;
				OnPropertyChanged("StartAngle");
			}
		}

		private double _endAngle;
		public double EndAngle
		{
			get { return _endAngle; }
			set
			{
				_endAngle = value;
				OnPropertyChanged("EndAngle");
			}
		}

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
	}
}
