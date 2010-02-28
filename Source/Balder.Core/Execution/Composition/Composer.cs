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

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

namespace Balder.Core.Execution.Composition
{
	public class Composer : IComposer
	{
		private readonly IObjectFactory _objectFactory;
		private readonly CompositionContainer _compositionContainer;

		public Composer(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
			var packageCatalog = new PackageCatalog();
			
			packageCatalog.AddPackage(Package.Current);

			var provider = new Exporter(objectFactory);
			
			_compositionContainer = new Container(packageCatalog,objectFactory); //, provider);
			
		}

		public void SatisfyImportsFor(object component)
		{
			_compositionContainer.SatisfyImportsOnce(component);
		}
	}
}