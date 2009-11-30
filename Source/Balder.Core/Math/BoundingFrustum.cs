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
using System.Globalization;

namespace Balder.Core.Math
{
	public class BoundingFrustum : IEquatable<BoundingFrustum>
	{
		public const int CornerCount = 8;
		private const int BottomPlaneIndex = 5;
		private const int FarPlaneIndex = 1;
		private const int LeftPlaneIndex = 2;
		private const int NearPlaneIndex = 0;
		private const int NumPlanes = 6;
		private const int RightPlaneIndex = 3;
		private const int TopPlaneIndex = 4;

		internal Vector[] cornerArray;

		private Gjk gjk;

		private Matrix matrix;
		private Plane[] planes;


		private BoundingFrustum()
		{
			planes = new Plane[6];
			cornerArray = new Vector[8];
		}

		public BoundingFrustum(Matrix value)
		{
			planes = new Plane[6];
			cornerArray = new Vector[8];
			SetMatrix(ref value);
		}

		private static Vector ComputeIntersection(ref Plane plane, ref Ray ray)
		{
			float num = (-plane.Distance - Vector.Dot(plane.Normal, ray.Position)) / Vector.Dot(plane.Normal, ray.Direction);
			return (ray.Position + ((Vector)(ray.Direction * num)));
		}

		private static Ray ComputeIntersectionLine(ref Plane p1, ref Plane p2)
		{
			var direction = Vector.Cross(p1.Normal, p2.Normal);
			var length = direction.LengthSquared();
			var position = (Vector)(Vector.Cross((Vector)((-p1.Distance * p2.Normal) + (p2.Distance * p1.Normal)), direction) / length);
			var ray = new Ray(position,direction);
			return ray;
		}

		public ContainmentType Contains(BoundingBox box)
		{
			bool flag = false;
			foreach (Plane plane in planes)
			{
				switch (box.Intersects(plane))
				{
					case PlaneIntersectionType.Front:
						return ContainmentType.Disjoint;

					case PlaneIntersectionType.Intersecting:
						flag = true;
						break;
				}
			}
			if (!flag)
			{
				return ContainmentType.Contains;
			}
			return ContainmentType.Intersects;
		}

		public ContainmentType Contains(BoundingFrustum frustum)
		{
			if (frustum == null)
			{
				throw new ArgumentNullException("frustum");
			}
			ContainmentType disjoint = ContainmentType.Disjoint;
			if (Intersects(frustum))
			{
				disjoint = ContainmentType.Contains;
				for (int i = 0; i < cornerArray.Length; i++)
				{
					if (Contains(frustum.cornerArray[i]) == ContainmentType.Disjoint)
					{
						return ContainmentType.Intersects;
					}
				}
			}
			return disjoint;
		}

		public ContainmentType Contains(BoundingSphere sphere)
		{
			Vector center = sphere.Center;
			float radius = sphere.Radius;
			int num2 = 0;
			foreach (Plane plane in planes)
			{
				float num5 = ((plane.Normal.X * center.X) + (plane.Normal.Y * center.Y)) + (plane.Normal.Z * center.Z);
				float num3 = num5 + plane.Distance;
				if (num3 > radius)
				{
					return ContainmentType.Disjoint;
				}
				if (num3 < -radius)
				{
					num2++;
				}
			}
			if (num2 != 6)
			{
				return ContainmentType.Intersects;
			}
			return ContainmentType.Contains;
		}

		public ContainmentType Contains(Vector point)
		{
			foreach (Plane plane in planes)
			{
				float num2 = (((plane.Normal.X * point.X) + (plane.Normal.Y * point.Y)) + (plane.Normal.Z * point.Z)) + plane.Distance;
				if (num2 > 1E-05f)
				{
					return ContainmentType.Disjoint;
				}
			}
			return ContainmentType.Contains;
		}

		public void Contains(ref BoundingBox box, out ContainmentType result)
		{
			bool flag = false;
			foreach (Plane plane in planes)
			{
				switch (box.Intersects(plane))
				{
					case PlaneIntersectionType.Front:
						result = ContainmentType.Disjoint;
						return;

					case PlaneIntersectionType.Intersecting:
						flag = true;
						break;
				}
			}
			result = flag ? ContainmentType.Intersects : ContainmentType.Contains;
		}

		public void Contains(ref BoundingSphere sphere, out ContainmentType result)
		{
			Vector center = sphere.Center;
			float radius = sphere.Radius;
			int num2 = 0;
			foreach (Plane plane in planes)
			{
				float num5 = ((plane.Normal.X * center.X) + (plane.Normal.Y * center.Y)) + (plane.Normal.Z * center.Z);
				float num3 = num5 + plane.Distance;
				if (num3 > radius)
				{
					result = ContainmentType.Disjoint;
					return;
				}
				if (num3 < -radius)
				{
					num2++;
				}
			}
			result = (num2 == 6) ? ContainmentType.Contains : ContainmentType.Intersects;
		}

		public void Contains(ref Vector point, out ContainmentType result)
		{
			foreach (Plane plane in planes)
			{
				float num2 = (((plane.Normal.X * point.X) + (plane.Normal.Y * point.Y)) + (plane.Normal.Z * point.Z)) + plane.Distance;
				if (num2 > 1E-05f)
				{
					result = ContainmentType.Disjoint;
					return;
				}
			}
			result = ContainmentType.Contains;
		}

		public bool Equals(BoundingFrustum other)
		{
			if (other == null)
			{
				return false;
			}
			return (matrix == other.matrix);
		}

		public override bool Equals(object obj)
		{
			bool flag = false;
			BoundingFrustum frustum = obj as BoundingFrustum;
			if (frustum != null)
			{
				flag = matrix == frustum.matrix;
			}
			return flag;
		}

		public Vector[] GetCorners()
		{
			return (Vector[])cornerArray.Clone();
		}

		public void GetCorners(Vector[] corners)
		{
			if (corners == null)
			{
				throw new ArgumentNullException("corners");
			}
			if (corners.Length < 8)
			{
				throw new ArgumentOutOfRangeException("corners", "Not enough corners, must have 8");
			}
			cornerArray.CopyTo(corners, 0);
		}

		public override int GetHashCode()
		{
			return matrix.GetHashCode();
		}

		public bool Intersects(BoundingBox box)
		{
			bool flag;
			Intersects(ref box, out flag);
			return flag;
		}

		public bool Intersects(BoundingFrustum frustum)
		{
			
			if (frustum == null)
			{
				throw new ArgumentNullException("frustum");
			}
			if (gjk == null)
			{
				gjk = new Gjk();
			}
			gjk.Reset();
			var closestPoint = cornerArray[0] - frustum.cornerArray[0];
			if (closestPoint.LengthSquared() < 1E-05f)
			{
				closestPoint = cornerArray[0] - frustum.cornerArray[1];
			}
			float maxValue = float.MaxValue;
			float num3 = 0f;
			do
			{
				Vector vector2;
				Vector vector3;
				Vector vector4;
				var vector5 = Vector.Zero;
				vector5.X = -closestPoint.X;
				vector5.Y = -closestPoint.Y;
				vector5.Z = -closestPoint.Z;
				vector4 = SupportMapping(ref vector5);
				vector3 = frustum.SupportMapping(ref closestPoint);
				vector2 = vector4 -  vector3;
				float num4 = ((closestPoint.X * vector2.X) + (closestPoint.Y * vector2.Y)) + (closestPoint.Z * vector2.Z);
				if (num4 > 0f)
				{
					return false;
				}
				gjk.AddSupportPoint(ref vector2);
				closestPoint = gjk.ClosestPoint;
				float num2 = maxValue;
				maxValue = closestPoint.LengthSquared();
				num3 = 4E-05f * gjk.MaxLengthSquared;
				if ((num2 - maxValue) <= (1E-05f * num2))
				{
					return false;
				}
			}
			while (!gjk.FullSimplex && (maxValue >= num3));
			return true;
		}

		public bool Intersects(BoundingSphere sphere)
		{
			bool flag;
			Intersects(ref sphere, out flag);
			return flag;
		}

		public PlaneIntersectionType Intersects(Plane plane)
		{
			var num = 0;
			for (var i = 0; i < 8; i++)
			{
				var num3 = Vector.Dot(cornerArray[i], plane.Normal);
				if ((num3 + plane.Distance) > 0f)
				{
					num |= 1;
				}
				else
				{
					num |= 2;
				}
				if (num == 3)
				{
					return PlaneIntersectionType.Intersecting;
				}
			}
			if (num != 1)
			{
				return PlaneIntersectionType.Back;
			}
			return PlaneIntersectionType.Front;
		}

		public float? Intersects(Ray ray)
		{
			float? nullable;
			Intersects(ref ray, out nullable);
			return nullable;
		}

		public void Intersects(ref BoundingBox box, out bool result)
		{
			Vector vector2;
			Vector vector3;
			Vector vector4;
			var vector5 = Vector.Zero;
			if (gjk == null)
			{
				gjk = new Gjk();
			}
			gjk.Reset();
			var closestPoint = cornerArray[0] - box.Min;
			if (closestPoint.LengthSquared() < 1E-05f)
			{
				closestPoint = cornerArray[0] - box.Max;
			}
			float maxValue = float.MaxValue;
			float num3 = 0f;
			result = false;
		Label_006D:
			vector5.X = -closestPoint.X;
			vector5.Y = -closestPoint.Y;
			vector5.Z = -closestPoint.Z;
			vector4 = SupportMapping(ref vector5);
			vector3 = box.SupportMapping(ref closestPoint);
			vector2 = vector4 - vector3;
			float num4 = ((closestPoint.X * vector2.X) + (closestPoint.Y * vector2.Y)) + (closestPoint.Z * vector2.Z);
			if (num4 <= 0f)
			{
				gjk.AddSupportPoint(ref vector2);
				closestPoint = gjk.ClosestPoint;
				float num2 = maxValue;
				maxValue = closestPoint.LengthSquared();
				if ((num2 - maxValue) > (1E-05f * num2))
				{
					num3 = 4E-05f * gjk.MaxLengthSquared;
					if (!gjk.FullSimplex && (maxValue >= num3))
					{
						goto Label_006D;
					}
					result = true;
				}
			}
		}

		public void Intersects(ref BoundingSphere sphere, out bool result)
		{
			Vector unitX;
			Vector vector2;
			Vector vector3;
			Vector vector4;
			var vector5 = Vector.Zero;
			if (gjk == null)
			{
				gjk = new Gjk();
			}
			gjk.Reset();
			unitX = cornerArray[0] - sphere.Center;
			if (unitX.LengthSquared() < 1E-05f)
			{
				unitX = Vector.UnitX;
			}
			float maxValue = float.MaxValue;
			float num3 = 0f;
			result = false;
		Label_005A:
			vector5.X = -unitX.X;
			vector5.Y = -unitX.Y;
			vector5.Z = -unitX.Z;
			vector4 = SupportMapping(ref vector5);
			vector3 = sphere.SupportMapping(ref unitX);
			vector2 = vector4 - vector3;
			float num4 = ((unitX.X * vector2.X) + (unitX.Y * vector2.Y)) + (unitX.Z * vector2.Z);
			if (num4 <= 0f)
			{
				gjk.AddSupportPoint(ref vector2);
				unitX = gjk.ClosestPoint;
				float num2 = maxValue;
				maxValue = unitX.LengthSquared();
				if ((num2 - maxValue) > (1E-05f * num2))
				{
					num3 = 4E-05f * gjk.MaxLengthSquared;
					if (!gjk.FullSimplex && (maxValue >= num3))
					{
						goto Label_005A;
					}
					result = true;
				}
			}
		}

		public void Intersects(ref Plane plane, out PlaneIntersectionType result)
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				var num3 = Vector.Dot(cornerArray[i], plane.Normal);
				if ((num3 + plane.Distance) > 0f)
				{
					num |= 1;
				}
				else
				{
					num |= 2;
				}
				if (num == 3)
				{
					result = PlaneIntersectionType.Intersecting;
					return;
				}
			}
			result = (num == 1) ? PlaneIntersectionType.Front : PlaneIntersectionType.Back;
		}

		public void Intersects(ref Ray ray, out float? result)
		{
			ContainmentType type;
			Contains(ref ray.Position, out type);
			if (type == ContainmentType.Contains)
			{
				result = 0f;
			}
			else
			{
				float minValue = float.MinValue;
				float maxValue = float.MaxValue;
				result = 0;
				foreach (Plane plane in planes)
				{
					Vector normal = plane.Normal;
					var num6 = Vector.Dot(ray.Direction, normal);
					var num3 = Vector.Dot(ray.Position, normal);
					num3 += plane.Distance;
					if (System.Math.Abs(num6) < 1E-05f)
					{
						if (num3 > 0f)
						{
							return;
						}
					}
					else
					{
						float num = -num3 / num6;
						if (num6 < 0f)
						{
							if (num > maxValue)
							{
								return;
							}
							if (num > minValue)
							{
								minValue = num;
							}
						}
						else
						{
							if (num < minValue)
							{
								return;
							}
							if (num < maxValue)
							{
								maxValue = num;
							}
						}
					}
				}
				float num7 = (minValue >= 0f) ? minValue : maxValue;
				if (num7 >= 0f)
				{
					result = new float?(num7);
				}
			}
		}

		public static bool operator ==(BoundingFrustum a, BoundingFrustum b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(BoundingFrustum a, BoundingFrustum b)
		{
			return !object.Equals(a, b);
		}

		private void SetMatrix(ref Matrix value)
		{
			matrix = value;
			planes[2].Normal.X = -value[0, 3] - value[0, 0];
			planes[2].Normal.Y = -value[1, 3] - value[1, 0];
			planes[2].Normal.Z = -value[2, 3] - value[2, 0];
			planes[2].Distance = -value[3, 3] - value[3, 0];
			planes[3].Normal.X = -value[0, 3] + value[0, 0];
			planes[3].Normal.Y = -value[1, 3] + value[1, 0];
			planes[3].Normal.Z = -value[2, 3] + value[2, 0];
			planes[3].Distance = -value[3, 3] + value[3, 0];
			planes[4].Normal.X = -value[0, 3] + value[0, 1];
			planes[4].Normal.Y = -value[1, 3] + value[1, 1];
			planes[4].Normal.Z = -value[2, 3] + value[2, 1];
			planes[4].Distance = -value[3, 3] + value[3, 1];
			planes[5].Normal.X = -value[0, 3] - value[0, 1];
			planes[5].Normal.Y = -value[1, 3] - value[1, 1];
			planes[5].Normal.Z = -value[2, 3] - value[2, 1];
			planes[5].Distance = -value[3, 3] - value[3, 1];
			planes[0].Normal.X = -value[0, 2];
			planes[0].Normal.Y = -value[1, 2];
			planes[0].Normal.Z = -value[2, 2];
			planes[0].Distance = -value[3, 2];
			planes[1].Normal.X = -value[0, 3] + value[0, 2];
			planes[1].Normal.Y = -value[1, 3] + value[1, 2];
			planes[1].Normal.Z = -value[2, 3] + value[2, 2];
			planes[1].Distance = -value[3, 3] + value[3, 2];
			for (int i = 0; i < 6; i++)
			{
				float num2 = planes[i].Normal.Length;
				planes[i].Normal = (Vector)(planes[i].Normal / num2);
				planes[i].Distance /= num2;
			}
			Ray ray = ComputeIntersectionLine(ref planes[0], ref planes[2]);
			cornerArray[0] = ComputeIntersection(ref planes[4], ref ray);
			cornerArray[3] = ComputeIntersection(ref planes[5], ref ray);
			ray = ComputeIntersectionLine(ref planes[3], ref planes[0]);
			cornerArray[1] = ComputeIntersection(ref planes[4], ref ray);
			cornerArray[2] = ComputeIntersection(ref planes[5], ref ray);
			ray = ComputeIntersectionLine(ref planes[2], ref planes[1]);
			cornerArray[4] = ComputeIntersection(ref planes[4], ref ray);
			cornerArray[7] = ComputeIntersection(ref planes[5], ref ray);
			ray = ComputeIntersectionLine(ref planes[1], ref planes[3]);
			cornerArray[5] = ComputeIntersection(ref planes[4], ref ray);
			cornerArray[6] = ComputeIntersection(ref planes[5], ref ray);
		}

		internal Vector SupportMapping(ref Vector v)
		{
			var result = Vector.Zero;
			int index = 0;
			var num3 = Vector.Dot(cornerArray[0], v);
			for (int i = 1; i < cornerArray.Length; i++)
			{
				var num2 = Vector.Dot(cornerArray[i], v);
				if (num2 > num3)
				{
					index = i;
					num3 = num2;
				}
			}
			result = cornerArray[index];
			return result;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{{Near:{0} Far:{1} Left:{2} Right:{3} Top:{4} Bottom:{5}}}", new object[] { Near.ToString(), Far.ToString(), Left.ToString(), Right.ToString(), Top.ToString(), Bottom.ToString() });
		}

		// Properties
		public Plane Bottom
		{
			get
			{
				return planes[5];
			}
		}

		public Plane Far
		{
			get
			{
				return planes[1];
			}
		}

		public Plane Left
		{
			get
			{
				return planes[2];
			}
		}

		public Matrix Matrix
		{
			get
			{
				return matrix;
			}
			set
			{
				SetMatrix(ref value);
			}
		}

		public Plane Near
		{
			get
			{
				return planes[0];
			}
		}

		public Plane Right
		{
			get
			{
				return planes[3];
			}
		}

		public Plane Top
		{
			get
			{
				return planes[4];
			}
		}
	}
}