using System.ComponentModel;
using System.Windows;
using Balder.Core.Execution;
using Balder.Core.TypeConverters;

namespace Balder.Core.Math
{
	[TypeConverter(typeof(VectorTypeConverter))]
	public partial class Vector : DependencyObject
	{

		public static readonly Property<Vector, float> XProp = Property<Vector, float>.Register(v => v.X);
		public static readonly DependencyProperty XProperty = XProp.ActualDependencyProperty;
		[TypeConverter(typeof(FloatTypeConverter))]
		public float X
		{
			get { return XProp.GetValue(this); }
			set { XProp.SetValue(this, value); }
		}

		public static readonly Property<Vector, float> YProp = Property<Vector, float>.Register(v => v.Y);
		public static readonly DependencyProperty YProperty = YProp.ActualDependencyProperty;
		[TypeConverter(typeof(FloatTypeConverter))]
		public float Y
		{
			get { return YProp.GetValue(this); }
			set { YProp.SetValue(this, value); }
		}

		public static readonly Property<Vector, float> ZProp = Property<Vector, float>.Register(v => v.Z);
		public static readonly DependencyProperty ZProperty = ZProp.ActualDependencyProperty;
		[TypeConverter(typeof(FloatTypeConverter))]
		public float Z
		{
			get { return ZProp.GetValue(this); }
			set { ZProp.SetValue(this, value); }
		}

		public static readonly Property<Vector, float> WProp = Property<Vector, float>.Register(v => v.W);
		public static readonly DependencyProperty WProperty = WProp.ActualDependencyProperty;
		[TypeConverter(typeof(FloatTypeConverter))]
		public float W
		{
			get { return WProp.GetValue(this); }
			set { WProp.SetValue(this, value); }
		}
	}
}
