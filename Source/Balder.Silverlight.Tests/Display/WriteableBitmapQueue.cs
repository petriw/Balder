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

using Balder.Silverlight.Display;
using CThru.Silverlight;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Display
{
	[TestFixture]
	public class WriteableBitmapQueueTest
	{
		[Test, SilverlightUnitTest]
		public void GettingClearBitmapShouldReturnValidWriteableBitmap()
		{
			var writeableBitmapQueue = new WriteableBitmapQueue(640, 480);
			var writeableBitmap = writeableBitmapQueue.GetClearBitmap();
			Assert.That(writeableBitmap,Is.Not.Null);
		}

		[Test, SilverlightUnitTest]
		public void CompletingClearBitmapShouldPutItInRenderQueue()
		{
			var writeableBitmapQueue = new WriteableBitmapQueue(640, 480);
			var clearBitmap = writeableBitmapQueue.GetClearBitmap();
			writeableBitmapQueue.ClearCompleteForBitmap(clearBitmap);
			var renderBitmap = writeableBitmapQueue.GetRenderBitmap();
			Assert.That(renderBitmap,Is.EqualTo(clearBitmap));
		}

		[Test, SilverlightUnitTest]
		public void CompletingRenderBitmapShouldPutItInShowQueue()
		{
			var writeableBitmapQueue = new WriteableBitmapQueue(640, 480);
			var renderBitmap = writeableBitmapQueue.GetRenderBitmap();
			writeableBitmapQueue.RenderCompleteForBitmap(renderBitmap);
			var showBitmap = writeableBitmapQueue.GetShowBitmap();
			Assert.That(showBitmap, Is.EqualTo(renderBitmap));
		}
	}
}

