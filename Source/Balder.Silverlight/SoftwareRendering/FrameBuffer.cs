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
using System.Windows.Media.Imaging;
using Balder.Core.SoftwareRendering;

namespace Balder.Silverlight.SoftwareRendering
{
	public class FrameBuffer : IFrameBuffer
	{

		private WriteableBitmap _backBufferBitmap;
		private WriteableBitmap _frontBufferBitmap;
		private WriteableBitmap _clearBufferBitmap;

		public void Initialize(int width, int height)
		{
			Stride = width;

			_backBufferBitmap = new WriteableBitmap(width, height);
			_frontBufferBitmap = new WriteableBitmap(width, height);
			_clearBufferBitmap = new WriteableBitmap(width, height);


			/*
			FillBuffer(_backBufferBitmap,0xffffff);
			FillBuffer(_clearBufferBitmap, 0xffffff);
			FillBuffer(_frontBufferBitmap, 0xffffff);
			 * */
		}

		private void FillBuffer(WriteableBitmap buffer, int color)
		{
			for( var index=0; index<buffer.Pixels.Length; index++)
			{
				buffer.Pixels[index] = color;
			}
			
		}


		public int Stride { get; private set; }
		public int RedPosition { get { return 2; } }
		public int BluePosition { get { return 0; } }
		public int GreenPosition { get { return 1; } }
		public int AlphaPosition { get { return 3; } }
		public WriteableBitmap BackBufferBitmap { get { return _backBufferBitmap; } }
		public WriteableBitmap FrontBufferBitmap { get { return _frontBufferBitmap; } }
		public WriteableBitmap ClearBufferBitmap { get { return _clearBufferBitmap; } }

		public int[] Pixels { get { return BackBufferBitmap.Pixels; } }

		public void Swap()
		{
			var frontBufferBitmap = _frontBufferBitmap;
			var backBufferBitmap = _backBufferBitmap;
			var clearBufferBitmap = _clearBufferBitmap;

			_frontBufferBitmap = backBufferBitmap;
			_clearBufferBitmap = frontBufferBitmap;
			_backBufferBitmap = clearBufferBitmap;
		}


		public void Clear()
		{
			Array.Clear(_clearBufferBitmap.Pixels, 0, _clearBufferBitmap.Pixels.Length);
		}


		public void Show()
		{
		}

		public void Update()
		{
		}
	}
}