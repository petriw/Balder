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
using System.Windows.Media.Animation;

namespace Balder.Silverlight.Extensions
{
	public static class VisualStateExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <param name="stateName"></param>
		public static void GoToState(this Control control, string stateName)
		{
			VisualStateManager.GoToState(control, stateName, true);
		}

		public static void GoToState(this Control control, string stateName, bool useTransitions)
		{
			VisualStateManager.GoToState(control, stateName, false);
		}

		public static Storyboard GetStoryboardForState(this UserControl control, string stateName)
		{
			Storyboard stateStoryboard = null;

			var root = control.FindName("LayoutRoot") as FrameworkElement;
			
			var groups = VisualStateManager.GetVisualStateGroups(root);
			foreach (VisualStateGroup group in groups)
			{
				foreach (VisualState state in group.States)
				{
					if (state.Name == stateName)
					{
						stateStoryboard = state.Storyboard;
					}
				}
			}
			return stateStoryboard;
		}


		public static void AddStateCompletedEventHandler(this UserControl control, string stateName, EventHandler stateChanged)
		{
			Storyboard stateStoryboard = control.GetStoryboardForState(stateName);
			if (null != stateStoryboard && null != stateChanged)
			{
				stateStoryboard.Completed += (s, e) => stateChanged(s, new EventArgs());
			}
		}


		/// <summary>
		/// Go to a specific state and provide a callback when the state change
		/// has completed its transition.
		/// 
		/// This overload will automatically use transitions
		/// </summary>
		/// <param name="control">UserControl to change state for</param>
		/// <param name="stateName">Name of state to change to</param>
		/// <param name="stateChanged">Delegate to call when transition is done</param>
		/// <remarks>
		/// You must have a LayoutRoot object that is named "LayoutRoot". Oherwize
		/// it won't find it.
		/// </remarks>
		public static void GoToState(this UserControl control, string stateName, EventHandler stateChanged)
		{
			GoToState(control, stateName, true, stateChanged);
		}

		/// <summary>
		/// Go to a specific state and provide a callback when the state change
		/// has completed its transition.
		/// </summary>
		/// <param name="control">UserControl to change state for</param>
		/// <param name="stateName">Name of state to change to</param>
		/// <param name="useTransitions">Use transitions or not</param>
		/// <param name="stateChanged">Delegate to call when transition is done</param>
		/// <remarks>
		/// You must have a LayoutRoot object that is named "LayoutRoot". Oherwize
		/// it won't find it.
		/// </remarks>
		public static void GoToState(this UserControl control, string stateName, bool useTransitions, EventHandler stateChanged)
		{
			Storyboard stateStoryboard = control.GetStoryboardForState(stateName);
			if (null != stateStoryboard && null != stateChanged)
			{
				stateStoryboard.Completed += (s, e) => stateChanged(s, new EventArgs());
			}
			VisualStateManager.GoToState(control, stateName, useTransitions);
		}

		public static void GoToStates(this Control control, params string[] stateNames)
		{
			GoToStates(control, true, stateNames);
		}

		public static void GoToStates(this Control control, bool useTransitions, params string[] stateNames)
		{
			foreach (string name in stateNames)
			{
				VisualStateManager.GoToState(control, name, useTransitions);
			}
		}

	}
}