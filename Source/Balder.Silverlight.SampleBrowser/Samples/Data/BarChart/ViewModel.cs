using System.Collections.ObjectModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.BarChart
{
	public class ViewModel
	{
		public ViewModel()
		{
			Objects = new ObservableCollection<BusinessObject>();

			Objects.Add(new BusinessObject { Value = 5 });
			Objects.Add(new BusinessObject { Value = 15 });
			Objects.Add(new BusinessObject { Value = 7 });
		}

		public ObservableCollection<BusinessObject> Objects { get; set; }
	}
}
