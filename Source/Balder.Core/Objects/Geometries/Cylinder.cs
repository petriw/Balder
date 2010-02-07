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
using Balder.Core.Execution;

namespace Balder.Core.Objects.Geometries
{
	public class Cylinder : Geometry
	{
		private bool _isLoaded = false;

		public static readonly Property<Cylinder, double> TopRadiusProp = Property<Cylinder, double>.Register(c => c.TopRadius);
		public double TopRadius
		{
			get { return TopRadiusProp.GetValue(this); }
			set { TopRadiusProp.SetValue(this, value); }
		}

		public static readonly Property<Cylinder, double> BottomRadiusProp = Property<Cylinder, double>.Register(c => c.BottomRadius);
		public double BottomRadius
		{
			get { return BottomRadiusProp.GetValue(this); }
			set { BottomRadiusProp.SetValue(this, value); }
		}


		public static readonly Property<Cylinder, bool> CapEndsProp = Property<Cylinder, bool>.Register(c => c.CapEnds);
		public bool CapEnds
		{
			get { return CapEndsProp.GetValue(this); }
			set { CapEndsProp.SetValue(this, value); }
		}

		public static readonly Property<Cylinder, int> SegmentsProp = Property<Cylinder, int>.Register(c => c.Segments);
		public int Segments
		{
			get { return SegmentsProp.GetValue(this); }
			set { SegmentsProp.SetValue(this, value); }
		}

		public static readonly Property<Cylinder, int> StacksProp = Property<Cylinder, int>.Register(c => c.Stacks);
		public int Stacks
		{
			get { return StacksProp.GetValue(this); }
			set { StacksProp.SetValue(this, value); }
		}

		public static readonly Property<Cylinder, double> SizeProperty = Property<Cylinder, double>.Register(c => c.Size);
		public double Size
		{
			get { return SizeProperty.GetValue(this); }
			set { SizeProperty.SetValue(this, value); }
		}


		protected override void OnLoaded()
		{
			_isLoaded = true;
			PrepareCylinder();
			base.OnLoaded();
		}


		private void PrepareCylinder()
		{
			if (!_isLoaded)
			{
				return;
			}

			var actualStacks = Stacks + 1;
			var actualSegments = Segments;

			var deltaRadius = BottomRadius - TopRadius;
			var radiusAdd = deltaRadius / actualStacks;
			var currentRadius = TopRadius;

			var vertexCount = (actualStacks * actualSegments) + 2;
			var vertexIndex = 0;

			GeometryContext.AllocateVertices(vertexCount);
			GeometryContext.AllocateTextureCoordinates(vertexCount);


			var deltaY = Size;
			var yAdd = (float)(deltaY / (actualStacks - 1));
			var currentY = (float)deltaY / 2;

			var uDelta = 1f;
			var vDelta = 2f;

			var uAdd = uDelta / actualSegments;
			var vAdd = vDelta / actualStacks;

			var currentV = 0f;

			var thetaAdd = (System.Math.PI * 2) / actualSegments;

			Vertex vertex;

			for (var y = 0; y < actualStacks; y++)
			{
				var currentU = 0f;
				var currentTheta = 0d;
				for (var x = 0; x < actualSegments; x++)
				{
					var currentX = (float)(System.Math.Sin(currentTheta) * currentRadius);
					var currentZ = (float)(System.Math.Cos(currentTheta) * currentRadius);

					vertex = new Vertex(currentX, currentY, currentZ);
					GeometryContext.SetVertex(vertexIndex, vertex);

					var textureCoordinate = new TextureCoordinate(1f - currentU, currentV);
					GeometryContext.SetTextureCoordinate(vertexIndex, textureCoordinate);

					vertexIndex++;
					currentTheta += thetaAdd;
					currentU += uAdd;
				}

				currentRadius += radiusAdd;
				currentY -= yAdd;
				currentV += vAdd;
			}

			var centerTextureCoordinate = new TextureCoordinate(0.5f, 0.5f);
			vertex = new Vertex(0, (float)Size / 2f, 0);
			GeometryContext.SetVertex(vertexCount - 2, vertex);
			GeometryContext.SetTextureCoordinate(vertexCount - 2, centerTextureCoordinate);
			vertex = new Vertex(0, (float)-Size / 2f, 0);
			GeometryContext.SetVertex(vertexCount - 1, vertex);
			GeometryContext.SetTextureCoordinate(vertexCount - 1, centerTextureCoordinate);


			var faceIndex = 0;
			Face face;

			var faceCount = actualStacks * (actualSegments * 2);
			if (CapEnds)
			{
				faceCount += actualSegments * 2;
			}
			GeometryContext.AllocateFaces(faceCount);

			var vertexOffset = 0;
			var nextSegmentVertexOffset = 0;

			for (var y = 0; y < actualStacks - 1; y++)
			{
				vertexOffset = y * actualSegments;
				nextSegmentVertexOffset = (y + 1) * actualSegments;

				for (var x = 0; x < actualSegments; x++)
				{
					var nextX = (x + 1) % actualSegments;
					face = new Face(vertexOffset + x,
									vertexOffset + nextX,
									nextSegmentVertexOffset + x);
					face.DiffuseA = face.A;
					face.DiffuseB = face.B;
					face.DiffuseC = face.C;
					GeometryContext.SetFace(faceIndex, face);
					faceIndex++;

					face = new Face(nextSegmentVertexOffset + nextX,
									nextSegmentVertexOffset + x,
									vertexOffset + nextX);
					face.DiffuseA = face.A;
					face.DiffuseB = face.B;
					face.DiffuseC = face.C;
					GeometryContext.SetFace(faceIndex, face);

					faceIndex++;
				}
			}

			if (CapEnds)
			{
				vertexOffset = 0;
				for (var x = 0; x < actualSegments; x++)
				{
					var nextX = (x + 1) % actualSegments;
					face = new Face(vertexCount - 2,
									vertexOffset + nextX,
									vertexOffset + x);
					face.DiffuseA = face.A;
					face.DiffuseB = face.B;
					face.DiffuseC = face.C;
					GeometryContext.SetFace(faceIndex, face);
					faceIndex++;
				}

				vertexOffset = (actualStacks-1)*actualSegments;
				for (var x = 0; x < actualSegments; x++)
				{
					var nextX = (x + 1) % actualSegments;
					face = new Face(vertexOffset + x,
									vertexOffset + nextX,
									vertexCount - 1);
					face.DiffuseA = face.A;
					face.DiffuseB = face.B;
					face.DiffuseC = face.C;
					GeometryContext.SetFace(faceIndex, face);
					faceIndex++;
				}

			}

			GeometryHelper.CalculateFaceNormals(GeometryContext);
			GeometryHelper.CalculateVertexNormals(GeometryContext);
		}
	}
}
