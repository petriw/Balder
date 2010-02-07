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
using System.Collections.Generic;
using Balder.Core.Display;
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using Balder.Core.SoftwareRendering.Rendering;
using Matrix = Balder.Core.Math.Matrix;

namespace Balder.Core.SoftwareRendering
{
	public class GeometryContext : IGeometryContext
	{
		private static readonly FlatTriangle FlatTriangleRenderer = new FlatTriangle();
		private static readonly FlatTriangleAdditive FlatTriangleAdditiveRenderer = new FlatTriangleAdditive();
		private static readonly GouraudTriangle GouraudTriangleRenderer = new GouraudTriangle();
		private static readonly TextureTriangle TextureTriangleRenderer = new TextureTriangle();
		private readonly IColorCalculator _colorCalculator;

		private bool _hasPrepared;

		public GeometryContext(IColorCalculator colorCalculator)
		{
			_colorCalculator = colorCalculator;
		}


		public Vertex[] Vertices { get; private set; }
		public Face[] Faces { get; private set; }
		public TextureCoordinate[] TextureCoordinates { get; private set; }
		public Line[] Lines { get; private set; }

		public int FaceCount { get { return Faces.Length; } }
		public int VertexCount { get { return Vertices.Length; } }
		public int TextureCoordinateCount { get { return TextureCoordinates.Length; } }
		public int LineCount { get { return Lines.Length; } }

		public void AllocateFaces(int count)
		{
			Faces = new Face[count];

		}

		public void SetFace(int index, Face face)
		{
			var v1 = Vertices[face.C].Vector - Vertices[face.A].Vector;
			var v2 = Vertices[face.B].Vector - Vertices[face.A].Vector;

			var cross = v1.Cross(v2);
			cross.Normalize();
			face.Normal = cross;

			var v = Vertices[face.A].Vector + Vertices[face.B].Vector + Vertices[face.C].Vector;
			face.Position = v / 3;

			Faces[index] = face;
		}

		public void SetFaceTextureCoordinateIndex(int index, int a, int b, int c)
		{
			Faces[index].DiffuseA = a;
			Faces[index].DiffuseB = b;
			Faces[index].DiffuseC = c;
		}

		public void SetMaterial(int index, Material material)
		{
			Faces[index].Material = material;
		}

		public void SetMaterialForAllFaces(Material material)
		{
			if (null == Faces)
			{
				return;
			}

			for (var index = 0; index < Faces.Length; index++)
			{
				Faces[index].Material = material;
			}
		}

		public Face[] GetFaces()
		{
			return Faces;
		}

		public void AllocateVertices(int count)
		{
			Vertices = new Vertex[count];
		}

		public void SetVertex(int index, Vertex vertex)
		{
			Vertices[index] = vertex;
		}

		public Vertex[] GetVertices()
		{
			return Vertices;
		}

		public void AllocateLines(int count)
		{
			Lines = new Line[count];
		}

		public void SetLine(int index, Line line)
		{
			Lines[index] = line;
		}

		public Line[] GetLines()
		{
			return Lines;
		}

		public void AllocateTextureCoordinates(int count)
		{
			TextureCoordinates = new TextureCoordinate[count];
		}

		public void SetTextureCoordinate(int index, TextureCoordinate textureCoordinate)
		{
			TextureCoordinates[index] = textureCoordinate;
		}

		private void Prepare()
		{
			if (null != TextureCoordinates && TextureCoordinates.Length > 0 && null != Faces)
			{
				for (var index = 0; index < Faces.Length; index++)
				{
					Faces[index].DiffuseTextureCoordinateA = TextureCoordinates[Faces[index].DiffuseA];
					Faces[index].DiffuseTextureCoordinateB = TextureCoordinates[Faces[index].DiffuseB];
					Faces[index].DiffuseTextureCoordinateC = TextureCoordinates[Faces[index].DiffuseC];
				}
			}
		}

		private static ISpanRenderer SpanRenderer = new SimpleSpanRenderer();

		public void Render(Viewport viewport, RenderableNode node, Matrix view, Matrix projection, Matrix world)
		{
			if (null == Vertices )
			{
				return;
			}
			if (!_hasPrepared)
			{
				Prepare();
				_hasPrepared = false;
			}


			TransformAndTranslateVertices(viewport, node, view, projection, world);

			RenderFaces(node, viewport, view, projection, world);
			RenderLines(node, viewport, view, projection, world);
		}

		private static void TransformAndTranslateVertex(ref Vertex vertex, Viewport viewport, Matrix view, Matrix projection)
		{
			vertex.Transform(view);
			vertex.Translate(projection, viewport.Width, viewport.Height);
			vertex.MakeScreenCoordinates();
			vertex.TransformedVectorNormalized = vertex.TransformedNormal;
			vertex.TransformedVectorNormalized.Normalize();
			var z = ((vertex.TransformedVector.Z / viewport.View.DepthDivisor) + viewport.View.DepthZero);
			vertex.DepthBufferAdjustedZ = z;
		}

		private void TransformAndTranslateVertices(Viewport viewport, Node node, Matrix view, Matrix projection, Matrix world)
		{
			var negativePivot = -(Vector)node.PivotPoint;

			var localView = (world * view);
			for (var vertexIndex = 0; vertexIndex < Vertices.Length; vertexIndex++)
			{
				var vertex = Vertices[vertexIndex];
				vertex.Vector += negativePivot;
				
				TransformAndTranslateVertex(ref vertex, viewport, localView, projection);
				CalculateColorForVertex(ref vertex, viewport, node);
				vertex.Vector -= negativePivot;
				Vertices[vertexIndex] = vertex;
			}
		}


		private void CalculateColorForVertex(ref Vertex vertex, Viewport viewport, Node node)
		{
			vertex.CalculatedColor = vertex.Color.Additive(_colorCalculator.Calculate(viewport, vertex.TransformedVector, vertex.TransformedNormal));
		}

		private void RenderFaces(Node node, Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if (null == Faces)
			{
				return;
			}
			for (var faceIndex = 0; faceIndex < Faces.Length; faceIndex++)
			{
				var face = Faces[faceIndex];

				var a = Vertices[face.A];
				var b = Vertices[face.B];
				var c = Vertices[face.C];

				face.Transform(world, view);
				face.Translate(projection, viewport.Width, viewport.Height);

				var mixedProduct = (b.TranslatedVector.X - a.TranslatedVector.X) * (c.TranslatedVector.Y - a.TranslatedVector.Y) -
								   (c.TranslatedVector.X - a.TranslatedVector.X) * (b.TranslatedVector.Y - a.TranslatedVector.Y);


				var visible = mixedProduct < 0; // && viewport.View.IsInView(a.TransformedVector);
				if( null != face.Material)
				{
					visible |= face.Material.DoubleSided;
				}
				if (!visible)
				{
					continue;
				}

				if (null != face.Material)
				{
					switch (face.Material.Shade)
					{
						case MaterialShade.None:
							{
								face.Color = face.Material.Diffuse;
								if( null != face.Material.DiffuseMap || null != face.Material.ReflectionMap )
								{
									TextureTriangleRenderer.Draw(face, Vertices);
								} else
								{
									FlatTriangleRenderer.Draw(face, Vertices);	 
								}
							}
							break;

						case MaterialShade.Flat:
							{
								var color = face.Material.Diffuse;
								face.Color = color.Additive(_colorCalculator.Calculate(viewport, face.TransformedPosition, face.TransformedNormal));
								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									TextureTriangleRenderer.Draw(face, Vertices);
								}
								else
								{
									FlatTriangleRenderer.Draw(face, Vertices);
								}
							}
							break;

						case MaterialShade.Gouraud:
							{
								var color = face.Material.Diffuse;
								Vertices[face.A].CalculatedColor = color.Additive(Vertices[face.A].CalculatedColor);
								Vertices[face.B].CalculatedColor = color.Additive(Vertices[face.B].CalculatedColor);
								Vertices[face.C].CalculatedColor = color.Additive(Vertices[face.C].CalculatedColor);

								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									TextureTriangleRenderer.Draw(face, Vertices);
								}
								else
								{
									GouraudTriangleRenderer.Draw(face, Vertices);
								}

							}
							break;
					}
				}
				else
				{
					var color = node.Color;
					var aColor = Vertices[face.A].CalculatedColor;
					var bColor = Vertices[face.B].CalculatedColor;
					var cColor = Vertices[face.C].CalculatedColor;
					Vertices[face.A].CalculatedColor = Vertices[face.A].CalculatedColor.Additive(color);
					Vertices[face.B].CalculatedColor = Vertices[face.B].CalculatedColor.Additive(color);
					Vertices[face.C].CalculatedColor = Vertices[face.C].CalculatedColor.Additive(color);
					GouraudTriangleRenderer.Draw(face, Vertices);
					Vertices[face.A].CalculatedColor = aColor;
					Vertices[face.B].CalculatedColor = bColor;
					Vertices[face.C].CalculatedColor = cColor;
				}
			}
		}

		private void RenderLines(Node node, Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if (null == Lines)
			{
				return;
			}
			for (var lineIndex = 0; lineIndex < Lines.Length; lineIndex++)
			{
				var line = Lines[lineIndex];
				var a = Vertices[line.A];
				var b = Vertices[line.B];
				var xstart = a.TranslatedScreenCoordinates.X;
				var ystart = a.TranslatedScreenCoordinates.Y;
				var xend = b.TranslatedScreenCoordinates.X;
				var yend = b.TranslatedScreenCoordinates.Y;
				Shapes.DrawLine(viewport,
								(int)xstart,
								(int)ystart,
								(int)xend,
								(int)yend, line.Color);
			}
		}
	}
}