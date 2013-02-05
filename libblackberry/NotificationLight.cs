using System;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class Vibration : IDisposable
	{
		[DllImport ("bps")]
		static extern int vibration_request (Intensity intensity, int msecs);

		public enum Intensity : int
		{
			Low = 1,
			Medium = 10,
			High = 100
		}

		public const int MAX_DURATION = 5000;

		public Vibration (Intensity intensity = Intensity.Medium)
		{
			vibration_request (intensity, MAX_DURATION);
		}

		public void Dispose ()
		{
			Stop ();
		}

		public static void Vibrate (Intensity intensity = Intensity.Medium, int msecs = MAX_DURATION)
		{
			vibration_request (intensity, msecs);
		}

		public static void Stop ()
		{
			vibration_request (Intensity.Low, 0);
		}
	}
}

