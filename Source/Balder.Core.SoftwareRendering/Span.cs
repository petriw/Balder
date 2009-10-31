using Balder.Core.Math;

namespace Balder.Core.SoftwareRendering
{
	public class Span
	{
		public int Y;
		public int XStart;
		public int XEnd;
		public int Length;

		public float ZStart;
		public float ZEnd;

		public float UStart;
		public float UEnd;

		public float VStart;
		public float VEnd;

		public ColorVector ColorStart;
		public ColorVector ColorEnd;

		public bool Swap;

		public ISpanDrawer Drawer;
	}
}
