using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Balder.Core.Helpers;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.BarChart
{
	public partial class BarChart
	{
		private Dictionary<object, BarChartValue> _hashedValues;
		private ObservableCollection<BarChartValue> _values;

		public BarChart()
		{
			InitializeComponent();

			_values = new ObservableCollection<BarChartValue>();
			_hashedValues = new Dictionary<object, BarChartValue>();
			NodesStack.ItemsSource = _values;
		}

		public static readonly DependencyProperty<BarChart, IEnumerable> ValuesSourceProperty =
			DependencyProperty<BarChart, IEnumerable>.Register(b => b.ValuesSource);
		public IEnumerable ValuesSource
		{
			get { return ValuesSourceProperty.GetValue(this); }
			set
			{
				PrepareValueCollection(value, ValuesSource);
				ValuesSourceProperty.SetValue(this, value);
				AdjustBarChartElements();
			}
		}

		public static readonly DependencyProperty<BarChart, string> ValueMemberProperty =
			DependencyProperty<BarChart, string>.Register(b => b.ValueMember);
		public string ValueMember
		{
			get { return ValueMemberProperty.GetValue(this); }
			set { ValueMemberProperty.SetValue(this, value); }
		}


		private void PrepareValueCollection(IEnumerable valuesSource, IEnumerable previousValuesSource)
		{
			if( null == valuesSource )
			{
				return;
			}
			if (null != previousValuesSource && previousValuesSource is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged)valuesSource).CollectionChanged -= ValuesCollectionChanged;
			}
			if (valuesSource is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged) valuesSource).CollectionChanged += ValuesCollectionChanged;
			}

			_values.Clear();
			AddValues(valuesSource);
		}

		private void ValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch( e.Action )
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
			AdjustBarChartElements();
		}

		private void AddValues(IEnumerable enumerable)
		{
			foreach( var valueItem in enumerable )
			{
				AddValue(valueItem);
			}
		}


		private void AddValue(object element)
		{
			var barChartValue = new BarChartValue(element, ValueMember);
			_values.Add(barChartValue);
			_hashedValues[element] = barChartValue;
		}

		private void AdjustBarChartElements()
		{
			BackgroundContainer.Scale.X = _values.Count;
			BackgroundContainer.Position.X = (_values.Count * 5)-5;
			NodesStack.Position.X = -(_values.Count * 5);
		}
	}
}
