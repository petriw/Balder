using System.ComponentModel;
using System.Reflection;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.BarChart
{
	public class BarChartValue : INotifyPropertyChanged
	{
		private readonly object _element;
		private readonly string _valueMember;

		private PropertyInfo _propertyInfo;
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		public BarChartValue(object element, string valueMember)
		{
			_element = element;
			_valueMember = valueMember;
			_propertyInfo = element.GetType().GetProperty(valueMember);
			if( element is INotifyPropertyChanged )
			{
				((INotifyPropertyChanged) element).PropertyChanged += ElementPropertyChanged;
			}
			GetValueFromSourceAndSetToValue();
		}

		private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if( e.PropertyName.Equals(_valueMember) )
			{
				GetValueFromSourceAndSetToValue();
			}
		}

		private void GetValueFromSourceAndSetToValue()
		{
			var value = (double)_propertyInfo.GetValue(_element, null);
			Value = value;
		}




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