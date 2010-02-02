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
using Balder.Core.Execution;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public class Heightmap : Geometry
	{
		private static readonly HeightmapEventArgs EventArgs = new HeightmapEventArgs();
		public event EventHandler<HeightmapEventArgs> HeightInput;
		private bool _isLoaded = false;

		public Heightmap()
		{
			LengthSegments = 1;
			HeightSegments = 1;
		}

		public static Property<Heightmap, int> LengthSegmentsProperty = Property<Heightmap, int>.Register(p => p.LengthSegments);
		public int LengthSegments
		{
			get { return LengthSegmentsProperty.GetValue(this); }
			set
			{
				LengthSegmentsProperty.SetValue(this, value);
				PreparePlane();
			}
		}

		public static Property<Heightmap, int> HeightSegmentsProperty = Property<Heightmap, int>.Register(p => p.HeightSegments);
		public int HeightSegments
		{
			get { return HeightSegmentsProperty.GetValue(this); }
			set
			{
				HeightSegmentsProperty.SetValue(this, value);
				PreparePlane();
			}
		}
		
		public static Property<Heightmap, Dimension> DimensionProperty = Property<Heightmap, Dimension>.Register(p => p.Dimension);
		public Dimension Dimension
		{
			get { return DimensionProperty.GetValue(this); }
			set
			{
				DimensionProperty.SetValue(this, value);
				PreparePlane();
			}
		}

		protected override void OnLoaded()
		{
			_isLoaded = true;
			PreparePlane();
			base.OnLoaded();
		}

		public override void PrepareForRendering(Display.Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if( null != HeightInput )
			{
				var vertexIndex = 0;
				var vertices = GeometryContext.GetVertices();
				for( var y=0; y<HeightSegments; y++ )
				{
					var offset = y*LengthSegments;
					for( var x=0; x<LengthSegments; x++ )
					{
						var vertex = vertices[offset + x];
						var heightBefore = vertex.Vector.Y;
						var colorBefore = vertex.Color;
						EventArgs.Color = Color.Black;
						EventArgs.ActualVertex = vertex;
						EventArgs.GridX = x;
						EventArgs.GridY = y;

						HeightInput(this,EventArgs);

						if( heightBefore != EventArgs.Height ||
							!colorBefore.Equals(EventArgs.Color) )
						{
							vertex.Vector.Y = EventArgs.Height;
							vertex.Color = EventArgs.Color;
							GeometryContext.SetVertex(vertexIndex,vertex);
						}

						vertexIndex++;
					}
				}
			}
			base.PrepareForRendering(viewport, view, projection, world);
		}

		public void SetHeightForGridPoint(int gridX, int gridY, float height)
		{
			SetHeightForGridPoint(gridX,gridY,height,Color.Black);
		}


		public void SetHeightForGridPoint(int gridX, int gridY, float height, Color color)
		{
			if( gridX >= LengthSegments || gridY >= HeightSegments )
			{
				throw new ArgumentException("Point outside grid");
			}

			var xStep = ((float)Dimension.Width) / (float)LengthSegments;
			var yStep = ((float)Dimension.Height) / (float)HeightSegments;
			var yStart = (float)-(Dimension.Height / 2);
			var xStart = (float)-(Dimension.Width / 2);

			var xPos = xStart + (xStep*gridX);
			var zPos = yStart + (yStep*gridY);

			var index = (gridY*LengthSegments)+gridX;

			var vertex = new Vertex(xPos, height, zPos);
			vertex.Color = color;
			GeometryContext.SetVertex(index,vertex);
		}


		private void PreparePlane()
		{
			if (!_isLoaded)
			{
				return;
			}
			if (LengthSegments <= 0 || HeightSegments <= 0)
			{
				throw new ArgumentException("LengthSegments and HeightSegments must be 1 or more");
			}

			PrepareVertices();
			PrepareFaces();
		}


		private void PrepareVertices()
		{
			GeometryContext.AllocateVertices(LengthSegments * HeightSegments);
			var yStart = (float)-(Dimension.Height / 2);
			var xStep = ((float)Dimension.Width) / (float)LengthSegments;
			var yStep = ((float)Dimension.Height) / (float)HeightSegments;

			var uStep = ((float)1.0f) / (float)LengthSegments;
			var vStep = ((float)1.0f) / (float)HeightSegments;

			var v = 0f;
			var vertexIndex = 0;
			for (var y = 0; y < HeightSegments; y++)
			{
				var xStart = (float)-(Dimension.Width / 2);

				var u = 0f;

				for (var x = 0; x < LengthSegments; x++)
				{
					var vertex = new Vertex(xStart, 0, yStart) { Normal = Vector.Up };
					GeometryContext.SetVertex(vertexIndex, vertex);
					xStart += xStep;
					u += uStep;
					vertexIndex++;
				}

				yStart += yStep;
				v += vStep;
			}
		}

		private void PrepareFaces()
		{
			var faceCount = ((LengthSegments - 1)*2)*(HeightSegments-1);
			GeometryContext.AllocateFaces(faceCount);
			var faceIndex = 0;

			for (var y = 0; y < HeightSegments - 1; y++)
			{
				for (var x = 0; x < LengthSegments - 1; x++)
				{
					var offset = (y*LengthSegments)+x;
					var offsetNextLine = offset + LengthSegments;
					var face = new Face(offset, offset + 1, offsetNextLine);
					face.Normal = Vector.Up;
					GeometryContext.SetFace(faceIndex,face);
					face = new Face(offsetNextLine + 1, offsetNextLine, offset + 1);
					face.Normal = Vector.Up;
					GeometryContext.SetFace(faceIndex+1, face);

					
					faceIndex += 2;
				}
			}
		}
	}
}
