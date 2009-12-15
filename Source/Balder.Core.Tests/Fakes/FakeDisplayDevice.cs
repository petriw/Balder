﻿#region License
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
using Moq;

namespace Balder.Core.Tests.Fakes
{
	public class FakeDisplayDevice : IDisplayDevice
	{
		public event DisplayEvent Update = (d) => { };
		public event DisplayEvent Render = (d) => { };



		public IDisplay CreateDisplay()
		{
			var mock = new Mock<IDisplay>();
			return mock.Object;
		}

		public void RemoveDisplay(IDisplay display)
		{
			throw new NotImplementedException();
		}

		public void FireUpdateEvent(IDisplay display)
		{
			Update(display);
		}

		public void FireRenderEvent(IDisplay display)
		{
			Render(display);
		}
	}
}
