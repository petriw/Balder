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
using Balder.Core.Collections;
using Balder.Core.Display;
using Balder.Core.Input;
using Balder.Core.Lighting;
using Balder.Core.Math;
using Balder.Core.Objects.Flat;
using Matrix = Balder.Core.Math.Matrix;
using Balder.Core.Extensions;

namespace Balder.Core
{
	public class Scene
	{
		private readonly NodeCollection _renderableNodes;
		private readonly NodeCollection _flatNodes;
		private readonly NodeCollection _environmentalNodes;
		private readonly NodeCollection _lights;

		public Color AmbientColor;

		public Scene()
		{
			_renderableNodes = new NodeCollection();
			_flatNodes = new NodeCollection();
			_environmentalNodes = new NodeCollection();
			_lights = new NodeCollection();

			AmbientColor = Color.FromArgb(0xff, 0x3f, 0x3f, 0x3f);
		}

		public void AddNode(Node node)
		{
			node.Scene = this;
			if (node is RenderableNode)
			{
				lock (_renderableNodes)
				{
					_renderableNodes.Add(node);

				}
				if (node is Sprite)
				{
					lock (_flatNodes)
					{
						_flatNodes.Add(node);
					}
				}
			}
			else
			{
				lock (_environmentalNodes)
				{
					_environmentalNodes.Add(node);
				}
				if( node is ILight )
				{
					lock( _lights )
					{
						_lights.Add(node);
					}
				}
			}
		}

		/// <summary>
		/// Gets all the renderable nodes in the scene
		/// </summary>
		public NodeCollection RenderableNodes { get { return _renderableNodes; } }

		/// <summary>
		/// Gets all the lights in the scene
		/// </summary>
		public NodeCollection Lights { get { return _lights; } }

		/*
		public Color CalculateColorForVector(Viewport viewport, Vector vector, Vector normal)
		{
			return CalculateColorForVector(viewport, vector, normal, Color.Black, Color.Black, Color.Black);
		}

		public Color CalculateColorForVector(Viewport viewport, Vector vector, Vector normal, Color vectorAmbient, Color vectorDiffuse, Color vectorSpecular)
		{
			var color = AmbientColor.Additive(vectorDiffuse);

			lock (_environmentalNodes)
			{
				foreach (var node in _environmentalNodes)
				{
					if (node is Light)
					{
						var light = node as Light;
						var currentLightResult = light.Calculate(viewport, vector, normal);
						var currentLightResultAsVector = currentLightResult;
						
						color += currentLightResultAsVector;
					}
				}
				color.Clamp();
				return color;
			}
		}
		 * */

		public void Render(Viewport viewport, Matrix view, Matrix projection)
		{
			lock (_renderableNodes)
			{
				foreach (RenderableNode node in _renderableNodes)
				{
					var world = Matrix.Identity;
					RenderNode(node, viewport, view, projection, world);
				}
			}
		}

		private static void RenderNode(RenderableNode node, Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			world = node.World * world;
			node.Render(viewport, view, projection, world);
			foreach (var child in node.Children)
			{
				if (child is RenderableNode)
				{
					RenderNode((RenderableNode)child, viewport, view, projection, world);
				}
			}
		}



		public void HandleMouseEvents(Viewport viewport, Mouse mouse)
		{
			var objectHit = GetNodeAtScreenCoordinate(viewport, mouse.XPosition, mouse.YPosition);
			if (null != objectHit)
			{
				objectHit.OnHover();
				if (mouse.LeftButton.IsEdge)
				{
					objectHit.OnClick();
				}
			}
		}


		/// <summary>
		/// Get a node at a specified screen coordinate relative to a specific viewport
		/// </summary>
		/// <param name="viewport">Viewport to find node in</param>
		/// <param name="x">X position</param>
		/// <param name="y">Y position</param>
		/// <returns>A RenderableNode - null if it didn't find any node at the position</returns>
		public RenderableNode GetNodeAtScreenCoordinate(Viewport viewport, int x, int y)
		{
			var nearSource = new Vector((float)x, (float)y, 0f);
			var farSource = new Vector((float)x, (float)y, 1f);
			var view = viewport.View;
			var world = Matrix.CreateTranslation(0, 0, 0);
			var nearPoint = viewport.Unproject(nearSource, view.ProjectionMatrix, view.ViewMatrix, world);
			var farPoint = viewport.Unproject(farSource, view.ProjectionMatrix, view.ViewMatrix, world);

			var direction = farPoint - nearPoint;
			direction.Normalize();

			var pickRay = new Ray(nearPoint, direction);

			var closestObjectDistance = float.MaxValue;
			RenderableNode closestObject = null;
			return null;
			lock (_renderableNodes)
			{
				foreach (var node in _renderableNodes)
				{
					var transformedSphere = node.BoundingSphere.Transform(node.World);
					var distance = pickRay.Intersects(transformedSphere);
					if (distance.HasValue)
					{
						if (distance < closestObjectDistance)
						{
							closestObject = node as RenderableNode;
							closestObjectDistance = distance.Value;
						}
					}
				}
			}

			return closestObject;
		}
	}
}
