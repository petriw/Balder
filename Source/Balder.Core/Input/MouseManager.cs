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
using Balder.Core.Utils;

namespace Balder.Core.Input
{
	public class MouseManager : IMouseManager
	{
		private readonly IMouseDevice _mouseDevice;

		public MouseManager(IMouseDevice mouseDevice)
		{
			_mouseDevice = mouseDevice;
		}

		public void HandleButtonSignals(Mouse mouse)
		{
			var buttons = EnumHelper.GetValues<MouseButton>();
			foreach( var button in buttons )
			{
				var mouseButtonState = GetMouseButtonStateObjectFromMouse(mouse, button);
				if (null != mouseButtonState)
				{

					/*
					psInput->vEdge = (~psInput->vPrev) & (psInput->vCurrent);
					psInput->vRepeat = psInput->vEdge;
					 * */

					mouseButtonState.IsPreviousEdge = mouseButtonState.IsDown;
					mouseButtonState.IsDown = _mouseDevice.IsButtonPressed(button);
					mouseButtonState.IsEdge = false;
					mouseButtonState.IsEdge = (mouseButtonState.IsEdge^!mouseButtonState.IsPreviousEdge) & (mouseButtonState.IsDown);
				}
			}
		}


		private static MouseButtonState GetMouseButtonStateObjectFromMouse(Mouse mouse, MouseButton button)
		{
			switch( button )
			{
				case MouseButton.Left:
					{
						return mouse.LeftButton;
					}
				
				case MouseButton.Right:
					{
						return mouse.RightButton;
					}
			}
			return null;
		}

		public void HandlePosition(Mouse mouse)
		{
			mouse.XPosition = _mouseDevice.GetXPosition();
			mouse.YPosition = _mouseDevice.GetYPosition();
		}
	}
}
