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
using Balder.Core.Execution;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public class Ring : Geometry
	{
		private bool _isLoaded = false;

		public static readonly Property<Ring, double> InnerRadiusProp = Property<Ring, double>.Register(c => c.InnerRadius);
		public double InnerRadius
		{
			get { return InnerRadiusProp.GetValue(this); }
			set { InnerRadiusProp.SetValue(this, value); }
		}

		public static readonly Property<Ring, double> OuterRadiusProp = Property<Ring, double>.Register(c => c.OuterRadius);
		public double OuterRadius
		{
			get { return OuterRadiusProp.GetValue(this); }
			set { OuterRadiusProp.SetValue(this, value); }
		}

		public static readonly Property<Ring, bool> CapEndsProp = Property<Ring, bool>.Register(c => c.CapEnds);
		public bool CapEnds
		{
			get { return CapEndsProp.GetValue(this); }
			set { CapEndsProp.SetValue(this, value); }
		}


		public static readonly Property<Ring, int> SegmentsProp = Property<Ring, int>.Register(c => c.Segments);
		public int Segments
		{
			get { return SegmentsProp.GetValue(this); }
			set { SegmentsProp.SetValue(this, value); }
		}

		public static readonly Property<Ring, int> StacksProp = Property<Ring, int>.Register(c => c.Stacks);
		public int Stacks
		{
			get { return StacksProp.GetValue(this); }
			set { StacksProp.SetValue(this, value); }
		}

		public static readonly Property<Ring, double> SizeProperty = Property<Ring, double>.Register(c => c.Size);
		public double Size
		{
			get { return SizeProperty.GetValue(this); }
			set { SizeProperty.SetValue(this, value); }
		}

		public static readonly Property<Ring, double> StartAngleProperty = Property<Ring, double>.Register(c => c.StartAngle);
		public double StartAngle
		{
			get { return StartAngleProperty.GetValue(this); }
			set { StartAngleProperty.SetValue(this, value); }
		}

		public static readonly Property<Ring, double> EndAngleProperty = Property<Ring, double>.Register(c => c.EndAngle);
		public double EndAngle
		{
			get { return EndAngleProperty.GetValue(this); }
			set { EndAngleProperty.SetValue(this, value); }
		}


		public Ring()
		{
			StartAngle = 0;
			EndAngle = 360;
			Segments = 8;
			Stacks = 1;
			CapEnds = true;
		}

		protected override void OnLoaded()
		{
			_isLoaded = true;
			Prepare();
			base.OnLoaded();
		}

		private void Validate()
		{
			if( InnerRadius <= 0 || OuterRadius <= 0 )
			{
				throw new ArgumentException("Top and Bottom radius must be set to a number higher than 0");
			}

			if( InnerRadius >= OuterRadius )
			{
				throw new ArgumentException("Inner radius must be less than outer radius");
			}

			if( Segments <= 2 )
			{
				throw new ArgumentException("You must have at least 2 segments");
			}

			if( StartAngle > EndAngle )
			{
				throw new ArgumentException("StartAngle must be less than EndAngle");
			}

			if( StartAngle < 0 || EndAngle < 0 )
			{
				throw new ArgumentException("Start or End angle must be 0 or more");
			}

			if( StartAngle > 360 || EndAngle > 360 )
			{
				throw new ArgumentException("Start or End angle must be 360 or less");
			}
			
			if( Stacks < 1 )
			{
				throw new ArgumentException("You must have at least 1 stack");
			}
		}

		private void Prepare()
		{
			if (!_isLoaded)
			{
				return;
			}

			Validate();

			var actualStacks = Stacks + 1;
			var actualSegments = Segments;

			var yStart = (float)Size/2;
			var yEnd = (float)-Size/2;

			var yDelta = yEnd - yStart;
			var yAdd = yDelta/actualStacks;

			var startRadian = MathHelper.ToRadians((float)StartAngle);
			var endRadian = MathHelper.ToRadians((float) EndAngle);
			var deltaRadian = endRadian - startRadian;
			var addRadian = deltaRadian/actualSegments;

			var vertexCount = (actualSegments*2)*actualStacks;
			GeometryContext.AllocateVertices(vertexCount);

			var vertexIndex = 0;
			var currentY = yStart;
			for( var stack=0; stack<actualStacks; stack++ )
			{
				var currentRadian = startRadian;
				for( var segment=0; segment<actualSegments; segment++)
				{
					var innerX = (float) (System.Math.Sin(currentRadian)*InnerRadius);
					var innerZ = (float) (System.Math.Cos(currentRadian)*InnerRadius);

					var innerVertex = new Vertex(innerX, currentY, innerZ);
					GeometryContext.SetVertex(vertexIndex, innerVertex);
					vertexIndex++;

					var outerX = (float)(System.Math.Sin(currentRadian) * OuterRadius);
					var outerZ = (float)(System.Math.Cos(currentRadian) * OuterRadius);

					var outerVertex = new Vertex(outerX, currentY, outerZ);
					GeometryContext.SetVertex(vertexIndex, outerVertex);
					vertexIndex++;
					currentRadian += addRadian;
				}

				currentY -= yAdd;
			}
		}
	}
}
