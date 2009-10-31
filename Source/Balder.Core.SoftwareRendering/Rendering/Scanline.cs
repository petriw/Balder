namespace Balder.Core.SoftwareRendering.Rendering
{
	public class Scanline
	{
		private readonly int _width;

		public Scanline(int width)
		{
			_width = width;
			Initialize();
		}

		private void Initialize()
		{
			RenderedSpans = new RenderedSpan[_width];
			for (var index = 0; index < RenderedSpans.Length; index++)
			{
				RenderedSpans[index] = new RenderedSpan();
			}
		}

		public RenderedSpan[] RenderedSpans { get; private set; }

		public void Reset()
		{
			RenderedSpans[0].IsActive = false;			
		}


		public int GetRenderedSpanCount()
		{
			var count = 0;
			for( var index=0; index<RenderedSpans.Length; index++ )
			{
				var span = RenderedSpans[index];
				if( !span.IsActive )
				{
					break;
				}
				count++;
			}
			return count;
		}

		private int GetFirstAvailableSpanIndex()
		{
			for( var index=0; index<RenderedSpans.Length; index++ )
			{
				var span = RenderedSpans[index];
				if( !span.IsActive )
				{
					span.XStart = -1;
					span.XEnd = -1;
					return index;
				}
			}
			return -1;
		}

		private int GetLinkedSpan(Span span, out bool isLinkedLeft, out bool isLinkedRight)
		{
			isLinkedLeft = false;
			isLinkedRight = false;
			for (var index = 0; index < RenderedSpans.Length; index++)
			{
				var renderedSpan = RenderedSpans[index];
				if (!renderedSpan.IsActive)
				{
					break;
				}
				else
				{
					if (span.XStart >= renderedSpan.XStart &&
						span.XStart <= renderedSpan.XEnd )
					{
						isLinkedRight = true;
					}

					if (span.XStart <= renderedSpan.XStart &&
						span.XEnd <= renderedSpan.XStart)
					{
						isLinkedLeft = true;
					}

					if( isLinkedRight || isLinkedLeft )
					{
						return index;
					}
				}
			}
			isLinkedLeft = false;
			isLinkedRight = false;
			return -1;
		}

		public Span	AddSpan(Span span)
		{
			var linkedRight = false;
			var linkedLeft = false;
			var spanIndex = GetLinkedSpan(span, out linkedLeft, out linkedRight);
			if (spanIndex == -1)
			{
				spanIndex = GetFirstAvailableSpanIndex();
				if (spanIndex == -1)
				{
					return span;
				}
			}

			var renderedSpan = RenderedSpans[spanIndex];
			if( linkedRight || linkedLeft )
			{
				if ( linkedRight )
				{
					renderedSpan.XEnd = span.XEnd;
				} else
				{
					renderedSpan.XStart = span.XStart;
				}
			} else
			{
				renderedSpan.IsActive = true;
				renderedSpan.XStart = span.XStart;
				renderedSpan.XEnd = span.XEnd;
			}
			
			return span;
		}
	}
}
