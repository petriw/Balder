using System.Collections.ObjectModel;
using System.Windows.Media;
using Balder.Core.Math;
using Color=Balder.Core.Color;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.NodesControl
{
	public class ViewModel
	{
		public ViewModel()
		{
			Objects = new ObservableCollection<BusinessObject>
			          	{
			          		new BusinessObject {Color = Color.FromSystemColor(Colors.Red), Position = new Coordinate(-120, 0, 0)},
			          		new BusinessObject {Color = Color.FromSystemColor(Colors.Green), Position = new Coordinate(0, 0, 0)},
			          		new BusinessObject {Color = Color.FromSystemColor(Colors.Blue), Position = new Coordinate(120, 0, 0)}
			          	};
		}

		public ObservableCollection<BusinessObject> Objects { get; set; }
	}
}