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
using Balder.Core.Assets;
using Balder.Core.Content;
using Balder.Core.Tests.Stubs;
using NUnit.Framework;

namespace Balder.Core.Tests.Content
{
	[TestFixture]
	public class ContentManagerTests
	{
		public class MyAsset : IAsset
		{
			public bool LoadCalled = false;
			public void Load(string assetName)
			{
				LoadCalled = true;
			}
		}

		[Test]
		public void LoadingAssetShouldCallLoadOnAsset()
		{
			var objectFactoryStub = new ObjectFactoryStub();
			var contentManager = new ContentManager(objectFactoryStub);
			var asset = contentManager.Load<MyAsset>("something");
			Assert.That(asset,Is.Not.Null);
			Assert.That(asset.LoadCalled,Is.True);
		}
	}
}

