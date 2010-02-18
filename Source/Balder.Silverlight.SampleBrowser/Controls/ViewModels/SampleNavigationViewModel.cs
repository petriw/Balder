using System;
using System.Collections.ObjectModel;
using System.Linq;

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
			var types = GetType().Assembly.GetTypes();
			var query = from t in types
						where t.Name.Equals("Content")
						select t;
			foreach (var type in query)
			{
				AddType(type);
			}
		}

		private void AddType(Type type)
		{
			var category = GetOrAddCategoryFromType(type);
			category.AddPageFromType(type);
		}


		private Category GetOrAddCategoryFromType(Type type)
		{
			var categoryName = GetCategoryName(type);
			foreach( var existingCategory in Categories )
			{
				if( existingCategory.Name.Equals(categoryName))
				{
					return existingCategory;
				}
			}

			var category = new Category
			               	{
			               		Name = categoryName
			               	};
			Categories.Add(category);
			return category;
		}


		private static string GetCategoryName(Type type)
		{
			var ns = type.Namespace;
			var lastDot = ns.LastIndexOf('.');
			var nsWithoutSampleName = ns.Substring(0, lastDot);
			lastDot = nsWithoutSampleName.LastIndexOf('.');
			var category = nsWithoutSampleName.Substring(lastDot + 1);

			return category;		
		}

		public ObservableCollection<Category> Categories { get; set; }
	}
}
