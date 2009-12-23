using Balder.Core.SoftwareRendering.Rendering;
using NUnit.Framework;

namespace Balder.Core.SoftwareRendering.Tests.Rendering
{
	[TestFixture]
	public class ScanlineTests
	{
		[Test]
		public void AddingFirstSpanShouldSetRootToThatSpanDefinition()
		{
			var scanline = new Scanline(640);
			var xstart = 0;
			var xend = 10;
			var renderedSpans = scanline.Add(xstart, xend,0,0);

			Assert.That(renderedSpans[0],Is.EqualTo(scanline.Root));

			Assert.That(renderedSpans[0].XStart, Is.EqualTo(xstart));
			Assert.That(renderedSpans[0].XEnd, Is.EqualTo(xend));
		}


		[Test]
		public void AddingSpanThatClipsWithStartOfScanlineShouldReturnARenderedSpanThatIsClipped()
		{
			var scanline = new Scanline(640);
			var xstart = -10;
			var xend = 10;
			var renderedSpans = scanline.Add(xstart, xend,0,0);

			Assert.That(renderedSpans.Length, Is.EqualTo(1));
			Assert.That(renderedSpans[0].XStart,Is.EqualTo(0));
			Assert.That(renderedSpans[0].XClip,Is.EqualTo(10));
			Assert.That(renderedSpans[0].XEnd, Is.EqualTo(10));
			Assert.That(renderedSpans[0].Length, Is.EqualTo(10));
		}

		[Test]
		public void AddingSpanThatClipsWithEndOfScanlingShouldReturnARenderedSpanThatIsClipped()
		{
			var scanline = new Scanline(640);
			var xstart = 630;
			var xend = 650;
			var renderedSpans = scanline.Add(xstart, xend, 0, 0);

			Assert.That(renderedSpans.Length, Is.EqualTo(1));
			Assert.That(renderedSpans[0].XStart, Is.EqualTo(630));
			Assert.That(renderedSpans[0].XClip, Is.EqualTo(10));
			Assert.That(renderedSpans[0].XEnd, Is.EqualTo(640));
			Assert.That(renderedSpans[0].Length, Is.EqualTo(10));
		}

		[Test]
		public void AddingSecondSpanAfterExistingShouldNotSetRootToTheSecond()
		{
			var scanline = new Scanline(640);
			var firstRenderedSpans = scanline.Add(0, 50, 0, 0);
			var secondRenderedSpans = scanline.Add(100, 150, 0, 0);

			Assert.That(scanline.Root, Is.Not.EqualTo(secondRenderedSpans[0]));
			Assert.That(scanline.Root, Is.EqualTo(firstRenderedSpans[0]));
		}

		[Test]
		public void AddingSecondSpanThatShouldBeBeforeExistingShouldSetItToRootAndUpdateLinkInformation()
		{
			var scanline = new Scanline(640);
			var firstRenderedSpans = scanline.Add(100, 150, 0, 0);
			var secondRenderedSpans = scanline.Add(0, 50, 0, 0);

			Assert.That(scanline.Root,Is.EqualTo(secondRenderedSpans[0]));
			Assert.That(secondRenderedSpans[0].Next, Is.Not.Null);
			Assert.That(secondRenderedSpans[0].Next, Is.EqualTo(firstRenderedSpans[0]));
			Assert.That(secondRenderedSpans[0].Previous,Is.Null);
			Assert.That(firstRenderedSpans[0].Previous,Is.EqualTo(secondRenderedSpans[0]));
		}



	}
}
