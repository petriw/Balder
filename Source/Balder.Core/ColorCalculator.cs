using Balder.Core.Display;
using Balder.Core.Lighting;
using Balder.Core.Math;

namespace Balder.Core
{
	public class ColorCalculator : IColorCalculator
	{
		public Color Calculate(Viewport viewport, Vector vector, Vector normal)
		{
			var color = viewport.Scene.AmbientColor;

			foreach( ILight light in viewport.Scene.Lights )
			{
				var lightColor = light.Calculate(viewport, vector, normal);
				color = color.Additive(lightColor);
			}

			return color;
		}
	}
}
