using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Balder.Core.Execution;

namespace Balder.Core
{
	public class RuntimeImports
	{
		public RuntimeImports()
		{
			var packageCatalog = new PackageCatalog();
			packageCatalog.AddPackage(Package.Current);
			
			var container = new CompositionContainer(packageCatalog);
			container.ComposeParts(this);
		}

		[Import]
		public IPlatform Platform { get; set; }
	}
}