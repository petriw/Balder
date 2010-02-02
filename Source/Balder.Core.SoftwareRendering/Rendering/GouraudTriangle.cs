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
	public class GouraudTriangle : Triangle
	{
		public override void Draw(Face face, Vertex[] vertices)
		{
			var vertexA = vertices[face.A];
			var vertexB = vertices[face.B];
			var vertexC = vertices[face.C];

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var za = vertexA.DepthBufferAdjustedZ;
			var ra = vertexA.CalculatedColor.RedAsFloat;
			var ga = vertexA.CalculatedColor.GreenAsFloat;
			var ba = vertexA.CalculatedColor.BlueAsFloat;
			var aa = vertexA.CalculatedColor.AlphaAsFloat;

			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var zb = vertexB.DepthBufferAdjustedZ;
			var rb = vertexB.CalculatedColor.RedAsFloat;
			var gb = vertexB.CalculatedColor.GreenAsFloat;
			var bb = vertexB.CalculatedColor.BlueAsFloat;
			var ab = vertexB.CalculatedColor.AlphaAsFloat;


			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;
			var zc = vertexC.DepthBufferAdjustedZ;
			var rc = vertexC.CalculatedColor.RedAsFloat;
			var gc = vertexC.CalculatedColor.GreenAsFloat;
			var bc = vertexC.CalculatedColor.BlueAsFloat;
			var ac = vertexC.CalculatedColor.AlphaAsFloat;


			var deltaX1 = xb - xa;
			var deltaX2 = xc - xb;
			var deltaX3 = xc - xa;

			var deltaY1 = yb - ya;
			var deltaY2 = yc - yb;
			var deltaY3 = yc - ya;

			var deltaZ1 = zb - za;
			var deltaZ2 = zc - zb;
			var deltaZ3 = zc - za;

			var deltaR1 = rb - ra;
			var deltaR2 = rc - rb;
			var deltaR3 = rc - ra;

			var deltaG1 = gb - ga;
			var deltaG2 = gc - gb;
			var deltaG3 = gc - ga;

			var deltaB1 = bb - ba;
			var deltaB2 = bc - bb;
			var deltaB3 = bc - ba;

			var deltaA1 = ab - aa;
			var deltaA2 = ac - ab;
			var deltaA3 = ac - aa;

			var x1 = xa;
			var x2 = xa;

			var z1 = za;
			var z2 = za;

			var r1 = ra;
			var r2 = ra;

			var g1 = ga;
			var g2 = ga;

			var b1 = ba;
			var b2 = ba;

			var a1 = aa;
			var a2 = aa;


			var xInterpolate1 = deltaX3 / deltaY3;
			var xInterpolate2 = deltaX1 / deltaY1;
			var xInterpolate3 = deltaX2 / deltaY2;

			var zInterpolate1 = deltaZ3 / deltaY3;
			var zInterpolate2 = deltaZ1 / deltaY1;
			var zInterpolate3 = deltaZ2 / deltaY2;

			var rInterpolate1 = deltaR3 / deltaY3;
			var rInterpolate2 = deltaR1 / deltaY1;
			var rInterpolate3 = deltaR2 / deltaY2;

			var gInterpolate1 = deltaG3 / deltaY3;
			var gInterpolate2 = deltaG1 / deltaY1;
			var gInterpolate3 = deltaG2 / deltaY2;

			var bInterpolate1 = deltaB3 / deltaY3;
			var bInterpolate2 = deltaB1 / deltaY1;
			var bInterpolate3 = deltaB2 / deltaY2;

			var aInterpolate1 = deltaA3 / deltaY3;
			var aInterpolate2 = deltaA1 / deltaY1;
			var aInterpolate3 = deltaA2 / deltaY2;


			var framebuffer = BufferContainer.Framebuffer;
			var depthBuffer = BufferContainer.DepthBuffer;
			var frameBufferWidth = BufferContainer.Width;
			var frameBufferHeight = BufferContainer.Height;

			var yStart = (int)ya;
			var yEnd = (int)yc;
			var yClipTop = 0;

			if (yStart < 0)
			{
				yClipTop = -yStart;
				yStart = 0;
			}

			if (yEnd >= frameBufferHeight)
			{
				yEnd = frameBufferHeight - 1;
			}

			var height = yEnd - yStart;
			if (height == 0)
			{
				return;
			}

			if (yClipTop > 0)
			{
				var yClipTopAsFloat = (float)yClipTop;
				x1 = xa + xInterpolate1 * yClipTopAsFloat;
				z1 = za + zInterpolate1 * yClipTopAsFloat;
				r1 = ra + rInterpolate1 * yClipTopAsFloat;
				g1 = ga + gInterpolate1 * yClipTopAsFloat;
				b1 = ba + bInterpolate1 * yClipTopAsFloat;
				a1 = aa + aInterpolate1 * yClipTopAsFloat;

				if (yb < 0)
				{
					var ySecondClipTop = -yb;

					x2 = xb + (xInterpolate3 * ySecondClipTop);
					xInterpolate2 = xInterpolate3;

					z2 = zb + (zInterpolate3 * ySecondClipTop);
					zInterpolate2 = zInterpolate3;

					r2 = rb + (rInterpolate3 * ySecondClipTop);
					rInterpolate2 = rInterpolate3;

					g2 = gb + (gInterpolate3 * ySecondClipTop);
					gInterpolate2 = gInterpolate3;

					b2 = bb + (bInterpolate3 * ySecondClipTop);
					bInterpolate2 = bInterpolate3;

					a2 = ab + (aInterpolate3 * ySecondClipTop);
					aInterpolate2 = aInterpolate3;
				}
				else
				{
					x2 = xa + xInterpolate2 * yClipTopAsFloat;
					z2 = za + zInterpolate2 * yClipTopAsFloat;
					r2 = ra + rInterpolate2 * yClipTopAsFloat;
					g2 = ga + gInterpolate2 * yClipTopAsFloat;
					b2 = ba + bInterpolate2 * yClipTopAsFloat;
					a2 = aa + aInterpolate2 * yClipTopAsFloat;
				}
			}

			var yoffset = BufferContainer.Width * yStart;

			var offset = 0;
			var length = 0;
			var originalLength = 0;

			var xStart = 0;
			var xEnd = 0;

			var zStart = 0f;
			var zEnd = 0f;
			var zAdd = 0f;

			var xClipStart = 0;

			var rStart = 0f;
			var rEnd = 0f;
			var rAdd = 0f;
			var gStart = 0f;
			var gEnd = 0f;
			var gAdd = 0f;
			var bStart = 0f;
			var bEnd = 0f;
			var bAdd = 0f;
			var aStart = 0f;
			var aEnd = 0f;
			var aAdd = 0f;

			var rgbaStart = 0L;
			var rgbaAdd = 0L;


			for (var y = yStart; y <= yEnd; y++)
			{
				if (x2 < x1)
				{
					xStart = (int)x2;
					xEnd = (int)x1;

					zStart = z2;
					zEnd = z1;

					rStart = r2;
					rEnd = r1;

					gStart = g2;
					gEnd = g1;

					bStart = b2;
					bEnd = b1;

					aStart = a2;
					aEnd = a1;
				}
				else
				{
					offset = yoffset + (int)x1;

					xStart = (int)x1;
					xEnd = (int)x2;

					zStart = z1;
					zEnd = z2;

					rStart = r1;
					rEnd = r2;

					gStart = g1;
					gEnd = g2;

					bStart = b1;
					bEnd = b2;

					aStart = a1;
					aEnd = a2;
				}
				originalLength = xEnd - xStart;

				if (xStart < 0)
				{
					xClipStart = -xStart;
					xStart = 0;
				}
				if (xEnd >= frameBufferWidth)
				{
					xEnd = frameBufferWidth - 1;
				}


				length = xEnd - xStart;

				if (length != 0)
				{
					var xClipStartAsFloat = (float)xClipStart;
					var lengthAsFloat = (float)originalLength;
					zAdd = (zEnd - zStart) / lengthAsFloat;
					rAdd = (rEnd - rStart) / lengthAsFloat;
					gAdd = (gEnd - gStart) / lengthAsFloat;
					bAdd = (bEnd - bStart) / lengthAsFloat;
					aAdd = (aEnd - aStart) / lengthAsFloat;

					if (xClipStartAsFloat > 0)
					{
						zStart += (zAdd*xClipStartAsFloat);
						rStart += (rAdd*xClipStartAsFloat);
						gStart += (gAdd*xClipStartAsFloat);
						bStart += (bAdd*xClipStartAsFloat);
						aStart += (aAdd*xClipStartAsFloat);
					}

					var rStartInt = ((int)(rStart * 255f))<<8;
					var rAddInt = (int)(rAdd * 65535f);

					var gStartInt = ((int)(gStart * 255f))<<8;
					var gAddInt = (int)(gAdd * 65535f);

					var bStartInt = ((int)(bStart * 255f))<<8;
					var bAddInt = (int)(bAdd * 65535f);

					var aStartInt = ((int)(aStart * 255f))<<8;
					var aAddInt = (int)(aAdd * 65535f);

					offset = yoffset + xStart;
					DrawSpan(	length, 
								zStart, 
								zAdd,
								rStartInt,
								rAddInt,
								gStartInt,
								gAddInt,
								bStartInt,
								bAddInt,
								aStartInt,
								aAddInt,
								depthBuffer, 
								offset, 
								framebuffer);
				}

				if (y == (int)yb)
				{
					x2 = xb;
					xInterpolate2 = xInterpolate3;

					z2 = zb;
					zInterpolate2 = zInterpolate3;

					r2 = rb;
					rInterpolate2 = rInterpolate3;

					g2 = gb;
					gInterpolate2 = gInterpolate3;

					b2 = bb;
					bInterpolate2 = bInterpolate3;

					a2 = ab;
					aInterpolate2 = aInterpolate3;
				}


				x1 += xInterpolate1;
				x2 += xInterpolate2;

				z1 += zInterpolate1;
				z2 += zInterpolate2;

				r1 += rInterpolate1;
				r2 += rInterpolate2;

				g1 += gInterpolate1;
				g2 += gInterpolate2;

				b1 += bInterpolate1;
				b2 += bInterpolate2;

				a1 += aInterpolate1;
				a2 += aInterpolate2;

				yoffset += BufferContainer.Width;
			}
		}

		protected virtual void DrawSpan(int length,
										float zStart,
										float zAdd,
										int rStart,
										int rAdd,
										int gStart,
										int gAdd,
										int bStart,
										int bAdd,
										int aStart,
										int aAdd,
										uint[] depthBuffer,
										int offset,
										int[] framebuffer)
		{
			
			for (var x = 0; x <= length; x++)
			{
				var bufferZ = (UInt32)((1.0f - zStart) * (float)UInt32.MaxValue);
				if (bufferZ > depthBuffer[offset] &&
					zStart >= 0f &&
					zStart < 1f
					)
				{


					var red = (uint)(rStart >> 8) & 0xff;
					var green = (uint)(gStart >> 8) & 0xff;
					var blue = (uint)(bStart >> 8) & 0xff;
					//var alpha = (uint)(aStart >> 8) & 0xff;

					uint colorAsInt = 0xff000000 |
					                  (red << 16) |
					                  (green << 8) |
					                  blue;


					framebuffer[offset] = (int)colorAsInt;
					depthBuffer[offset] = bufferZ;
				}

				offset++;
				zStart += zAdd;
				rStart += rAdd;
				gStart += gAdd;
				bStart += bAdd;
				aStart += aAdd;
			}
		}
	}
}
