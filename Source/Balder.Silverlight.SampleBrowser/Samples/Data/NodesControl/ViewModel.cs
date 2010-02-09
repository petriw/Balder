using System.Collections.ObjectModel;
using Balder.Core.Math;
using Color = Balder.Core.Color;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.NodesControl
{
	public class ViewModel
	{
		public ViewModel()
		{
			/*
			Objects = new ObservableCollection<BusinessObject>
			          	{
			          		new BusinessObject {Color = Color.FromSystemColor(Colors.Red), Position = new Coordinate(-120, 0, 0)},
			          		new BusinessObject {Color = Color.FromSystemColor(Colors.Green), Position = new Coordinate(0, 0, 0)},
			          		new BusinessObject {Color = Color.FromSystemColor(Colors.Blue), Position = new Coordinate(120, 0, 0)}
			          	};*/

			Objects = new ObservableCollection<BusinessObject>();

			var zCount = 25;
			var xCount = 25;

			var zDistance = 12d;
			var xDistance = 12d;

			var currentZ = -((zCount / 2d) * zDistance);
			for (var z = 0; z < zCount; z++)
			{
				var currentX = -((xCount / 2d) * xDistance);
				for (var x = 0; x < xCount; x++)
				{
					var obj = new BusinessObject
								{
									Position = new Coordinate(currentX, 0, currentZ),
									Color = Color.Random()
								};
					Objects.Add(obj);
					currentX += xDistance;
				}

				currentZ += zDistance;
			}
		}

		public ObservableCollection<BusinessObject> Objects { get; set; }
	}
}