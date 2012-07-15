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
		static int eventDomain;

		static void Initialize ()
		{
			if (initialized) {
				return;
			}
			PlatformServices.Initialize ();
			dialog_request_events (0);
			eventDomain = dialog_get_domain ();
			PlatformServices.AddEventHandler (eventDomain, HandleEvent);
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

		public static void Alert (string title, string message, params Button[] buttons)
		{
			using (var a = new Dialog (title, message)) {
				foreach (var b in buttons) {
					a.AddButton (b);
				}
				a.Show ();
				HandleEvent (PlatformServices.NextDomainEvent (eventDomain));
			}
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
	
}
