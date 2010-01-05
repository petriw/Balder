using Balder.Core.Execution;

namespace Balder.Silverlight.TestApp
{
	public class MyGame : Game
	{
		private double sin = 0;
		private double sin2 = 0;
		public override void OnUpdate()
		{
			//Camera.Position.X = (float)(System.Math.Sin(sin) * 50.0);
			//Camera.Position.Y = (float) (System.Math.Sin(sin2)*150.0)+150;
			//Camera.Position.Z = (float)(System.Math.Cos(sin) * 50.0);

			Camera.Position.X = 20;
			Camera.Target.X = Camera.Position.X;
			Camera.Position.Y = 0;
			Camera.Target.Y = Camera.Position.Y;

			Camera.Position.Z = -100;

			sin += 0.05;
			sin2 += 0.025;
			
		}
	}
}
