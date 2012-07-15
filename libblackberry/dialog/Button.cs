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
	
}
