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
using System.Windows;
using System.Windows.Input;
using Balder.Core.Input;

namespace Balder.Silverlight.Input
{
	public class MouseDevice : IMouseDevice
	{
		private UIElement _element;
		private int _xPosition;
		private int _yPosition;
		private bool _isLeftButtonPressed;

		public void Initialize(UIElement element)
		{
			_element = element;
			element.MouseLeftButtonDown += MouseLeftButtonDown;
			element.MouseLeftButtonUp += MouseLeftButtonUp;
			element.MouseMove += MouseMove;
		}

		private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isLeftButtonPressed = false;
		}

		private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_isLeftButtonPressed = true;
		}

		private void MouseMove(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(_element);
			_xPosition = (int) position.X;
			_yPosition = (int) position.Y;
		}

		public bool IsButtonPressed(MouseButton button)
		{
			return _isLeftButtonPressed;
		}

		public int GetXPosition()
		{
			return _xPosition;
		}

		public int GetYPosition()
		{
			return _yPosition;
		}
	}
}
