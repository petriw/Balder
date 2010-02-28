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
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows;
using Balder.Core.Execution;
using Balder.Core.Execution.Composition;
using CThru.Silverlight;
using Moq;
using NUnit.Framework;

namespace Balder.Core.Tests.Execution
{
	[TestFixture]
	public class ComposerTests
	{
		public interface IContract
		{
		}

		[Export]
		public class ExportClass : IContract
		{
		}

		public class ImportClass
		{
			[Import]
			public ExportClass Imported { get; set; }
			
		}

		[SetUp]
		public void BeforeEachTest()
		{
			var assemblies = new Assembly[]
			                 	{
									GetType().Assembly
			                 	};
			var uri = new Uri(string.Empty, UriKind.Relative);

			var packageType = typeof (Package);
			var currentField = packageType.GetField("_current",BindingFlags.Static|BindingFlags.NonPublic);

			
			var package = new Package(uri,assemblies);
			currentField.SetValue(null,package);
		}

		[Test, SilverlightUnitTest]
		public void ImportSingleShouldBeSatisfied()
		{
			var objectFactory = new Mock<IObjectFactory>();
			var composer = new Composer(objectFactory.Object);

			var import = new ImportClass();
			composer.SatisfyImportsFor(import);

			Assert.That(import.Imported,Is.Not.Null);
		}

		[Test]
		public void ExportsShouldBeCreatedThroughObjectFactory()
		{
			var objectFactoryMock = new Mock<IObjectFactory>();
			objectFactoryMock.Expect(o => o.Get(It.IsAny<Type>()));
			var composer = new Composer(objectFactoryMock.Object);

			var import = new ImportClass();
			composer.SatisfyImportsFor(import);

			objectFactoryMock.VerifyAll();
		}
	}
}

