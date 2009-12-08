using Balder.Core.Execution;

namespace Balder.Core.Math
{
	public partial class Coordinate
	{
		public Coordinate()
		{
			
		}


		public Coordinate(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}


		public static readonly Property<Coordinate, float> XProp = Property<Coordinate, float>.Register(c => c.X);
		public float X
		{
			get { return XProp.GetValue(this); }
			set { XProp.SetValue(this, value); }
		}

		public static readonly Property<Coordinate, float> YProp = Property<Coordinate, float>.Register(c => c.Y);
		public float Y
		{
			get { return YProp.GetValue(this); }
			set { YProp.SetValue(this, value); }
		}

		public static readonly Property<Coordinate, float> ZProp = Property<Coordinate, float>.Register(c => c.Z);
		public float Z
		{
			get { return ZProp.GetValue(this); }
			set { ZProp.SetValue(this, value); }
		}

		public Vector ToVector()
		{
			var vector = new Vector(X, Y, Z);
			return vector;
		}

		public static implicit operator Coordinate(Vector vector)
		{
			var coordinate = new Coordinate(vector.X, vector.Y, vector.Z);
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
