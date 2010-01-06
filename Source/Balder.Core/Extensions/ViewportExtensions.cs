#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
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

namespace Balder.Core.Extensions
{
	public static class ViewportExtensions
	{
		private const float MinDepth = 0f;
		private const float MaxDepth = 1f;

		private static bool WithinEpsilon(float a, float b)
		{
			var num = a - b;
			return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
		}

		public static Vector Transform(Vector position, Matrix matrix)
		{
			Vector vector = Vector.Zero;
			float num3 = (((position.X * matrix[0, 0]) + (position.Y * matrix[1, 0])) + (position.Z * matrix[2, 0])) + matrix[3, 0];
			float num2 = (((position.X * matrix[0, 1]) + (position.Y * matrix[1, 1])) + (position.Z * matrix[2, 1])) + matrix[3, 1];
			float num = (((position.X * matrix[0, 2]) + (position.Y * matrix[1, 2])) + (position.Z * matrix[2, 2])) + matrix[3, 2];
			vector.X = num3;
			vector.Y = num2;
			vector.Z = num;
			return vector;
		}

		public static Vector Unproject(this Viewport viewport, Vector source, Matrix projection, Matrix view, Matrix world)
		{
			var combinedMatrix = (world * view) * projection;
			var matrix = Matrix.Invert(combinedMatrix);
			source.X = (((source.X - viewport.XPosition) / ((float)viewport.Width)) * 2f) - 1f;
			source.Y = 1f-((((source.Y - viewport.YPosition) / ((float)viewport.Height)) * 2f));
			source.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);
			source.W = 1f;
			//var vector = Vector.Transform(source, matrix);
			var vector = Transform(source, matrix);
			var a = (((source.X * matrix[0, 3]) + (source.Y * matrix[1, 3])) + (source.Z * matrix[2, 3])) + matrix[3, 3];
			if (!WithinEpsilon(a, 1f))
			{
				vector = (Vector)(vector / (a*2f));
			}
			return vector;
		}
	}
}
