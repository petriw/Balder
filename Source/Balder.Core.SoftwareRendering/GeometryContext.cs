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
using System.Collections.Generic;
using Balder.Core.Display;
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using Matrix=Balder.Core.Math.Matrix;

namespace Balder.Core.SoftwareRendering
{
	public class GeometryContext : IGeometryContext
	{
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

		public void Prepare()
		{
			var vertexCount = new Dictionary<Vertex, int>();
			var vertexNormal = new Dictionary<Vertex, Vector>();

			Func<Vertex, Vector, int> AddNormal =
				delegate(Vertex vertex, Vector normal)
				{
					if (!vertexNormal.ContainsKey(vertex))
					{
						vertexNormal[vertex] = normal;
						vertexCount[vertex] = 1;
					}
					else
					{
						vertexNormal[vertex] += normal;
						vertexCount[vertex]++;
					}
					return 0;
				};

			foreach (var face in Faces)
			{
				AddNormal(Vertices[face.A], face.Normal);
				AddNormal(Vertices[face.B], face.Normal);
				AddNormal(Vertices[face.C], face.Normal);
			}

			/*
			foreach (var vertex in vertexNormal.Keys)
			{
				var addedNormals = vertexNormal[vertex];
				var count = vertexCount[vertex];

				var normal = new Vector(addedNormals.X / count,
											addedNormals.Y / count,
											addedNormals.Z / count);

				

				vertex.Normal = normal;
			}
			 * */
		}

		private static ISpanRenderer SpanRenderer = new SimpleSpanRenderer();

		public void Render(Viewport viewport, RenderableNode node, Matrix view, Matrix projection, Matrix world)
		{
			TransformAndTranslateVertices(viewport, node,view,projection,world);
			RenderFaces(viewport, view, projection, world);
			RenderLines(viewport, view, projection, world);
		}

		private void TransformAndTranslateVertex(ref Vertex vertex, Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			var matrix = (world*view)*projection;
			vertex.Transform(world, matrix);
			vertex.Translate(projection, viewport.Width, viewport.Height);
			vertex.MakeScreenCoordinates();
			vertex.TransformedVectorNormalized = vertex.TransformedNormal;
			vertex.TransformedVectorNormalized.Normalize();
			var z = ((vertex.TransformedVector.Z/viewport.View.DepthDivisor) + viewport.View.DepthZero);
			vertex.DepthBufferAdjustedZ = z;
			
		}

		private void TransformAndTranslateVertices(Viewport viewport, RenderableNode node, Matrix view, Matrix projection, Matrix world)
		{
			for (var vertexIndex = 0; vertexIndex < Vertices.Length; vertexIndex++)
			{
				var vertex = Vertices[vertexIndex];
				TransformAndTranslateVertex(ref vertex, viewport, view, projection, world);
				CalculateColorForVertex(ref vertex, viewport, node);
				Vertices[vertexIndex] = vertex;
			}
		}


		private void CalculateColorForVertex(ref Vertex vertex, Viewport viewport, RenderableNode node)
		{
			vertex.Color = viewport.Scene.CalculateColorForVector(viewport, vertex.TransformedVector, vertex.Normal, node.Color, node.Color, node.Color);
		}

		private void RenderFaces(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if( null == Faces )
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

				var visible = (mixedProduct < 0) && viewport.View.IsInView(a.TransformedVector);
				if (!visible)
				{
					continue;
				}

				face.Color = viewport.Scene.CalculateColorForVector(viewport, face.TransformedPosition, face.TransformedNormal);
				Triangle.Draw(BufferManager.Instance.Current, SpanRenderer, TriangleShade.Gouraud, face, Vertices,
							  TextureCoordinates);
			}
		}

		private void RenderLines(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if( null == Lines )
			{
				return;
			}
			for( var lineIndex=0; lineIndex< Lines.Length; lineIndex++)
			{
				var line = Lines[lineIndex];
				var a = Vertices[line.A];
				var b = Vertices[line.B];
				var xstart = a.TranslatedScreenCoordinates.X;
				var ystart = a.TranslatedScreenCoordinates.Y;
				var xend = b.TranslatedScreenCoordinates.X;
				var yend = b.TranslatedScreenCoordinates.Y;
				Shapes.DrawLine(viewport,
								BufferManager.Instance.Current, 
								(int)xstart, 
								(int)ystart, 
								(int)xend, 
								(int)yend, line.Color);
			}
		}
	}
}