using Balder.Core.Math;
using CThru.Silverlight;
using NUnit.Framework;

namespace Balder.Core.Tests.Math
{
	[TestFixture]
	public class CoordinateTests
	{

		[Test,SilverlightUnitTest]
		public void ConvertingToVectorShouldReturnSameValuesInVector()
		{
			var coordinate = new Coordinate { X = 5, Y = 6, Z = 7 };
			var vector = coordinate.ToVector();
			Assert.That(vector.X, Is.EqualTo(coordinate.X));
			Assert.That(vector.Y, Is.EqualTo(coordinate.Y));
			Assert.That(vector.Z, Is.EqualTo(coordinate.Z));
		}

		[Test,SilverlightUnitTest]
		public void ImplicitlySettingAVectorAsCoordinateShouldReturnSameValuesAsInVector()
		{
			var vector = new Vector(5f, 6f, 7f);
			Coordinate coordinate = vector;
			Assert.That(coordinate.X, Is.EqualTo(vector.X));
			Assert.That(coordinate.Y, Is.EqualTo(vector.Y));
			Assert.That(coordinate.Z, Is.EqualTo(vector.Z));
		}

		[Test,SilverlightUnitTest]
		public void ImplicitlySettingACoordinateAsVectorShouldReturnSameValuesAsInCoordinate()
		{
			var coordinate = new Coordinate { X = 5f, Y = 6f, Z = 7f };
			Vector vector = coordinate;
			Assert.That(vector.X, Is.EqualTo(coordinate.X));
			Assert.That(vector.Y, Is.EqualTo(coordinate.Y));
			Assert.That(vector.Z, Is.EqualTo(coordinate.Z));
		}

		[Test,SilverlightUnitTest]
		public void SubtractingTwoCoordinatesShouldReturnAVectorWithCorrectResult()
		{
			var c1 = new Coordinate { X = 2f, Y = 4f, Z = 6f };
			var c2 = new Coordinate { X = 1f, Y = 2f, Z = 3f };
			var vector = c1 - c2;
			Assert.That(vector.X, Is.EqualTo(1f));
			Assert.That(vector.Y, Is.EqualTo(2f));
			Assert.That(vector.Z, Is.EqualTo(3f));
		}

		[Test,SilverlightUnitTest]
		public void AddingTwoCoordinatesShouldReturnAVectorWithCorrectResult()
		{
			var c1 = new Coordinate { X = 2f, Y = 4f, Z = 6f };
			var c2 = new Coordinate { X = 1f, Y = 2f, Z = 3f };
			var vector = c1 + c2;
			Assert.That(vector.X, Is.EqualTo(3f));
			Assert.That(vector.Y, Is.EqualTo(6f));
			Assert.That(vector.Z, Is.EqualTo(9f));
		}

		[Test,SilverlightUnitTest]
		public void SubtractingAVectorFromACoordinateShouldReturnAVectorWithCorrectResult()
		{
			var c1 = new Coordinate { X = 2f, Y = 4f, Z = 6f };
			var v2 = new Vector(1f, 2f, 3f);
			var vector = c1 - v2;
			Assert.That(vector.X, Is.EqualTo(1f));
			Assert.That(vector.Y, Is.EqualTo(2f));
			Assert.That(vector.Z, Is.EqualTo(3f));
		}

		[Test,SilverlightUnitTest]
		public void SubtractingACoordinateFromAVectorShouldReturnAVectorWithCorrectResult()
		{
			var v1 = new Vector(2f, 4f, 6f);
			var c2 = new Coordinate { X = 1f, Y = 2f, Z = 3f };
			var vector = v1 - c2;
			Assert.That(vector.X, Is.EqualTo(1f));
			Assert.That(vector.Y, Is.EqualTo(2f));
			Assert.That(vector.Z, Is.EqualTo(3f));
		}

		[Test,SilverlightUnitTest]
		public void AddingAVectorToACoordinateShouldReturnAVectorWithCorrectResult()
		{
			var c1 = new Coordinate { X = 2f, Y = 4f, Z = 6f };
			var v2 = new Vector(1f, 2f, 3f);
			var vector = c1 + v2;
			Assert.That(vector.X, Is.EqualTo(3f));
			Assert.That(vector.Y, Is.EqualTo(6f));
			Assert.That(vector.Z, Is.EqualTo(9f));
		}

		[Test,SilverlightUnitTest]
		public void AddingACoordinateToAVectorShouldReturnAVectorWithCorrectResult()
		{
			var v1 = new Vector(2f, 4f, 6f);
			var c2 = new Coordinate { X = 1f, Y = 2f, Z = 3f };
			var vector = v1 + c2;
			Assert.That(vector.X, Is.EqualTo(3f));
			Assert.That(vector.Y, Is.EqualTo(6f));
			Assert.That(vector.Z, Is.EqualTo(9f));
		}
	}
}
