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
		private Mesh _teapot;

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

			Camera.Position.X = 0;
			Camera.Position.Y = 20;
			Camera.Position.Z = -100;

			Camera.Target.Y = 0;

			base.OnInitialize();
		}

		public override void OnLoadContent()
		{
			_teapot = ContentManager.Load<Mesh>("teapot.ase");
			Scene.AddNode(_teapot);

			base.OnLoadContent();
		}

		public override void OnUpdate()
		{
			_teapot.Rotation.Y += 1;
			base.OnUpdate();
		}
	}
}
