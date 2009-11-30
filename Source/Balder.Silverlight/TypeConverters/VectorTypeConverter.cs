using System;
using System.ComponentModel;
using Balder.Silverlight.Controls.Math;

namespace Balder.Silverlight.TypeConverters
{
	public class VectorTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType.Equals(typeof (string));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType.Equals(typeof (Vector));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var stringValue = value as string;
			var trimmedStringValue = stringValue.Replace(" ", string.Empty);
			var values = trimmedStringValue.Split(',');
			if( values.Length != 3 )
			{
				throw new ArgumentException("The format needs to be ([x],[y],[z])");
			}
			var vector = new Vector();
			vector.X = double.Parse(values[0]);
			vector.Y = double.Parse(values[1]);
			vector.Z = double.Parse(values[2]);
			return vector;
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			var vector = value as Vector;
			var vectorAsString = string.Format("{0},{1},{2}", vector.X, vector.Y, vector.Z);
			return vectorAsString;
		}
	}
}
