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
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public class Box : Geometry
	{
		private bool _isLoaded = false;

		public static Property<Box, Coordinate> DimensionProperty = Property<Box, Coordinate>.Register(p => p.Dimension);
		public Coordinate Dimension
		{
			get { return DimensionProperty.GetValue(this); }
			set
			{
				DimensionProperty.SetValue(this, value);
				PrepareBox();
			}
		}


		protected override void OnLoaded()
		{
			_isLoaded = true;
			PrepareBox();
			base.OnLoaded();
		}


		private void PrepareBox()
		{
			if (!_isLoaded)
			{
				return;
			}

			var dimensionAsVector = (Vector)Dimension;
			var halfDimension = dimensionAsVector / 2f;

			var frontUpperRight = new Vertex(-halfDimension.X, halfDimension.Y, -halfDimension.Z);
			var frontUpperLeft = new Vertex(halfDimension.X, halfDimension.Y, -halfDimension.Z);
			var frontLowerRight = new Vertex(-halfDimension.X, -halfDimension.Y, -halfDimension.Z);
			var frontLowerLeft = new Vertex(halfDimension.X, -halfDimension.Y, -halfDimension.Z);

			var backUpperRight = new Vertex(-halfDimension.X, halfDimension.Y, halfDimension.Z);
			var backUpperLeft = new Vertex(halfDimension.X, halfDimension.Y, halfDimension.Z);
			var backLowerRight = new Vertex(-halfDimension.X, -halfDimension.Y, halfDimension.Z);
			var backLowerLeft = new Vertex(halfDimension.X, -halfDimension.Y, halfDimension.Z);

			GeometryContext.AllocateVertices(8);
			GeometryContext.SetVertex(0, frontUpperRight);
			GeometryContext.SetVertex(1, frontUpperLeft);
			GeometryContext.SetVertex(2, frontLowerRight);
			GeometryContext.SetVertex(3, frontLowerLeft);

			GeometryContext.SetVertex(4, backUpperRight);
			GeometryContext.SetVertex(5, backUpperLeft);
			GeometryContext.SetVertex(6, backLowerRight);
			GeometryContext.SetVertex(7, backLowerLeft);

			GeometryContext.AllocateTextureCoordinates(4);
			GeometryContext.SetTextureCoordinate(0, new TextureCoordinate(0f, 0f));
			GeometryContext.SetTextureCoordinate(1, new TextureCoordinate(1f, 0f));
			GeometryContext.SetTextureCoordinate(2, new TextureCoordinate(0f, 1f));
			GeometryContext.SetTextureCoordinate(3, new TextureCoordinate(1f, 1f));

			GeometryContext.AllocateFaces(12);

			GeometryContext.SetFace(0, new Face(2, 1, 0) { Normal = Vector.Backward, DiffuseA = 2, DiffuseB = 1, DiffuseC = 0});
			GeometryContext.SetFace(1, new Face(1, 2, 3) { Normal = Vector.Backward, DiffuseA = 1, DiffuseB = 2, DiffuseC = 3});

			GeometryContext.SetFace(2, new Face(4, 5, 6) { Normal = Vector.Forward, DiffuseA = 1, DiffuseB = 0, DiffuseC = 3 });
			GeometryContext.SetFace(3, new Face(7, 6, 5) { Normal = Vector.Forward, DiffuseA = 2, DiffuseB = 3, DiffuseC = 0 });

			GeometryContext.SetFace(4, new Face(0, 4, 2) { Normal = Vector.Left, DiffuseA = 1, DiffuseB = 0, DiffuseC = 3 });
			GeometryContext.SetFace(5, new Face(6, 2, 4) { Normal = Vector.Left, DiffuseA = 2, DiffuseB = 3, DiffuseC = 0 });

			GeometryContext.SetFace(6, new Face(3, 5, 1) { Normal = Vector.Right, DiffuseA = 2, DiffuseB = 1, DiffuseC = 0 });
			GeometryContext.SetFace(7, new Face(5, 3, 7) { Normal = Vector.Right, DiffuseA = 1, DiffuseB = 2, DiffuseC = 3 });

			GeometryContext.SetFace(8, new Face(0, 1, 4) { Normal = Vector.Up, DiffuseA = 2, DiffuseB = 3, DiffuseC = 0 });
			GeometryContext.SetFace(9, new Face(5, 4, 1) { Normal = Vector.Up, DiffuseA = 1, DiffuseB = 0, DiffuseC = 3 });

			GeometryContext.SetFace(10, new Face(6, 3, 2) { Normal = Vector.Down, DiffuseA = 2, DiffuseB = 1, DiffuseC = 0 });
			GeometryContext.SetFace(11, new Face(3, 6, 7) { Normal = Vector.Down, DiffuseA = 1, DiffuseB = 2, DiffuseC = 3 });


			GeometryHelper.CalculateVertexNormals(GeometryContext);
		}
	}
}
