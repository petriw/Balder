using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Balder.Core.Execution.Composition
{
	public class Container : CompositionContainer
	{
		private readonly IObjectFactory _objectFactory;

		public Container(ComposablePartCatalog catalog, IObjectFactory objectFactory, params ExportProvider[] providers)
			: base(catalog, providers)
		{
			_objectFactory = objectFactory;
		}

		
		protected override System.Collections.Generic.IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			var type = Type.GetType(definition.ContractName);
			Export e;
			var exports = base.GetExportsCore(definition, atomicComposition);

			



			return exports;
		}
	}
}