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
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Silverlight.Controls;


namespace Balder.Silverlight.Services
{
	public static class TargetDevice
	{
		private static FrameworkElement _root;

		[Obsolete("Balder for Silverlight now has a control called Game in Balder.Silverlight.Controls - use this. This method and class will be removed in future versions.")]
		public static T Initialize<T>()
			where T:Core.Execution.Game, new()
		{
			var game = new Game();
			var gameClass = new T();
			game.GameClass = gameClass;

			_root = GetRoot();
			AddGameToRoot(game);
			AutomaticallyAdjustDimensions(game);

			return gameClass;
		}

		#region Visual Tree stuff
		private static T FindFirst<T>()
			where T : UIElement
		{
			return FindFirst<T>(Application.Current.RootVisual);

		}

		private static T FindFirst<T>(UIElement element)
			where T : UIElement
		{
			if (null == element)
			{
				return null;
			}
			if (element is ContentControl)
			{
				return FindFirst<T>(((ContentControl)element).Content as UIElement);
			}
			if (element is T)
			{
				return element as T;
			}
			var child = VisualTreeHelper.GetChild(element, 0);
			if (null != child && child is UIElement)
			{
				return FindFirst<T>(child as UIElement);
			}
			return null;
		}

		private static FrameworkElement GetRoot()
		{
			var panelRoot = FindFirst<Panel>();
			if (null != panelRoot)
			{
				return panelRoot;
			}
			else
			{
				var root = FindFirst<ItemsControl>();
				if (null != root)
				{
					return root;
				}
			}

			return null;
		}


		private static void AddGameToRoot(Game game)
		{
			if (null != _root)
			{
				if (_root is Panel)
				{
					((Panel)_root).Children.Add(game);
				}
				else if (_root is ItemsControl)
				{
					((ItemsControl)_root).Items.Add(game);
				}
			}
		}

		private static FrameworkElement FindParentWithDimensionsSet(FrameworkElement element)
		{
			if (!element.Width.Equals(Double.NaN) &&
				element.Width != 0 &&
				!element.Height.Equals(Double.NaN) &&
				element.Height != 0)
			{
				return element;
			}

			if (null != element.Parent && element.Parent is FrameworkElement)
			{
				return FindParentWithDimensionsSet(element.Parent as FrameworkElement);
			}
			return null;
		}


		private static void AutomaticallyAdjustDimensions(Game game)
		{
			if (null != _root)
			{
				var elementWithDimensions = FindParentWithDimensionsSet(_root);
				if (null != elementWithDimensions)
				{
					game.Width = elementWithDimensions.Width;
					game.Height = elementWithDimensions.Height;
				}
			}
		}
		#endregion
	}
}
