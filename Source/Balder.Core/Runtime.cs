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
using System.Collections.Generic;
using Balder.Core.Assets;
using Balder.Core.Collections;
using Balder.Core.Content;
using Balder.Core.Debug;
using Balder.Core.Display;
using Balder.Core.Execution;
using Ninject.Core;

namespace Balder.Core
{
	[Singleton]
	public class Runtime : IRuntime
	{
		private static Runtime _instance;
		private static readonly object InstanceLockObject = new object();

		private readonly IObjectFactory _objectFactory;
		private readonly IAssetLoaderService _assetLoaderService;
		
		private readonly Dictionary<IDisplay, ActorCollection> _gamesPerDisplay;
		private readonly IPlatform _platform;

		private bool _hasPlatformInitialized;
		private bool _hasPlatformLoaded;
		private bool _hasPlatformRun;

		public Runtime(IPlatform platform, IObjectFactory objectFactory, IAssetLoaderService assetLoaderService, IContentManager contentManager)
		{
			_platform = platform;
			_gamesPerDisplay = new Dictionary<IDisplay, ActorCollection>();
			_objectFactory = objectFactory;
			_assetLoaderService = assetLoaderService;
			ContentManager = contentManager;
			InitializePlatformEventHandlers();
			_assetLoaderService.RegisterAssembly(GetType().Assembly);
			platform.RegisterAssetLoaders(_assetLoaderService);
		}

		public static Runtime Instance
		{
			get
			{
				lock (InstanceLockObject)
				{
					if (null == _instance)
					{
						var runtimeImports = new RuntimeImports();
						KernelContainer.Initialize(runtimeImports.Platform);
						_instance = KernelContainer.Kernel.Get<IRuntime>() as Runtime;
					}
					return _instance;
				}
			}
		}

		public IPlatform Platform { get { return _platform; } }
		public IContentManager ContentManager { get; private set; }

		public DebugLevel DebugLevel { get; set; }


		public T CreateGame<T>() where T : Game
		{
			var game = _objectFactory.Get<T>();
			return game;
		}

		public Game CreateGame(Type type)
		{
			var game = _objectFactory.Get(type) as Game;
			return game;
		}


		public void RegisterGame(IDisplay display, Game game)
		{
			WireUpGame(display, game);
			var actorCollection = GetGameCollectionForDisplay(display);
			actorCollection.Add(game);
			HandleEventsForActor(game);
		}

		public void UnregisterGame(Game game)
		{
			var displaysToRemove = new List<IDisplay>();
			foreach( var display in _gamesPerDisplay.Keys )
			{
				var gameFound = false;
				var actorCollection = _gamesPerDisplay[display];
				foreach( var actor in actorCollection )
				{
					if( actor is Game && actor.Equals(game) )
					{
						gameFound = true;
						break;
					}
				}
				if( gameFound )
				{
					actorCollection.Remove(game);
					if( actorCollection.Count == 0 )
					{
						displaysToRemove.Add(display);
					}
				}
			}

			foreach( var display in displaysToRemove )
			{
				_gamesPerDisplay.Remove(display);
				Platform.DisplayDevice.RemoveDisplay(display);
			}
		}


		public void WireUpDependencies(object objectToWire)
		{
			_objectFactory.WireUpDependencies(objectToWire);
		}

		private void WireUpGame(IDisplay display, Game objectToWire)
		{
			if (null != KernelContainer.Kernel)
			{
				var scope = KernelContainer.Kernel.CreateScope();
				var displayActivationContext = new DisplayActivationContext(display, objectToWire.GetType(), scope);
				KernelContainer.Kernel.Inject(objectToWire, displayActivationContext);
			}
			else
			{
				_objectFactory.WireUpDependencies(objectToWire);
			}
		}

		private ActorCollection GetGameCollectionForDisplay(IDisplay display)
		{
			ActorCollection actorCollection = null;
			if (_gamesPerDisplay.ContainsKey(display))
			{
				actorCollection = _gamesPerDisplay[display];
			}
			else
			{
				actorCollection = new ActorCollection();
				_gamesPerDisplay[display] = actorCollection;
			}
			return actorCollection;
		}




		private void InitializePlatformEventHandlers()
		{
			_platform.StateChanged += PlatformStateChanged;
			_platform.DisplayDevice.Render += PlatformRender;
			_platform.DisplayDevice.Update += PlatformUpdate;
		}

		private void HandleEventsForGames()
		{
			foreach (var games in _gamesPerDisplay.Values)
			{
				foreach (var game in games)
				{
					HandleEventsForActor(game);
				}
			}
		}

		private void HandleEventsForActor<T>(T actor) where T : Actor
		{
			if (!actor.HasInitialized && HasPlatformInitialized)
			{
				actor.ChangeState(ActorState.Initialize);
			}
			if (!actor.HasLoaded && HasPlatformLoaded)
			{
				actor.ChangeState(ActorState.Load);
				actor.ChangeState(ActorState.Run);
			}
			if (!actor.HasUpdated &&
				HasPlatformRun &&
				actor.State == ActorState.Run)
			{
				actor.OnUpdate();
			}
		}

		private bool IsPlatformInStateOrLater(PlatformState state, ref bool field)
		{
			if (field)
			{
				return true;
			}
			if (_platform.CurrentState >= state)
			{
				return true;
			}
			return false;
		}

		private bool HasPlatformLoaded { get { return IsPlatformInStateOrLater(PlatformState.Load, ref _hasPlatformLoaded); } }
		private bool HasPlatformInitialized { get { return IsPlatformInStateOrLater(PlatformState.Initialize, ref _hasPlatformInitialized); } }
		private bool HasPlatformRun { get { return IsPlatformInStateOrLater(PlatformState.Run, ref _hasPlatformRun); } }


		private void PlatformUpdate(IDisplay display)
		{
			if (_platform.CurrentState == PlatformState.Run)
			{
				CallMethodOnGames(display, g => g.OnUpdate(), g => g.State == ActorState.Run);
			}
		}

		private void PlatformRender(IDisplay display)
		{
			if (_platform.CurrentState == PlatformState.Run)
			{
				CallMethodOnGames(display, g => g.OnRender(), g => g.State == ActorState.Run);
			}
		}

		private void CallMethodOnGames(IDisplay display, Action<Game> action, Func<Game, bool> advice)
		{
			if (_gamesPerDisplay.ContainsKey(display))
			{
				var games = _gamesPerDisplay[display];
				foreach (Game game in games)
				{
					if (null != advice)
					{
						if (advice(game))
						{
							action(game);
						}
					}
					else
					{
						action(game);
					}
				}
			}
		}

		private void PlatformStateChanged(IPlatform platform, PlatformState state)
		{
			switch (state)
			{
				case PlatformState.Initialize:
					{
						_hasPlatformInitialized = true;
					}
					break;
				case PlatformState.Load:
					{
						_hasPlatformLoaded = true;
					}
					break;
				case PlatformState.Run:
					{
						_hasPlatformRun = true;
					}
					break;
			}
			HandleEventsForGames();
		}

	}
}
