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
using Balder.Core.SoftwareRendering;

namespace Balder.Silverlight.SoftwareRendering
{
    public class FrameBuffer : IFrameBuffer
    {
    	private int[] _renderbuffer;
		private int[] _clearBuffer;
		private int[] _showBuffer;
    	private int[] _emptyBuffer;
    	private int _length;

    	public void Initialize(int width, int height)
    	{
    		Stride = width;
			_length = width * height;

			_renderbuffer = new int[_length];
			_clearBuffer = new int[_length];
			_showBuffer = new int[_length];
			_emptyBuffer = new int[_length];

			Swap();
    	}


    	public int Stride { get; private set; }
		public int RedPosition { get { return 2; } }
		public int BluePosition { get { return 0; } }
		public int GreenPosition { get { return 1; } }
		public int AlphaPosition { get { return 3; } }
		public int[] Pixels { get { return _renderbuffer; } }
		public int[] BackBuffer { get { return _showBuffer;  } }

		public void Swap()
		{
			var renderBuffer = _renderbuffer;
			var clearBuffer = _clearBuffer;
			var showBuffer = _showBuffer;

			_renderbuffer = clearBuffer;
			_showBuffer = renderBuffer;
			_clearBuffer = showBuffer;
		}


		public void Clear()
		{
			_emptyBuffer.CopyTo(_clearBuffer, 0);
			//_clearBuffer = new int[_length];
		}


		public void Show()
		{
		}

		public void Update()
		{
		}
	}
}