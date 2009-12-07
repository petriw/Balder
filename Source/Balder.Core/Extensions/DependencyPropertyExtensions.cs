using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.ComponentModel;
using Balder.Silverlight.Extensions;
using DependencyPropertyHelper = Balder.Core.Helpers.DependencyPropertyHelper;

namespace Balder.Core.Extensions
{
	public static class DependencyPropertyExtensions
	{
		public static void SetValue<T>(this DependencyObject obj, DependencyProperty property, T value)
		{
			object oldValue = obj.GetValue(property);
			if (null != oldValue && null != value)
			{
				if (oldValue.Equals(value))
				{
					return;
				}
			}
			obj.SetValue(property, value);
		}

		public static T GetValue<T>(this DependencyObject obj, DependencyProperty property)
		{
			return (T)obj.GetValue(property);
		}

		public static DependencyProperty GetDependencyPropertyByPropertyName(this DependencyObject obj, string propertyName)
		{
			var dependencyPropertyName = string.Format("{0}Property", propertyName);
			var dependencyProperty = obj.GetType().GetField(dependencyPropertyName, BindingFlags.Static | BindingFlags.Public);
			if( null == dependencyProperty )
			{
				throw new ArgumentException("Property '"+propertyName+"' does not exist");
			}
			if( !dependencyProperty.FieldType.Equals(typeof(DependencyProperty)) )
			{
				throw new ArgumentException("Property '"+propertyName+"' is not a DependencyProperty");
			}

			return dependencyProperty.GetValue(null) as DependencyProperty;
		}


		private static readonly Dictionary<DependencyObject, IDependencyPropertySubscription> ExpressionSubscriptions = new Dictionary<DependencyObject, IDependencyPropertySubscription>();

		public static void AddChangeHandler<T>(this T element, DependencyProperty dependencyProperty, PropertyChangedHandler<T> handler)
			where T : FrameworkElement
		{

			var key = element; 
			IDependencyPropertySubscription subscription = null;
			if (ExpressionSubscriptions.ContainsKey(key))
			{
				subscription = ExpressionSubscriptions[key];
			}
			else
			{
				subscription = new DependencyPropertySubscription<T>((T)element, dependencyProperty);
				ExpressionSubscriptions[key] = subscription;
			}

			((INotifyPropertyChanged) subscription).SubscribeToChange(() => subscription.Value,  o => handler((T)element));
		}


		public static void AddChangeHandler<T>(this T element, Expression<Func<object>> expression, PropertyChangedHandler<T> handler)
			where T : FrameworkElement
		{
			string propertyPath;
			var propertyInfo = expression.GetPropertyInfo();
			var dependencyProperty = DependencyPropertyHelper.GetDependencyPropertyByPropertyName(element, propertyInfo.Name, out propertyPath);
			AddChangeHandler(element,dependencyProperty,handler);
		}
	}
}