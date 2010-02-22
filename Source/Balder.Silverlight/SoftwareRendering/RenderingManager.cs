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
using System.ComponentModel;
using System.Threading;
using System.Windows.Media;
using Balder.Core.Diagnostics;
using Balder.Core.Extensions;
using Balder.Silverlight.Notification;

namespace Balder.Silverlight.SoftwareRendering
{
	public delegate void RenderEventHandler();

	public class RenderingManager
	{
		public static readonly RenderingManager Instance = new RenderingManager();

		public event RenderEventHandler Updated = () => { };
		public event RenderEventHandler Render = () => { };
		public event RenderEventHandler Clear = () => { };
		public event RenderEventHandler Swapped = () => { };
		public event RenderEventHandler Show = () => { };

		private ManualResetEvent _renderWait;
		private ManualResetEvent _clearWait;

		private Thread _renderThread;
		private Thread _clearThread;

		private bool _frameBufferManagerAlive;

		private RenderingManager()
		{
		}

		public void Start()
		{
			_frameBufferManagerAlive = true;

			_renderWait = new ManualResetEvent(false);
			_clearWait = new ManualResetEvent(false);

			_renderThread = new Thread(RenderThread);
			_clearThread = new Thread(ClearThread);

			//_renderThread.Start();
			//_clearThread.Start();

			CompositionTarget.Rendering += ShowTimer;
		}

		public void Stop()
		{
			_frameBufferManagerAlive = false;
		}


		private void RenderThread()
		{
			while (_frameBufferManagerAlive)
			{
				//_renderWait.WaitOne();
				Render();
				//_renderWait.Reset();
			}
		}

		
		private void ClearThread()
		{
			while (_frameBufferManagerAlive)
			{
				//_clearWait.WaitOne();
				Clear();
				//_clearWait.Reset();
			}
		}

		private void ShowTimer(object sender, EventArgs e)
		{
			//_renderWait.Set();
			//_clearWait.Set();
			//Show();
			Updated();
			//return;
			//if (_hasCleared && _hasRendered)
			{			
				Render();
				Swapped();

				Clear();

				Show();
			}

			
		}

		
	}
}
