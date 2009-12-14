using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Balder.Silverlight.SampleBrowser.Content;

namespace Balder.Silverlight.SampleBrowser.Controls.ViewModels
{
	public class SampleNavigationViewModel
	{
		public SampleNavigationViewModel()
		{
			Categories = new ObservableCollection<Category>();
			Populate();
		}

		private void Populate()
		{
			Func<Type, bool> isSamplePage = t =>
			{
				var samplePageAttributes = t.GetCustomAttributes(typeof(SamplePageAttribute), true);
				return samplePageAttributes.Length == 1;
			};
			var types = GetType().Assembly.GetTypes();
			var query = from t in types
						where isSamplePage(t)
						select t;
			foreach (var type in query)
			{
				AddType(type);
			}
		}

		private void AddType(Type type)
		{
			var categoryAttributes = type.GetCustomAttributes(typeof(CategoryAttribute), true);
			if (null != categoryAttributes && categoryAttributes.Length == 1)
			{
				var categoryAttribute = categoryAttributes[0] as CategoryAttribute;
				if (null != categoryAttribute)
				{
					Category currentCategory = null;

					foreach (var category in Categories)
					{
						if (category.Name.Equals(categoryAttribute.Category))
						{
							currentCategory = category;
							break;
						}
					}

					if( null == currentCategory )
					{
						currentCategory = new Category {Name = categoryAttribute.Category};
						Categories.Add(currentCategory);
					}

					currentCategory.AddPageFromType(type);
				}
			}
		}

		public ObservableCollection<Category> Categories { get; set; }

	}
}
