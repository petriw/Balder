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
using Balder.Core.Display;

namespace Balder.Core.SoftwareRendering
{
	public static class Shapes
	{
		public static void DrawLine(Viewport viewport, int xstart, int ystart, int xend, int yend, Color color)
		{
			var stride = BufferContainer.Stride;
			var framebuffer = BufferContainer.Framebuffer;

			var colorAsInt = (int)color.ToUInt32();

			var deltaX = xend - xstart;
			var deltaY = yend - ystart;

			var lengthX = deltaX >= 0 ? deltaX : -deltaX;
			var lengthY = deltaY >= 0 ? deltaY : -deltaY;

			var actualLength = lengthX > lengthY ? lengthX : lengthY;

			if( actualLength != 0 )
			{
				var slopeX = (float) deltaX/(float) actualLength;
				var slopeY = (float) deltaY/(float) actualLength;

				var currentX = (float)xstart;
				var currentY = (float)ystart;

				for( var pixel=0; pixel<actualLength; pixel++)
				{
					if (currentX > 0 && currentY > 0 && currentX < viewport.Width && currentY < viewport.Height)
					{
						var bufferOffset = (stride*(int) currentY) + (int) currentX;
						framebuffer[bufferOffset] = colorAsInt;
					}

					currentX += slopeX;
					currentY += slopeY;
				}
			}
		}
	}
}
