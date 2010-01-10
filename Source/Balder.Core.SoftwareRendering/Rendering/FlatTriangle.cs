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


		public void Draw(IBuffers buffers, Face face, Vertex[] vertices)
		{
			var vertexA = vertices[face.A];
			var vertexB = vertices[face.B];
			var vertexC = vertices[face.C];

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;


			var deltaX1 = xb - xa;
			var deltaX2 = xc - xb;
			var deltaX3 = xc - xa;

			var deltaY1 = yb - ya;
			var deltaY2 = yc - yb;
			var deltaY3 = yc - ya;


		}
	}
}
