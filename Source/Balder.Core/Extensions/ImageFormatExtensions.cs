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
using Balder.Core.Imaging;

namespace Balder.Core.Extensions
{
	public static class ImageFormatExtensions
	{
		public static bool IsSupported(this ImageFormat[] formats, ImageFormat desiredFormat)
		{
			for( var formatIndex=0; formatIndex<formats.Length; formatIndex++ )
			{
				if( formats[formatIndex].Equals(desiredFormat) )
				{
					return true;
				}
			}
			return false;
		}

		public static ImageFormat GetBestSuitedFormat(this ImageFormat[] formats, ImageFormat desiredFormat)
		{
			// Todo: Also look for PixelFormat - if there is a format that matches with both Depth and PixelFormat, choose this first.
			// Do a prioritizing of formats and depths - needs some thinking. :)
			for (var formatIndex = 0; formatIndex < formats.Length; formatIndex++)
			{
				if (formats[formatIndex].Depth >= desiredFormat.Depth)
				{
					return formats[formatIndex];
				}
			}

			return null;
		}
	}
}
