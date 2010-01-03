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
using Balder.Core.Display;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using Balder.Core.View;
using CThru.Silverlight;
using NUnit.Framework;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class SceneTests
	{
		public class MyRenderableNode : RenderableNode
		{
			public bool RenderCalled = false;
			public override void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world)
			{
				RenderCalled = true;
			}
		}

		public class RenderableNodeMock : RenderableNode
		{
			private readonly Action _actionToCall;
			public RenderableNodeMock()
			{
				
			}

			public RenderableNodeMock(Action actionToCall)
			{
				_actionToCall = actionToCall;
			}

			public Matrix WorldResult;
			public override void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world)
			{
				WorldResult = world;
				if (null != _actionToCall)
				{
					_actionToCall();
				}
			}
		}

	
		[Test, SilverlightUnitTest]
		public void GettingObjectAtCenterOfScreenWithSingleObjectAtCenterOfSceneShouldReturnTheObject()
		{
			var viewport = new Viewport {Width = 640, Height = 480};
			var scene = new Scene();
			var camera = new Camera() {Position = {Z = -10}};
			viewport.View = camera;
			camera.Update(viewport);

			var node = new Geometry
			           	{
			           		BoundingSphere = new BoundingSphere(Vector.Zero, 10f)
			           	};
			scene.AddNode(node);

			var nodeAtCoordinate = scene.GetNodeAtScreenCoordinate(viewport, viewport.Width / 2, viewport.Height / 2);
			Assert.That(nodeAtCoordinate, Is.Not.Null);
			Assert.That(nodeAtCoordinate, Is.EqualTo(node));
		}

		[Test, SilverlightUnitTest]
		public void GettingObjectAtTopLeftOfScreenWithSingleObjectAtCenterOfSceneShouldReturnTheObject()
		{
			var viewport = new Viewport { Width = 640, Height = 480 };
			var scene = new Scene();
			var camera = new Camera();
			viewport.View = camera;
			camera.Position.Z = -100;

			camera.Update(viewport);

			var node = new Geometry
			           	{
			           		BoundingSphere = new BoundingSphere(Vector.Zero, 10f)
			           	};
			scene.AddNode(node);

			var nodeAtCoordinate = scene.GetNodeAtScreenCoordinate(viewport, 0, 0);
			Assert.That(nodeAtCoordinate, Is.Null);
		}


		[Test, SilverlightUnitTest]
		public void AddedNodeShouldGetRendered()
		{
			var viewport = new Viewport { Width = 640, Height = 480 };
			var camera = new Camera();
			viewport.View = camera;
			camera.Position.Z = -100;
			camera.Update(viewport);
			var scene = new Scene();

			var renderableNode = new MyRenderableNode();

			scene.AddNode(renderableNode);

			scene.Render(viewport,camera.ViewMatrix,camera.ProjectionMatrix);

			Assert.That(renderableNode.RenderCalled,Is.True);
		}

		[Test, SilverlightUnitTest]
		public void AddingNodeWithChildShouldCallRenderOnChild()
		{
			var viewport = new Viewport { Width = 640, Height = 480 };
			var camera = new Camera();
			viewport.View = camera;
			camera.Position.Z = -100;
			camera.Update(viewport);
			var scene = new Scene();

			var topLevelNode = new MyRenderableNode();
			var childNode = new MyRenderableNode();
			topLevelNode.Children.Add(childNode);

			scene.AddNode(topLevelNode);
			scene.Render(viewport, camera.ViewMatrix, camera.ProjectionMatrix);
			Assert.That(childNode.RenderCalled,Is.True);
		}

		[Test, SilverlightUnitTest]
		public void ChildNodeShouldHaveParentNodesWorldAppliedToWorldMatrix()
		{
			var viewport = new Viewport { Width = 640, Height = 480 };
			var camera = new Camera();
			viewport.View = camera;
			camera.Position.Z = -100;
			camera.Update(viewport);
			var scene = new Scene();

			var topLevelNode = new MyRenderableNode();
			topLevelNode.Position.X = 50;
			var childNode = new RenderableNodeMock();
			topLevelNode.Children.Add(childNode);

			scene.AddNode(topLevelNode);
			scene.Render(viewport, camera.ViewMatrix, camera.ProjectionMatrix);

			var actualPosition = new Coordinate();
			actualPosition.X = childNode.WorldResult[3, 0];
			actualPosition.Y = childNode.WorldResult[3, 1];
			actualPosition.Z = childNode.WorldResult[3, 2];
			Assert.That(actualPosition.X, Is.EqualTo(topLevelNode.Position.X));
		}
	}
}
