using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class PlatformServices
	{
		[DllImport ("bps", EntryPoint = "bps_initialize")]
		public static extern int Initialize ();

		[DllImport ("bps", EntryPoint = "bps_shutdown")]
		public static extern void Shutdown ();

		[DllImport ("bps")]
		private static extern int dialog_set_title_text(IntPtr dialog, String title_text);

		[DllImport ("bps")]
		private static extern int dialog_create_alert(ref IntPtr dialog);

		[DllImport ("bps")]
		private static extern int dialog_set_alert_message_text(IntPtr dialog, string text);

		[DllImport ("bps")]
		private static extern int dialog_show(IntPtr dialog);

		[DllImport ("bps")]
		private static extern int dialog_request_events(int flags);

		public static void Alert(string title, string message)
		{
			IntPtr dialog;
			dialog_request_events (0);
			dialog_create_alert(ref dialog);
			dialog_set_title_text(dialog, title);
			dialog_set_alert_message_text(dialog, message);
			dialog_show(dialog);
		}
	}
}

