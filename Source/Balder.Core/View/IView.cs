using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core.View
{
	public interface IView
	{
		Coordinate Position { get; set; }
		Matrix ViewMatrix { get; }
		Matrix ProjectionMatrix { get; }
		float Near { get; set; }
		float Far { get; set; }
		float DepthDivisor { get; }
		float DepthZero { get; }
		bool IsInView(Vector vector);
		bool IsInView(Coordinate coordinate);
		void Update(Viewport viewport);
	}
}