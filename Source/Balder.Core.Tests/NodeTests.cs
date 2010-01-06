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

using System.Windows.Media;
using CThru.Silverlight;
using NUnit.Framework;
using Geometry=Balder.Core.Objects.Geometries.Geometry;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class NodeTests
	{
		[Test, SilverlightUnitTest]
		public void SettingColorOnNodeShouldSetColorOnChildren()
		{
			var parent = new Geometry();
			var child = new Geometry();
			parent.Children.Add(child);

			parent.Color = Color.FromSystemColor(Colors.Green);

			Assert.That(child.Color, Is.EqualTo(parent.Color));
		}

		[Test, SilverlightUnitTest]
		public void SettingColorOnNodeShouldSetColorOnEntireHierarchy()
		{
			var parent = new Geometry();
			var child = new Geometry();
			parent.Children.Add(child);
			var childOfChild = new Geometry();
			child.Children.Add(childOfChild);

			parent.Color = Color.FromSystemColor(Colors.Green);

			Assert.That(child.Color, Is.EqualTo(parent.Color));
			Assert.That(childOfChild.Color, Is.EqualTo(parent.Color));
		}

	}
}
