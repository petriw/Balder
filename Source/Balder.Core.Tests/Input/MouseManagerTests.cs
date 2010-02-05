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
using System;
using Balder.Core.Input;
using Moq;
using NUnit.Framework;

namespace Balder.Core.Tests.Input
{
	[TestFixture]
	public class MouseManagerTests
	{

		[Test]
		public void LeftMouseButtonPressedShouldResultInEdgeSetForButton()
		{
			var mouse = new Mouse();

			var mouseDeviceMock = new Mock<IMouseDevice>();
			mouseDeviceMock.Expect(m => m.IsButtonPressed(MouseButton.Left)).Returns(true);
			var mouseManager = new MouseManager(mouseDeviceMock.Object);
			mouseManager.HandleButtonSignals(mouse);

			Assert.That(mouse.LeftButton.IsEdge,Is.True);
		}

		[Test]
		public void LeftMouseButtonPressedShouldResultInDownSetForButton()
		{
			var mouse = new Mouse();

			var mouseDeviceMock = new Mock<IMouseDevice>();
			mouseDeviceMock.Expect(m => m.IsButtonPressed(MouseButton.Left)).Returns(true);
			var mouseManager = new MouseManager(mouseDeviceMock.Object);
			mouseManager.HandleButtonSignals(mouse);

			Assert.That(mouse.LeftButton.IsDown, Is.True);
		}

		[Test]
		public void LeftMouseButtonHeldForMultipleHandlesShouldSetEdgeSignalToFalse()
		{
			var mouse = new Mouse();

			var mouseDeviceMock = new Mock<IMouseDevice>();
			mouseDeviceMock.Expect(m => m.IsButtonPressed(MouseButton.Left)).Returns(true);
			var mouseManager = new MouseManager(mouseDeviceMock.Object);
			for (var i = 0; i < 10; i++)
			{
				mouseManager.HandleButtonSignals(mouse);
			}

			Assert.That(mouse.LeftButton.IsEdge, Is.False);
		}

		[Test]
		public void MousePositionsShouldGetUpdatedFromDevice()
		{
			var mouse = new Mouse();

			var random = new Random();
			var expectedXPosition = random.Next();
			var expectedYPosition = random.Next();

			var mouseDeviceMock = new Mock<IMouseDevice>();
			mouseDeviceMock.Expect(m => m.GetXPosition()).Returns(expectedXPosition);
			mouseDeviceMock.Expect(m => m.GetYPosition()).Returns(expectedYPosition);

			var mouseManager = new MouseManager(mouseDeviceMock.Object);
			mouseManager.HandlePosition(mouse);

			Assert.That(mouse.XPosition, Is.EqualTo(expectedXPosition));
			Assert.That(mouse.YPosition, Is.EqualTo(expectedYPosition));
		}
	}
}
