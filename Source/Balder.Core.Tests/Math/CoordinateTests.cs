using Balder.Core.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balder.Core.Tests.Math
{
	[TestClass]
	public class CoordinateTests
	{
		[TestMethod]
		public void ConstructingWithArgumentedConstructorShouldSetValues()
		{
			const float x = 5f;
			const float y = 6f;
			const float z = 7f;
			var coordinate = new Coordinate(x, y, z);

			Assert.AreEqual(coordinate.X,x);
			Assert.AreEqual(coordinate.X, x);
			Assert.AreEqual(coordinate.Y, y);
			Assert.AreEqual(coordinate.Z, z);
		}

		[TestMethod]
		public void ConvertingToVectorShouldReturnSameValuesInVector()
		{
			var coordinate = new Coordinate { X = 5, Y = 6, Z = 7 };
			var vector = coordinate.ToVector();
			Assert.AreEqual(vector.X, coordinate.X);
			Assert.AreEqual(vector.Y, coordinate.Y);
			Assert.AreEqual(vector.Z, coordinate.Z);
		}

		[TestMethod]
		public void ImplicitlySettingAVectorAsCoordinateShouldReturnSameValuesAsInVector()
		{
			var vector = new Vector(5f, 6f, 7f);
			Coordinate coordinate = vector;
			Assert.AreEqual(coordinate.X, vector.X);
			Assert.AreEqual(coordinate.Y, vector.Y);
			Assert.AreEqual(coordinate.Z, vector.Z);
		}

		[TestMethod]
		public void ImplicitlySettingACoordinateAsVectorShouldReturnSameValuesAsInCoordinate()
		{
			var coordinate = new Coordinate(5f, 6f, 7f);
			Vector vector = coordinate;
			Assert.AreEqual(vector.X, coordinate.X);
			Assert.AreEqual(vector.Y, coordinate.Y);
			Assert.AreEqual(vector.Z, coordinate.Z);
		}

		[TestMethod]
		public void SubtractingTwoCoordinatesShouldReturnAVectorWithCorrectResult()
		{
			var c1 = new Coordinate(2f, 4f, 6f);
			var c2 = new Coordinate(1f, 2f, 3f);
			var vector = c1 - c2;
			Assert.AreEqual(vector.X, 1f);
			Assert.AreEqual(vector.Y, 2f);
			Assert.AreEqual(vector.Z, 3f);
		}

		[TestMethod]
		public void AddingTwoCoordinatesShouldReturnAVectorWithCorrectResult()
		{
			var c1 = new Coordinate(2f, 4f, 6f);
			var c2 = new Coordinate(1f, 2f, 3f);
			var vector = c1 + c2;
			Assert.AreEqual(vector.X, 3f);
			Assert.AreEqual(vector.Y, 6f);
			Assert.AreEqual(vector.Z, 9f);
		}

		[TestMethod]
		public void SubtractingAVectorFromACoordinateShouldReturnAVectorWithCorrectResult()
		{
			var c1 = new Coordinate(2f, 4f, 6f);
			var v2 = new Vector(1f, 2f, 3f);
			var vector = c1 - v2;
			Assert.AreEqual(vector.X, 1f);
			Assert.AreEqual(vector.Y, 2f);
			Assert.AreEqual(vector.Z, 3f);
		}

		[TestMethod]
		public void SubtractingACoordinateFromAVectorShouldReturnAVectorWithCorrectResult()
		{
			var v1 = new Vector(2f, 4f, 6f);
			var c2 = new Coordinate(1f, 2f, 3f);
			var vector = v1 - c2;
			Assert.AreEqual(vector.X, 1f);
			Assert.AreEqual(vector.Y, 2f);
			Assert.AreEqual(vector.Z, 3f);
		}

		[TestMethod]
		public void AddingAVectorToACoordinateShouldReturnAVectorWithCorrectResult()
		{
			var c1 = new Coordinate(2f, 4f, 6f);
			var v2 = new Vector(1f, 2f, 3f);
			var vector = c1 + v2;
			Assert.AreEqual(vector.X, 3f);
			Assert.AreEqual(vector.Y, 6f);
			Assert.AreEqual(vector.Z, 9f);
		}

		[TestMethod]
		public void AddingACoordinateToAVectorShouldReturnAVectorWithCorrectResult()
		{
			var v1 = new Vector(2f, 4f, 6f);
			var c2 = new Coordinate(1f, 2f, 3f);
			var vector = v1 + c2;
			Assert.AreEqual(vector.X, 3f);
			Assert.AreEqual(vector.Y, 6f);
			Assert.AreEqual(vector.Z, 9f);
		}
	}
}
