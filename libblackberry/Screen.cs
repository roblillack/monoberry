using System;
using System.Runtime.InteropServices;

namespace BlackBerry
{
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

