using System;

namespace Balder.Silverlight.SampleBrowser.Content
{
	[AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
	public class SamplePageAttribute : Attribute
	{
		public SamplePageAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

	}
}
