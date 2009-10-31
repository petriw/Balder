using Balder.Core.Interfaces;
using Balder.Core.SoftwareRendering.Rendering;
using Moq;
using NUnit.Framework;

namespace Balder.Core.SoftwareRendering.Tests.Rendering
{
	[TestFixture]
	public class SpanManagerTests
	{
		private const int ViewportWidth = 640;
		private const int ViewportHeight = 480;

		[Test]
		public void ScanlineCountShouldBeSameAsHeightOfViewport()
		{
			var viewportMock = new Mock<IViewport>();
			viewportMock.ExpectGet(v => v.Width).Returns(ViewportWidth);
			viewportMock.ExpectGet(v => v.Height).Returns(ViewportHeight);

			var spanManager = new SpanManager(viewportMock.Object);
			Assert.That(spanManager.Scanlines.Length,Is.EqualTo(ViewportHeight));
		}
	}
}
