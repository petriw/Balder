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

namespace Balder.Core.Math
{
	public struct Vector
	{
		#region Constructors

		public Vector(float x, float y, float z)
			: this()
		{
			X = x;
			Y = y;
			Z = z;
			W = 1;
		}

		public Vector(float x, float y, float z, float w)
			: this()
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector(Vector v1)
			: this()
		{
			X = v1.X;
			Y = v1.Y;
			Z = v1.Z;
			W = v1.W;
		}

		#endregion

		#region Public fields

		public static Vector UnitX = new Vector(1f, 0f, 0f);
		public static Vector UnitY = new Vector(0f, 1f, 0f);
		public static Vector UnitZ = new Vector(0f, 0f, 1f);

		public static Vector Zero = new Vector(0f, 0f, 0f);
		public static Vector Forward = new Vector(0f, 0f, 1f);
		public static Vector Backward = new Vector(0f, 0f, -1f);
		public static Vector Left = new Vector(-1f, 0f, 0f);
		public static Vector Right = new Vector(1f, 0f, 0f);
		public static Vector Up = new Vector(0f, 1f, 0f);
		public static Vector Down = new Vector(0f, -1f, 0f);

		public float X;
		public float Y;
		public float Z;
		public float W;

		#endregion

		#region Operators

		public static Vector operator +(Vector v1, Vector v2)
		{
			return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}

		public static Vector operator -(Vector v1, Vector v2)
		{
			return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}

		public static Vector operator *(Vector v1, float s2)
		{
			return new Vector(v1.X * s2, v1.Y * s2, v1.Z * s2);
		}

		public static Vector operator *(float s1, Vector v2)
		{
			return v2 * s1;
		}

		public static Vector operator *(Vector v1, Vector v2)
		{
			return v1.Cross(v2);
		}

		public static Vector operator /(Vector v1, float s2)
		{
			return new Vector(v1.X / s2, v1.Y / s2, v1.Z / s2);
		}

		public static Vector operator -(Vector v1)
		{
			return new Vector(-v1.X, -v1.Y, -v1.Z);
		}

		public static bool operator <(Vector v1, Vector v2)
		{
			return v1.Length < v2.Length;
		}

		public static bool operator >(Vector v1, Vector v2)
		{
			return v1.Length > v2.Length;
		}

		public static bool operator <=(Vector v1, Vector v2)
		{
			return v1.Length <= v2.Length;
		}

		public static bool operator >=(Vector v1, Vector v2)
		{
			return v1.Length >= v2.Length;
		}

		public static bool operator ==(Vector v1, Vector v2)
		{
			if (+(v1.X - v2.X) < float.Epsilon && +(v1.Y - v2.Y) < float.Epsilon && +(v1.Z - v2.Z) < float.Epsilon)
			{
				return true;
			}
			return false;
		}

		public static bool operator !=(Vector v1, Vector v2)
		{
			return !(v1 == v2);
		}

		#endregion

		#region Public Methods
		public float LengthSquared()
		{
			return (((X * X) + (Y * Y)) + (Z * Z));
		}
		#endregion

		#region Public Static Methods

		public static Vector Multiply(Vector vector1, Vector vector2)
		{
			var result = Vector.Zero;
			result.X = vector1.X * vector2.X;
			result.Y = vector1.Y * vector2.Y;
			result.Z = vector1.Z * vector2.Z;
			return result;
		}


		public static Vector Clamp(Vector value1, Vector min, Vector max)
		{
			var vector = Vector.Zero;
			var x = value1.X;
			x = (x > max.X) ? max.X : x;
			x = (x < min.X) ? min.X : x;
			var y = value1.Y;
			y = (y > max.Y) ? max.Y : y;
			y = (y < min.Y) ? min.Y : y;
			var z = value1.Z;
			z = (z > max.Z) ? max.Z : z;
			z = (z < min.Z) ? min.Z : z;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			return vector;
		}


		public static float Distance(Vector value1, Vector value2)
		{
			var num3 = value1.X - value2.X;
			var num2 = value1.Y - value2.Y;
			var num = value1.Z - value2.Z;
			var num4 = ((num3 * num3) + (num2 * num2)) + (num * num);
			return MathHelper.Sqrt(num4);
		}


		public static float DistanceSquared(Vector value1, Vector value2)
		{
			var num3 = value1.X - value2.X;
			var num2 = value1.Y - value2.Y;
			var num = value1.Z - value2.Z;
			return (((num3 * num3) + (num2 * num2)) + (num * num));
		}


		public static Vector Lerp(Vector value1, Vector value2, float amount)
		{
			var vector = Vector.Zero;
			vector.X = value1.X + ((value2.X - value1.X) * amount);
			vector.Y = value1.Y + ((value2.Y - value1.Y) * amount);
			vector.Z = value1.Z + ((value2.Z - value1.Z) * amount);
			return vector;
		}


		public static Vector Min(Vector value1, Vector value2)
		{
			var vector = Vector.Zero;
			vector.X = (value1.X < value2.X) ? value1.X : value2.X;
			vector.Y = (value1.Y < value2.Y) ? value1.Y : value2.Y;
			vector.Z = (value1.Z < value2.Z) ? value1.Z : value2.Z;
			return vector;
		}

		public static Vector Max(Vector value1, Vector value2)
		{
			var vector = Vector.Zero;
			vector.X = (value1.X > value2.X) ? value1.X : value2.X;
			vector.Y = (value1.Y > value2.Y) ? value1.Y : value2.Y;
			vector.Z = (value1.Z > value2.Z) ? value1.Z : value2.Z;
			return vector;
		}


		public static Vector Cross(Vector vector1, Vector vector2)
		{
			var vector = Vector.Zero;
			vector.X = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
			vector.Y = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
			vector.Z = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
			return vector;
		}


		public static float MixedProduct(Vector v1, Vector v2, Vector v3)
		{
			return Dot(Cross(v1, v2), v3);
		}

		public static float Dot(Vector v1, Vector v2)
		{
			return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
		}

		public static Vector Normalize(Vector v)
		{
			if (v.X * 2 + v.Y * 2 + v.Z * 2 == 0)
			{
				return v;
			}

			float r = 1 / v.Length;

			v.X *= r;
			v.Y *= r;
			v.Z *= r;

			return v;
		}

		public static Vector Reflect(Vector vI, Vector vN)
		{
			var reflectedVector = 2 * vI.Dot(vN) * vN - vI;
			return reflectedVector;
		}

		public static Vector Transform(Vector vector, Matrix matrix)
		{
			var vector2 = Vector.Zero;
			var x = (((vector.X * matrix[0, 0]) + (vector.Y * matrix[1, 0])) + (vector.Z * matrix[2, 0])) + (vector.W * matrix[3, 0]);
			var y = (((vector.X * matrix[0, 1]) + (vector.Y * matrix[1, 1])) + (vector.Z * matrix[2, 1])) + (vector.W * matrix[3, 1]);
			var z = (((vector.X * matrix[0, 2]) + (vector.Y * matrix[1, 2])) + (vector.Z * matrix[2, 2])) + (vector.W * matrix[3, 2]);
			var w = (((vector.X * matrix[0, 3]) + (vector.Y * matrix[1, 3])) + (vector.Z * matrix[2, 3])) + (vector.W * matrix[3, 3]);
			vector2.X = (x / w);
			vector2.Y = (y / w);
			vector2.Z = (z / w);
			vector2.W = w;
			return vector2;
		}

		public static Vector TransformNormal(Vector position, Matrix matrix)
		{
			var x = (((position.X * matrix[0, 0]) + (position.Y * matrix[1, 0])) + (position.Z * matrix[2, 0]));
			var y = (((position.X * matrix[0, 1]) + (position.Y * matrix[1, 1])) + (position.Z * matrix[2, 1]));
			var z = (((position.X * matrix[0, 2]) + (position.Y * matrix[1, 2])) + (position.Z * matrix[2, 2]));
			var vector = new Vector(x, y, z);
			return vector;
		}


		public static Vector Transform(Vector vector, Matrix world, Matrix view)
		{
			var transformedVector = Vector.Transform(vector, world);
			transformedVector = Vector.Transform(transformedVector, view);
			return transformedVector;
		}

		public static Vector Translate(Vector vector, Matrix projection, float width, float height)
		{
			var translated = Vector.Transform(vector, projection);

			translated.X = (translated.X * width) + (width / 2);
			translated.Y = (-translated.Y * height) + (height / 2);
			translated.Z = 0;

			return translated;
		}

		public void MultiplyWith(Vector vector)
		{
			X = X * vector.X;
			Y = Y * vector.Y;
			Z = Z * vector.Z;
		}

		public Vector Cross(Vector v)
		{
			return Cross(this, v);
		}

		public float Dot(Vector v)
		{
			return Dot(this, v);
		}

		public float MixedProduct(Vector v1, Vector v2)
		{
			return Dot(Cross(this, v1), v2);
		}

		public void Normalize()
		{
			if (X * 2 + Y * 2 + Z * 2 == 0)
			{
				return;
			}

			var r = 1 / Length;

			X *= r;
			Y *= r;
			Z *= r;
		}

		public float Length
		{
			get
			{
				return MathHelper.Sqrt(X * X + Y * Y + Z * Z);
			}
			set
			{
				var newVector = this * (value / Length);
				X = newVector.X;
				Y = newVector.Y;
				Z = newVector.Z;
			}
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return ToString(null, null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			// If no format is passed
			if (string.IsNullOrEmpty(format))
			{
				return String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
			}

			var firstChar = format[0];
			string remainder = null;

			if (format.Length > 1)
			{
				remainder = format.Substring(1);
			}

			switch (firstChar)
			{
				case 'x':
					{
						return X.ToString(remainder, formatProvider);
					}
				case 'y':
					{
						return Y.ToString(remainder, formatProvider);
					}
				case 'z':
					{
						return Z.ToString(remainder, formatProvider);
					}
				case 'w':
					{
						return W.ToString(remainder, formatProvider);
					}
				default:
					return String.Format
						(
							"({0}, {1}, {2}, {3})",
							X.ToString(format, formatProvider),
							Y.ToString(format, formatProvider),
							Z.ToString(format, formatProvider),
							W.ToString(format, formatProvider)
						);
			}
		}

		/*
        public override int GetHashCode()
        {
            return 0;
        }
		 * */

		public override bool Equals(object other)
		{
			// Check object other is a Vector3 object
			if (other is Vector)
			{
				// Convert object to Vector3
				Vector otherVector = (Vector)other;

				// Check for equality
				return otherVector == this;
			}
			else
			{
				return false;
			}
		}

		public bool Equals(Vector other)
		{
			return other == this;
		}

		public int CompareTo(Vector other)
		{
			if (this < other)
			{
				return -1;
			}
			else if (this > other)
			{
				return 1;
			}
			return 0;
		}

		public int CompareTo(object other)
		{
			if (other is Vector)
			{
				return CompareTo((Vector)other);
			}
			else
			{
				return -1;
			}
		}

		#endregion

	}
}