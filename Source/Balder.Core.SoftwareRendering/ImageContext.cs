#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
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
using Balder.Core.Exceptions;
using Balder.Core.Extensions;
using Balder.Core.Imaging;

namespace Balder.Core.SoftwareRendering
{
	public class ImageContext : IImageContext
	{
		private static readonly ImageFormat[] ImageFormats = new[]
																{
		                                            				new ImageFormat {PixelFormat = PixelFormat.RGBAlpha, Depth = 32}
		                                            			};
		public int[] Pixels { get; private set; }

		public void SetFrame(byte[] frameBytes)
		{
			Pixels = new int[frameBytes.Length/4];
			Buffer.BlockCopy(frameBytes,0,Pixels,0,frameBytes.Length);
		}

		public void SetFrame(ImageFormat format, byte[] frameBytes)
		{
			SetFrame(format,frameBytes,null);
		}

		public void SetFrame(ImageFormat format, byte[] frameBytes, ImagePalette palette)
		{
			var targetFormat = ImageFormats.GetBestSuitedFormat(format);
			var canConvertFrom = ImageHelper.CanConvertFrom(format);
			if (null == targetFormat || !canConvertFrom )
			{
				throw new UnsupportedImageFormatException(format);
			}

			// Special case - no need to convert
			if( targetFormat.Equals(format))
			{
				SetFrame(frameBytes);
			} else
			{
				var convertedFrameBytes = ImageHelper.Convert(targetFormat, frameBytes, format);
				SetFrame(convertedFrameBytes);
			}
		}

		public ImageFormat[] SupportedImageFormats { get { return ImageFormats; } }
	}
}
