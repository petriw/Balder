namespace Balder.Core.SoftwareRendering.Rendering
{
	public class RenderedSpan
	{
		public RenderedSpan Next;
		public RenderedSpan Previous;

		public int XStart;
		public int XEnd;
		public float ZStart;
		public float ZEnd;
		public int Length;

		public int XClip;

		public override string ToString()
		{
			string.Format("XStart: {0}, XEnd: {1}, XClip: {2}, Length: {3}, ZStart: {4}, ZEnd: {5}",
			              XStart, XEnd, XClip, Length, ZStart, ZEnd);
			return base.ToString();
		}

	}
}
