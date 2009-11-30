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
using NUnit.Framework;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class ColorTests
	{
		[Test]
		public void SettingRedShouldSetRedAsFloatToEquivalentColor()
		{
			var color = new Color();
			color.Red = 0xff;
			Assert.That(color.RedAsFloat, Is.EqualTo(1f));
			color.Red = 0x80;
			Assert.That(color.RedAsFloat, Is.EqualTo(0.5f));
		}

		[Test]
		public void SettingGreenShouldSetGreenAsFloatToEquivalentColor()
		{
			var color = new Color();
			color.Green = 0xff;
			Assert.That(color.GreenAsFloat, Is.EqualTo(1f));
			color.Green = 0x80;
			Assert.That(color.GreenAsFloat, Is.EqualTo(0.5f));
		}

		[Test]
		public void SettingBlueShouldSetBlueAsFloatToEquivalentColor()
		{
			var color = new Color();
			color.Blue = 0xff;
			Assert.That(color.BlueAsFloat, Is.EqualTo(1f));
			color.Blue = 0x80;
			Assert.That(color.BlueAsFloat, Is.EqualTo(0.5f));
		}

		[Test]
		public void SettingAlphaShouldSetAlphaAsFloatToEquivalentColor()
		{
			var color = new Color();
			color.Alpha = 0xff;
			Assert.That(color.AlphaAsFloat, Is.EqualTo(1f));
			color.Alpha = 0x80;
			Assert.That(color.AlphaAsFloat, Is.EqualTo(0.5f));
		}

		[Test]
		public void SettingRedAsFloatShouldSetRedToEquivalentColor()
		{
			var color = new Color();
			color.RedAsFloat = 1f;
			Assert.That(color.Red, Is.EqualTo(0xff));
			color.RedAsFloat = 0.5f;
			Assert.That(color.Red, Is.EqualTo(0x7f));
		}

		[Test]
		public void SettingGreenAsFloatShouldSetGreenToEquivalentColor()
		{
			var color = new Color();
			color.GreenAsFloat = 1f;
			Assert.That(color.Green, Is.EqualTo(0xff));
			color.GreenAsFloat = 0.5f;
			Assert.That(color.Green, Is.EqualTo(0x7f));
		}

		[Test]
		public void SettingBlueAsFloatShouldSetBlueToEquivalentColor()
		{
			var color = new Color();
			color.BlueAsFloat = 1f;
			Assert.That(color.Blue, Is.EqualTo(0xff));
			color.BlueAsFloat = 0.5f;
			Assert.That(color.Blue, Is.EqualTo(0x7f));
		}

		[Test]
		public void SettingAlphaAsFloatShouldSetAlphaToEquivalentColor()
		{
			var color = new Color();
			color.AlphaAsFloat = 1f;
			Assert.That(color.Alpha, Is.EqualTo(0xff));
			color.AlphaAsFloat = 0.5f;
			Assert.That(color.Alpha, Is.EqualTo(0x7f));
		}


		[Test]
		public void AddingShouldNotClampToMaximumValue()
		{
			var firstColor = new Color { RedAsFloat = 1f };
			var secondColor = new Color { RedAsFloat = 1f };

			var result = firstColor + secondColor;
			Assert.That(result.RedAsFloat, Is.EqualTo(2f));
		}

		[Test]
		public void SubtractingShouldClampToMinimumValue()
		{
			var firstColor = new Color { RedAsFloat = 0f };
			var secondColor = new Color { RedAsFloat = 1f };

			var result = firstColor - secondColor;
			Assert.That(result.RedAsFloat, Is.EqualTo(-1f));
		}

		[Test]
		public void ClampingShouldKeepValuesWithinLegalRange()
		{
			var firstColor = new Color { RedAsFloat = 1f };
			var secondColor = new Color { RedAsFloat = 1f };

			var result = firstColor + secondColor;
			result.Clamp();
			Assert.That(result.RedAsFloat, Is.EqualTo(1f));

			result = firstColor - secondColor - secondColor;
			result.Clamp();
			Assert.That(result.RedAsFloat,Is.EqualTo((0f)));
		}


		[Test]
		public void ConvertingToUInt32ShouldConvertAllComponentsCorrectly()
		{
			var redColor = new Color { Red = 0xff };
			var redResult = redColor.ToUInt32();
			Assert.That(redResult, Is.EqualTo(0x00ff0000));
			var greenColor = new Color { Green = 0xff };
			var greenResult = greenColor.ToUInt32();
			Assert.That(greenResult, Is.EqualTo(0x0000ff00));
			var blueColor = new Color { Blue = 0xff };
			var blueResult = blueColor.ToUInt32();
			Assert.That(blueResult, Is.EqualTo(0x000000ff));
			var alphaColor = new Color { Alpha = 0xff };
			var alphaResult = alphaColor.ToUInt32();
			Assert.That(alphaResult, Is.EqualTo(0xff000000));
			var redAndAlphaColor = new Color { Red = 0xff, Alpha = 0xff };
			var redAndAlphaResult = redAndAlphaColor.ToUInt32();
			Assert.That(redAndAlphaResult, Is.EqualTo(0xffff0000));
			var greenAndAlphaColor = new Color { Green = 0xff, Alpha = 0xff };
			var greenAndAlphaResult = greenAndAlphaColor.ToUInt32();
			Assert.That(greenAndAlphaResult, Is.EqualTo(0xff00ff00));
			var redAndBlueColor = new Color { Red = 0xff, Blue = 0xff };
			var redAndBlueResult = redAndBlueColor.ToUInt32();
			Assert.That(redAndBlueResult, Is.EqualTo(0x00ff00ff));
		}

		[Test]
		public void ConvertingToSystemColorShouldConvertAllComponentsCorrectly()
		{
			const byte red = 0x10;
			const byte green = 0x20;
			const byte blue = 0x30;
			const byte alpha = 0x40;

			var color = new Color
							{
								Red = red,
								Green = green,
								Blue = blue,
								Alpha = alpha
							};
			var sysColor = color.ToSystemColor();
			Assert.That(sysColor.R, Is.EqualTo(red));
			Assert.That(sysColor.G, Is.EqualTo(green));
			Assert.That(sysColor.B, Is.EqualTo(blue));
			Assert.That(sysColor.A, Is.EqualTo(alpha));
		}

		[Test]
		public void ScalingColorShouldScaleAllComponentsCorrectly()
		{
			var color = new Color
							{
								RedAsFloat = 0.1f,
								GreenAsFloat = 0.2f,
								BlueAsFloat = 0.3f,
								AlphaAsFloat = 0.4f
							};
			var scaledColor = color*2f;
			Assert.That(scaledColor.RedAsFloat, Is.EqualTo(0.2f));
			Assert.That(scaledColor.GreenAsFloat, Is.EqualTo(0.4f));
			Assert.That(scaledColor.BlueAsFloat, Is.EqualTo(0.6f));
			Assert.That(scaledColor.AlphaAsFloat, Is.EqualTo(0.8f));
		}

		[Test]
		public void AdditiveColoringShouldBeAnAverageOfTwoColors()
		{
			var firstColor = new Color {RedAsFloat = 0.2f };
			var secondColor = new Color {RedAsFloat = 0.4f };

			var result = firstColor.Additive(secondColor);

			Assert.That(result.RedAsFloat,Is.EqualTo(0.3f));
		}
	}
}
