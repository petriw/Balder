using System;
using System.Collections.ObjectModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.BarChart
{
	public class ViewModel
	{
		private static readonly Random Rnd = new Random();
		public ViewModel()
		{
			Objects = new ObservableCollection<BusinessObject>();

			Objects.Add(new BusinessObject { Value = Rnd.Next(0, 100) });
			Objects.Add(new BusinessObject { Value = Rnd.Next(0, 100) });
			Objects.Add(new BusinessObject { Value = Rnd.Next(0, 100) });
		}

		public ObservableCollection<BusinessObject> Objects { get; set; }
	}
}
