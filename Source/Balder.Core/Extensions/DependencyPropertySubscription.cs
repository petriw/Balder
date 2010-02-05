#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Balder.Core.Extensions;
using Balder.Core.Helpers;
using DependencyProperty = System.Windows.DependencyProperty;

namespace Balder.Silverlight.Extensions
{
	public interface IDependencyPropertySubscription : INotifyPropertyChanged
	{
		object Value { get; set; }
	}

	public class DependencyPropertySubscription<T> : FrameworkElement, IDependencyPropertySubscription
		where T:FrameworkElement
	{
		public T Element { get; private set; }
		public DependencyProperty DependencyProperty { get; private set; }
		private DependencyProperty _hiddenAttachedProperty;
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		public DependencyPropertySubscription(T element, DependencyProperty dependencyProperty)
		{
			this.Element = element;
			this.DependencyProperty = dependencyProperty;

			var sourceBinding = new System.Windows.Data.Binding("Value") { Source = this, Mode = BindingMode.TwoWay };
			element.SetBinding(this.DependencyProperty, sourceBinding);
		}


		private static readonly DependencyProperty<DependencyPropertySubscription<T>, object> ValueProperty =
			DependencyProperty<DependencyPropertySubscription<T>, object>.Register(o => o.Value);
		public object Value
		{
			get { return ValueProperty.GetValue(this); }
			set
			{
				ValueProperty.SetValue(this, value);
				this.PropertyChanged.Notify(()=>Value);
			}
		}
	}
}