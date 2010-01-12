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

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.SoftwareRendering;
using Balder.Silverlight.SoftwareRendering;
using Color = Balder.Core.Color;

namespace Balder.Silverlight.Display
{
	public class Display : IDisplay, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private readonly IPlatform _platform;
		private FrameBuffer _framebuffer;

		private bool _initialized;

		public Display(IPlatform platform)
		{
			_framebuffer = new FrameBuffer();
			_platform = platform;
		}

		public void Initialize(int width, int height)
		{
			_framebuffer.Initialize(width, height);
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

		public void PrepareRender()
		{
			BufferContainer.Framebuffer = _framebuffer.BackBufferBitmap.Pixels;
			BufferContainer.DepthBuffer = _framebuffer.BackDepthBuffer;
		}


		public void Swap()
		{
			if (_initialized)
			{
				_framebuffer.Swap();
			}
		}

		public void Clear()
		{
			if (_initialized)
			{
				_framebuffer.Clear();
			}
		}

		public void Show()
		{
			if (_initialized)
			{
				_framebuffer.Show();
				if (null != _image)
				{
					var bitmap = _framebuffer.FrontBufferBitmap;
					if (null != bitmap)
					{
						_image.Source = bitmap;
						bitmap.Invalidate();
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