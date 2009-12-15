#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;

namespace Balder.Core.Extensions
{
	public delegate void PropertyChangedHandler<T>(T sender);

	public static class NotificationExtensions
	{
		public static void Notify(this PropertyChangedEventHandler eventHandler, Expression<Func<object>> expression)
		{
			if (null == eventHandler)
			{
				return;
			}

			var memberExpression = expression.GetMemberExpression();
			var constantExpression = memberExpression.Expression as ConstantExpression;
			var propertyInfo = memberExpression.Member as PropertyInfo;

			foreach (var del in eventHandler.GetInvocationList())
			{
				try
				{
					del.DynamicInvoke(new object[] { constantExpression.Value, new PropertyChangedEventArgs(propertyInfo.Name) });
				}
				catch
				{
				}
			}
		}

		public static void SubscribeToChange<T>(this T objectThatNotifies, Expression<Func<object>> expression, PropertyChangedHandler<T> handler)
			where T : INotifyPropertyChanged
		{
			var thread = Thread.CurrentThread;
			objectThatNotifies.PropertyChanged +=
				(s, e) =>
					{
						var memberExpression = expression.GetMemberExpression();
						var propertyInfo = memberExpression.Member as PropertyInfo;

						if (e.PropertyName.Equals(propertyInfo.Name))
						{
							var dispatcher = Application.Current.RootVisual.Dispatcher;
							if (null == dispatcher)
							{
								handler(objectThatNotifies);
							}
							else
							{
								dispatcher.BeginInvoke(handler);
							}
						}
					};
		}


		public static void SubscribeToChange(this INotifyPropertyChanged objectThatNotifies, Expression<Func<object>> expression, PropertyChangedHandler<INotifyPropertyChanged> handler)
		{
			SubscribeToChange<INotifyPropertyChanged>(objectThatNotifies, expression, handler);
		}

		public static void SubscribeToChange(this INotifyPropertyChanged objectThatNotifies, string propertyName, PropertyChangedHandler<INotifyPropertyChanged> handler)
		{
			objectThatNotifies.PropertyChanged +=
				(s, e) =>
					{
						if (e.PropertyName.Equals(propertyName))
						{
							handler(objectThatNotifies);
						}
					};
		}
	}
}