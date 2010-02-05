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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.SoftwareRendering;
using Color = Balder.Core.Color;

namespace Balder.Silverlight.Display
{
	public class Display : IDisplay, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private readonly IPlatform _platform;
		private WriteableBitmapQueue _bitmapQueue;

		private bool _initialized;

		public Display(IPlatform platform)
		{
			_platform = platform;
		}

		public void Initialize(int width, int height)
		{
			//_framebuffer.Initialize(width, height);


			_bitmapQueue = new WriteableBitmapQueue(width,height);
			_frontDepthBuffer = new UInt32[width*height];

			BufferContainer.Width = width;
			BufferContainer.Height = height;
			BufferContainer.RedPosition = 2;
			BufferContainer.GreenPosition = 0;
			BufferContainer.BluePosition = 1;
			BufferContainer.AlphaPosition = 3;
			BufferContainer.Stride = width;
			_initialized = true;
			BackgroundColor = Color.FromArgb(0xff, 0, 0, 0);
		}

		private Image _image;
		public void InitializeContainer(object container)
		{
			if (container is Grid)
			{
				_image = new Image
				{
					Stretch = Stretch.None
				};
				((Grid)container).Children.Add(_image);
			}
		}

		public Color BackgroundColor { get; set; }

		private WriteableBitmap _currentFrontBitmap;
		private WriteableBitmap _currentRenderBitmap;
		
		private UInt32[] _frontDepthBuffer;

		public void PrepareRender()
		{
			if (_initialized)
			{
				_currentRenderBitmap = _bitmapQueue.GetRenderBitmap();
				
				BufferContainer.Framebuffer = _currentRenderBitmap.Pixels;
				BufferContainer.DepthBuffer = _frontDepthBuffer;
				Array.Clear(_frontDepthBuffer,0,_frontDepthBuffer.Length);
			}
		}

		public void AfterRender()
		{
			if (_initialized)
			{
				_bitmapQueue.RenderCompleteForBitmap(_currentRenderBitmap);
			}

		}

		public void Render()
		{
			
		}


		public void Swap()
		{
			if (_initialized)
			{
			}
		}

		public void Clear()
		{
			if (_initialized)
			{
				var clearBitmap = _bitmapQueue.GetClearBitmap();
				Array.Clear(clearBitmap.Pixels,0,clearBitmap.Pixels.Length);

				_bitmapQueue.ClearCompleteForBitmap(clearBitmap);
			}
		}


		public void Show()
		{
			if (_initialized)
			{
				_bitmapQueue.UpdateStatistics();
				if (null != _image)
				{
					if( null != _currentFrontBitmap )
					{
						_bitmapQueue.ShowCompleteForBitmap(_currentFrontBitmap);
					}
					_currentFrontBitmap = _bitmapQueue.GetShowBitmap();
					if (null != _currentFrontBitmap)
					{
						_image.Source = _currentFrontBitmap;
						_currentFrontBitmap.Invalidate();
						
					}
				}
			}
		}

		public void Update()
		{
			if (_initialized)
			{
			}
		}
	}
}