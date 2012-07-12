using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class Dialog : IDisposable
	{
		[DllImport ("bps")]
		static extern int dialog_set_title_text (IntPtr dialog, String title_text);

		[DllImport ("bps")]
		static extern int dialog_create_alert (out IntPtr dialog);

		[DllImport ("bps")]
		static extern int dialog_destroy (IntPtr handle);

		[DllImport ("bps")]
		static extern int dialog_set_alert_message_text (IntPtr dialog, string text);

		[DllImport ("bps")]
		static extern int dialog_show (IntPtr dialog);

		[DllImport ("bps")]
		static extern int dialog_update (IntPtr dialog);

		static IDictionary<IntPtr, Dialog> dialogs = new Dictionary<IntPtr, Dialog> ();

		internal Dialog FindDialog (IntPtr handle) {
			return dialogs [handle];
		}

		public bool Visible { get; set; }

		IntPtr handle;
		public Dialog ()
		{
			PlatformServices.RequestDialogEvents ();
			dialog_create_alert (out handle);
			dialogs.Add (handle, this);
		}

		public Dialog (string title, string message) : this ()
		{
			Title = title;
			Message = message;
		}

		public static Dialog Show (string title, string message)
		{
			var a = new Dialog (title, message);
			a.Show ();
			return a;
		}

		public string Title {
			get {
				// TODO: implement dialog_get_title_text ()
				return "";
			}
			set {
				dialog_set_title_text (handle, value);
				if (Visible) {
					dialog_update (handle);
				}
			}
		}

		public string Message {
			get {
				// TODO: implement dialog_get_alert_message_text ()
				return "";
			}
			set {
				dialog_set_alert_message_text (handle, value);
				if (Visible) {
					dialog_update (handle);
				}
			}
		}

		public void Show ()
		{
			dialog_show (handle);
			Visible = true;
		}

		public void Dispose ()
		{
			dialog_destroy (handle);
			Visible = false;
			dialogs.Remove (handle);
		}
	}

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

	public class DialogEvent : Event
	{
		[DllImport ("bps")]
		static extern IntPtr dialog_event_get_dialog_instance (IntPtr handle);

		internal DialogEvent (IntPtr h) : base (h) {}

		public Dialog Dialog {
			get {
				return Dialog.FindDialog (dialog_event_get_dialog_instance (handle));
			}
		}
	}

	public class PlatformServices : IDisposable
	{
		[DllImport ("bps")]
		static extern int bps_initialize ();

		[DllImport ("bps")]
		static extern void bps_shutdown ();

		[DllImport ("bps")]
		static extern void bps_get_event (out IntPtr handle, int timeout_ms);

		[DllImport ("bps")]
		static extern int dialog_request_events (int flags);

		[DllImport ("bps")]
		static extern int dialog_get_domain ();

		[DllImport ("bps")]
		static extern int bps_event_get_domain (IntPtr handle);

		static bool initialized = false;
		static bool dialogEventsRequested = false;

		public PlatformServices ()
		{
			Initialize ();
		}

		public static void Initialize ()
		{
			if (initialized) {
				return;
			}

			bps_initialize ();
			initialized = true;
		}

		public static void RequestDialogEvents ()
		{
			if (dialogEventsRequested) {
				return;
			}

			Initialize ();
			dialog_request_events (0);
			dialogEventsRequested = true;
		}

		public Event NextEvent ()
		{
			return NextEvent (-1);
		}

		public Event NextEvent (int timeoutMillis)
		{
			IntPtr handle;
			bps_get_event (out handle, timeoutMillis);

			if (dialogEventsRequested && bps_event_get_domain (handle) == dialog_get_domain ()) {
				return new DialogEvent (handle);
			}

			return new Event (handle);
		}

		public void Dispose ()
		{
			bps_shutdown ();
		}
	}
}

