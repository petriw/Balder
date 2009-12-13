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

		private Thread _renderThread;
		private Thread _clearThread;

		private bool _frameBufferManagerAlive;

		private bool _hasCleared;
		private bool _hasRendered;

		private RenderingManager()
		{
		}

		public void Start()
		{
			_frameBufferManagerAlive = true;

			_renderEvent = new ManualResetEvent(true);
			_clearEvent = new ManualResetEvent(true);

			
			_renderThread = new Thread(RenderThread);
			_clearThread = new Thread(ClearThread);

			_renderThread.Start();
			_clearThread.Start();


			_hasCleared = false;
			_hasRendered = false;

			CompositionTarget.Rendering += ShowTimer;
		}

		public void Stop()
		{
			_frameBufferManagerAlive = false;

			_renderEvent.Set();
			_clearEvent.Set();
		}


		private void RenderThread()
		{
			while (_frameBufferManagerAlive)
			{
				_renderEvent.WaitOne(200);
				_renderEvent.Reset();
				//Render();

				_hasRendered = true;
			}
		}

		private void ClearThread()
		{
			while (_frameBufferManagerAlive)
			{
				_clearEvent.WaitOne(200);
				_clearEvent.Reset();
				//Clear();

				_hasCleared = true;
			}
		}

		private void ShowTimer(object sender, EventArgs e)
		{
			var startEvents = new[]
			                  	{
			                  		_renderEvent,
			                  		_clearEvent
			                  	};

			Updated();
			//if (_hasCleared && _hasRendered)
			{
				Render();
				Swapped();
				Clear();
				Show();
				
				_hasCleared = false;
				_hasRendered = false;
			}
			startEvents.SetAll();
			
			
		}
	}
}
