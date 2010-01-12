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
	public class Cylinder : Geometry
	{
		public static readonly Property<Cylinder, float> RadiusProp = Property<Cylinder, float>.Register(c => c.Radius);
		public float Radius
		{
			get { return RadiusProp.GetValue(this); }
			set { RadiusProp.SetValue(this, value); }
		}

		public static readonly Property<Cylinder, float> DepthProp = Property<Cylinder, float>.Register(c => c.Depth);
		public float Depth
		{
			get { return DepthProp.GetValue(this); }
			set { DepthProp.SetValue(this, value); }
		}

		public static readonly Property<Cylinder, bool> CapEndsProp = Property<Cylinder, bool>.Register(c => c.CapEnds);
		public bool CapEnds
		{
			get { return CapEndsProp.GetValue(this); }
			set { CapEndsProp.SetValue(this, value); }
		}

		public static readonly Property<Cylinder, int> SidesProp = Property<Cylinder, int>.Register(c => c.Sides);
		public int Sides
		{
			get { return SidesProp.GetValue(this); }
			set { SidesProp.SetValue(this, value); }
		}

		public static readonly Property<Cylinder, int> SegmentsProp = Property<Cylinder, int>.Register(c => c.Segments);
		public int Segments
		{
			get { return SegmentsProp.GetValue(this); }
			set { SegmentsProp.SetValue(this, value); }
		}
	}
}
