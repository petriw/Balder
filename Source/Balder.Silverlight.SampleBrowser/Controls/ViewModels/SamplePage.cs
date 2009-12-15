using System;

namespace Balder.Silverlight.SampleBrowser.Controls.ViewModels
{
	public class SamplePage
	{
		public Type Type { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public string NavigationUrl
		{
			get
			{
				var name = Type.Namespace.Replace("Balder.Silverlight.SampleBrowser.Samples.", string.Empty);
				name = name.Replace(".", "/");
				return name;
			}
		}
	}
}