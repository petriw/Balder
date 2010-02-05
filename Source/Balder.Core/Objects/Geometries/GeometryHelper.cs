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
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public static class GeometryHelper
	{
		public static void GeneratePlane(IGeometryContext context, int lengthSegments, int heightSegments, float width, float height, Vector lengthDirection, Vector heightDirection)
		{
			PrepareVertices(context, lengthSegments, heightSegments, width, height, lengthDirection, heightDirection);
			PrepareFaces(context, lengthSegments, heightSegments);
		}

		private static void PrepareVertices(IGeometryContext context, int lengthSegments, int heightSegments, float width, float height, Vector lengthDirection, Vector heightDirection)
		{
			context.AllocateVertices(lengthSegments * heightSegments);

			lengthDirection.Normalize();
			heightDirection.Normalize();


			var yStartVector = new Vector(0, 0, height/2);
			yStartVector.MultiplyWith(-heightDirection);
			var xStartVector = new Vector(width/2, 0, 0);
			xStartVector.MultiplyWith(-lengthDirection);

			var xStep = ((float)width) / (float)lengthSegments;
			var yStep = ((float)height) / (float)heightSegments;

			var uStep = ((float)1.0f) / (float)lengthSegments;
			var vStep = ((float)1.0f) / (float)heightSegments;

			var lengthStepVector = lengthDirection*xStep;
			var heightStepVector = heightDirection*yStep;

			var currentY = yStartVector;
			var v = 0f;
			var vertexIndex = 0;
			for (var y = 0; y < heightSegments; y++)
			{
				var currentX = xStartVector+currentY;

				var u = 0f;

				for (var x = 0; x < lengthSegments; x++)
				{
					var vertex = new Vertex(currentX.X, currentX.Y, currentX.Z) { Normal = Vector.Up };
					context.SetVertex(vertexIndex, vertex);
					currentX += lengthStepVector;
					u += uStep;
					vertexIndex++;
				}

				currentY += heightStepVector;
				v += vStep;
			}
		}

		private static void PrepareFaces(IGeometryContext context, int lengthSegments, int heightSegments)
		{
			var faceCount = ((lengthSegments - 1) * 2) * (heightSegments - 1);
			context.AllocateFaces(faceCount);
			var faceIndex = 0;

			for (var y = 0; y < heightSegments - 1; y++)
			{
				for (var x = 0; x < lengthSegments - 1; x++)
				{
					var offset = (y * lengthSegments) + x;
					var offsetNextLine = offset + lengthSegments;
					var face = new Face(offset, offset + 1, offsetNextLine);
					face.Normal = Vector.Up;
					context.SetFace(faceIndex, face);
					face = new Face(offsetNextLine + 1, offsetNextLine, offset + 1);
					face.Normal = Vector.Up;
					context.SetFace(faceIndex + 1, face);
					faceIndex += 2;
				}
			}
		}




		public static void CalculateVertexNormals(IGeometryContext context)
		{
			var vertexCount = new Dictionary<int, int>();
			var vertexNormal = new Dictionary<int, Vector>();

			var vertices = context.GetVertices();
			var faces = context.GetFaces();

			Func<int, Vector, int> addNormal =
				delegate(int vertex, Vector normal)
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

			foreach (var face in faces)
			{
				addNormal(face.A, face.Normal);
				addNormal(face.B, face.Normal);
				addNormal(face.C, face.Normal);
			}

			foreach (var vertex in vertexNormal.Keys)
			{
				var addedNormals = vertexNormal[vertex];
				var count = vertexCount[vertex];

				var normal = new Vector(addedNormals.X / count,
				                        addedNormals.Y / count,
				                        addedNormals.Z / count);
				vertices[vertex].Normal = normal;
			}
		}
	}
}