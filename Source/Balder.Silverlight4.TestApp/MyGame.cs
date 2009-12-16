using Balder.Core.Execution;
using Balder.Core.Objects.Geometries;
using System;
using Balder.Core.Lighting;
using Balder.Core;
using Colors=System.Windows.Media.Colors;

namespace Balder.Silverlight4.TestApp
{
    public class MyGame : Game
    {
        public override void OnInitialize()
        {

            Camera.Position.X = 0;
            Camera.Position.Y = 0;
            Camera.Position.Z = -80;

            var light = new OmniLight();
            light.Diffuse = Color.FromSystemColor(Colors.Green);
            light.Ambient = Color.FromSystemColor(Colors.Green);
            light.Specular = Color.FromSystemColor(Colors.White);
            light.Position.X = 0;
            light.Position.Y = 0;
            light.Position.Z = -130;

            Scene.AddNode(light);

            base.OnInitialize();
        }


        public override void OnLoadContent()
        {
            var teapot = ContentManager.Load<Mesh>("teapot.ase");
            Scene.AddNode(teapot);
            base.OnLoadContent();
        }


        private double _sin;

        public override void OnUpdate()
        {

            Camera.Position.X = (float)(Math.Sin(_sin)*80);
            Camera.Position.Y = 0;
            Camera.Position.Z = (float)(Math.Cos(_sin) * 80);

            _sin += 0.05;
            base.OnUpdate();
        }

    }
}
