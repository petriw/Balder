namespace Balder.Core.SoftwareRendering.Rendering
{
	public class Scanline
	{
		public RenderedSpan Root;
		public int Width { get; private set; }

		public Scanline(int width)
		{
			Width = width;
		}

		public RenderedSpan[] Add(int xstart, int xend, float zstart, float zend)
		{
			var xclip = 0;
			var length = xend - xstart;
			if( xstart < 0 )
			{
				xclip = System.Math.Abs(xstart);
				xstart = 0;
			}
			if( xend > Width )
			{
				xclip = xend - Width;
				xend = Width;
			}
			length -= xclip;


			var renderedSpan = new RenderedSpan
			                   	{
			                   		XStart = xstart,
			                   		XEnd = xend,
									XClip = xclip,
									Length = length
			                   	};
			if (null == Root)
			{
				Root = renderedSpan;
			}

			return new[] {renderedSpan};
		}
	}
}
