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
using Balder.Core.Imaging;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;

namespace Balder.Core.SoftwareRendering
{
	public enum TriangleShade
	{
		Flat,
		Gouraud
	}

	public class Triangle
	{
		private static readonly Interpolator FlatInterpolator;
		private static readonly Interpolator GouraudInterpolator;
		private static readonly Interpolator TextureInterpolator;

		static Triangle()
		{
			FlatInterpolator = new Interpolator();
			FlatInterpolator.SetNumberOfInterpolationPoints(4);

			GouraudInterpolator = new Interpolator();
			GouraudInterpolator.SetNumberOfInterpolationPoints(12);

			TextureInterpolator = new Interpolator();
			TextureInterpolator.SetNumberOfInterpolationPoints(8);
		}

		private static Interpolator GetInterpolatorForFace(Face face, TriangleShade shade)
		{
			var useTexture = false;
			if( null != face.Material && null != face.Material.DiffuseMap)
			{
				useTexture = true;
			}
			switch (shade)
			{
				case TriangleShade.Flat:
					{
						if( useTexture )
						{
							return TextureInterpolator;	
						}
						return FlatInterpolator;
					}
					break;

				case TriangleShade.Gouraud:
					{
						if (useTexture)
						{
							return TextureInterpolator;
						}
						return GouraudInterpolator;
					}
					break;
			}
			return null;
		}


		private static void GetSortedPoints(ref Vertex vertexA,
											ref Vertex vertexB,
											ref Vertex vertexC,
											ref TextureCoordinate textureA,
											ref TextureCoordinate textureB,
											ref TextureCoordinate textureC)
		{
			var point1 = vertexA;
			var point2 = vertexB;
			var point3 = vertexC;

			if (point2.TranslatedScreenCoordinates.Y < point1.TranslatedScreenCoordinates.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;

				var t = textureA;
				textureA = textureB;
				textureB = t;
			}

			if (point3.TranslatedScreenCoordinates.Y < point2.TranslatedScreenCoordinates.Y)
			{
				var p = point2;
				point2 = point3;
				point3 = p;

				var t = textureB;
				textureB = textureC;
				textureC = t;
			}


			if (point2.TranslatedScreenCoordinates.Y < point1.TranslatedScreenCoordinates.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;

				var t = textureA;
				textureA = textureB;
				textureB = t;
			}

			vertexA = point1;
			vertexB = point2;
			vertexC = point3;
		}

		private static Span? GetSpan(int interpolationIndex, Interpolator interpolator, Vertex vertexA, bool useTexture)
		{
			var y = ((int)vertexA.TranslatedScreenCoordinates.Y) + interpolationIndex;
			var xstart = interpolator.Points[0].InterpolatedValues[interpolationIndex];
			var xend = interpolator.Points[1].InterpolatedValues[interpolationIndex];

			var ustart = 0f;
			var vstart = 0f;
			var uend = 0f;
			var vend = 0f;
		

			if (useTexture)
			{
				ustart = interpolator.Points[4].InterpolatedValues[interpolationIndex];
				uend = interpolator.Points[5].InterpolatedValues[interpolationIndex];
				vstart = interpolator.Points[6].InterpolatedValues[interpolationIndex];
				vend = interpolator.Points[7].InterpolatedValues[interpolationIndex];
			}

			var swap = false;

			if (xstart > xend)
			{
				var temp = xstart;
				xstart = xend;
				xend = temp;
				swap = true;

				temp = ustart;
				ustart = uend;
				uend = temp;

				temp = vstart;
				vstart = vend;
				vend = temp;
			}

			xend += 0.5f;
			var length = xend - xstart;

			var absLength = (int)System.Math.Round((double)length + 0.5);

			if (absLength >= 1)
			{
				var span = new Span
							{
								Y = y,
								XStart = (int)xstart,
								XEnd = (int)xend,
								UStart = ustart,
								UEnd = uend,
								VStart = vstart,
								VEnd = vend,
								Length = absLength,
								Swap = swap
							};
				return span;
			}

			return null;
		}

		private static void SetSphericalEnvironmentMapTextureCoordinate(ref Vertex vertex, ref TextureCoordinate textureCoordinate)
		{
			var u = vertex.TransformedVectorNormalized;
			var n = vertex.TransformedNormal;
			var r = Vector.Reflect(n, u);
			var m = Math.Core.Sqrt((r.X * r.X) + (r.Y * r.Y) +
									 ((r.Z + 0f) * (r.Z + 0f)));
			var s = (r.X/m); // +0.5f;
			var t = (r.Y/m); // +0.5f;
			textureCoordinate.U = (s*0.5f)+0.5f;
			textureCoordinate.V = (t * 0.5f) + 0.5f;
		}

		private static TextureCoordinate ZeroTextureCoordinate = new TextureCoordinate(0, 0);


		public static void Draw(IBuffers buffers, ISpanRenderer renderer, TriangleShade shade, Face face, Vertex[] vertices, TextureCoordinate[] textureCoordinates)
		{
			var vertexA = vertices[face.A];
			var vertexB = vertices[face.B];
			var vertexC = vertices[face.C];
			var textureA = ZeroTextureCoordinate;
			var textureB = ZeroTextureCoordinate;
			var textureC = ZeroTextureCoordinate;
			Image image = null;
			ImageContext texture = null;

			var useTexture = false;
			if( null != face.Material && null != face.Material.DiffuseMap)
			{
				useTexture = true;
				image = face.Material.DiffuseMap;
				texture = face.Material.DiffuseMap.ImageContext as ImageContext;
				textureA = textureCoordinates[face.DiffuseA];
				textureB = textureCoordinates[face.DiffuseB];
				textureC = textureCoordinates[face.DiffuseC];
			}

			
			/*
			SetSphericalEnvironmentMapTextureCoordinate(ref vertexA, ref textureA);
			SetSphericalEnvironmentMapTextureCoordinate(ref vertexB, ref textureB);
			SetSphericalEnvironmentMapTextureCoordinate(ref vertexC, ref textureC);
			*/

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC, ref textureA, ref textureB, ref textureC);

			var interpolator = GetInterpolatorForFace(face, shade);

			var secondaryStartY = (int)(vertexB.TranslatedScreenCoordinates.Y - vertexA.TranslatedScreenCoordinates.Y);
			var spreadCount = ((int)(vertexC.TranslatedScreenCoordinates.Y - vertexA.TranslatedScreenCoordinates.Y)) + 1;

			interpolator.SetPoint(0, (int)vertexA.TranslatedScreenCoordinates.X, (int)vertexC.TranslatedScreenCoordinates.X);
			interpolator.SetPoint(1, (int)vertexA.TranslatedScreenCoordinates.X, (int)vertexB.TranslatedScreenCoordinates.X, (int)vertexB.TranslatedScreenCoordinates.X, (int)vertexC.TranslatedScreenCoordinates.X, secondaryStartY);
			interpolator.SetPoint(2, vertexA.DepthBufferAdjustedZ, vertexC.DepthBufferAdjustedZ);
			interpolator.SetPoint(3, vertexA.DepthBufferAdjustedZ, vertexB.DepthBufferAdjustedZ, vertexB.DepthBufferAdjustedZ, vertexC.DepthBufferAdjustedZ, secondaryStartY);

			if (useTexture)
			{
				interpolator.SetPoint(4, textureA.U, textureC.U);
				interpolator.SetPoint(5, textureA.U, textureB.U, textureB.U, textureC.U, secondaryStartY);
				interpolator.SetPoint(6, textureA.V, textureC.V);
				interpolator.SetPoint(7, textureA.V, textureB.V, textureB.V, textureC.V, secondaryStartY);
			}

			var color = Color.FromArgb(0xff, 0xff, 0xff, 0xff);

			if (shade == TriangleShade.Gouraud && !useTexture)
			{
				var vertexAColor = vertexA.Color;
				var vertexBColor = vertexB.Color;
				var vertexCColor = vertexC.Color;

				interpolator.SetPoint(4, vertexAColor.RedAsFloat, vertexCColor.RedAsFloat);
				interpolator.SetPoint(5, vertexAColor.RedAsFloat, vertexBColor.RedAsFloat, vertexBColor.RedAsFloat, vertexCColor.RedAsFloat, secondaryStartY);

				interpolator.SetPoint(6, vertexAColor.GreenAsFloat, vertexCColor.GreenAsFloat);
				interpolator.SetPoint(7, vertexAColor.GreenAsFloat, vertexBColor.GreenAsFloat, vertexBColor.GreenAsFloat, vertexCColor.GreenAsFloat, secondaryStartY);

				interpolator.SetPoint(8, vertexAColor.BlueAsFloat, vertexCColor.BlueAsFloat);
				interpolator.SetPoint(9, vertexAColor.BlueAsFloat, vertexBColor.BlueAsFloat, vertexBColor.BlueAsFloat, vertexCColor.BlueAsFloat, secondaryStartY);

				interpolator.SetPoint(10, vertexAColor.AlphaAsFloat, vertexCColor.AlphaAsFloat);
				interpolator.SetPoint(11, vertexAColor.AlphaAsFloat, vertexBColor.AlphaAsFloat, vertexBColor.AlphaAsFloat, vertexCColor.AlphaAsFloat, secondaryStartY);
			}
			else
			{
				color = face.Color;
			}

			
			interpolator.Interpolate(spreadCount);



			var yPosition = vertexA.TranslatedScreenCoordinates.Y;
			var clip = false;

			if( null == buffers )
			{
				return;
			}


			for (var index = 0; index < spreadCount; index++)
			{

				if( yPosition < 0 || yPosition >= buffers.Height )
				{
					clip = true;
				} else
				{
					clip = false;
				}

				

				var span = GetSpan(index, interpolator, vertexA, useTexture);
				if (null != span && !clip )
				{
					var actualSpan = (Span)span;
					var swapIndex = ((Span)span).Swap ? 1 : 0;

					actualSpan.ZStart = interpolator.Points[2 + swapIndex].InterpolatedValues[index];
					actualSpan.ZEnd = interpolator.Points[3 - swapIndex].InterpolatedValues[index];
					switch (shade)
					{
						case TriangleShade.Flat:
							{
								if (useTexture)
								{
									renderer.Texture(buffers, actualSpan, image, texture);
								}
								else
								{
									renderer.Flat(buffers, actualSpan, color);
								}
							}
							break;

						case TriangleShade.Gouraud:
							{
								if (useTexture)
								{
									renderer.Texture(buffers, actualSpan, image, texture);
								}
								else
								{
									actualSpan.ColorStart.RedAsFloat = interpolator.Points[4 + swapIndex].InterpolatedValues[index];
									actualSpan.ColorEnd.RedAsFloat = interpolator.Points[5 - swapIndex].InterpolatedValues[index];
									actualSpan.ColorStart.GreenAsFloat = interpolator.Points[6 + swapIndex].InterpolatedValues[index];
									actualSpan.ColorEnd.GreenAsFloat = interpolator.Points[7 - swapIndex].InterpolatedValues[index];
									actualSpan.ColorStart.BlueAsFloat = interpolator.Points[8 + swapIndex].InterpolatedValues[index];
									actualSpan.ColorEnd.BlueAsFloat = interpolator.Points[9 - swapIndex].InterpolatedValues[index];
									actualSpan.ColorStart.AlphaAsFloat = interpolator.Points[10 + swapIndex].InterpolatedValues[index];
									actualSpan.ColorEnd.AlphaAsFloat = interpolator.Points[11 - swapIndex].InterpolatedValues[index];
									renderer.Gouraud(buffers, actualSpan);
								}
							}
							break;
					}
				}
				yPosition++;
			}
		}
	}
}
