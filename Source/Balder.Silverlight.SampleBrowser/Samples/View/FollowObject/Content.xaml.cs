using System.Windows;
using System.Windows.Media.Animation;
using Balder.Core.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.View.FollowObject
{
	public partial class Content
	{
		private Storyboard _positionStoryboard;
		private Storyboard _targetStoryboard;


		public Content()
		{
			InitializeComponent();

			_positionStoryboard = LayoutRoot.Resources["PositionStoryboard"] as Storyboard;
			_targetStoryboard = LayoutRoot.Resources["TargetStoryboard"] as Storyboard;

		}

		private void Reset(object sender, RoutedEventArgs e)
		{
			Position(new Coordinate(0, 20, -200), 0);
			Target(Vector.Zero);

		}

		private void RedTarget(object sender, RoutedEventArgs e)
		{
			Target(RedBox.Position);
		}

		private void GreenTarget(object sender, RoutedEventArgs e)
		{
			Target(GreenBox.Position);
		}

		private void BlueTarget(object sender, RoutedEventArgs e)
		{
			Target(BlueBox.Position);
		}


		private void RedPosition(object sender, RoutedEventArgs e)
		{
			Position(RedBox.Position,30);
		}

		private void GreenPosition(object sender, RoutedEventArgs e)
		{
			Position(GreenBox.Position, 30);
		}

		private void BluePosition(object sender, RoutedEventArgs e)
		{
			Position(BlueBox.Position, 30);
		}

		private void Position(Coordinate position, float yOffset)
		{
			var xTimeline = _positionStoryboard.Children[0] as DoubleAnimation;
			var yTimeline = _positionStoryboard.Children[1] as DoubleAnimation;
			var zTimeline = _positionStoryboard.Children[2] as DoubleAnimation;
			xTimeline.From = Game.Camera.Position.X;
			xTimeline.To = position.X;

			yTimeline.From = Game.Camera.Position.Y;
			yTimeline.To = position.Y+yOffset;

			zTimeline.From = Game.Camera.Position.Z;
			zTimeline.To = position.Z;

			_positionStoryboard.Begin();
			
		}

		private void Target(Coordinate target)
		{
			var xTimeline = _targetStoryboard.Children[0] as DoubleAnimation;
			var yTimeline = _targetStoryboard.Children[1] as DoubleAnimation;
			var zTimeline = _targetStoryboard.Children[2] as DoubleAnimation;
			xTimeline.From = Game.Camera.Target.X;
			xTimeline.To = target.X;

			yTimeline.From = Game.Camera.Target.Y;
			yTimeline.To = target.Y;

			zTimeline.From = Game.Camera.Target.Z;
			zTimeline.To = target.Z;

			_targetStoryboard.Begin();
		}

		private void GameUpdate(object sender, System.EventArgs e)
		{
			CameraInfo.Text = string.Format("Position : {0}, Target : {1}", Game.Camera.Position, Game.Camera.Target);
		}

		private void LeftPosition(object sender, RoutedEventArgs e)
		{
			Position(new Coordinate(-200,20,-30), 0);
		}

		private void RightPosition(object sender, RoutedEventArgs e)
		{
			Position(new Coordinate(200, 20, -30), 0);
		}

		private void FrontPosition(object sender, RoutedEventArgs e)
		{
			Position(new Coordinate(0, 20, -200), 0);
		}

		private void BackPosition(object sender, RoutedEventArgs e)
		{
			Position(new Coordinate(0, 20, 200), 0);
		}

		private void LeftTarget(object sender, RoutedEventArgs e)
		{
			Target(new Coordinate(-200, 20, -30));
		}

		private void RightTarget(object sender, RoutedEventArgs e)
		{
			Target(new Coordinate(200, 20, -30));
		}

		private void FrontTarget(object sender, RoutedEventArgs e)
		{
			Target(new Coordinate(0, 20, -200));
		}

		private void BackTarget(object sender, RoutedEventArgs e)
		{
			Target(new Coordinate(0, 20, 200));
		}

	}
}
