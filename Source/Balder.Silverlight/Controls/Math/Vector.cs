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
using System.Windows;
using Balder.Core.Extensions;
using Balder.Core.Helpers;
using Balder.Silverlight.TypeConverters;

namespace Balder.Silverlight.Controls.Math
{
	[TypeConverter(typeof(VectorTypeConverter))]
	public class Vector : DependencyObject, INotifyPropertyChanged
	{
		private Action<float, float, float> _setNativeAction;

		public void SetNativeAction(Action<float,float,float> setNativeAction)
		{
			_setNativeAction = setNativeAction;
			UpdateNativeVector();
		}

		private void UpdateNativeVector()
		{
			if( null != _setNativeAction)
			{
				_setNativeAction((float)X, (float)Y, (float)Z);	
			}
			
		}


		public static readonly DependencyProperty<Vector, double> XProperty =
			DependencyProperty<Vector, double>.Register(o => o.X);
		public double X
		{
			get { return XProperty.GetValue(this); }
			set
			{
				XProperty.SetValue(this, value);
				PropertyChanged.Notify(() => X);
				UpdateNativeVector();
			}
		}

		public static readonly DependencyProperty<Vector, double> YProperty =
			DependencyProperty<Vector, double>.Register(o => o.Y);
		public double Y
		{
			get { return YProperty.GetValue(this); }
			set
			{
				YProperty.SetValue(this, value);
				PropertyChanged.Notify(() => Y);
				UpdateNativeVector();
			}
		}

		public static readonly DependencyProperty<Vector, double> ZProperty =
			DependencyProperty<Vector, double>.Register(o => o.Z);
		public double Z
		{
			get { return ZProperty.GetValue(this); }
			set
			{
				ZProperty.SetValue(this, value);
				PropertyChanged.Notify(() => Z);
				UpdateNativeVector();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
	}
}
