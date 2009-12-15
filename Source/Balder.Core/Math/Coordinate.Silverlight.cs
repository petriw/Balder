using System.ComponentModel;
using System.Windows;
using Balder.Core.Execution;
using Balder.Core.TypeConverters;

namespace Balder.Core.Math
{
	[TypeConverter(typeof(CoordinateTypeConverter))]
	public partial class Coordinate : DependencyObject
	{
		public static readonly Property<Coordinate, double> XProp = Property<Coordinate, double>.Register(c => c.X);
		public double X
		{
			get { return XProp.GetValue(this); }
			set { XProp.SetValue(this, value); }
		}

		public static readonly Property<Coordinate, double> YProp = Property<Coordinate, double>.Register(c => c.Y);
		public double Y
		{
			get { return YProp.GetValue(this); }
			set { YProp.SetValue(this, value); }
		}

		public static readonly Property<Coordinate, double> ZProp = Property<Coordinate, double>.Register(c => c.Z);
		public double Z
		{
			get { return ZProp.GetValue(this); }
			set { ZProp.SetValue(this, value); }
		}
	}
}
