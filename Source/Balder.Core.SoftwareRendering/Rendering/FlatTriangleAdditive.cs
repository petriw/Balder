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

namespace Balder.Core.SoftwareRendering.Rendering
{
	public class FlatTriangleAdditive : FlatTriangle
	{
		protected override void DrawSpan(int length, float zStart, float zAdd, uint[] depthBuffer, int offset, int[] framebuffer, int colorAsInt)
		{
			for (var x = 0; x <= length; x++)
			{
				var bufferZ = (UInt32)((1.0f - zStart) * (float)UInt32.MaxValue);
				if (zStart >= 0f &&
					zStart < 1f
					)
				{
					framebuffer[offset] = colorAsInt;
					depthBuffer[offset] = bufferZ;
				}

				offset++;
				zStart += zAdd;
			}
		}
	}
}
