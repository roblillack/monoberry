using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class Event
	{
		[DllImport ("bps")]
		static extern uint bps_event_get_code (IntPtr handle);

		[DllImport ("bps")]
		static extern IntPtr bps_event_get_payload (IntPtr handle);

		protected IntPtr handle;

		internal Event (IntPtr h)
		{
			handle = h;
		}

		public uint Code {
			get {
				return bps_event_get_code (handle);
			}
		}
	}
	
}
