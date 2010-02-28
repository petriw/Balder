using System;
using System.Windows.Media.Animation;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.BarChart
{
	public partial class Content
	{
		private static readonly Random Rnd = new Random();
		private readonly Storyboard _cameraMoveStoryboard;
		private readonly DoubleAnimation _cameraAnimation;

		public Content()
		{
			InitializeComponent();

			_cameraMoveStoryboard = Resources["CameraMoveStoryboard"] as Storyboard;
			if (null != _cameraMoveStoryboard)
			{
				_cameraAnimation = _cameraMoveStoryboard.Children[0] as DoubleAnimation;
			}
		}

		private ViewModel	ViewModel
		{
			get
			{
				return (ViewModel) DataContext;
			}
		}

		private void AddValueClick(object sender, System.Windows.RoutedEventArgs e)
		{
			var value = new BusinessObject {Value = Rnd.Next(0, 100)};
			ViewModel.Objects.Add(value);

			if (null != _cameraAnimation)
			{
				_cameraAnimation.From = Camera.Position.Z;
				_cameraAnimation.To = _cameraAnimation.To - 20;
				_cameraMoveStoryboard.Begin();
			}
		}
	}
}
