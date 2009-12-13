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

using Balder.Core.Helpers;
using Balder.Silverlight.Controls.Math;

namespace Balder.Silverlight.Controls
{
	public class Camera : Node
	{
		public Core.View.Camera ActualCamera
		{
			get { return ActualNode as Core.View.Camera; }
			set
			{
				ActualNode = value;
				InitializeActualCamera(value);
			}
		}

		public Camera()
		{
		}

		public Camera(Core.View.Camera camera)
		{
			InitializeActualCamera(camera);
		}

		private void InitializeActualCamera(Core.View.Camera camera)
		{
			var position = camera.Position;
			ActualNode = camera;
			var target = new Vector();

			Position.X = position.X;
			Position.Y = position.Y;
			Position.Z = position.Z;

			target.X = camera.Target.X;
			target.Y = camera.Target.Y;
			target.Z = camera.Target.Z;

			Target = target;
			
		}


		private void InitializeTarget()
		{
			Target.SetNativeAction((x, y, z) =>
			{
				if (null != ActualCamera)
				{
					ActualCamera.Target.X = x;
					ActualCamera.Target.Y = y;
					ActualCamera.Target.Z = z;
				}
			});

		}

		public static readonly DependencyProperty<Camera, Vector> TargetProperty =
			DependencyProperty<Camera, Vector>.Register(o => o.Target);
		public Vector Target
		{
			get { return TargetProperty.GetValue(this); }
			set
			{
				TargetProperty.SetValue(this, value);
				InitializeTarget();
			}
		}
	}
}
