using Balder.Core.Execution;
using Balder.Core.Math;

namespace Balder.Silverlight.TestApp
{
	public class MyGame : Game
	{
		public override void OnInitialize()
		{
			var projection = Matrix.CreatePerspectiveFieldOfView(
							MathHelper.ToRadians(40.0f),
							1.33f,
							0.1f,
							10000.0f);

			var LookAt = Vector.Forward;

			var Position = new Vector(0, 1, 2);

			var View = Matrix.CreateLookAt(
				Position,
				LookAt,
				Vector.Up);
	
			base.OnInitialize();
		}

		private double sin = 0;
		private double sin2 = 0;
		public override void OnUpdate()
		{
			Camera.Position.X = (float)(System.Math.Sin(sin) * 400.0);
			Camera.Position.Y = (float) (System.Math.Sin(sin2)*150.0)+150;
			Camera.Position.Z = (float)(System.Math.Cos(sin) * 400.0);

			sin += 0.05;
			sin2 += 0.025;
			
		}
	}
}
