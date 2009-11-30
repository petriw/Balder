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
using Balder.Core.Objects.Geometries;

namespace Balder.Core.Debug
{
	public class BoundingSphereDebugShape : DebugShape
	{
		private const int NumberOfCirclePoints = 32;


		public override void Initialize()
		{
			InitializeVertices();
			InitializeLines();
		}

		private void InitializeVertices()
		{
			const float angle = MathHelper.TwoPi / NumberOfCirclePoints;

			GeometryContext.AllocateVertices(NumberOfCirclePoints);

			for (var vertexIndex = 0; vertexIndex < NumberOfCirclePoints; vertexIndex++)
			{
				var x = (float)System.Math.Round(System.Math.Sin(angle * vertexIndex), 4);
				var y = (float)System.Math.Round(System.Math.Cos(angle * vertexIndex), 4);
				var vertex = new Vertex(x, y, 0f);
				GeometryContext.SetVertex(vertexIndex, vertex);
			}
		}

		private void InitializeLines()
		{
			GeometryContext.AllocateLines(NumberOfCirclePoints + 1);
			for (var vertexIndex = 0; vertexIndex < NumberOfCirclePoints-1; vertexIndex++)
			{
				var line = new Line(vertexIndex, vertexIndex + 1);
				line.Color = DebugRenderer.DebugInfoColor;
				GeometryContext.SetLine(vertexIndex,line);
			}
			var lastLine = new Line(NumberOfCirclePoints - 1, 0);
			lastLine.Color = DebugRenderer.DebugInfoColor;
			GeometryContext.SetLine(NumberOfCirclePoints-1, lastLine);
		}
	}
}
