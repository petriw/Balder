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
using System.Collections.Generic;
using System.Globalization;

namespace Balder.Core.Math
{
	public class BoundingSphere : IEquatable<BoundingSphere>
	{
		public Vector Center;
		public float Radius;
		public BoundingSphere(Vector center, float radius)
		{
			if (radius < 0f)
			{
				throw new ArgumentException("Radius can't be negative");
			}
			this.Center = center;
			this.Radius = radius;
		}

		public bool Equals(BoundingSphere other)
		{
			return ((this.Center == other.Center) && (this.Radius == other.Radius));
		}

		public override bool Equals(object obj)
		{
			bool flag = false;
			if (obj is BoundingSphere)
			{
				flag = this.Equals((BoundingSphere)obj);
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.Center.GetHashCode() + this.Radius.GetHashCode());
		}

		public override string ToString()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			return string.Format(currentCulture, "{{Center:{0} Radius:{1}}}", new object[] { this.Center.ToString(), this.Radius.ToString(currentCulture) });
		}

		public static BoundingSphere CreateMerged(BoundingSphere original, BoundingSphere additional)
		{
			var vector2 = additional.Center - original.Center;
			var num = vector2.Length;
			var radius = original.Radius;
			var num2 = additional.Radius;
			if ((radius + num2) >= num)
			{
				if ((radius - num2) >= num)
				{
					return original;
				}
				if ((num2 - radius) >= num)
				{
					return additional;
				}
			}
			var vector = (Vector)(vector2 * (1f / num));
			var num5 = MathHelper.Min(-radius, num - num2);
			var num4 = (MathHelper.Max(radius, num + num2) - num5) * 0.5f;
			var center = original.Center + ((Vector)(vector * (num4 + num5)));
			radius = num4;
			var sphere = new BoundingSphere(center,radius);
			return sphere;
		}

		public static BoundingSphere CreateFromBoundingBox(BoundingBox box)
		{
			var center = Vector.Lerp(box.Min, box.Max, 0.5f);
			var num = Vector.Distance(box.Min, box.Max);
			var radius = num * 0.5f;
			var sphere = new BoundingSphere(center,radius);
			return sphere;
		}

		public static BoundingSphere CreateFromPoints(IEnumerable<Vector> points)
		{
			float num;
			float num2;
			Vector vector2;
			float num4;
			float num5;
			Vector vector5;
			Vector vector6;
			Vector vector7;
			Vector vector8;
			Vector vector9;
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			IEnumerator<Vector> enumerator = points.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				throw new ArgumentException("Bounding sphere can't have zero points");
			}
			var vector4 = vector5 = vector6 = vector7 = vector8 = vector9 = enumerator.Current;
			foreach (var vector in points)
			{
				if (vector.X < vector4.X)
				{
					vector4 = vector;
				}
				if (vector.X > vector5.X)
				{
					vector5 = vector;
				}
				if (vector.Y < vector6.Y)
				{
					vector6 = vector;
				}
				if (vector.Y > vector7.Y)
				{
					vector7 = vector;
				}
				if (vector.Z < vector8.Z)
				{
					vector8 = vector;
				}
				if (vector.Z > vector9.Z)
				{
					vector9 = vector;
				}
			}
			num5 = Vector.Distance(vector5, vector4);
			num4 = Vector.Distance(vector7, vector6);
			num2 = Vector.Distance(vector9, vector8);
			if (num5 > num4)
			{
				if (num5 > num2)
				{
					vector2 = Vector.Lerp(vector5, vector4, 0.5f);
					num = num5 * 0.5f;
				}
				else
				{
					vector2 = Vector.Lerp(vector9, vector8, 0.5f);
					num = num2 * 0.5f;
				}
			}
			else if (num4 > num2)
			{
				vector2 = Vector.Lerp(vector7, vector6, 0.5f);
				num = num4 * 0.5f;
			}
			else
			{
				vector2 = Vector.Lerp(vector9, vector8, 0.5f);
				num = num2 * 0.5f;
			}
			foreach (Vector vector10 in points)
			{
				Vector vector3 = Vector.Zero;
				vector3.X = vector10.X - vector2.X;
				vector3.Y = vector10.Y - vector2.Y;
				vector3.Z = vector10.Z - vector2.Z;
				var num3 = vector3.Length;
				if (num3 > num)
				{
					num = (num + num3) * 0.5f;
					vector2 += (Vector)((1f - (num / num3)) * vector3);
				}
			}
			var center = vector2;
			var radius = num;
			var sphere = new BoundingSphere(center, radius);
			return sphere;
		}

		public static BoundingSphere CreateFromFrustum(BoundingFrustum frustum)
		{
			if (frustum == null)
			{
				throw new ArgumentNullException("frustum");
			}
			return CreateFromPoints(frustum.cornerArray);
		}

		public bool Intersects(BoundingBox box)
		{
			var vector = Vector.Clamp(Center, box.Min, box.Max);
			var num = Vector.DistanceSquared(this.Center, vector);
			return (num <= (this.Radius * this.Radius));
		}

		public void Intersects(ref BoundingBox box, out bool result)
		{
			var vector = Vector.Clamp(this.Center, box.Min, box.Max);
			var num = Vector.DistanceSquared(this.Center, vector);
			result = num <= (this.Radius * this.Radius);
		}

		public bool Intersects(BoundingFrustum frustum)
		{
			if (null == frustum)
			{
				throw new ArgumentNullException("frustum", "Null is not allowed");
			}
			var flag = frustum.Intersects(this);
			return flag;
		}

		public PlaneIntersectionType Intersects(Plane plane)
		{
			return plane.Intersects(this);
		}

		public float? Intersects(Ray ray)
		{
			return ray.Intersects(this);
		}

		public bool Intersects(BoundingSphere sphere)
		{
			var num3 = Vector.DistanceSquared(this.Center, sphere.Center);
			var radius = this.Radius;
			var num = sphere.Radius;
			if ((((radius * radius) + ((2f * radius) * num)) + (num * num)) <= num3)
			{
				return false;
			}
			return true;
		}

		public void Intersects(ref BoundingSphere sphere, out bool result)
		{
			var num3 = Vector.DistanceSquared(this.Center, sphere.Center);
			var radius = this.Radius;
			var num = sphere.Radius;
			result = (((radius * radius) + ((2f * radius) * num)) + (num * num)) > num3;
		}

		public ContainmentType Contains(BoundingBox box)
		{
			var vector = Vector.Zero;
			if (!box.Intersects(this))
			{
				return ContainmentType.Disjoint;
			}
			float num = this.Radius * this.Radius;
			vector.X = this.Center.X - box.Min.X;
			vector.Y = this.Center.Y - box.Max.Y;
			vector.Z = this.Center.Z - box.Max.Z;
			
			if (vector.LengthSquared() > num)
			{
				return ContainmentType.Intersects;
			}
			vector.X = this.Center.X - box.Max.X;
			vector.Y = this.Center.Y - box.Max.Y;
			vector.Z = this.Center.Z - box.Max.Z;
			if (vector.LengthSquared() > num)
			{
				return ContainmentType.Intersects;
			}
			vector.X = this.Center.X - box.Max.X;
			vector.Y = this.Center.Y - box.Min.Y;
			vector.Z = this.Center.Z - box.Max.Z;
			if (vector.LengthSquared() > num)
			{
				return ContainmentType.Intersects;
			}
			vector.X = this.Center.X - box.Min.X;
			vector.Y = this.Center.Y - box.Min.Y;
			vector.Z = this.Center.Z - box.Max.Z;
			if (vector.LengthSquared() > num)
			{
				return ContainmentType.Intersects;
			}
			vector.X = this.Center.X - box.Min.X;
			vector.Y = this.Center.Y - box.Max.Y;
			vector.Z = this.Center.Z - box.Min.Z;
			if (vector.LengthSquared() > num)
			{
				return ContainmentType.Intersects;
			}
			vector.X = this.Center.X - box.Max.X;
			vector.Y = this.Center.Y - box.Max.Y;
			vector.Z = this.Center.Z - box.Min.Z;
			if (vector.LengthSquared() > num)
			{
				return ContainmentType.Intersects;
			}
			vector.X = this.Center.X - box.Max.X;
			vector.Y = this.Center.Y - box.Min.Y;
			vector.Z = this.Center.Z - box.Min.Z;
			if (vector.LengthSquared() > num)
			{
				return ContainmentType.Intersects;
			}
			vector.X = this.Center.X - box.Min.X;
			vector.Y = this.Center.Y - box.Min.Y;
			vector.Z = this.Center.Z - box.Min.Z;
			if (vector.LengthSquared() > num)
			{
				return ContainmentType.Intersects;
			}
			return ContainmentType.Contains;
		}

		public void Contains(ref BoundingBox box, out ContainmentType result)
		{
			var flag = box.Intersects(this);
			if (!flag)
			{
				result = ContainmentType.Disjoint;
			}
			else
			{
				var vector = Vector.Zero;
				float num = this.Radius * this.Radius;
				result = ContainmentType.Intersects;
				vector.X = this.Center.X - box.Min.X;
				vector.Y = this.Center.Y - box.Max.Y;
				vector.Z = this.Center.Z - box.Max.Z;
				if (vector.LengthSquared() <= num)
				{
					vector.X = this.Center.X - box.Max.X;
					vector.Y = this.Center.Y - box.Max.Y;
					vector.Z = this.Center.Z - box.Max.Z;
					if (vector.LengthSquared() <= num)
					{
						vector.X = this.Center.X - box.Max.X;
						vector.Y = this.Center.Y - box.Min.Y;
						vector.Z = this.Center.Z - box.Max.Z;
						if (vector.LengthSquared() <= num)
						{
							vector.X = this.Center.X - box.Min.X;
							vector.Y = this.Center.Y - box.Min.Y;
							vector.Z = this.Center.Z - box.Max.Z;
							if (vector.LengthSquared() <= num)
							{
								vector.X = this.Center.X - box.Min.X;
								vector.Y = this.Center.Y - box.Max.Y;
								vector.Z = this.Center.Z - box.Min.Z;
								if (vector.LengthSquared() <= num)
								{
									vector.X = this.Center.X - box.Max.X;
									vector.Y = this.Center.Y - box.Max.Y;
									vector.Z = this.Center.Z - box.Min.Z;
									if (vector.LengthSquared() <= num)
									{
										vector.X = this.Center.X - box.Max.X;
										vector.Y = this.Center.Y - box.Min.Y;
										vector.Z = this.Center.Z - box.Min.Z;
										if (vector.LengthSquared() <= num)
										{
											vector.X = this.Center.X - box.Min.X;
											vector.Y = this.Center.Y - box.Min.Y;
											vector.Z = this.Center.Z - box.Min.Z;
											if (vector.LengthSquared() <= num)
											{
												result = ContainmentType.Contains;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public ContainmentType Contains(BoundingFrustum frustum)
		{
			if (null == frustum)
			{
				throw new ArgumentNullException("frustum", "Frustum can't be null");
			}
			if (!frustum.Intersects(this))
			{
				return ContainmentType.Disjoint;
			}
			float num2 = this.Radius * this.Radius;
			foreach (var vector2 in frustum.cornerArray)
			{
				var vector = Vector.Zero;
				vector.X = vector2.X - this.Center.X;
				vector.Y = vector2.Y - this.Center.Y;
				vector.Z = vector2.Z - this.Center.Z;
				if (vector.LengthSquared() > num2)
				{
					return ContainmentType.Intersects;
				}
			}
			return ContainmentType.Contains;
		}

		public ContainmentType Contains(Vector point)
		{
			if (Vector.DistanceSquared(point, this.Center) >= (this.Radius * this.Radius))
			{
				return ContainmentType.Disjoint;
			}
			return ContainmentType.Contains;
		}

		public void Contains(ref Vector point, out ContainmentType result)
		{
			float num;
			num = Vector.DistanceSquared(point, this.Center);
			result = (num < (this.Radius * this.Radius)) ? ContainmentType.Contains : ContainmentType.Disjoint;
		}

		public ContainmentType Contains(BoundingSphere sphere)
		{
			var num3 = Vector.Distance(this.Center, sphere.Center);
			var radius = this.Radius;
			var num = sphere.Radius;
			if ((radius + num) < num3)
			{
				return ContainmentType.Disjoint;
			}
			if ((radius - num) < num3)
			{
				return ContainmentType.Intersects;
			}
			return ContainmentType.Contains;
		}

		public void Contains(ref BoundingSphere sphere, out ContainmentType result)
		{
			var num3 = Vector.Distance(this.Center, sphere.Center);
			float radius = this.Radius;
			float num = sphere.Radius;
			result = ((radius + num) >= num3) ? (((radius - num) >= num3) ? ContainmentType.Contains : ContainmentType.Intersects) : ContainmentType.Disjoint;
		}

		internal Vector SupportMapping(ref Vector v)
		{
			var result = Vector.Zero;
			float num2 = v.Length;
			float num = this.Radius / num2;
			result.X = this.Center.X + (v.X * num);
			result.Y = this.Center.Y + (v.Y * num);
			result.Z = this.Center.Z + (v.Z * num);
			return result;
		}

		public BoundingSphere Transform(Matrix matrix)
		{
			var center = Vector.Transform(Center, matrix);
			float num4 = ((matrix[0, 0] * matrix[0, 0]) + (matrix[0, 1] * matrix[0, 1])) + (matrix[0, 2] * matrix[0, 2]);
			float num3 = ((matrix[1, 0] * matrix[1, 0]) + (matrix[1, 1] * matrix[1, 1])) + (matrix[1, 2] * matrix[1, 2]);
			float num2 = ((matrix[2, 0] * matrix[2, 0]) + (matrix[2, 1] * matrix[2, 1])) + (matrix[2, 2] * matrix[2, 2]);
			float num = System.Math.Max(num4, System.Math.Max(num3, num2));
			var radius = this.Radius * ((float)System.Math.Sqrt((double)num));
			var sphere = new BoundingSphere(center,radius);
			return sphere;
		}


		public static bool operator ==(BoundingSphere a, BoundingSphere b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BoundingSphere a, BoundingSphere b)
		{
			if (!(a.Center != b.Center))
			{
				return (a.Radius != b.Radius);
			}
			return true;
		}
	}
}
