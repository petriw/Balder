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
using System.Windows;
using System.Windows.Controls;
using Balder.Core;
using Balder.Core.Content;
using Balder.Core.Execution;
using Ninject.Core;

namespace Balder.Silverlight.Controls
{
	public class BalderControl : Grid
	{
		public BalderControl()
		{
			Loaded += ControlLoaded;
			
		}


		private void ControlLoaded(object sender, RoutedEventArgs e)
		{
			if( null == Runtime )
			{

				Runtime = Core.Runtime.Instance;// Execution.Platform.Runtime;
				Core.Runtime.Instance.WireUpDependencies(this);
				OnLoaded();
				
				Initialize();
				InitializeProperties();
			}
		}

		protected virtual void InitializeProperties() {}
		protected virtual void OnLoaded() {}
		protected virtual void Initialize() {}


		public IRuntime Runtime { get; set; }

		[Inject]
		public IPlatform Platform { get; set; }

		[Inject]
		public IContentManager ContentManager { get; set; }
	}
}
