#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
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
using Balder.Core.Debug;
using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core
{
	public abstract class RenderableNode : Node
	{
		protected virtual void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world) { }
		protected virtual void RenderDebugInfo(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if (viewport.DebugInfo.BoundingSpheres)
			{
				DebugRenderer.Instance.RenderBoundingSphere(BoundingSphere, viewport, view, projection, world);
			}
		}

		internal void OnRender(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			Render(viewport, view, projection, world);
		}

		internal void OnRenderDebugInfo(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			RenderDebugInfo(viewport, view, projection, world);
		}
	}
}
