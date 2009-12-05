using Balder.Core;
using Balder.Core.Debug;
using Balder.Core.Execution;
using Balder.Core.Lighting;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;

namespace Balder.Silverlight.TestApp
{
	public class MyGame : Game
	{
		private Mesh _teapot;


		public override void Initialize()
		{
			Runtime.Instance.DebugLevel |= DebugLevel.BoundingSpheres;
			Camera.Position.Z = -100;
		}

		public override void LoadContent()
		{
			_teapot = ContentManager.Load<Mesh>("teapot.ASE");
			_teapot.Color = Color.FromArgb(0xff,0,0,0xff);
			_teapot.Click += teapotClick;
			Scene.AddNode(_teapot);
			

			var light = new OmniLight();
			light.Range = 2.0f;
			light.Position.X = 0;
			light.Position.Y = 0;
			light.Position.Z = -130;
			light.Diffuse = Color.FromArgb(0xff, 255, 121, 32);
			light.Specular = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
			light.Ambient = Color.FromArgb(0xff, 0x7f, 0x3f, 0x10);
			
			Scene.AddNode(light);
		}

		void teapotClick(object sender, System.EventArgs e)
		{
			_teapot.Color = Color.FromArgb(0xff, 0xff, 0, 0);
		}


		private double sin = 0;
		public override void Update()
		{
			//Camera.Position.X = (float)(System.Math.Sin(sin) * 100.0);
			//Camera.Position.Z = (float)(System.Math.Cos(sin) * 100.0);

			sin += 0.1;
			
		}
	}
}
