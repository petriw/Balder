using System;
using System.ComponentModel;
using System.Globalization;

namespace Balder.Core.TypeConverters
{
	public class FloatTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType.Equals(typeof (string)) || sourceType.Equals(typeof (double));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType.Equals(typeof (string)) || destinationType.Equals(typeof (double));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if( value is double )
			{
				return (float) value;
			}
			if( value is string )
			{
				return float.Parse((string) value);
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if( destinationType.Equals(typeof(string)))
			{
				return value.ToString();
			}
			if( destinationType.Equals(typeof(double)))
			{
				return (double) value;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
