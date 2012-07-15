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

		internal static IntPtr NextDomainEvent (int domain)
		{
			while (true) {
				IntPtr handle;
				bps_get_event (out handle, -1);
				if (domain == bps_event_get_domain (handle)) {
					return handle;
				}
			}
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

