using System;
using System.Runtime.InteropServices;
using InvocationHandle = System.IntPtr;
using EventHandle = System.IntPtr;

namespace BlackBerry
{
	public enum InvokeReplyError {
		NoError,
		UnknownError,
		NoTarget,
		BadRequest,
		Internal
	}

	internal class InvokeReplyErrorFactory
	{
		public static InvokeReplyError CreateError (string id)
		{
			switch (id) {
			case null:
			case "":
				return BlackBerry.InvokeReplyError.NoError;
			case "INVOKE_BAD_REQUEST_ERROR":
				return BlackBerry.InvokeReplyError.BadRequest;
			default:
				return BlackBerry.InvokeReplyError.UnknownError;
			}
		}
	}
}
