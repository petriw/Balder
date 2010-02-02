using System;
using Balder.Core.Execution;

namespace Balder.Core.Math
{
	public partial class Coordinate
	{
		public Coordinate()
		{
		}

		public Coordinate(Coordinate coordinate)
		{
			Set(coordinate);
		}

		public Coordinate(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Set(Coordinate coordinate)
		{
			if (null != coordinate)
			{
				X = coordinate.X;
				Y = coordinate.Y;
				Z = coordinate.Z;
			}
		}

		public Vector ToVector()
		{
			var vector = new Vector((float)X, (float)Y, (float)Z);
			return vector;
		}

		public static implicit operator Coordinate(Vector vector)
		{
			var coordinate = new Coordinate {X = vector.X, Y = vector.Y, Z = vector.Z};
			return coordinate;
		}

		public static implicit operator Vector(Coordinate coordinate)
		{
			var vector = coordinate.ToVector();
			return vector;
		}

		public static Vector operator +(Coordinate c1, Vector v2)
		{
			Vector v1 = c1;
			return v1 + v2;
		}

		public static Vector operator +(Vector v1, Coordinate c2)
		{
			Vector v2 = c2;
			return v1 + v2;
		}


		public static Vector operator +(Coordinate c1, Coordinate c2)
		{
			Vector v1 = c1;
			Vector v2 = c2;
			return v1 + v2;
		}

		public static Vector operator -(Coordinate c1, Vector v2)
		{
			Vector v1 = c1;
			return v1 - v2;
		}

		public static Vector operator -(Vector v1, Coordinate c2)
		{
			Vector v2 = c2;
			return v1 - v2;
		}

		public static Vector operator -(Coordinate c1, Coordinate c2)
		{
			Vector v1 = c1;
			Vector v2 = c2;
			return v1 - v2;
		}


		public override string ToString()
		{
			return ToString(null, null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			// If no format is passed
			if (string.IsNullOrEmpty(format))
			{
				return string.Format("({0}, {1}, {2})", X, Y, Z);
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
				default:
					return string.Format
						(
							"({0}, {1}, {2})",
							X.ToString(format, formatProvider),
							Y.ToString(format, formatProvider),
							Z.ToString(format, formatProvider)
						);
			}
		}


	}
}
