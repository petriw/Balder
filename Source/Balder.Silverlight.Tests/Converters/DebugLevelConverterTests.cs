using System;
using System.Globalization;
using Balder.Core.Debug;
using Balder.Silverlight.Converters;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Converters
{
	[TestFixture]
	public class DebugLevelConverterTests
	{
		[Test]
		public void ConverterShouldBeAbleToConvertFromString()
		{
			var converter = new DebugLevelConverter();
			var canConvert = converter.CanConvertFrom(null, typeof (string));
			Assert.That(canConvert,Is.True);
		}

		[Test]
		public void ConverterShouldBeAbleToConvertToDebugLevel()
		{
			var converter = new DebugLevelConverter();
			var canConvert = converter.CanConvertTo(null, typeof(DebugLevel));
			Assert.That(canConvert, Is.True);
		}

		[Test]
		public void ConvertingSingleValueShouldReturnOnlyThatValueSet()
		{
			var converter = new DebugLevelConverter();
			var expected = DebugLevel.BoundingBoxes;
			var valueAsString = expected.ToString();
			var converted = converter.ConvertFrom(null, CultureInfo.InvariantCulture, valueAsString);
			Assert.That(converted, Is.EqualTo(expected));
		}

		[Test]
		public void ConvertingMultipleValuesShouldReturnAllValuesSet()
		{
			var converter = new DebugLevelConverter();
			var firstValue = DebugLevel.BoundingBoxes;
			var secondValue = DebugLevel.BoundingSpheres;
			var mergedValuesAsString = string.Format("{0}|{1}", firstValue, secondValue);
			var converted = (DebugLevel)converter.ConvertFrom(null, CultureInfo.InvariantCulture, mergedValuesAsString);

			Assert.That(converted & firstValue, Is.EqualTo(firstValue));
			Assert.That(converted & secondValue, Is.EqualTo(secondValue));
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void ConvertingWithWrongFormatShouldCauseException()
		{
			var converter = new DebugLevelConverter();
			var firstValue = DebugLevel.BoundingBoxes;
			var secondValue = DebugLevel.BoundingSpheres;
			var mergedValuesAsString = string.Format("{0}&{1}", firstValue, secondValue);
			converter.ConvertFrom(null, CultureInfo.InvariantCulture, mergedValuesAsString);
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void ConvertingWithInvalidValuesShouldCauseException()
		{
			var converter = new DebugLevelConverter();
			var valuesAsString = "Something|SomethingElse";
			converter.ConvertFrom(null, CultureInfo.InvariantCulture, valuesAsString);
		}
	}
}
