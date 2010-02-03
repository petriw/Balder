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

namespace Balder.Core.Objects.Geometries
{
	public struct Vertex
	{
		public Vertex(float x, float y, float z) : this()
		{
			Vector = new Vector(x, y, z);
			TransformedVector = new Math.Vector(x, y, z);
			TranslatedVector = new Math.Vector(x, y, z);
			Normal = Vector.Zero;
			TranslatedScreenCoordinates = Vector.Zero;
		}

		public static Vertex Zero = new Vertex(0,0,0);

		public Vector Vector;
		public Vector TransformedVector;
		public Vector TranslatedVector;
		public Vector Normal;
		public Vector TransformedNormal;
		public Vector TransformedVectorNormalized;
		public Vector TranslatedScreenCoordinates;
		public float DepthBufferAdjustedZ;
		public Color Color;
		public Color CalculatedColor;

		public TextureCoordinate TextureCoordinate;

		public void Transform(Matrix matrix)
		{
			TransformedVector = Vector.Transform(Vector, matrix);
			TransformedNormal = Vector.TransformNormal(Normal, matrix);
		}


		public void Translate(Matrix projectionMatrix, float width, float height)
		{
			TranslatedVector = Vector.Translate(TransformedVector, projectionMatrix, width, height);
		}

		public void MakeScreenCoordinates()
		{
			TranslatedScreenCoordinates.X = (int)TranslatedVector.X;
			TranslatedScreenCoordinates.Y = (int)TranslatedVector.Y;
			TranslatedScreenCoordinates.Z = (int)TranslatedVector.Z;
		}
	}
}