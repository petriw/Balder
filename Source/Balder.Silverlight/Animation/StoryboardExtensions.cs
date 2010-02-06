#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Balder.Core.Math;

namespace Balder.Silverlight.Animation
{
	public class CoordinateAnimation
	{
		public string TargetName { get; set; }
		public string TargetProperty { get; set; }


		public Coordinate From { get; set; }
		public Coordinate To { get; set; }

		public IEasingFunction EasingFunction { get; set; }

		public TimeSpan? BeginTime { get; set; }
		public Duration Duration { get; set; }
	}


	public static class StoryboardExtensions
	{
		public static readonly DependencyProperty CoordinateAnimationProperty =
			DependencyProperty.RegisterAttached("CoordinateAnimation", typeof (CoordinateAnimation),
			                                    typeof (StoryboardExtensions), null);

		public static void SetCoordinateAnimation(DependencyObject obj, CoordinateAnimation coordinateAnimation)
		{
			var storyboard = obj as Storyboard;

			var xAnimation = new DoubleAnimation();
			xAnimation.From = coordinateAnimation.From.X;
			xAnimation.To = coordinateAnimation.To.X;
			xAnimation.BeginTime = coordinateAnimation.BeginTime;
			xAnimation.Duration = coordinateAnimation.Duration;
			xAnimation.EasingFunction = coordinateAnimation.EasingFunction;
			
			Storyboard.SetTargetName(xAnimation, coordinateAnimation.TargetName);
			Storyboard.SetTargetProperty(xAnimation, new PropertyPath(string.Format("{0}.(X)",coordinateAnimation.TargetProperty)));
			storyboard.Children.Add(xAnimation);


			var yAnimation = new DoubleAnimation();
			yAnimation.From = coordinateAnimation.From.Y;
			yAnimation.To = coordinateAnimation.To.Y;
			yAnimation.BeginTime = coordinateAnimation.BeginTime;
			yAnimation.Duration = coordinateAnimation.Duration;
			yAnimation.EasingFunction = coordinateAnimation.EasingFunction;
			Storyboard.SetTargetName(yAnimation, coordinateAnimation.TargetName);
			Storyboard.SetTargetProperty(yAnimation, new PropertyPath(string.Format("{0}.(Y)", coordinateAnimation.TargetProperty)));
			storyboard.Children.Add(yAnimation);


			var zAnimation = new DoubleAnimation();
			zAnimation.From = coordinateAnimation.From.Z;
			zAnimation.To = coordinateAnimation.To.Z;
			zAnimation.BeginTime = coordinateAnimation.BeginTime;
			zAnimation.Duration = coordinateAnimation.Duration;
			zAnimation.EasingFunction = coordinateAnimation.EasingFunction;
			Storyboard.SetTargetName(zAnimation, coordinateAnimation.TargetName);
			Storyboard.SetTargetProperty(zAnimation, new PropertyPath(string.Format("{0}.(Z)", coordinateAnimation.TargetProperty)));
			storyboard.Children.Add(zAnimation);
			
			obj.SetValue(CoordinateAnimationProperty,coordinateAnimation);
		}

		public static CoordinateAnimation GetCoordinateAnimation(DependencyObject obj)
		{
			var animation = obj.GetValue(CoordinateAnimationProperty) as CoordinateAnimation;
			return animation;
		}
		
	}
}
