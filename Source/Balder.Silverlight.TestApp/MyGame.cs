using Balder.Core.Execution;

namespace Balder.Silverlight.TestApp
{
	public class MyGame : Game
	{
		private double sin = 0;
		private double sin2 = 0;


		public override void OnInitialize()
		{
			int val1 = 0x00010001;
			int add = 0x00030001;

			int val3 = val1 + add;

			int v1 = val3 >> 16;
			int v2 = val3 & 0xffff;
			
			base.OnInitialize();
		}
		public override void OnUpdate()
		{
			Camera.Position.X = (float)(System.Math.Sin(sin) * 150.0);
			Camera.Position.Y = (float) (System.Math.Sin(sin2)*150.0)+150;
			Camera.Position.Z = (float)(System.Math.Cos(sin) * 150.0);

			/*
			Camera.Position.X = 0;
			Camera.Target.X = Camera.Position.X;
			Camera.Position.Y = 0;
			Camera.Target.Y = Camera.Position.Y;


			Camera.Position.Z += 0.5;
			 * */

			sin += 0.05;
			sin2 += 0.025;
			
		}
	}
}
