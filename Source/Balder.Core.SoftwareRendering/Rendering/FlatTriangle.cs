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
using Balder.Core.Objects.Geometries;

namespace Balder.Core.SoftwareRendering.Rendering
{
	public class FlatTriangle
	{
		private static void GetSortedPoints(ref Vertex vertexA,
											ref Vertex vertexB,
											ref Vertex vertexC)
		{
			var point1 = vertexA;
			var point2 = vertexB;
			var point3 = vertexC;

			if (point2.TranslatedScreenCoordinates.Y < point1.TranslatedScreenCoordinates.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;
			}

			if (point3.TranslatedScreenCoordinates.Y < point2.TranslatedScreenCoordinates.Y)
			{
				var p = point2;
				point2 = point3;
				point3 = p;
			}


			if (point2.TranslatedScreenCoordinates.Y < point1.TranslatedScreenCoordinates.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;
			}

			vertexA = point1;
			vertexB = point2;
			vertexC = point3;
		}


		public static void Draw(Face face, Vertex[] vertices)
		{
			var vertexA = vertices[face.A];
			var vertexB = vertices[face.B];
			var vertexC = vertices[face.C];

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var za = vertexA.DepthBufferAdjustedZ;
			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var zb = vertexB.DepthBufferAdjustedZ;
			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;
			var zc = vertexC.DepthBufferAdjustedZ;

			var deltaX1 = xb - xa;
			var deltaX2 = xc - xb;
			var deltaX3 = xc - xa;

			var deltaY1 = yb - ya;
			var deltaY2 = yc - yb;
			var deltaY3 = yc - ya;

			var deltaZ1 = zb - za;
			var deltaZ2 = zc - zb;
			var deltaZ3 = zc - za;

			var x1 = xa;
			var x2 = xa;

			var z1 = za;
			var z2 = za;

			var xInterpolate1 = deltaX3 / deltaY3;
			var xInterpolate2 = deltaX1 / deltaY1;
			var xInterpolate3 = deltaX2 / deltaY2;

			var zInterpolate1 = deltaZ3 / deltaY3;
			var zInterpolate2 = deltaZ1 / deltaY1;
			var zInterpolate3 = deltaZ2 / deltaY2;

			var framebuffer = BufferContainer.Framebuffer;
			var depthBuffer = BufferContainer.DepthBuffer;
			var frameBufferWidth = BufferContainer.Width;
			var frameBufferHeight = BufferContainer.Height;

			var yStart = (int)ya;
			var yEnd = (int) yc;
			var yClipTop = 0;
			
			if( yStart < 0 )
			{
				yClipTop = -yStart;
				yStart = 0;
			}

			if( yEnd >= frameBufferHeight )
			{
				yEnd = frameBufferHeight-1;
			}

			var height = yEnd - yStart;
			if (height == 0)
			{
				return;
			}

			if( yClipTop > 0 )
			{
				x1 = xa+xInterpolate1*(float) yClipTop;
				z1 = za+zInterpolate1*(float) yClipTop;

				if( yb < 0 )
				{
					var ySecondClipTop = -yb;

					x2 = xb + (xInterpolate3*ySecondClipTop);
					xInterpolate2 = xInterpolate3;

					z2 = zb + (zInterpolate3 * ySecondClipTop);
					zInterpolate2 = zInterpolate3;
				} else
				{
					x2 = xa + xInterpolate2*(float) yClipTop;
					z2 = za + zInterpolate2*(float) yClipTop;
				}
			}

			var yoffset = BufferContainer.Width * yStart;

			var colorAsInt = (int) face.Color.ToUInt32();

			var offset = 0;
			var length = 0;
			var originalLength = 0;

			var xStart = 0;
			var xEnd = 0;

			var zStart = 0f;
			var zEnd = 0f;
			var z = 0f;
			var zAdd = 0f;

			var xClipStart = 0;



			for (var y = yStart; y <= yEnd; y++)
			{
				if (x2 < x1)
				{
					xStart = (int)x2;
					xEnd = (int)x1;

					zStart = z2;
					zEnd = z1;
				}
				else
				{
					xStart = (int)x1;
					xEnd = (int)x2;

					offset = yoffset + (int)x1;
					zStart = z1;
					zEnd = z2;
				}
				originalLength = xEnd - xStart;

				if( xStart < 0 )
				{
					xClipStart = -xStart;
					xStart = 0;
				}
				if( xEnd >= frameBufferWidth )
				{
					xEnd = frameBufferWidth - 1;
				}


				length = xEnd - xStart;

				if (length != 0)
				{
					z = zStart;
					zAdd = (zEnd - zStart)/(float)length;
					z += zAdd*(float) xClipStart;

					offset = yoffset + xStart;

					for (var x = 0; x <= length; x++)
					{
						var bufferZ = (UInt32)((1.0f - z) * (float)UInt32.MaxValue);
						if (bufferZ > BufferContainer.DepthBuffer[offset] &&
							z >= 0f &&
							z < 1f
							)
						{
							framebuffer[offset] = colorAsInt;
							depthBuffer[offset] = bufferZ;
						}

						
						offset++;
						z += zAdd;
					}
				}

				if (y == (int)yb)
				{
					x2 = xb;
					xInterpolate2 = xInterpolate3;

					z2 = zb;
					zInterpolate2 = zInterpolate3;
				}


				x1 += xInterpolate1;
				x2 += xInterpolate2;

				z1 += zInterpolate1;
				z2 += zInterpolate2;

				yoffset += BufferContainer.Width;
			}
		}
	}
}
