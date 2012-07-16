using System;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class Navigator : IDisposable
	{

		public enum EventType {
			NAVIGATOR_INVOKE = 0x01,
			NAVIGATOR_EXIT = 0x02,   
			NAVIGATOR_WINDOW_STATE = 0x03,   
			NAVIGATOR_SWIPE_DOWN = 0x04,   
			NAVIGATOR_SWIPE_START = 0x05,   
			NAVIGATOR_LOW_MEMORY = 0x06,   
			NAVIGATOR_ORIENTATION_CHECK = 0x07,   
			NAVIGATOR_ORIENTATION = 0x08,   
			NAVIGATOR_BACK = 0x09,   
			NAVIGATOR_WINDOW_ACTIVE = 0x0a,   
			NAVIGATOR_WINDOW_INACTIVE = 0x0b,   
			NAVIGATOR_ORIENTATION_DONE = 0x0c,   
			NAVIGATOR_ORIENTATION_RESULT = 0x0d,   
			NAVIGATOR_WINDOW_LOCK = 0x0e,   
			NAVIGATOR_WINDOW_UNLOCK = 0x0f,   
			NAVIGATOR_INVOKE_TARGET = 0x10,   
			NAVIGATOR_INVOKE_QUERY_RESULT = 0x11,   
			NAVIGATOR_INVOKE_VIEWER = 0x12,   
			NAVIGATOR_OTHER = 0xff
		}

		public enum BadgeType {
			Splat = 0
		}

		[DllImport ("bps")]
		static extern int navigator_request_events (int flags);

		[DllImport ("bps")]
		static extern int navigator_invoke(string uri,
		                                   /*out*/ IntPtr err);

		[DllImport ("bps")]
		static extern int navigator_add_uri(string icon_path,
		                                    string icon_label,
		                                    string default_category,
		                                    string url,
		                                    /*out*/ IntPtr err);

		[DllImport ("bps")]
		static extern int navigator_clear_badge ();

		[DllImport ("bps")]
		static extern int navigator_set_badge (BadgeType badge);

		[DllImport ("bps")]
		static extern int navigator_get_domain ();

		private int eventDomain;

		public Action OnExit;

		public Navigator ()
		{
			PlatformServices.Initialize ();
			navigator_request_events (0);
			eventDomain = navigator_get_domain ();
			PlatformServices.AddEventHandler (eventDomain, HandleEvent);
		}

		public void AddUri (string iconPath, string iconLabel, string defaultCategory, string url)
		{
			navigator_add_uri (iconPath, iconLabel, defaultCategory, url, IntPtr.Zero);
		}

		public void Invoke (string uri)
		{
			navigator_invoke (uri, IntPtr.Zero);
		}

		public bool HasBadge {
			set {
				if (value) {
					navigator_set_badge (BadgeType.Splat);
				} else {
					navigator_clear_badge ();
				}
			}
		}

		void HandleEvent (IntPtr eventHandle)
		{
			var type = (EventType)new Event (eventHandle).Code;

			switch (type) {
			case EventType.NAVIGATOR_EXIT:
				if (OnExit != null) {
					OnExit ();
				}
				break;
			default:
				Console.WriteLine ("UNHANDLED NAVIGATOR EVENT, TYPE: {0}", type);
				break;
			}

		}

		public void Dispose ()
		{
			PlatformServices.RemoveEventHandler (eventDomain);
		}
	}
}

