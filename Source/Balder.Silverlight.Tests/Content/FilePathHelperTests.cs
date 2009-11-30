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
using Balder.Core.Content;
using Balder.Core.Execution;
using Balder.Core.Utils;
using Balder.Silverlight.Content;
using Moq;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Content
{
	[TestFixture]
	public class FilePathHelperTests
	{
		[Test]
		public void GettingAssetWithoutPathSpecifiedShouldBePrefixedWithComponentPath()
		{
			var platformMock = new Mock<IPlatform>();
			var assemblyName = GetType().Assembly.FullName;
			var assemblyShortName = AssemblyHelper.GetAssemblyShortName(assemblyName);
			platformMock.Expect(p => p.EntryAssemblyName).Returns(assemblyName);

			var contentManagerMock = new Mock<IContentManager>();
			var assetsRoot = string.Format("{0}.Assets", GetType().Namespace);
			contentManagerMock.Expect(c => c.AssetsRoot).Returns(assetsRoot);

			var filePathHelper = new FilePathHelper(contentManagerMock.Object, platformMock.Object);
			var assetName = "teapot.ase";
			var fileName = filePathHelper.GetFileNameForAsset("teapot.ase");

			var expected = string.Format("/{0};component/{1}/{2}", assemblyShortName,assetsRoot, assetName);
			Assert.That(fileName, Is.EqualTo(expected));
		}

		[Test]
		public void GettingAssetWithSpecifiedComponentPathShouldReturnSamePath()
		{
			var platformMock = new Mock<IPlatform>();
			var assemblyName = GetType().Assembly.FullName;
			platformMock.Expect(p => p.EntryAssemblyName).Returns(assemblyName);

			var contentManagerMock = new Mock<IContentManager>();
			var assetsRoot = string.Format("{0}.Assets", GetType().Namespace);
			contentManagerMock.Expect(c => c.AssetsRoot).Returns(assetsRoot);

			var filePathHelper = new FilePathHelper(contentManagerMock.Object, platformMock.Object);
			var assetName = "/SomeAssembly;component/Resources/tepot.ase";
			var filename = filePathHelper.GetFileNameForAsset(assetName);

			Assert.That(filename,Is.EqualTo(assetName));
		}
	
	}
}
