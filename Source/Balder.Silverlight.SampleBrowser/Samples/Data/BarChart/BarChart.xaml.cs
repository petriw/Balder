using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
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
			BackgroundContainer.Scale.X = _values.Count;
			NodesStack.Position.X = -(_values.Count*5);
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
	}

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
