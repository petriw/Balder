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
using System.ComponentModel;
using System.Windows;
using Balder.Core;
using Balder.Core.Assets;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Input;
using Balder.Core.SoftwareRendering;
using Balder.Core.SoftwareRendering.Rendering;
using Balder.Silverlight.Content;
using Balder.Silverlight.Display;
using Balder.Silverlight.Input;

namespace Balder.Silverlight.Execution
{
	public class Platform : IPlatform
	{
		public static IRuntime Runtime;

		public event PlatformStateChange BeforeStateChange = (p, s) => { };
		public event PlatformStateChange StateChanged = (p, s) => { };

		static Platform()
		{
			var platform = new Platform();
			Core.Runtime.Initialize(platform);
			Runtime = Core.Runtime.Instance;
		}

		public Platform()
		{
			CurrentState = PlatformState.Idle;
			InitializeObjects();
			Initialize();
		}

		private void InitializeObjects()
		{
			DisplayDevice = new DisplayDevice(this);
			MouseDevice = new MouseDevice();
		}

		private void Initialize()
		{
			ChangeState(PlatformState.Initialize);
			ChangeState(PlatformState.Load);
			ChangeState(PlatformState.Run);
		}


		public string PlatformName
		{
			get { return "Silverlight"; }
		}

		public string EntryAssemblyName
		{
			get
			{
				var fullAssemblyName = Application.Current.GetType().Assembly.FullName;
				return fullAssemblyName;
			}
		}

		public bool IsInDesignMode
		{
			get 
			{
				var isInDesignMode = DesignerProperties.IsInDesignTool;
				if( !isInDesignMode )
				{
					try
					{
						var host = Application.Current.Host.Source;
						isInDesignMode = false;
					} catch
					{
						isInDesignMode = true;
					}
				}
				return isInDesignMode;
			}
		}

		public IDisplayDevice DisplayDevice { get; private set; }
		public IMouseDevice MouseDevice { get; private set; }
		public Type FileLoaderType { get { return typeof(FileLoader); } }
		public Type GeometryContextType { get { return typeof(GeometryContext); } }
		public Type SpriteContextType { get { return typeof(SpriteContext); } }
		public Type ImageContextType { get { return typeof(ImageContext); } }
		public Type ShapeContextType { get { return typeof(ShapeContext); } }

		public PlatformState CurrentState { get; private set; }

		private void ChangeState(PlatformState platformState)
		{
			BeforeStateChange(this, platformState);
			CurrentState = platformState;
			StateChanged(this, platformState);
		}

		public void RegisterAssetLoaders(IAssetLoaderService assetLoaderService)
		{
			var type = GetType();
			var assembly = type.Assembly;

			// Todo: Look into the literal below - my enemy number one: Literals
			assetLoaderService.RegisterNamespace(assembly, "Balder.Silverlight.AssetLoaders");
		}
	}
}
