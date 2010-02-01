using System;

namespace Balder.Core.Objects.Geometries
{
	public class PlaneHeightEventArgs : EventArgs
	{
		public Vertex ActualVertex { get; set; }
		public int GridX { get; set; }
		public int GridY { get; set; }
		public float Height { get; set; }
	}
}