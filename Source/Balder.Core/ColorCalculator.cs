using System;
using Balder.Core.Display;
using Balder.Core.Lighting;
using Balder.Core.Materials;
using Balder.Core.Math;

namespace Balder.Core
{
	public class ColorCalculator : IColorCalculator
	{
		public Color Calculate(Viewport viewport, Vector vector, Vector normal, Material material)
		{
			throw new NotImplementedException();
		}

		public Color Calculate(Viewport viewport, Vector vector, Vector normal, Color diffuseColor)
		{
			var color = viewport.Scene.AmbientColor;
			color = color.Additive(diffuseColor);
			

			foreach( ILight light in viewport.Scene.Lights )
			{
				var lightColor = light.Calculate(viewport, vector, normal);
				color = color.Additive(lightColor);
			}

			return color;
		}
	}
}
