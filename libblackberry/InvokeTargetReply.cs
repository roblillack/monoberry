using System;
using System.Runtime.InteropServices;
using InvocationHandle = System.IntPtr;
using EventHandle = System.IntPtr;

namespace BlackBerry
{
	public class InvokeTargetReply : Event
	{
		[DllImport ("bps")]
		static extern InvocationHandle navigator_invoke_event_get_invocation (EventHandle handle);

		internal InvokeTargetReply (EventHandle hndl) : base (hndl) {}

		public InvokeRequest Invocation {
			get {
				var hndl = navigator_invoke_event_get_invocation (handle);
				return hndl == InvocationHandle.Zero ? null : new InvokeRequest (hndl);
			}
		}

		public InvokeReplyError Error {
			get {
				return InvokeReplyErrorFactory.CreateError (ErrorMessage);
			}
		}
	}
}

