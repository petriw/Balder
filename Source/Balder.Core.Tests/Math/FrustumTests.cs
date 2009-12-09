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
using Balder.Core.Display;
using Balder.Core.Math;
using CThru.Silverlight;
using NUnit.Framework;

namespace Balder.Core.Tests.Math
{
	[TestFixture]
	public class FrustumTests
	{
		private Frustum _frustum;

		[TestFixtureSetUp, SilverlightSetUp]
		public void Setup()
		{
			var viewport = new Viewport { Width = 640, Height = 480 };
			var camera = new Camera(viewport) { Target = Vector.Forward, Position = Vector.Zero };
			camera.Prepare(viewport);
			camera.Update();
			_frustum = new Frustum();
			_frustum.SetCameraDefinition(camera);
		}

		[Test, SilverlightUnitTest]
		public void VectorInsideShouldNotBeClipped()
		{
			var vectorToTest = new Vector(0f, 0f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Inside));
		}

		[Test]
		public void VectorAboveTopShouldBeClipped()
		{
			var vectorToTest = new Vector(0f, 1000f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorBelowBottomShouldBeClipped()
		{
			var vectorToTest = new Vector(0f, -1000f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorLeftOfLeftShouldBeClipped()
		{
			var vectorToTest = new Vector(-1000f, 0f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorRightOfRightShouldBeClipped()
		{
			var vectorToTest = new Vector(1000f, 0f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorBehindNearShouldBeClipped()
		{
			var vectorToTest = new Vector(0f, 0f, -1000f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorBeyondFarShouldBeClipped()
		{
			var vectorToTest = new Vector(0f, 0f, 1000f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}
	}
}
