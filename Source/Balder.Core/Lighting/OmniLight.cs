#region License
//
// Author: Petri Wilhelmsen
// Copyright (c) 2007-2009, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core.Lighting
{
	public class OmniLight : Light
	{
		public float Strength;
        public float Range;

		public OmniLight()
		{
			Strength = 1f;
            Range = 10.0f;
		}

		public override Color Calculate(Viewport viewport, Vector point, Vector normal)
		{
			var actualAmbient = Ambient;
			var actualDiffuse = Diffuse;
			var actualSpecular = Specular;

            // Use dotproduct for diffuse lighting. Add point functionality as this now is a directional light.
            // Ambient light
            var ambient = actualAmbient  * Strength;

            // Diffuse light
            var lightDir = point - Position;
            lightDir.Normalize();
            normal.Normalize();
            var dfDot = lightDir.Dot(normal);
            MathHelper.Saturate(ref dfDot);
            var diffuse = actualDiffuse * dfDot * Strength;

            // Specular highlight
            var reflection = 2f * dfDot * normal - lightDir;
            reflection.Normalize();
            var view = viewport.Camera.Position - point;
            view.Normalize();
            var spDot = reflection.Dot(view);
            MathHelper.Saturate(ref spDot);
            var specular = actualSpecular * spDot * Strength;

            // Compute self shadowing
            var shadow = 4.0f * lightDir.Dot(normal);
            MathHelper.Saturate(ref shadow);

            // Compute range for the light
            var attenuation = ((lightDir / Range).Dot(lightDir / Range));
            MathHelper.Saturate(ref attenuation);
            attenuation = 1f - attenuation;

            // Final result
            var colorVector = ambient + shadow * (diffuse + specular) * attenuation;
            //var colorVector = ambient + diffuse;

			return colorVector;
		}

	}
}
