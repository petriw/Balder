using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Balder.Silverlight.SampleBrowser.Content;

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
			SamplePageAttribute samplePageAttribute = null;
			DescriptionAttribute descriptionAttribute = null;
			var customAttributes = type.GetCustomAttributes(true);
			foreach( var attribute in customAttributes )
			{
				if( attribute is SamplePageAttribute )
				{
					samplePageAttribute = attribute as SamplePageAttribute;
				}
				if( attribute is DescriptionAttribute )
				{
					descriptionAttribute = attribute as DescriptionAttribute;
				}
			}
			
			if( null != samplePageAttribute )
			{
				var page = new SamplePage
				           	{
				           		Type = type,
				           		Name = samplePageAttribute.Name
				           	};
				if( null != descriptionAttribute )
				{
					page.Description = descriptionAttribute.Description;
				}
				Pages.Add(page);
			}
		}
	}
}