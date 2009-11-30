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
using System;
using Balder.Core.Display;
using System.Collections.Generic;
using Balder.Core.Execution;
using Balder.Silverlight.SoftwareRendering;

namespace Balder.Silverlight.Display
{
	public class DisplayDevice : IDisplayDevice
	{
		public event DisplayEvent Update = (d) => { };
		public event DisplayEvent Render = (d) => { };

		private readonly List<Display> _displays;
		private readonly IPlatform _platform;

		public DisplayDevice(IPlatform platform)
		{
			_platform = platform;
			_displays = new List<Display>();
			InitializeRendering();
		}

		private void InitializeRendering()
		{
			if (!_platform.IsInDesignMode)
			{
				RenderingManager.Instance.Render += RenderingManagerRender;
				RenderingManager.Instance.Show += RenderingManagerShow;
				RenderingManager.Instance.Clear += RenderingManagerClear;
				RenderingManager.Instance.Swapped += RenderingManagerSwapped;
				RenderingManager.Instance.Updated += RenderingManagerUpdated;
				RenderingManager.Instance.Start();
			}
		}


		private void RenderingManagerRender()
		{
			foreach (var display in _displays)
			{
				display.PrepareRender();
				Render(display);
			}
		}

		private void RenderingManagerClear()
		{
			CallMethodOnDisplays(d => d.Clear());
		}

		private void RenderingManagerShow()
		{
			CallMethodOnDisplays(d => d.Show());
		}

		private void RenderingManagerSwapped()
		{
			CallMethodOnDisplays(d => d.Swap());
		}

		private void RenderingManagerUpdated()
		{
			CallMethodOnDisplays(d => d.Update());
			foreach (var display in _displays)
			{
				Update(display);
			}
		}

		public IDisplay CreateDisplay()
		{
			var display = new Display(_platform);
			_displays.Add(display);
			return display;
		}

		private void CallMethodOnDisplays(Action<Display> displayAction)
		{
			foreach (var display in _displays)
			{
				displayAction(display);
			}
		}

		public void RenderAndShow(Display display)
		{
			display.PrepareRender();
			display.Swap();
			display.Clear();
			Render(display);
			display.Swap();
			display.Show();
			display.Update();
		}
	}
}
