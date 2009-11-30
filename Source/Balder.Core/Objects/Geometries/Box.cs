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
using System.Collections.Generic;

namespace Balder.Core.Objects.Geometries
{
	public enum BoxSide
	{
		Front = 1,
		Back,
		Top,
		Bottom,
		Left,
		Right
	}

	public class Box : Geometry
	{
		public struct BoxSideDescriptor
		{
			internal BoxSideDescriptor(Face face1, Face face2)
				: this()
			{
				Face1 = face1;
				Face2 = face2;
			}

			public Face Face1 { get; set; }
			public Face Face2 { get; set; }
		}

		public Dictionary<BoxSide, BoxSideDescriptor> Sides { get; private set; }

		public Box()
			: this(10, 10, 10)
		{

		}

		public Box(float width, float height, float depth)
		{
			Width = width;
			Height = height;
			Depth = depth;

			Sides = new Dictionary<BoxSide, BoxSideDescriptor>();

			PrepareVertices();
			PrepareFaces();
		}

		public float Width { get; private set; }
		public float Height { get; private set; }
		public float Depth { get; private set; }

		private void PrepareVertices()
		{
			var absX = Width / 2f;
			var absY = Height / 2f;
			var absZ = Depth / 2f;
			
			GeometryContext.AllocateVertices(8);
			GeometryContext.SetVertex(0,new Vertex(-absX, -absY, -absZ));
			GeometryContext.SetVertex(1,new Vertex(absX, -absY, -absZ));
			GeometryContext.SetVertex(2,new Vertex(-absX, absY, -absZ));
			GeometryContext.SetVertex(3,new Vertex(absX, absY, -absZ));

			GeometryContext.SetVertex(4,new Vertex(-absX, -absY, absZ));
			GeometryContext.SetVertex(5,new Vertex(absX, -absY, absZ));
			GeometryContext.SetVertex(6,new Vertex(-absX, absY, absZ));
			GeometryContext.SetVertex(7,new Vertex(absX, absY, absZ));
		}

		private void PrepareFaces()
		{
			GeometryContext.AllocateFaces(12);


			Sides[BoxSide.Front] = new BoxSideDescriptor(
				new Face(2, 1, 0),
				new Face(1, 2, 3));

			Sides[BoxSide.Back] = new BoxSideDescriptor(
				new Face(4, 5, 6),
				new Face(7, 6, 5));

			Sides[BoxSide.Left] = new BoxSideDescriptor(
				new Face(0, 4, 6),
				new Face(0, 6, 2));

			Sides[BoxSide.Right] = new BoxSideDescriptor(
				new Face(7, 5, 1),
				new Face(3, 7, 1));

			Sides[BoxSide.Top] = new BoxSideDescriptor(
				new Face(5, 4, 0),
				new Face(1, 5, 0));

			Sides[BoxSide.Bottom] = new BoxSideDescriptor(
				new Face(2, 6, 7),
				new Face(2, 7, 3));

			var faceIndex = 0;
			var enumerator = Sides.Values.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var boxSide = (BoxSideDescriptor)enumerator.Current;

				GeometryContext.SetFace(faceIndex, boxSide.Face1);
				GeometryContext.SetFace(faceIndex + 1, boxSide.Face2);
				faceIndex += 2;
			}
		}
	}
}