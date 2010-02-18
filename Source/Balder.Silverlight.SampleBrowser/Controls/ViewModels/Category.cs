using System;
using System.Collections.ObjectModel;

namespace Balder.Silverlight.SampleBrowser.Controls.ViewModels
{
	public class Category
	{
		public Category()
		{
			Pages = new ObservableCollection<SamplePage>();
		}

		public string Name { get; set; }
		public ObservableCollection<SamplePage> Pages { get; set; }

		public void AddPageFromType(Type type)
		{
			var page = new SamplePage
			           	{
			           		Type = type,
			           		Name = GetSampleName(type),
							Description = GetSampleDescription(type)
			           	};
			Pages.Add(page);
		}

		private static string GetSampleName(Type type)
		{
			var ns = GetSampleNamespaceName(type);
			return ns;
		}

		private static string GetSampleNamespaceName(Type type)
		{
			var ns = type.Namespace;
			var lastDot = ns.LastIndexOf('.');
			var sampleNamespace = ns.Substring(lastDot + 1);
			return sampleNamespace;
		}

		private static string GetSampleDescription(Type type)
		{
			var ns = GetSampleNamespaceName(type);

			return string.Empty;
		}

	}
}