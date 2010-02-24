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
using Balder.Core.Materials;
using Balder.Core.Math;
using Matrix = Balder.Core.Math.Matrix;

namespace Balder.Core.Objects.Geometries
{
	public struct Face
	{
		public static readonly float DebugNormalLength = 5f;
		public Vector Normal;
		public Vector TransformedNormal;
		public Vector Position;
		public Vector TransformedPosition;
		public Vector TranslatedPosition;
		public Vector TransformedDebugNormal;
		public Vector TranslatedDebugNormal;

		public Material Material;
		public Color Color;

		public int A;
		public int B;
		public int C;

		public int DiffuseA;
		public int DiffuseB;
		public int DiffuseC;

		public TextureCoordinate DiffuseTextureCoordinateA;
		public TextureCoordinate DiffuseTextureCoordinateB;
		public TextureCoordinate DiffuseTextureCoordinateC;

		public int SmoothingGroupA;
		public int SmoothingGroupB;
		public int SmoothingGroupC;

		public Vector VertexNormalA;
		public Vector VertexNormalB;
		public Vector VertexNormalC;

		public Face(int a, int b, int c)
			: this()
		{
			A = a;
			B = b;
			C = c;
		}

		public void Transform(Matrix matrix)
		{
			TransformedNormal = Vector.TransformNormal(Normal, matrix);
			TransformedPosition = Vector.Transform(Position, matrix);
		}

		public void Transform(Matrix world, Matrix view)
		{
			TransformedNormal = Vector.TransformNormal(Normal, world);
			TransformedNormal = Vector.TransformNormal(TransformedNormal, view);
			TransformedPosition = Vector.Transform(Position, world, view);

			TransformedDebugNormal = TransformedPosition +(TransformedNormal); //*DebugNormalLength);
		}

		public void Translate(Matrix projectionMatrix, float width, float height)
		{
			TranslatedPosition = Vector.Translate(TransformedPosition, projectionMatrix, width, height);
			TranslatedDebugNormal = Vector.Translate(TransformedDebugNormal, projectionMatrix, width, height);
		}
	}
}