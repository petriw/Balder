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
using Balder.Core.Objects.Geometries;
using NUnit.Framework;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class SceneTests
	{
		[Test]
		public void GettingObjectAtCenterOfScreenWithSingleObjectAtCenterOfSceneShouldReturnTheObject()
		{
			var viewport = new Viewport {Width = 640, Height = 480};
			var scene = new Scene();
			var camera = new Camera(viewport) {Position = {Z = -10}};
			camera.Update();

			var node = new Geometry
			           	{
			           		BoundingSphere = new BoundingSphere(Vector.Zero, 10f)
			           	};
			scene.AddNode(node);

			var nodeAtCoordinate = scene.GetNodeAtScreenCoordinate(viewport, viewport.Width / 2, viewport.Height / 2);
			Assert.That(nodeAtCoordinate, Is.Not.Null);
			Assert.That(nodeAtCoordinate, Is.EqualTo(node));
		}

		[Test]
		public void GettingObjectAtTopLeftOfScreenWithSingleObjectAtCenterOfSceneShouldReturnTheObject()
		{
			var viewport = new Viewport { Width = 640, Height = 480 };
			var scene = new Scene();
			var camera = new Camera(viewport);

			camera.Position.Z = -100;

			camera.Update();

			var node = new Geometry
			           	{
			           		BoundingSphere = new BoundingSphere(Vector.Zero, 10f)
			           	};
			scene.AddNode(node);

			var nodeAtCoordinate = scene.GetNodeAtScreenCoordinate(viewport, 0, 0);
			Assert.That(nodeAtCoordinate, Is.Null);
		}

	}
}
