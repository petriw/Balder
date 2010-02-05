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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Windows;

namespace Balder.Core.Execution
{
	public class Container : CompositionContainer
	{
		private readonly IObjectFactory _objectFactory;

		public Container(PackageCatalog catalog, IObjectFactory objectFactory, params ExportProvider[] providers)
			: base(catalog, providers)
		{
			_objectFactory = objectFactory;
		}

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			//Export e = new Export(definition.ContractName,()=>_objectFactory.Get(definition.));
			
			
			
			return base.GetExportsCore(definition, atomicComposition);
		}
	}


	public class Exporter : ExportProvider
	{
		private readonly IObjectFactory _objectFactory;

		public Exporter(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
		}

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			throw new NotImplementedException();
		}
	}

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
			_compositionContainer = new Container(packageCatalog,objectFactory, provider);
		}

		public void SatisfyImportsFor(object component)
		{
			_compositionContainer.SatisfyImportsOnce(component);
			//_compositionContainer.ComposeParts(component);
		}
	}
}