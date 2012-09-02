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

		IntPtr handle;
		public IntPtr Handle { get { return handle; } }
		int eventDomain;
		bool disposed = false;
		ContextType type;

		public Action<Window> OnCloseWindow { get; set; }
		public Action<Window> OnCreateWindow { get; set; }
		public Action<int,int> OnFingerTouch { get; set; }
		public Action<int,int> OnFingerMove { get; set; }
		public Action<int,int> OnFingerRelease { get; set; }

		private static IDictionary<ContextType, Context> instances = new Dictionary<ContextType, Context> ();

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

		void HandleEvent (IntPtr eventHandle)
		{
			var e = ScreenEvent.FromEventHandle (eventHandle);

			switch (e.Type) {
			case EventType.SCREEN_EVENT_CLOSE:
				if (OnCloseWindow != null) {
					OnCloseWindow (new Window (this, e.GetIntPtrProperty (Property.SCREEN_PROPERTY_WINDOW)));
				}
				break;
			case EventType.SCREEN_EVENT_CREATE:
				if (OnCreateWindow != null) {
					OnCreateWindow (new Window (this, e.GetIntPtrProperty (Property.SCREEN_PROPERTY_WINDOW)));
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
