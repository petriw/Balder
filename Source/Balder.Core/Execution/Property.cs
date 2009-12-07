using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using Balder.Core.Helpers;
using Balder.Core.Extensions;

namespace Balder.Core.Execution
{
	public class Property<T,TP>
		where T:DependencyObject, INotifyPropertyChanged
	{
		private readonly DependencyProperty<T, TP> _internalDependencyProperty;
		private readonly Expression<Func<T, TP>> _expression;
		private readonly FieldInfo _propertyChangedEvent;
		private readonly string _propertyName;
		private TP _value;
		

		private Property(Expression<Func<T, TP>> expression)
		{
			_internalDependencyProperty = DependencyProperty<T, TP>.Register(expression);
			_expression = expression;

			var type = typeof (T);
			_propertyChangedEvent = type.GetField("PropertyChanged",BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);
			var propertyInfo = expression.GetPropertyInfo();
			_propertyName = propertyInfo.Name;
		}

		public DependencyProperty ActualDependencyProperty { get { return _internalDependencyProperty.ActualDependencyProperty; } }

		public static Property<T, TP> Register(Expression<Func<T,TP>> expression)
		{
			var property = new Property<T, TP>(expression);
			return property;
		}

		public void SetValue(T obj, TP value)
		{
			if( obj.Dispatcher.CheckAccess())
			{
				_internalDependencyProperty.SetValue(obj, value);	
			} else
			{
				obj.Dispatcher.BeginInvoke(() => _internalDependencyProperty.SetValue(obj, value));
			}
			NotifyChanges(obj, _value, value);
			_value = value;
		}

		public TP GetValue(T obj)
		{
			return _value;
		}

		private void NotifyChanges(T obj, TP oldValue, TP value)
		{
			var oldValueAsObject = (object) oldValue;
			var valueAsObject = (object) value;

			var notify = (null == oldValueAsObject && null != valueAsObject) ||
						 (null != oldValueAsObject && null == valueAsObject) ||
						 (null != oldValueAsObject && !oldValueAsObject.Equals(valueAsObject));

			if (notify)
			{
				var propertyChanged = _propertyChangedEvent.GetValue(obj) as PropertyChangedEventHandler;
				if (null != propertyChanged)
				{
					foreach (var handler in propertyChanged.GetInvocationList())
					{
						handler.Method.Invoke(handler.Target, new object[] {value, new PropertyChangedEventArgs(_propertyName)});
					}
				}
			}
		}
	}
}
