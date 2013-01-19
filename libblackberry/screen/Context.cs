using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BlackBerry.Screen
{

	public class Context : IDisposable
	{

		[DllImport ("screen")]
		static extern int screen_create_context (out IntPtr pctx, ContextType flags);

		[DllImport ("screen")]
		static extern int screen_destroy_context (IntPtr ctx);

		[DllImport ("bps")]
		static extern int screen_request_events (IntPtr ctx);

		[DllImport ("bps")]
		static extern int screen_stop_events (IntPtr ctx);

		[DllImport ("screen")]
		static extern int screen_flush_context (IntPtr ctx, Flushing flags);

		[DllImport ("screen")]
		static extern int screen_flush_blits (IntPtr ctx, Flushing flags);

		[DllImport ("screen")]
		static extern int screen_get_domain ();

		[DllImport ("screen")]
		static extern int screen_get_context_property_iv (IntPtr ctx, Property property, out int param);

		[DllImport ("screen")]
		static extern int screen_get_context_property_pv (IntPtr ctx, Property property, [Out] IntPtr[] param);

		IntPtr handle;
		public IntPtr Handle { get { return handle; } }
		int eventDomain;
		bool disposed = false;
		ContextType type;

		public Action<ScreenEvent> OnCloseWindow { get; set; }
		public Action<ScreenEvent> OnCreateWindow { get; set; }
		public Action<int,int> OnFingerTouch { get; set; }
		public Action<int,int> OnFingerMove { get; set; }
		public Action<int,int> OnFingerRelease { get; set; }

		private static IDictionary<ContextType, Context> instances = new Dictionary<ContextType, Context> ();
		IDictionary<IntPtr, Window> windows = new Dictionary<IntPtr, Window> ();

		public static Context GetInstance (ContextType type)
		{
			lock (typeof (Context)) {
				Context ctx;
				if (!instances.TryGetValue (type, out ctx)) {
					ctx = new Context (type);
					instances.Add (type, ctx);
				}
				return ctx;
			}
		}

		static void RemoveInstance (ContextType type)
		{
			lock (typeof (Context)) {
				if (instances.ContainsKey (type)) {
					instances.Remove (type);
				}
			}
		} 

		Context (ContextType type)
		{
			PlatformServices.Initialize ();
			this.type = type;
			if (screen_create_context (out handle, type) != 0) {
				// TODO: read errno to describe problem
				throw new Exception ("Unable to create screen context");
			}
			screen_request_events (handle);
			eventDomain = screen_get_domain ();
			PlatformServices.AddEventHandler (eventDomain, HandleEvent);
		}

		internal void RegisterWindow (Window win) {
			windows.Add (win.Handle, win);
		}

		internal void UnregisterWindow (Window win) {
			windows.Remove (win.Handle);
		}

		bool HandleWindowEvent (ScreenEvent ev) {
			var handle = ev.GetIntPtrProperty (Property.SCREEN_PROPERTY_WINDOW);
			if (!windows.ContainsKey (handle)) {
				return false;
			}
			windows [handle].HandleEvent (ev);
			return true;
		}

		void HandleEvent (IntPtr eventHandle)
		{
			var e = ScreenEvent.FromEventHandle (eventHandle);

			if (e.Type == EventType.SCREEN_EVENT_CLOSE ||
			    e.Type == EventType.SCREEN_EVENT_CREATE) {
				if (HandleWindowEvent (e) ) {
					return;
				}
			}

			switch (e.Type) {
			case EventType.SCREEN_EVENT_CLOSE:
				if (OnCloseWindow != null) {
					OnCloseWindow (e);
				}
				break;
			case EventType.SCREEN_EVENT_CREATE:
				if (OnCreateWindow != null) {
					OnCreateWindow (e);
				}
				break;
			case EventType.SCREEN_EVENT_MTOUCH_TOUCH:
				if (OnFingerTouch != null) {
					OnFingerTouch (e.X, e.Y);
				}
				break;
			case EventType.SCREEN_EVENT_MTOUCH_MOVE:
				if (OnFingerMove != null) {
					OnFingerMove (e.X, e.Y);
				}
				break;
			case EventType.SCREEN_EVENT_MTOUCH_RELEASE:
				if (OnFingerRelease != null) {
					OnFingerRelease (e.X, e.Y);
				}
				break;
			default:
				Console.WriteLine ("UNHANDLED SCREEN EVENT, TYPE: {0}", e.Type);
				if (e.Type == EventType.SCREEN_EVENT_PROPERTY) {
					Console.WriteLine (" - Name: {0}", (Property)e.GetIntProperty (Property.SCREEN_PROPERTY_NAME));
				} else if (e.Type == EventType.SCREEN_EVENT_DEVICE ||
				           e.Type == EventType.SCREEN_EVENT_GAMEPAD ||
				           e.Type == EventType.SCREEN_EVENT_JOYSTICK ||
				           e.Type == EventType.SCREEN_EVENT_POINTER) {
					Console.WriteLine (" - Buttons: {0}", e.GetIntProperty (Property.SCREEN_PROPERTY_BUTTONS));
				} else if (e.Type == EventType.SCREEN_EVENT_KEYBOARD) {
					Console.WriteLine (" - CAP: {0}", e.GetIntProperty (Property.SCREEN_PROPERTY_KEY_CAP));
					Console.WriteLine (" - FLAGS: {0}", e.GetIntProperty (Property.SCREEN_PROPERTY_KEY_FLAGS));
					Console.WriteLine (" - MODIFIERS: {0}", e.GetIntProperty (Property.SCREEN_PROPERTY_KEY_MODIFIERS));
					Console.WriteLine (" - SCAN: {0}", e.GetIntProperty (Property.SCREEN_PROPERTY_KEY_SCAN));
					Console.WriteLine (" - SYM: {0}", e.GetIntProperty (Property.SCREEN_PROPERTY_KEY_SYM));
				}

				break;
			}

		}

		public void FlushBlits ()
		{
			if (screen_flush_blits (handle, Flushing.SCREEN_WAIT_IDLE) != 0) {
				throw new Exception ("Unable to flush blits.");
			}
		}

		public void Flush ()
		{
			if (screen_flush_context (handle, 0) != 0) {
				throw new Exception ("Unable to flush context");
			}
		}

		public List<Display> Displays {
			get {
				int count;
				screen_get_context_property_iv (handle, Property.SCREEN_PROPERTY_DISPLAY_COUNT, out count);
				var handles = new IntPtr [count];
				screen_get_context_property_pv (handle, Property.SCREEN_PROPERTY_DISPLAYS, handles);
				var displays = new List<Display> ();
				foreach (var i in handles) {
					displays.Add (new Display (i));
				}
				return displays;
			}
		}

		public void Dispose ()
		{
			if (disposed) {
				return;
			}

			PlatformServices.RemoveEventHandler (eventDomain);
			screen_stop_events (handle);
			screen_destroy_context (handle);
			RemoveInstance (type);

			disposed = true;
		}
	}
	
}
