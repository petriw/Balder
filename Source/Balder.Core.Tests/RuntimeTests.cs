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
using System.Linq.Expressions;
using Balder.Core.Assets;
using Balder.Core.Display;
using Balder.Core.Tests.Fakes;
using CThru.Silverlight;
using Moq;
using NUnit.Framework;
using Balder.Core.Execution;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class RuntimeTests
	{
		private static void EventShouldBeCalledForStateDuringRegistration(Expression<Action<Game>> eventExpression, PlatformState state, bool changeStateFirst)
		{
			var eventCalled = false;
			var stateChanged = false;
			var platform = new FakePlatform();
			var objectFactoryMock = new Mock<IObjectFactory>();

			platform.StateChanged +=
				(p, s) =>
				{
					if (s == state)
					{
						stateChanged = true;
					}
				};


			var gameMock = new Mock<Game>();
			gameMock.Expect(g => g.OnInitialize());
			gameMock.Expect(g => g.OnBeforeInitialize());
			gameMock.Expect(g => g.OnLoadContent());
			gameMock.Expect(g => g.OnBeforeUpdate());
			gameMock.Expect(g => g.OnUpdate());
			gameMock.Expect(g => g.OnAfterUpdate());
			gameMock.Expect(g => g.OnStopped());
			gameMock.Expect(eventExpression).Callback(

				() =>
				{
					Assert.That(stateChanged, Is.True);
					eventCalled = true;
				});

			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var runtime = new Core.Runtime(platform, objectFactoryMock.Object, assetLoaderServiceMock.Object, null);

			if (changeStateFirst)
			{
				platform.ChangeState(state);
			}

			var displayMock = new Mock<IDisplay>();
			runtime.RegisterGame(displayMock.Object, gameMock.Object);

			if (!changeStateFirst)
			{
				platform.ChangeState(state);
			}

			Assert.That(eventCalled, Is.True);
		}

		[Test, SilverlightUnitTest]
		public void RegisteredGameShouldHaveItsInitializeCalledAfterInitializeStateChangeOccursOnPlatform()
		{
			EventShouldBeCalledForStateDuringRegistration(g => g.OnInitialize(), PlatformState.Initialize, false);
		}

		[Test, SilverlightUnitTest]
		public void GameRegisteredAfterInitializeStateChangeOccuredOnPlatformShouldHaveItsInitializeEventCalledDirectly()
		{
			EventShouldBeCalledForStateDuringRegistration(g => g.OnInitialize(), PlatformState.Initialize, true);
		}

		[Test, SilverlightUnitTest]
		public void RegisteredGameShouldHaveItsLoadCalledAfterLoadStateChangeOccursOnPlatform()
		{
			EventShouldBeCalledForStateDuringRegistration(g => g.OnLoadContent(), PlatformState.Load, false);
		}

		[Test, SilverlightUnitTest]
		public void GameRegisteredAfterLoadStateChangeOccuredOnPlatformShouldHaveItsLoadEventCalledDirectly()
		{
			EventShouldBeCalledForStateDuringRegistration(g => g.OnLoadContent(), PlatformState.Load, true);
		}

		[Test, SilverlightUnitTest]
		public void OnRenderForGamesShouldNotBeCalledBeforePlatformIsInRunState()
		{
			var platform = new FakePlatform();
			var objectFactoryMock = new Mock<IObjectFactory>();
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var runtime = new Core.Runtime(platform, objectFactoryMock.Object, assetLoaderServiceMock.Object, null);
			var displayMock = new Mock<IDisplay>();
			var gameMock = new Mock<Game>();
			var onRenderCalled = false;

			gameMock.Expect(g => g.OnRender()).Callback(() => onRenderCalled = true);
			runtime.RegisterGame(displayMock.Object, gameMock.Object);
			((FakeDisplayDevice)platform.DisplayDevice).FireRenderEvent(displayMock.Object);
			Assert.That(onRenderCalled, Is.False);
		}

		[Test, SilverlightUnitTest]
		public void OnRenderForGamesShouldBeCalledWhenPlatformAndGameIsInRunState()
		{
			var platform = new FakePlatform();
			var objectFactoryMock = new Mock<IObjectFactory>();
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var runtime = new Core.Runtime(platform, objectFactoryMock.Object, assetLoaderServiceMock.Object, null);
			var displayMock = new Mock<IDisplay>();
			var gameMock = new Mock<Game>();
			var onRenderCalled = false;

			gameMock.Expect(g => g.OnRender()).Callback(() => onRenderCalled = true);
			runtime.RegisterGame(displayMock.Object, gameMock.Object);
			platform.ChangeState(PlatformState.Run);
			gameMock.Object.ChangeState(ActorState.Run);
			((FakeDisplayDevice)platform.DisplayDevice).FireRenderEvent(displayMock.Object);
			Assert.That(onRenderCalled, Is.True);
		}

		[Test, SilverlightUnitTest]
		public void OnUpdateForGamesShouldNotBeCalledBeforePlatformIsInRunState()
		{
			var platform = new FakePlatform();
			var objectFactoryMock = new Mock<IObjectFactory>();
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var runtime = new Core.Runtime(platform, objectFactoryMock.Object, assetLoaderServiceMock.Object, null);
			var displayMock = new Mock<IDisplay>();
			var gameMock = new Mock<Game>();
			var onUpdateCalled = false;

			gameMock.Expect(g => g.OnUpdate()).Callback(() => onUpdateCalled = true);
			runtime.RegisterGame(displayMock.Object, gameMock.Object);
			((FakeDisplayDevice)platform.DisplayDevice).FireUpdateEvent(displayMock.Object);
			Assert.That(onUpdateCalled, Is.False);
		}

		[Test, SilverlightUnitTest]
		public void OnUpdateForGamesShouldBeCalledWhenPlatformAndGameIsInRunState()
		{
			var platform = new FakePlatform();
			var objectFactoryMock = new Mock<IObjectFactory>();
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var runtime = new Core.Runtime(platform, objectFactoryMock.Object, assetLoaderServiceMock.Object, null);
			var displayMock = new Mock<IDisplay>();
			var gameMock = new Mock<Game>();
			var onUpdateCalled = false;

			gameMock.Expect(g => g.OnUpdate()).Callback(() => onUpdateCalled = true);
			runtime.RegisterGame(displayMock.Object, gameMock.Object);
			platform.ChangeState(PlatformState.Run);
			gameMock.Object.ChangeState(ActorState.Run);
			((FakeDisplayDevice)platform.DisplayDevice).FireUpdateEvent(displayMock.Object);
			Assert.That(onUpdateCalled, Is.True);
		}

		[Test,SilverlightUnitTest]
		public void OnUpdateForGamesShouldNotBeCalledBeforeGameIsInRunState()
		{
			var platform = new FakePlatform();
			var objectFactoryMock = new Mock<IObjectFactory>();
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var runtime = new Core.Runtime(platform, objectFactoryMock.Object, assetLoaderServiceMock.Object, null);
			var displayMock = new Mock<IDisplay>();
			var gameMock = new Mock<Game>();
			var onUpdateCalled = false;

			gameMock.Expect(g => g.OnUpdate()).Callback(() => onUpdateCalled = true);

			runtime.RegisterGame(displayMock.Object, gameMock.Object);
			platform.ChangeState(PlatformState.Run);

			((FakeDisplayDevice)platform.DisplayDevice).FireUpdateEvent(displayMock.Object);
			if (onUpdateCalled)
			{
				Assert.That(gameMock.Object.State, Is.EqualTo(ActorState.Run));
			}
		}

		[Test,SilverlightUnitTest]
		public void OnRenderForGamesShouldNotBeCalledBeforeGameIsInRunState()
		{
			var platform = new FakePlatform();
			var objectFactoryMock = new Mock<IObjectFactory>();
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var runtime = new Core.Runtime(platform, objectFactoryMock.Object, assetLoaderServiceMock.Object, null);
			var displayMock = new Mock<IDisplay>();
			var gameMock = new Mock<Game>();
			var onRenderCalled = false;

			gameMock.Expect(g => g.OnRender()).Callback(() => onRenderCalled = true);

			runtime.RegisterGame(displayMock.Object, gameMock.Object);
			platform.ChangeState(PlatformState.Run);

			((FakeDisplayDevice)platform.DisplayDevice).FireUpdateEvent(displayMock.Object);
			if (onRenderCalled)
			{
				Assert.That(gameMock.Object.State, Is.EqualTo(ActorState.Run));
			}
		}


		/*
		[Test,SilverlightUnitTest]
		public void ActorsWithinGameShouldHaveItsInitializeCalledAfterGamesInitializeIsCalled()
		{
			Assert.Fail();
		}

		[Test,SilverlightUnitTest]
		public void ActorsRegisteredInGameAfterGameHasStartedRunningShouldHaveItsInitializeCalled()
		{
			Assert.Fail();
		}

		[Test,SilverlightUnitTest]
		public void ActorsRegisteredInGameAfterGameHasStartedRunningShouldHaveItsLoadCalled()
		{
			Assert.Fail();
		}
		 * */
	}
}