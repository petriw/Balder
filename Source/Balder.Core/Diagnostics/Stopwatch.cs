using System;

namespace Balder.Core.Diagnostics
{
	// Based upon Tiaan Gelenhuys Stopwatch : http://blog.tiaan.com/link/2009/02/03/stopwatch-silverlight
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
