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
using Balder.Core.Display;
using Balder.Core.Extensions;
using Balder.Core.Math;
using NUnit.Framework;

namespace Balder.Core.Tests.Extensions
{
	[TestFixture]
	public class ViewportExtensionsTests
	{
		[Test]
		public void UnprojectingCenterOfViewportWithIdentityViewShouldGenerateAVectorAtCenter()
		{
			var viewport = new Viewport { Width = 640, Height = 480 };

			var aspect = (float) viewport.Height/(float) viewport.Width;
			var projection = Matrix.CreatePerspectiveFieldOfView(40f, aspect, 1, 4000f);
			var view = Matrix.Identity;
			var world = Matrix.Identity;
			var position = new Vector((float)viewport.Width/2, (float)viewport.Height/2,0);
			var result = viewport.Unproject(position, projection, view, world);
			Assert.That(result,Is.EqualTo(new Vector(0,0,-1)));
		}

		[Test]
		public void UnprojectingCenterOfViewportWithViewRotated90DegreesAroundYAxisShouldGenerateAVectorRotated90DegreesInOpositeDirection()
		{
			var viewport = new Viewport { Width = 640, Height = 480 };
			var aspect = (float)viewport.Height / (float)viewport.Width;
			var projection = Matrix.CreatePerspectiveFieldOfView(40f, aspect, 1, 4000f);
			var view = Matrix.CreateRotationY(90);
			var world = Matrix.Identity;
			var position = new Vector((float)viewport.Width / 2, (float)viewport.Height / 2, 0);
			var result = viewport.Unproject(position, projection, view, world);

			var negativeView = Matrix.CreateRotationY(-90);
			var expected = new Vector(0, 0, -1);
			var rotatedExpected = expected*negativeView;

			Assert.That(result, Is.EqualTo(rotatedExpected));
		}

	}
}
