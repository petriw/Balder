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

namespace Balder.Core.Diagnostics
{
	public class Stopwatch
	{
		public static readonly bool IsHighResolution = false;
		public static readonly long Frequency = TimeSpan.TicksPerSecond;

		public TimeSpan Elapsed
		{
			get
			{
				if (!StartUtc.HasValue)
				{
					return TimeSpan.Zero;
				}
				if (!EndUtc.HasValue)
				{
					return (DateTime.UtcNow - StartUtc.Value);
				}
				return (EndUtc.Value - StartUtc.Value);
			}
		}

		public long ElapsedMilliseconds
		{
			get
			{
				return ElapsedTicks / TimeSpan.TicksPerMillisecond;
			}
		}
		public long ElapsedTicks { get { return Elapsed.Ticks; } }
		public bool IsRunning { get; private set; }
		private DateTime? StartUtc { get; set; }
		private DateTime? EndUtc { get; set; }

		public static long GetTimestamp()
		{
			return DateTime.UtcNow.Ticks;
		}

		public void Reset()
		{
			Stop();
			EndUtc = null;
			StartUtc = null;
		}

		public void Start()
		{
			if (IsRunning)
			{
				return;
			}
			if ((StartUtc.HasValue) &&
				(EndUtc.HasValue))
			{
				// Resume the timer from its previous state
				StartUtc = StartUtc.Value +
					(DateTime.UtcNow - EndUtc.Value);
			}
			else
			{
				// Start a new time-interval from scratch
				StartUtc = DateTime.UtcNow;
			}
			IsRunning = true;
			EndUtc = null;
		}

		public void Stop()
		{
			if (IsRunning)
			{
				IsRunning = false;
				EndUtc = DateTime.UtcNow;
			}
		}

		public static Stopwatch StartNew()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}
	}
}
