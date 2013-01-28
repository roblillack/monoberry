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
			NAVIGATOR_INVOKE_TARGET_RESULT = 0x13,
			NAVIGATOR_INVOKE_VIEWER_RESULT = 0x14,
			NAVIGATOR_INVOKE_VIEWER_RELAY = 0x15,
			NAVIGATOR_INVOKE_VIEWER_STOPPED = 0x16,
			NAVIGATOR_KEYBOARD_STATE = 0x17,
			NAVIGATOR_KEYBOARD_POSITION = 0x18,
			NAVIGATOR_INVOKE_VIEWER_RELAY_RESULT = 0x19,
			NAVIGATOR_DEVICE_LOCK_STATE = 0x1a,
			NAVIGATOR_WINDOW_COVER = 0x1b,
			NAVIGATOR_WINDOW_COVER_ENTER = 0x1c,
			NAVIGATOR_WINDOW_COVER_EXIT = 0x1d,
			NAVIGATOR_CARD_PEEK_STARTED = 0x1e,
			NAVIGATOR_CARD_PEEK_STOPPED = 0x1f,
			NAVIGATOR_CARD_RESIZE = 0x20,
			NAVIGATOR_CHILD_CARD_CLOSED = 0x21,
			NAVIGATOR_CARD_CLOSED = 0x22,
			NAVIGATOR_INVOKE_GET_FILTERS_RESULT = 0x23,
			NAVIGATOR_APP_STATE = 0x24,
			NAVIGATOR_INVOKE_SET_FILTERS_RESULT = 0x25,
			NAVIGATOR_PEEK_STARTED = 0x26,
			NAVIGATOR_PEEK_STOPPED = 0x27,
			NAVIGATOR_CARD_READY_CHECK = 0x28,
			NAVIGATOR_POOLED = 0x29,
			NAVIGATOR_OTHER = 0xff
		}

		public enum BadgeType {
			Splat = 0
		}

		[DllImport ("bps")]
		static extern int navigator_request_events (int flags);

		[DllImport ("bps")]
		static extern int navigator_stop_events (int flags);

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

		private int eventDomain = 0;
		private const int ALL_EVENTS = 0;

		public Action OnExit;
		public Action OnSwipeDown;

		public Navigator ()
		{
			PlatformServices.Initialize ();
			RequestEvents = true;
		}

		public bool RequestEvents {
			get {
				return eventDomain != 0;
			}
			set {
				if (value && eventDomain == 0) {
					navigator_request_events (ALL_EVENTS);
					eventDomain = navigator_get_domain ();
					PlatformServices.AddEventHandler (eventDomain, HandleEvent);
				} else {
					navigator_stop_events (ALL_EVENTS);
					PlatformServices.RemoveEventHandler (eventDomain);
					eventDomain = 0;
				}
			}
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
			case EventType.NAVIGATOR_SWIPE_DOWN:
				if (OnSwipeDown != null) {
					OnSwipeDown ();
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

