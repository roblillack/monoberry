using System;
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

	public class Screen
	{
		[DllImport ("screen")]
		public static extern int screen_create_context(IntPtr pctx, int flags);

		[DllImport ("screen")]
		public static extern int screen_create_window(IntPtr pwin, IntPtr ctx);

		[DllImport ("screen")]
		public static extern int screen_set_window_property_iv(IntPtr win, int pname, ref int param);

		[DllImport ("screen")]
		public static extern int screen_create_window_buffers(IntPtr win, int count);

		[DllImport ("screen")]
		public static extern int screen_get_window_property_pv(IntPtr win, int pname, ref IntPtr param);

		//[DllImport ("screen")]
		//[DllImport ("screen")]
		//[DllImport ("screen")]
	}
}

