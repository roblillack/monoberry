using System;
using System.Runtime.InteropServices;
using InvocationHandle = System.IntPtr;

namespace BlackBerry
{
	public class InvokeRequest : IDisposable
	{
		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_create (ref InvocationHandle hndl);

		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_set_action (InvocationHandle hndl, string action);

		[DllImport ("bps")]
		static extern string navigator_invoke_invocation_get_action (InvocationHandle hndl);

		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_set_type (InvocationHandle hndl, string type);
		[DllImport ("bps")]
		static extern string navigator_invoke_invocation_get_type (InvocationHandle hndl);

		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_set_data (InvocationHandle hndl, byte[] data);

		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_set_metadata (InvocationHandle hndl, string metadata);

		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_set_uri (InvocationHandle hndl, string uri);

		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_set_target (InvocationHandle hndl, string target);

		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_set_source (InvocationHandle hndl, string source);

		[DllImport ("bps")]
		internal static extern int navigator_invoke_invocation_send (InvocationHandle hndl);

		[DllImport ("bps")]
		static extern int navigator_invoke_invocation_destroy (InvocationHandle hndl);

		internal InvocationHandle handle;
		
		public InvokeRequest ()
		{
			if (navigator_invoke_invocation_create (ref handle) != 0) {
				throw new InvalidOperationException ("Unable to create Invocation");
			}
		}

		internal InvokeRequest (InvocationHandle hndl)
		{
			handle = hndl;
		}

		public string Action {
			get {
				return navigator_invoke_invocation_get_action (handle);
			}
			set {
				navigator_invoke_invocation_set_action (handle, value);
			}
		}

		public string MimeType {
			get {
				return navigator_invoke_invocation_get_type (handle);
			}
			set {
				navigator_invoke_invocation_set_type (handle, value);
			}
		}

		public Uri Uri {
			set {
				navigator_invoke_invocation_set_uri (handle, value.AbsoluteUri);
			}
		}

		public string Target {
			set {
				navigator_invoke_invocation_set_target (handle, value);
			}
		}

		public string Source {
			set {
				navigator_invoke_invocation_set_source (handle, value);
			}
		}

		public byte[] Data {
			set {
				navigator_invoke_invocation_set_data (handle, value);
			}
		}

		public string Metadata {
			set {
				navigator_invoke_invocation_set_metadata (handle, value);
			}
		}


		public void Dispose ()
		{
			navigator_invoke_invocation_destroy (handle);
		}
	}

}
