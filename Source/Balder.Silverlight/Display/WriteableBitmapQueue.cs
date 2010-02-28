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

using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Balder.Silverlight.Display
{
	public enum BufferType
	{
		Clear,
		Render,
		Show
	}


	public class WriteableBitmapQueue
	{


		private readonly Dictionary<BufferType,Queue<WriteableBitmap>> _bufferQueues;
		private readonly int _width;
		private readonly int _height;

		public WriteableBitmapQueue(int width, int height)
		{
			_width = width;
			_height = height;
			_bufferQueues = new Dictionary<BufferType, Queue<WriteableBitmap>>();

			EnsureQueueExistence(BufferType.Clear);
			EnsureQueueExistence(BufferType.Show);
			EnsureQueueExistence(BufferType.Render);

			/*
			for( var i=0; i<20; i++ )
			{
				CreateAndEnqueue(BufferType.Clear);
				CreateAndEnqueue(BufferType.Show);
				CreateAndEnqueue(BufferType.Render);
			}*/

		}

		private void CreateAndEnqueue(BufferType bufferType)
		{
			var writeableBitmap = new WriteableBitmap(_width, _height);
			Enqueue(bufferType,writeableBitmap);
		}

		public WriteableBitmap GetClearBitmap()
		{
			var bitmap = Dequeue(BufferType.Clear);
			return bitmap;
		}

		public void ClearCompleteForBitmap(WriteableBitmap bitmap)
		{
			Enqueue(BufferType.Render,bitmap);
		}

		public WriteableBitmap GetRenderBitmap()
		{
			var bitmap = Dequeue(BufferType.Render);
			return bitmap;
		}

		public void RenderCompleteForBitmap(WriteableBitmap bitmap)
		{
			Enqueue(BufferType.Show, bitmap);
		}

		public WriteableBitmap GetShowBitmap()
		{
			var bitmap = Dequeue(BufferType.Show);
			return bitmap;
		}

		public void ShowCompleteForBitmap(WriteableBitmap bitmap)
		{
			Enqueue(BufferType.Clear, bitmap);
		}

		private void EnsureQueueExistence(BufferType bufferType)
		{
			if (!_bufferQueues.ContainsKey(bufferType))
			{
				_bufferQueues[bufferType] = new Queue<WriteableBitmap>();
			}
		}

		private WriteableBitmap Dequeue(BufferType bufferType)
		{
			var queue = _bufferQueues[bufferType];
			lock (queue)
			{
				WriteableBitmap bitmap;
				
				if (queue.Count == 0) 
				{
					bitmap = new WriteableBitmap(_width,_height);
				}
				else
				{
					bitmap = queue.Dequeue();
				}
				
				
				return bitmap;
			}
		}

		private void Enqueue(BufferType bufferType, WriteableBitmap bitmap)
		{
			var queue = _bufferQueues[bufferType];
			lock (queue)
			{
				queue.Enqueue(bitmap);
			}
		}
	}
}
