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
using Balder.Core.Objects.Geometries;
using Geometry = Balder.Core.Objects.Geometries.Geometry;
using Matrix = Balder.Core.Math.Matrix;
using Balder.Core.Extensions;

namespace Balder.Core
{
	public class Scene
	{
		private readonly NodeCollection _renderableNodes;
		private readonly NodeCollection _flatNodes;
		private readonly NodeCollection _environmentalNodes;

		public Color AmbientColor;

		public Scene()
		{
			_renderableNodes = new NodeCollection();
			_flatNodes = new NodeCollection();
			_environmentalNodes = new NodeCollection();

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
			}
		}

		/// <summary>
		/// Gets all the renderable nodes in the scene
		/// </summary>
		public NodeCollection RenderableNodes { get { return _renderableNodes; } }

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

		public void Render(Viewport viewport, Matrix view, Matrix projection)
		{
			lock (_renderableNodes)
			{
				foreach (RenderableNode node in _renderableNodes)
				{
					node.PrepareRender();
					node.Render(viewport, view, projection);
				}
			}
		}


		public void HandleMouseEvents(Viewport viewport, Mouse mouse)
		{
			var objectHit = GetNodeAtScreenCoordinate(viewport, mouse.XPosition, mouse.YPosition);
			if (null != objectHit)
			{
				objectHit.OnHover();
				if( mouse.LeftButton.IsEdge )
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
			var nearPoint = viewport.Unproject(nearSource, view.ProjectionMatrix, view.ViewMatrix, Matrix.Identity);
			var farPoint = viewport.Unproject(farSource, view.ProjectionMatrix, view.ViewMatrix, Matrix.Identity);

			var direction = farPoint - nearPoint;
			direction.Normalize();

			var pickRay = new Ray(nearPoint, direction);

			var closestObjectDistance = float.MaxValue;
			RenderableNode closestObject = null;

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
