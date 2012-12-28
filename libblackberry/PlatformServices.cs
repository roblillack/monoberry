using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BlackBerry
{
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

		public static void RemoveEventHandler (int domain)
		{
			eventHandlers.Remove (domain);
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
					if (timeoutMillis < 0) {
						continue;
					} else {
						break;
					}
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
				var evDomain = bps_event_get_domain (handle);
				if (eventHandlers.ContainsKey (evDomain)) {
					eventHandlers [evDomain] (handle);
				}
				if (domain == evDomain) {
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

