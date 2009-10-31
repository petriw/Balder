using Balder.Core.Interfaces;

namespace Balder.Core.SoftwareRendering.Rendering
{
	public class SpanManager
	{
		private readonly IViewport _viewport;

		public SpanManager(IViewport viewport)
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
