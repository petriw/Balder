using Balder.Core.SoftwareRendering.Rendering;
using NUnit.Framework;

namespace Balder.Core.SoftwareRendering.Tests.Rendering
{
	[TestFixture]
	public class ScanlineTests
	{
		private const int ViewportWidth = 640;

		[Test]
		public void AddingFirstSpanShouldHaveOneInCount()
		{
			var scanline = new Scanline(ViewportWidth);

			var span = new Span();

			scanline.AddSpan(span);

			var count = scanline.GetRenderedSpanCount();
			Assert.That(count,Is.EqualTo(1));
		}

		[Test]
		public void ScanlineWithSpansShouldBeEmptyAfterReset()
		{
			var scanline = new Scanline(ViewportWidth);

			var span = new Span();

			scanline.AddSpan(span);

			scanline.Reset();

			var count = scanline.GetRenderedSpanCount();
			Assert.That(count, Is.EqualTo(0));
		}

		[Test]
		public void SingleSpanOnLineShouldBeAddedToList()
		{
			var scanline = new Scanline(ViewportWidth);

			var span = new Span { XStart = 40, XEnd = 100 };
			scanline.AddSpan(span);

			var renderedSpan = scanline.RenderedSpans[0];
			Assert.That(renderedSpan.XStart, Is.EqualTo(span.XStart));
			Assert.That(renderedSpan.XEnd, Is.EqualTo(span.XEnd));
		}

		[Test]
		public void AddingSecondSpanThatDoesNotClipShouldHaveTwoInCount()
		{
			var scanline = new Scanline(ViewportWidth);

			var span = new Span { XStart = 40, XEnd = 100 };
			scanline.AddSpan(span);

			span = new Span { XStart = 140, XEnd = 140 };
			scanline.AddSpan(span);

			var count = scanline.GetRenderedSpanCount();
			Assert.That(count, Is.EqualTo(2));
		}

		[Test]
		public void AddingSecondSpanThatDoesNotClipShouldBeAddedToList()
		{
			var scanline = new Scanline(ViewportWidth);

			var span = new Span { XStart = 40, XEnd = 100 };
			scanline.AddSpan(span);

			span = new Span { XStart = 140, XEnd = 240 };
			scanline.AddSpan(span);

			var renderedSpan = scanline.RenderedSpans[1];
			Assert.That(renderedSpan.XStart, Is.EqualTo(span.XStart));
			Assert.That(renderedSpan.XEnd, Is.EqualTo(span.XEnd));
		}

		[Test]
		public void AddingSpanThatLinksWithExistingToTheRightShouldHaveOneInCount()
		{
			var scanline = new Scanline(ViewportWidth);

			var firstSpan = new Span { XStart = 40, XEnd = 100 };
			scanline.AddSpan(firstSpan);

			var secondSpan = new Span { XStart = 100, XEnd = 140 };
			scanline.AddSpan(secondSpan);

			var count = scanline.GetRenderedSpanCount();
			Assert.That(count, Is.EqualTo(1));
		}

		[Test]
		public void AddingSpanThatLinksWithExistingToTheRightShouldExpandExistingSpan()
		{
			var scanline = new Scanline(ViewportWidth);

			var firstSpan = new Span { XStart = 40, XEnd = 100 };
			scanline.AddSpan(firstSpan);

			var secondSpan = new Span { XStart = 100, XEnd = 140 };
			scanline.AddSpan(secondSpan);

			var renderedSpan = scanline.RenderedSpans[0];
			Assert.That(renderedSpan.XStart, Is.EqualTo(firstSpan.XStart));
			Assert.That(renderedSpan.XEnd, Is.EqualTo(secondSpan.XEnd));
		}


		[Test]
		public void AddingSpanThatLinksWithExistingToTheLeftShouldHaveOneInCount()
		{
			var scanline = new Scanline(ViewportWidth);

			var firstSpan = new Span { XStart = 40, XEnd = 100 };
			scanline.AddSpan(firstSpan);

			var secondSpan = new Span { XStart = 0, XEnd = 40 };
			scanline.AddSpan(secondSpan);

			var count = scanline.GetRenderedSpanCount();
			Assert.That(count, Is.EqualTo(1));
		}

		[Test]
		public void AddingSpanThatLinksWithExistingToTheLeftShouldExpandExistingSpan()
		{
			var scanline = new Scanline(ViewportWidth);

			var firstSpan = new Span { XStart = 40, XEnd = 100 };
			scanline.AddSpan(firstSpan);

			var secondSpan = new Span { XStart = 0, XEnd = 40 };
			scanline.AddSpan(secondSpan);

			var renderedSpan = scanline.RenderedSpans[0];
			Assert.That(renderedSpan.XStart, Is.EqualTo(secondSpan.XStart));
			Assert.That(renderedSpan.XEnd, Is.EqualTo(firstSpan.XEnd));
		}


		/*
		[Test]
		public void AddingSpanThatClipsToRightWithExitingShouldReturnClippedSpan()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsToLeftWithExistingShouldReturnClippedSpan()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsToLeftAndRightWithExistingShouldReturnClippedSpans()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheLeftShouldReturnClippedSpan()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheRightShouldReturnClippedSpan()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheLeftAndClipsWithExistingSpanToTheRightShouldReturnClippedSpan()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheLeftAndClipsWithExistingSpanToTheRightShouldExpandExistingSpan()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheLeftAndClipsWithExistingSpanToTLeftAndRightShouldReturnClippedSpans()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheLeftAndClipsWithExistingSpanToTLeftAndRightShouldExpandExistingSpan()
		{
			Assert.Fail();
		}


		[Test]
		public void AddingSpanThatClipsWithScreenToTheRightAndClipsWithExistingSpanToTheLeftShouldReturnClippedSpan()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheRightAndClipsWithExistingSpanToTheLeftShouldExpandExistingSpan()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheRightAndClipsWithExistingSpanToTLeftAndRightShouldReturnClippedSpans()
		{
			Assert.Fail();
		}

		[Test]
		public void AddingSpanThatClipsWithScreenToTheRightAndClipsWithExistingSpanToTLeftAndRightShouldExpandExistingSpan()
		{
			Assert.Fail();
		}
		*/
	}
}
