

using Balder.Core.Display;

namespace Balder.Core.SoftwareRendering.Rendering
{
	public class SpanManager
	{
		private readonly Viewport _viewport;

		public SpanManager(Viewport viewport)
		{
			_viewport = viewport;
			Initialize();
		}

		private void Initialize()
		{
			Scanlines = new Scanline[_viewport.Height];
		}


		public Scanline[] Scanlines { get; private set; }
	}
}
