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
using System.Collections.Generic;
using Balder.Core.Assets;
using Balder.Core.Collections;
using Balder.Core.Content;
using Balder.Core.Debug;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Imaging;
using Balder.Core.Input;
using Balder.Core.Objects.Flat;
using Balder.Core.Objects.Geometries;
using Balder.Core.Rendering;
using Ninject.Core;
using Ninject.Core.Activation;
using Ninject.Core.Behavior;
using Ninject.Core.Binding;
using Ninject.Core.Binding.Syntax;

namespace Balder.Core
{
	[Singleton]
	public class Runtime : IRuntime
	{
		private static AutoKernel _kernel;
		private static Runtime _instance;
		private static readonly object InstanceLockObject = new object();

		private readonly IPlatform _platform;
		private readonly IObjectFactory _objectFactory;
		private readonly IAssetLoaderService _assetLoaderService;
		private readonly Dictionary<IDisplay, ActorCollection> _gamesPerDisplay;

		private bool _hasPlatformInitialized;
		private bool _hasPlatformLoaded;
		private bool _hasPlatformRun;

		public Runtime(IPlatform platform, IObjectFactory objectFactory, IAssetLoaderService assetLoaderService)
		{
			_gamesPerDisplay = new Dictionary<IDisplay, ActorCollection>();
			_platform = platform;
			_objectFactory = objectFactory;
			_assetLoaderService = assetLoaderService;
			InitializePlatformEventHandlers();
			_assetLoaderService.RegisterAssembly(GetType().Assembly);
			platform.RegisterAssetLoaders(_assetLoaderService);
		}

		public static Runtime Instance
		{
			get
			{
				lock( InstanceLockObject )
				{
					if( null == _instance )
					{
						_instance = _kernel.Get<IRuntime>() as Runtime;
					}
					return _instance;
				}
			}
		}

		public static void Initialize(IPlatform platform)
		{
			var runtimeModule = GetRuntimeModule(platform);
			_kernel = new AutoKernel(runtimeModule);
			_kernel.AddBindingResolver<IDisplay>(DisplayBindingResolver);
		}

		public DebugLevel DebugLevel { get; set; }

		private static IBinding DisplayBindingResolver(IContext context)
		{
			var binding = new StandardBinding(_kernel, typeof(IDisplay));
			IBindingTargetSyntax binder = new StandardBindingBuilder(binding);

			if (null != context.ParentContext &&
				context.ParentContext is DisplayActivationContext)
			{
				var display = ((DisplayActivationContext)context.ParentContext).Display;
				binder.ToConstant(display);
			}

			return binding;
		}

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

		public void WireUpDependencies(object objectToWire)
		{
			_objectFactory.WireUpDependencies(objectToWire);
		}

		private static void WireUpGame(IDisplay display, Game objectToWire)
		{
			var displayActivationContext = new DisplayActivationContext(display, _kernel, objectToWire.GetType(),
			                                                            _kernel.CreateScope());
			_kernel.Inject(objectToWire,displayActivationContext);
		}

		private ActorCollection GetGameCollectionForDisplay(IDisplay display)
		{
			ActorCollection actorCollection = null;
			if( _gamesPerDisplay.ContainsKey(display) )
			{
				actorCollection = _gamesPerDisplay[display];
			} else
			{
				actorCollection = new ActorCollection();
				_gamesPerDisplay[display] = actorCollection;
			}
			return actorCollection;
		}


		private static InlineModule GetRuntimeModule(IPlatform platform)
		{
			var module = new InlineModule(
				m => m.Bind<IPlatform>().ToConstant(platform),
				m => m.Bind<IDisplayDevice>().ToConstant(platform.DisplayDevice),
				m => m.Bind<IMouseDevice>().ToConstant(platform.MouseDevice),
				m => m.Bind<IFileLoader>().To(platform.FileLoaderType).Using<SingletonBehavior>(),
				m => m.Bind<IGeometryContext>().To(platform.GeometryContextType),
				m => m.Bind<ISpriteContext>().To(platform.SpriteContextType),
				m => m.Bind<IImageContext>().To(platform.ImageContextType),
				m => m.Bind<IShapeContext>().To(platform.ShapeContextType)
			);
			return module;
		}


		private void InitializePlatformEventHandlers()
		{
			_platform.StateChanged += PlatformStateChanged;
			_platform.DisplayDevice.Render += PlatformRender;
			_platform.DisplayDevice.Update += PlatformUpdate;
		}

		private void HandleEventsForGames()
		{
			foreach( var games in _gamesPerDisplay.Values )
			{
				foreach (var game in games )
				{
					HandleEventsForActor(game);
				}
			}
		}

		private void HandleEventsForActor<T>(T actor) where T : Actor
		{
			if( !actor.HasInitialized && HasPlatformInitialized )
			{
				actor.ChangeState(ActorState.Initialize);
			}
			if( !actor.HasLoaded && HasPlatformLoaded )
			{
				actor.ChangeState(ActorState.Load);
				actor.ChangeState(ActorState.Run);
			}
			if( !actor.HasUpdated && 
				HasPlatformRun && 
				actor.State == ActorState.Run )
			{
				actor.OnUpdate();
			}
		}

		private bool IsPlatformInStateOrLater(PlatformState state, ref bool field)
		{
			if( field )
			{
				return true;
			}
			if( _platform.CurrentState >= state )
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
			if( _platform.CurrentState == PlatformState.Run )
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

		private void CallMethodOnGames(IDisplay display, Action<Game> action, Func<Game,bool> advice)
		{
			if( _gamesPerDisplay.ContainsKey(display))
			{
				var games = _gamesPerDisplay[display];
				foreach (Game game in games)
				{
					if( null != advice )
					{
						if( advice(game))
						{
							action(game);
						}
					} else
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
