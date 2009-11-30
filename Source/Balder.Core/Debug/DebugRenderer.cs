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
using Balder.Core.Execution;
using Balder.Core.Math;
using Ninject.Core;
using Matrix = Balder.Core.Math.Matrix;

namespace Balder.Core.Debug
{
	[Singleton]
	public class DebugRenderer : IDebugRenderer
	{
		public static readonly Color DebugInfoColor = Color.FromArgb(0xFF,0xFF,0xFF,0);
		private readonly IObjectFactory _objectFactory;

		private DebugShape _boundingSphereDebugShape;

		public DebugRenderer(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
			CreateShapes();
		}


		private void CreateShapes()
		{
			_boundingSphereDebugShape = _objectFactory.Get<BoundingSphereDebugShape>();
			_boundingSphereDebugShape.Initialize();
		}

		public void RenderBoundingSphere(BoundingSphere sphere, Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			var scaleMatrix = Matrix.CreateScale(sphere.Radius);
			var translationMatrix = Matrix.CreateTranslation(sphere.Center) * world;
			var rotateYMatrix = Matrix.CreateRotationY(90);
			var rotateXMatrix = Matrix.CreateRotationX(90);

			_boundingSphereDebugShape.World = scaleMatrix * translationMatrix;
			_boundingSphereDebugShape.Render(viewport, view, projection);

			_boundingSphereDebugShape.World = rotateYMatrix * scaleMatrix * translationMatrix;
			_boundingSphereDebugShape.Render(viewport, view, projection);

			_boundingSphereDebugShape.World = rotateXMatrix * scaleMatrix * translationMatrix;
			_boundingSphereDebugShape.Render(viewport, view, projection);
		}
	}
}
