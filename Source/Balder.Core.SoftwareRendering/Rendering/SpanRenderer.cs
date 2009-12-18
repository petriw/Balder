using System;
using System.Windows.Media;
using Balder.Core.Imaging;

namespace Balder.Core.SoftwareRendering
{
	public class SpanRenderer : ISpanRenderer
	{
		public bool SupportsDepthBuffer
		{
			get { return false; }
		}

		public void Flat(IBuffers buffer, Span span, Color color)
		{
			throw new NotImplementedException();
		}

		public void Gouraud(IBuffers buffer, Span span)
		{
			throw new NotImplementedException();
		}

		public void Texture(IBuffers buffer, Span span, Image image, ImageContext texture)
		{
			throw new NotImplementedException();
		}
	}
}
