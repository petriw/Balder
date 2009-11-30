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
using Balder.Core.Extensions;

namespace Balder.Core.Imaging
{
	public static class ImageHelper
	{
		private static ImageFormat[] SupportedFromFormats = new[]
		                                                    	{
																	new ImageFormat {PixelFormat=PixelFormat.RGBAlpha,Depth=32},
																	new ImageFormat {PixelFormat=PixelFormat.RGBAlpha,Depth=24}
		                                                    	};


		public static bool CanConvertFrom(ImageFormat format)
		{
			return SupportedFromFormats.IsSupported(format);
		}


		public static byte[] Convert(ImageFormat destinationFormat, byte[] sourcePixels, ImageFormat sourceFormat)
		{
			if( destinationFormat.Depth == 32 && sourceFormat.Depth == 24 )
			{
				return ConvertFrom24BppTo32Bpp(sourcePixels, destinationFormat.PixelFormat, sourceFormat.PixelFormat);
			}
			return null;
		}

		public static byte[] Convert(ImageFormat destinationFormat, byte[] sourcePixels, ImageFormat sourceFormat, ImagePalette palette)
		{
			throw new NotImplementedException();
		}


		private static byte[] ConvertFrom24BppTo32Bpp(byte[] sourcePixels, PixelFormat destinationPixelFormat, PixelFormat sourcePixelFormat)
		{
			var newLength = (sourcePixels.Length / 3) << 2;
			var destinationPixels = new byte[newLength];
			var destinationPixelIndex = 0;

			var sourceColorSpace = ColorSpaces.RGB24BPP;
			var destinationColorSpace = ColorSpaces.RGBAlpha32BPP;

			for (var pixelIndex = 0; pixelIndex < sourcePixels.Length; pixelIndex += 3)
			{
				destinationPixels[destinationPixelIndex + destinationColorSpace.RedPosition] =
					sourcePixels[pixelIndex + sourceColorSpace.RedPosition];

				destinationPixels[destinationPixelIndex + destinationColorSpace.GreenPosition] =
					sourcePixels[pixelIndex + sourceColorSpace.GreenPosition];

				destinationPixels[destinationPixelIndex + destinationColorSpace.BluePosition] =
					sourcePixels[pixelIndex + sourceColorSpace.BluePosition];

				destinationPixels[destinationPixelIndex + destinationColorSpace.AlphaPosition] = 0xff;

				destinationPixelIndex += 4;
			}

			return destinationPixels;
		}
	}
}
