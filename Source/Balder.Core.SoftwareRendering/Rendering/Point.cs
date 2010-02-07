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

using Balder.Core.Math;

namespace Balder.Core.SoftwareRendering.Rendering
{
	public class Point
	{
		public void Draw(int x, int y, Color color, int size)
		{
			var framebuffer = BufferContainer.Framebuffer;
			var frameBufferWidth = BufferContainer.Width;
			var frameBufferHeight = BufferContainer.Height;
			var colorAsInt = (int)color.ToUInt32();
			
			for( var yIndex=y; yIndex<y+size; yIndex++ )
			{
				if (yIndex >= frameBufferHeight)
				{
					break;
				}
				if (yIndex < 0)
				{
					continue;
				}
				var yoffset = frameBufferWidth * yIndex;

				for( var xIndex=x; xIndex<x+size; xIndex++)
				{
					if (xIndex >= frameBufferHeight)
					{
						break;
					}
					if (xIndex < 0)
					{
						continue;
					}
					var offset = yoffset + xIndex;


					framebuffer[offset] = colorAsInt;
				}
			}
		}
	}
}
