using Balder.Core;
using Balder.Core.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.NodesControl
{
	public class BusinessObject
	{
		private Coordinate _position;
		public Coordinate Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public Color Color { get; set; }

		public bool IsVisible { get; set; }
	}
}
