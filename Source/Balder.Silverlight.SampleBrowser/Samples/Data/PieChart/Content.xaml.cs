using System;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
{
	public partial class Content
	{
		private static readonly Random Rnd = new Random();

		public Content()
		{
			InitializeComponent();
		}

		private ViewModel ViewModel
		{
			get
			{
				return (ViewModel)DataContext;
			}
		}

		private void AddValueClick(object sender, System.Windows.RoutedEventArgs e)
		{
			var value = new BusinessObject { Value = Rnd.Next(0, 100) };
			ViewModel.Objects.Add(value);
		}
	}
}
