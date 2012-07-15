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

		IntPtr handle;
		public IntPtr Handle { get { return handle; } }

		public Context()
		{
			if (screen_create_context (out handle, ContextType.SCREEN_APPLICATION_CONTEXT) != 0) {
				// TODO: read errno
				throw new Exception ("Unable to create screen context");
			}
		}

		public void Dispose ()
		{
			if (screen_destroy_context (handle) != 0) {
				// TODO: read errno
				throw new Exception ("Unable to destroy screen context");
			}
		}
	}
	
}
