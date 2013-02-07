using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EventHandle = System.IntPtr;

namespace BlackBerry
{
	public class Event
	{
		[DllImport ("bps")]
		static extern uint bps_event_get_code (EventHandle handle);

		[DllImport ("bps")]
		static extern IntPtr bps_event_get_payload (EventHandle handle);

		[DllImport ("bps")]
		static extern string navigator_event_get_err (EventHandle handle);

		[DllImport ("bps")]
		static extern string navigator_event_get_id (EventHandle handle);

		internal EventHandle handle;

		internal Event (EventHandle h)
		{
			handle = h;
		}

		public uint Code {
			get {
				return bps_event_get_code (handle);
			}
		}

		public string ErrorMessage {
			get {
				return navigator_event_get_err (handle);
			}
		}

		public string Id {
			get {
				return navigator_event_get_id (handle);
			}
		}
	}
	
}
