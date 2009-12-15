using System;
using System.ComponentModel;
using System.Globalization;

namespace Balder.Core.TypeConverters
{
	public class ColorConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType.Equals(typeof(string));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType.Equals(typeof(string));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{

				var type = typeof (System.Windows.Media.Colors);
				var colorProperty = type.GetProperty((string) value);
				if (null != colorProperty)
				{
					var color = (System.Windows.Media.Color) colorProperty.GetValue(null, null);
					return Color.FromSystemColor(color);
				}
				else
				{
					return Color.Black;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}