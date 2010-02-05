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
using Balder.Core.Assets;
using Balder.Core.Execution;
using Balder.Core.Objects.Geometries;
using Balder.Core.Tests.Fakes.AssetLoaders;
using Balder.Core.Tests.Fakes.AssetLoaders.Nested;
using Moq;
using NUnit.Framework;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class AssetLoaderServiceTests
	{
		private Mock<IObjectFactory>	GetObjectFactoryMock()
		{
			var asm = GetType().Assembly;
			var objectFactoryMock = new Mock<IObjectFactory>();
			objectFactoryMock.Expect(o => o.Get(It.IsAny<Type>())).Returns(
				(Type t) =>
				{
					if (t.Assembly.FullName.Equals(asm.FullName))
					{
						var objectToCreate = Activator.CreateInstance(t);
						return objectToCreate;
					}
					return null;
				}
			);
			return objectFactoryMock;
			
		}


		[Test]
		public void RegisterAssemblyShouldRegisterAllAssetLoaderTypesInAssembly()
		{
			var asm = GetType().Assembly;
			var rootAssetLoaderType = typeof(RootAssetLoader);

			var objectFactoryMock = GetObjectFactoryMock();

			var service = new AssetLoaderService(objectFactoryMock.Object);
			service.RegisterAssembly(asm);
			var loaders = service.AvailableLoaders;

			Assert.That(loaders.Length,Is.EqualTo(2));

			var foundLoader = false;
			foreach (var loader in loaders)
			{
				if (loader.GetType().Equals(rootAssetLoaderType))
				{
					foundLoader = true;
					break;
				}
			}

			Assert.That(foundLoader, Is.True);
		}

		[Test]
		public void RegisterAssemblyByNameShouldRegisterAllAssetLoaderTypesInAssembly()
		{
			var asm = GetType().Assembly;
			var rootAssetLoaderType = typeof(RootAssetLoader);

			var objectFactoryMock = GetObjectFactoryMock();

			var service = new AssetLoaderService(objectFactoryMock.Object);
			service.RegisterAssembly(asm.FullName);
			var loaders = service.AvailableLoaders;

			Assert.That(loaders.Length, Is.EqualTo(2));

			var foundLoader = false;
			foreach (var loader in loaders)
			{
				if (loader.GetType().Equals(rootAssetLoaderType))
				{
					foundLoader = true;
					break;
				}
			}

			Assert.That(foundLoader, Is.True);
		}
		

		[Test]
		public void RegisteringNamespaceNonRecursivelyShouldRegisterAllAssetLoaderTypesInNamespace()
		{
			var asm = GetType().Assembly;
			var rootAssetLoaderType = typeof(RootAssetLoader);

			var objectFactoryMock = GetObjectFactoryMock();

			var service = new AssetLoaderService(objectFactoryMock.Object);

			service.RegisterNamespace(asm, rootAssetLoaderType.Namespace);
			var loaders = service.AvailableLoaders;

			Assert.That(loaders.Length, Is.EqualTo(1));
			Assert.That(loaders[0], Is.InstanceOf(rootAssetLoaderType));
		}

		

		[Test]
		public void RegisteringNamespaceRecursivelyShouldRegisterAllAssetLoaderTypesRecursively()
		{
			var asm = GetType().Assembly;
			var rootAssetLoaderType = typeof(RootAssetLoader);

			var objectFactoryMock = GetObjectFactoryMock();
			var service = new AssetLoaderService(objectFactoryMock.Object);

			service.RegisterNamespace(asm, rootAssetLoaderType.Namespace, true);
			var loaders = service.AvailableLoaders;

			Assert.That(loaders.Length, Is.EqualTo(2));
			Assert.That(loaders[0], Is.InstanceOf(rootAssetLoaderType));
			Assert.That(loaders[1], Is.InstanceOf(typeof(NestedAssetLoader)));
		}

		
		[Test]
		public void GettingSpecificRegisteredLoaderShouldReturnValidLoader()
		{
			var rootAssetLoader = new RootAssetLoader();

			var objectFactoryMock = GetObjectFactoryMock();
			var service = new AssetLoaderService(objectFactoryMock.Object);
			service.RegisterLoader(rootAssetLoader);

			var assetName = string.Format("test.{0}", rootAssetLoader.FileExtensions[0]);
			var loader = service.GetLoader<Geometry>(assetName);
			Assert.That(loader, Is.Not.Null);
			Assert.That(loader, Is.SameAs(rootAssetLoader));
		}

		[Test]
		public void InitializeShouldAddLoadersInCore()
		{
			var loaderCount = 0;
			var objectFactoryMock = new Mock<IObjectFactory>();
			objectFactoryMock.Expect(o => o.Get(It.IsAny<Type>())).Returns(
				(Type t) =>
					{
						if( t.Namespace.StartsWith("Balder.Core") )
						{
							loaderCount++;
						}
						return new RootAssetLoader();
					});
			var service = new AssetLoaderService(objectFactoryMock.Object);
			service.Initialize();
			Assert.That(loaderCount,Is.GreaterThan(0));			
		}



		[Test,ExpectedException(typeof(ArgumentException))]
		public void GettingLoaderForAnUnsupportedExtensionShouldCauseException()
		{
			var objectFactoryMock = GetObjectFactoryMock();
			var service = new AssetLoaderService(objectFactoryMock.Object);
			service.GetLoader<Geometry>("something.something");
		}
	}
}