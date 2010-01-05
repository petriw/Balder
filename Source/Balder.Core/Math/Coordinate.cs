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
			X = coordinate.X;
			Y = coordinate.Y;
			Z = coordinate.Z;
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
	}
}
