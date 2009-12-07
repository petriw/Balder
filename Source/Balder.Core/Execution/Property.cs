using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows;
using Balder.Core.Helpers;
using Balder.Core.Extensions;

namespace Balder.Core.Execution
{
	public class Property<T,TP>
		where T:DependencyObject
	{
		private readonly DependencyProperty<T, TP> _internalDependencyProperty;
		private readonly Expression<Func<T, TP>> _expression;
		private readonly string _propertyName;
		private readonly Dictionary<object, TP> _valueHash;
		private readonly bool _canNotiify;
		

		private Property(Expression<Func<T, TP>> expression)
		{
			_internalDependencyProperty = DependencyProperty<T, TP>.Register(expression);
			_expression = expression;
			_valueHash = new Dictionary<object, TP>();

			var ownerType = typeof (T);

			if ( null != ownerType.GetInterface(typeof(ICanNotifyChanges).Name,false))
			{
				_canNotiify = true;
			} else
			{
				_canNotiify = false;
			}
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

			var oldValue = default(TP);
			if( _valueHash.ContainsKey(obj))
			{
				oldValue = _valueHash[obj];
			}
			
			NotifyChanges(obj, oldValue, value);
			_valueHash[obj] = value;
		}

		public TP GetValue(T obj)
		{
			var value = default(TP);
			if (_valueHash.ContainsKey(obj))
			{
				value = _valueHash[obj];
			}
			return value;
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
				((ICanNotifyChanges)obj).Notify(_propertyName,oldValue,value);
			}
		}
	}
}
