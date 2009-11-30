using System;
using System.ComponentModel;
using Balder.Core.Debug;

namespace Balder.Silverlight.Converters
{
	public class DebugLevelConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType.Equals(typeof (string));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType.Equals(typeof (DebugLevel));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var debugLevelsAsString = value.ToString();
			var debugLevels = debugLevelsAsString.Split('|');
			
			var debugLevel = DebugLevel.None;
			foreach( var debugLevelString in debugLevels )
			{
				try
				{
					var actualDebugLevel = (DebugLevel) Enum.Parse(typeof (DebugLevel), debugLevelString, false);
					debugLevel |= actualDebugLevel;
				} catch
				{
					var exceptionString = string.Format("Value ({0}) in string is not valid for type {1}", debugLevelString,
					                                    typeof (DebugLevel).Name);
					throw new ArgumentException(exceptionString);
				}
			}

			return debugLevel;
		}

	}
}
