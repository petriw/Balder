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
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Input;

namespace Balder.Core.Tests.Fakes
{
	public class FakePlatform : IPlatform
	{
		public event PlatformStateChange BeforeStateChange = (p, s) => { };
		public event PlatformStateChange StateChanged = (p, s) => { };


		public FakePlatform()
		{
			DisplayDevice = new FakeDisplayDevice();
		}

		public string PlatformName
		{
			get { return "Fake"; }
		}

		public string EntryAssemblyName
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsInDesignMode
		{
			get { return false; }
		}

		public IDisplayDevice DisplayDevice { get; set; }
		public IMouseDevice MouseDevice { get; set; }
		public Type FileLoaderType
		{
			get { throw new NotImplementedException(); }
		}

		public Type GeometryContextType
		{
			get { throw new NotImplementedException(); }
		}

		public Type SpriteContextType
		{
			get { throw new NotImplementedException(); }
		}

		public Type ImageContextType
		{
			get { throw new NotImplementedException(); }
		}

		public Type ShapeContextType
		{
			get { throw new NotImplementedException(); }
		}

		public PlatformState CurrentState { get; set; }
		public void RegisterAssetLoaders(IAssetLoaderService assetLoaderService)
		{
			
		}

		public void ChangeState(PlatformState state)
		{
			BeforeStateChange(this, state);
			CurrentState = state;
			StateChanged(this, state);
		}
	}
}