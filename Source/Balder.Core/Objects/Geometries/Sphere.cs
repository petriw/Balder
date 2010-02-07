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

using Balder.Core.Execution;

namespace Balder.Core.Objects.Geometries
{
	public class Sphere : Geometry
	{
		private bool _isLoaded = false;

		public static readonly Property<Sphere, double> RadiusProperty = Property<Sphere, double>.Register(s => s.Radius);
		public double Radius
		{
			get { return RadiusProperty.GetValue(this); }
			set
			{
				RadiusProperty.SetValue(this, value);
				PrepareSphere();
			}
		}

		public static readonly Property<Sphere, int> SegmentsProperty = Property<Sphere, int>.Register(s => s.Segments);
		public int Segments
		{
			get { return SegmentsProperty.GetValue(this); }
			set
			{
				SegmentsProperty.SetValue(this, value);
				PrepareSphere();
			}
		}

		protected override void OnLoaded()
		{
			_isLoaded = true;
			PrepareSphere();
			base.OnLoaded();
		}


		private void PrepareSphere()
		{
			if (!_isLoaded)
			{
				return;
			}

			var radius = (float)Radius;
			var segments = Segments;
			var segmentRadius = System.Math.PI / 2 / (segments + 1);

			var ySegments = (int)segments * 2;
			var xSegments = 4 * segments + 4;
			int ySegment = 0;

			var vertexCount = (int)(xSegments) * (int)ySegments;
			GeometryContext.AllocateVertices(vertexCount);

			Vertex vertex;
			var vertexIndex = 0;
			var currentYSegment = -segments;
			for (ySegment = -segments; ySegment < segments; ySegment++)
			{
				var r = radius * System.Math.Cos(segmentRadius * ySegment);
				var y = radius * System.Math.Sin(segmentRadius * ySegment);

				for (var xSegment = 0; xSegment < xSegments - 1; xSegment++)
				{
					var z = r * System.Math.Sin(segmentRadius * xSegment) * (-1);
					var x = r * System.Math.Cos(segmentRadius * xSegment);

					vertex = new Vertex((float)x, (float)y, (float)z);
					GeometryContext.SetVertex(vertexIndex, vertex);

					vertexIndex++;
				}
			}

			vertex = new Vertex(0, radius, 0);
			GeometryContext.SetVertex(vertexIndex++, vertex);
			vertex = new Vertex(0, -1 * radius, 0);
			GeometryContext.SetVertex(vertexIndex, vertex);

			var faceCount = ((ySegments * segments) * 2);
			GeometryContext.AllocateFaces(faceCount);

			var faceIndex = 0;

			for (ySegment = 0; ySegment < ySegments; ySegment++)
			{
				for (var xSegment = 0; xSegment < segments; xSegment++)
				{

					var index = ySegment * segments + xSegment;
					var face = new Face(
							index,
							index + segments,
							index + 1 % segments + segments);
					GeometryContext.SetFace(faceIndex, face);
					faceIndex++;

					face = new Face(
							index + 1 % segments + segments,
							index + 1 % segments,
							index);
					GeometryContext.SetFace(faceIndex, face);
					faceIndex++;
				}
			}

			/*
			for (var xSegment = 0; xSegment < segments; xSegment++)
			{
				var index = ySegment * segments + xSegment;
				var face = new Face(
						index,
						index + 1 % segments + segments,
						segments * (2 * xSegment + 1));
				GeometryContext.SetFace(faceIndex, face);
			}

			for (var xSegment = 0; xSegment < segments; xSegment++)
			{
				var face = new Face(
						ySegment,
						ySegment + 1 % segments,
						segments * (2 * xSegment + 1)+1);
				GeometryContext.SetFace(faceIndex, face);
			}*/

			GeometryHelper.CalculateVertexNormals(GeometryContext);
		}


	}
}
