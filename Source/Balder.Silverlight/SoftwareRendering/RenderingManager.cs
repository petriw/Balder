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
using System.Threading;
using System.Windows.Media;
using Balder.Core.Extensions;

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

		private ManualResetEvent _renderEvent;
		private ManualResetEvent _clearEvent;
		private ManualResetEvent _showEvent;
		private ManualResetEvent _renderFinishedEvent;
		private ManualResetEvent _clearFinishedEvent;
		private ManualResetEvent _showFinishedEvent;

		private Thread _renderThread;
		private Thread _clearThread;
		private Thread _showThread;
		private Thread _swapThread;

		private bool _frameBufferManagerAlive;

		private RenderingManager()
		{
		}

		public void Start()
		{
			_frameBufferManagerAlive = true;

			_renderEvent = new ManualResetEvent(false);
			_clearEvent = new ManualResetEvent(false);
			_showEvent = new ManualResetEvent(false);
			_renderFinishedEvent = new ManualResetEvent(true);
			_clearFinishedEvent = new ManualResetEvent(true);
			_showFinishedEvent = new ManualResetEvent(true);

			CompositionTarget.Rendering += ShowTimer;

			_swapThread = new Thread(SwapThread);
			_showThread = new Thread(ShowThread);
			_renderThread = new Thread(RenderThread);
			_clearThread = new Thread(ClearThread);

			_swapThread.Start();
			_showThread.Start();
			_renderThread.Start();
			_clearThread.Start();
		}

		public void Stop()
		{
			_frameBufferManagerAlive = false;

			_renderEvent.Set();
			_clearEvent.Set();
			_showEvent.Set();

			_renderFinishedEvent.Set();
			_clearFinishedEvent.Set();
			_showFinishedEvent.Set();
		}

		private void SwapThread()
		{
			var waitEvents = new[]
			                 	{
									_showFinishedEvent,
									_renderFinishedEvent,
									_clearFinishedEvent
			                 	};
			var startEvents = new[]
			                  	{
			                  		_showEvent,
			                  		_renderEvent,
			                  		_clearEvent
			                  	};

			while (_frameBufferManagerAlive)
			{
				WaitHandle.WaitAll(waitEvents);
				Swapped();
				waitEvents.ResetAll();
				startEvents.SetAll();
			}
		}

		private void ShowThread()
		{
			while (_frameBufferManagerAlive)
			{
				_showEvent.WaitOne();
				Show();
				_showEvent.Reset();
				_showFinishedEvent.Set();
			}
		}


		private void RenderThread()
		{
			while (_frameBufferManagerAlive)
			{
				_renderEvent.WaitOne();
				Render();
				_renderEvent.Reset();
				_renderFinishedEvent.Set();
			}
		}

		private void ClearThread()
		{
			while (_frameBufferManagerAlive)
			{
				_clearEvent.WaitOne();
				Clear();
				_clearEvent.Reset();
				_clearFinishedEvent.Set();
			}
		}

		private void ShowTimer(object sender, EventArgs e)
		{
			Updated();
		}
	}
}
