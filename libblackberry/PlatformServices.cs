using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class Button {
		[DllImport ("bps")]
		static extern int dialog_update_button (IntPtr dialog, int index, string label, bool enabled, string id, bool visible);

		internal string id = Guid.NewGuid ().ToString ();
		internal bool enabled = true;
		internal string label = null;
		internal bool visible = true;
		public Action OnClick { get; set; }
		public Dialog Dialog { get; internal set; }

		public bool IsEnabled {
			get {
				return enabled;
			}
			set {
				enabled = value;
				if (Dialog != null) {
					dialog_update_button (Dialog.handle, Index, null, enabled, null, visible);
				}
			}
		}

		public bool IsVisible {
			get {
				return visible;
			}
			set {
				visible = value;
				if (Dialog != null) {
					dialog_update_button (Dialog.handle, Index, null, enabled, null, visible);
				}
			}
		}

		public string Label {
			get {
				return label;
			}
			set {
				label = value;
				if (Dialog != null) {
					dialog_update_button (Dialog.handle, Index, label, enabled, null, visible);
				}
			}
		}

		public int Index {
			get {
				for (int i = 0; i < Dialog.buttons.Count; i++) {
					if (this == Dialog.buttons [i]) {
						return i;
					}
				}
				return -1;
			}
		}

		public Button (string lbl)
		{
			label = lbl;
		}

		public Button (string lbl, Action onClick) : this (lbl)
		{
			OnClick = onClick;
		}
	}

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

		[DllImport ("bps")]
		static extern int dialog_add_button (IntPtr dialog, string label, bool enabled, string id, bool visible);

		[DllImport ("bps")]
		static extern int dialog_request_events (int flags);

		[DllImport ("bps")]
		static extern int dialog_get_domain ();

		[DllImport ("bps")]
		static extern IntPtr dialog_event_get_dialog_instance (IntPtr handle);

		[DllImport ("bps")]
		static extern int dialog_event_get_selected_index(IntPtr handle);

		static IDictionary<IntPtr, Dialog> dialogs = new Dictionary<IntPtr, Dialog> ();
		static bool initialized = false;

		static void Initialize ()
		{
			if (initialized) {
				return;
			}
			PlatformServices.Initialize ();
			dialog_request_events (0);
			PlatformServices.AddEventHandler (dialog_get_domain (), HandleEvent);
			initialized = true;
		}

		static void HandleEvent (IntPtr eventHandle)
		{
			IntPtr dialogHandle = dialog_event_get_dialog_instance (eventHandle);
			int btnIndex = dialog_event_get_selected_index (eventHandle);
			if (!dialogs.ContainsKey (dialogHandle)) {
				throw new ArgumentException ("Dialog not found.");
			}

			var dlg = dialogs [dialogHandle];
			dlg.Visible = false;
			if (btnIndex < 0 || btnIndex >= dlg.buttons.Count) {
				throw new ArgumentException ("Button not found.");
			}

			var btn = dlg.buttons [btnIndex];
			if (btn.OnClick != null) {
				btn.OnClick ();
			}
		}
			                                 

		public bool Visible { get; set; }

		internal IntPtr handle;
		public Dialog ()
		{
			Initialize ();
			dialog_create_alert (out handle);
			dialogs.Add (handle, this);
		}

		internal IList<Button> buttons = new List<Button> ();

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

		public void AddButton (Button button) {
			buttons.Add (button);
			button.Dialog = this;
			dialog_add_button (handle, button.label, button.enabled, button.id, button.visible);
			if (Visible) {
				dialog_update (handle);
			}
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
			dialogs.Remove (handle);
			Visible = false;
			dialog_destroy (handle);
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

	public class PlatformServices
	{
		[DllImport ("bps")]
		static extern int bps_initialize ();

		[DllImport ("bps")]
		static extern void bps_shutdown ();

		[DllImport ("bps")]
		static extern void bps_get_event (out IntPtr handle, int timeout_ms);

		[DllImport ("bps")]
		static extern int bps_event_get_domain (IntPtr handle);

		static bool initialized = false;
		public static bool IsRunning {
			get;
			private set;
		}

		static IDictionary<int, Action<IntPtr>> eventHandlers = new Dictionary<int, Action<IntPtr>> ();

		public static void Initialize ()
		{
			if (initialized) {
				return;
			}

			bps_initialize ();
			initialized = true;
			IsRunning = true;
		}

		public static void AddEventHandler (int domain, Action<IntPtr> handler)
		{
			eventHandlers.Add (domain, handler);
		}

		public static Event NextEvent ()
		{
			return NextEvent (-1);
		}

		public static Event NextEvent (int timeoutMillis)
		{
			IntPtr handle;
			Action<IntPtr> handler = null;

			IsRunning = true;

			while (IsRunning) {
				bps_get_event (out handle, timeoutMillis);
				if (handle == IntPtr.Zero) {
					break;
				}
				var domain = bps_event_get_domain (handle);
				if (!eventHandlers.ContainsKey (domain)) {
					return new Event (handle);
				}
				handler = eventHandlers [domain];
				handler (handle);
			}

			return null;
		}

		public static void Run ()
		{
			while (NextEvent () != null) {}
		}

		public static void Stop ()
		{
			IsRunning = false;
		}

		public static void Shutdown (int code = 0)
		{
			bps_shutdown ();
			System.Environment.Exit (code);
		}
	}
}

