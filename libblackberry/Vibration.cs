using System;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class NotificationLight : IDisposable
	{
		[DllImport ("bps")]
		static extern int led_request_rgb (string id, Color color, int blinkCount);

		[DllImport ("bps")]
		static extern int led_cancel (string id);

		public enum Color : int
		{
			Blue    = 0x000000FF,
      		Green   = 0x0000FF00,   
      		Cyan    = 0x0000FFFF,   
      		Red     = 0x00FF0000,   
      		Magenta = 0x00FF00FF,   
      		Yellow  = 0x00FFFF00,   
      		White   = 0x00FFFFFF
		}

		string id;

		public NotificationLight (Color color)
		{
			id = new Guid ().ToString ();
			led_request_rgb (id, color, 0);
		}

		public void Dispose ()
		{
			led_cancel (id);
		}

		public static void Blink (Color color, int blinkCount = 1)
		{
			led_request_rgb ("MONOBERRY", color, blinkCount);
		}
	}
}

