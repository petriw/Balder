using System;
using System.ComponentModel;
using Balder.Silverlight.SampleBrowser.Constants;
using Balder.Silverlight.SampleBrowser.Content;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.BarChart
{
	[Category(Categories.Data)]
	[SamplePage("Bar Chart")]
	[Description("Sample showing how one could build a bar chart")]
	public partial class Content
	{
		private static readonly Random Rnd = new Random();

		public Content()
		{
			InitializeComponent();
		}

		private ViewModel	ViewModel
		{
			get
			{
				return (ViewModel) DataContext;
			}
		}

		private void AddValueClick(object sender, System.Windows.RoutedEventArgs e)
		{
			var value = new BusinessObject {Value = Rnd.Next(0, 100)};
			ViewModel.Objects.Add(value);
		}
	}
}
