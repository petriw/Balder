using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Balder.Core.Helpers;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
{
	public partial class PieChart
	{
		private Dictionary<object, PieChartValue> _hashedValues;
		private ObservableCollection<PieChartValue> _values;

		public PieChart()
		{
			InitializeComponent();

			_values = new ObservableCollection<PieChartValue>();
			_hashedValues = new Dictionary<object, PieChartValue>();
			NodesControl.ItemsSource = _values;

		}

		public static readonly DependencyProperty<PieChart, IEnumerable> ValuesSourceProperty =
			DependencyProperty<PieChart, IEnumerable>.Register(b => b.ValuesSource);
		public IEnumerable ValuesSource
		{
			get { return ValuesSourceProperty.GetValue(this); }
			set
			{
				PrepareValueCollection(value, ValuesSource);
				ValuesSourceProperty.SetValue(this, value);
				AdjustPieChartElements();
			}
		}

		public static readonly DependencyProperty<PieChart, string> ValueMemberProperty =
			DependencyProperty<PieChart, string>.Register(b => b.ValueMember);
		public string ValueMember
		{
			get { return ValueMemberProperty.GetValue(this); }
			set { ValueMemberProperty.SetValue(this, value); }
		}


		private void PrepareValueCollection(IEnumerable valuesSource, IEnumerable previousValuesSource)
		{
			if (null == valuesSource)
			{
				return;
			}
			if (null != previousValuesSource && previousValuesSource is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged)valuesSource).CollectionChanged -= ValuesCollectionChanged;
			}
			if (valuesSource is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged)valuesSource).CollectionChanged += ValuesCollectionChanged;
			}

			_values.Clear();
			AddValues(valuesSource);
		}

		private void ValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						AddValues(e.NewItems);

					}
					break;

				case NotifyCollectionChangedAction.Reset:
					{
						_values.Clear();
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					{

					}
					break;
			}
			AdjustPieChartElements();
		}

		private void AddValues(IEnumerable enumerable)
		{
			foreach (var valueItem in enumerable)
			{
				AddValue(valueItem);
			}
		}


		private void AddValue(object element)
		{
			var pieChartValue = new PieChartValue();

			if( element is INotifyPropertyChanged )
			{
				((INotifyPropertyChanged) element).PropertyChanged += (s, e) =>
				                                                      	{
																			if( e.PropertyName == ValueMember )
																			{
																				AdjustPieChartElements();
																			}
				                                                      	};
			}

			_values.Add(pieChartValue);
			_hashedValues[element] = pieChartValue;
		}


		private double GetValueFromElement(object element)
		{
			var property = element.GetType().GetProperty(ValueMember);
			if( null != property )
			{
				var value = property.GetValue(element, null);
				var valueAsDouble = double.Parse(value.ToString(), CultureInfo.InvariantCulture);
				return valueAsDouble;
			}

			return 0;
		}		


		private void AdjustPieChartElements()
		{
			var total = 0d;
			foreach (var valueElement in ValuesSource)
			{
				var value = GetValueFromElement(valueElement);
				total += value;
			}

			var multiplier = 360d/total;

			if( multiplier > 0 )
			{
				var startAngle = 0d;
				foreach (var valueElement in ValuesSource)
				{
					var value = GetValueFromElement(valueElement);
					var angleCoverage = value*multiplier;
					var endAngle = startAngle + angleCoverage;
					if( endAngle > 360 )
					{
						endAngle = 360;
					}
					var pieChartValue = _hashedValues[valueElement];

					if( endAngle < pieChartValue.StartAngle )
					{
						pieChartValue.StartAngle = startAngle;
						pieChartValue.EndAngle = endAngle;
					} else
					{
						pieChartValue.EndAngle = endAngle;
						pieChartValue.StartAngle = startAngle;
						
					}

					

					startAngle += angleCoverage;
				}
			}
		}
	}
}
