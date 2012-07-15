using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BlackBerry.Screen
{

	public class Buffer
	{
		[DllImport ("screen")]
		static extern int screen_fill (IntPtr ctx, IntPtr buf, [In] int[] attribs);

		Context context;
		internal IntPtr buffer;

		public Buffer (Context ctx, IntPtr buf)
		{
			context = ctx;
			buffer = buf;
		}

		public void Fill (int color)
		{
			var attribs = new int[] {
				(int)BlitAttribute.SCREEN_BLIT_COLOR, color, (int)BlitAttribute.SCREEN_BLIT_END
			};
			screen_fill (context.Handle, buffer, attribs);
		}
	}
	
}
