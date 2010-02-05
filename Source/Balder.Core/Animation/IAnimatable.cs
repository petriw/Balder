#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
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

namespace Balder.Core.Animation
{
	public enum AnimationStatus
	{
		Stopped=1,
		Playing,
		Paused
	}

	public enum AnimationDirection
	{
		Forward=1,
		Backwards,
	}

	public enum AnimationBehaviour
	{
		Once=1,
		Loop,
		PingPoing
	}

	public delegate void AnimationEventHandler(object sender, AnimationEventArgs e);

	public class AnimationEventArgs
	{
		private readonly Animatable _object;

		internal AnimationEventArgs(Animatable theObject)
		{
			_object = theObject;
		}

		public Animatable Object
		{
			get { return _object; }
		}
	}


	public abstract class Animatable
	{
		public AnimationStatus Status { get; private set; }
		public AnimationDirection Direction { get; private set; }
		public AnimationBehaviour Behaviour { get; private set; }

		public event AnimationEventHandler AnimationLooped;
		public event AnimationEventHandler AnimationFinished;

		protected abstract void HandleAnimation(TimeSpan deltaTime);

		public void Play()
		{
			
		}


		public void Play(AnimationBehaviour behaviour)
		{
			
		}

		public void Play(AnimationBehaviour behaviour, AnimationDirection direction)
		{
			
		}

		public void Play(AnimationBehaviour behaviour, AnimationDirection direction, int loopCount)
		{
			
		}

		public void Stop()
		{
			
		}

		public void Pause()
		{
			
		}
	}
}