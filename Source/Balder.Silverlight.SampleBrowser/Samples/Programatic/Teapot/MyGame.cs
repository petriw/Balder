using System;
using System.Windows.Media;
using Balder.Core.Execution;
using Balder.Core.Lighting;
using Balder.Core.Objects.Geometries;
using Color=Balder.Core.Color;

namespace Balder.Silverlight.SampleBrowser.Samples.Programatic.Teapot
{
	public class MyGame : Game
	{
		private double _sin = 0;

		public override void OnInitialize()
		{
			ContentManager.AssetsRoot = "Samples/Programatic/Teapot/Assets";

			var light = new OmniLight();
			light.Diffuse = Color.FromArgb(0xff, 255, 121, 32);
			light.Specular = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
			light.Ambient = Color.FromArgb(0xff, 0x7f, 0x3f, 0x10);

			light.Position.X = -100;
			light.Position.Y = 0;
			light.Position.Z = 0;
			Scene.AddNode(light);

			base.OnInitialize();
		}

		public override void OnLoadContent()
		{
			var teapot = ContentManager.Load<Mesh>("teapot.ase");
			Scene.AddNode(teapot);

			base.OnLoadContent();
		}

		public override void OnUpdate()
		{
			Camera.Position.X = Math.Sin(_sin)*80.0;
			Camera.Position.Y = 0;
			Camera.Position.Z = Math.Cos(_sin)*80.0;

			_sin += 0.05;
			base.OnUpdate();
		}
	}
}
