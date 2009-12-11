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

namespace Balder.Core.Execution
{
	public partial class Game : Actor
	{
		public Game()
		{
			Constructed();
		}

		partial void Constructed();

		public Scene Scene { get; private set; }
		public Viewport Viewport { get; private set; }
		public Camera Camera { get; set; }

		public override void OnBeforeInitialize()
		{
			Scene = new Scene();
			Viewport = new Viewport {Scene = Scene, Width = 800, Height = 600};
			Camera = new Camera(Viewport) {Target = Vector.Forward, Position = Vector.Zero};

			// Todo: bi-directional referencing..  Not a good idea!
			Viewport.Camera = Camera;

			base.OnBeforeInitialize();
		}

		public override void OnBeforeUpdate()
		{
			if( null != MouseManager)
			{
				MouseManager.HandleButtonSignals(Mouse);
				MouseManager.HandlePosition(Mouse);
			}
		}

		public virtual void OnRender()
		{
			Camera.Update();
			Scene.Render(Viewport, Camera.ViewMatrix, Camera.ProjectionMatrix);
			Scene.HandleMouseEvents(Viewport, Mouse);
		}
	}
}