using System.ComponentModel;
using Balder.Core.Math;
using Balder.Silverlight.SampleBrowser.Constants;
using Balder.Silverlight.SampleBrowser.Content;

namespace Balder.Silverlight.SampleBrowser.Samples.View.CameraManipulation
{
	[Category(Categories.View)]
	[SamplePage("Camera Manipulation")]
	[Description("Sample showing how to manipulate camera in Xaml")]
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Loaded += Content_Loaded;
		}

		void Content_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			CalculateCameraPosition();
		}

		private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (null != _xSlider)
			{
				CalculateCameraPosition();
			}
		}

		private void CalculateCameraPosition()
		{
			var rotationX = Matrix.CreateRotationX((float)_xSlider.Value);
			var rotationY = Matrix.CreateRotationY((float)_ySlider.Value);

			var combined = rotationX*rotationY;
			var forward = Vector.Forward;
			var zoomedForward = forward*(float) _zoomSlider.Value;
			var position = zoomedForward*combined;

			var target = new Vector((float)_game.Camera.Target.X, 
			                        (float)_game.Camera.Target.Y, 
			                        (float)_game.Camera.Target.Z);
			var actualPosition = target - position;

			_game.Camera.Position.X = actualPosition.X;
			_game.Camera.Position.Y = actualPosition.Y;
			_game.Camera.Position.Z = actualPosition.Z;
		}
	}
}