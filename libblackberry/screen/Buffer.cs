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

		[DllImport ("screen")]
		static extern int screen_blit (IntPtr ctx, IntPtr dst, IntPtr src, [In] int[] attribs);

		Context context;
		internal IntPtr buffer;

		public Buffer (Context ctx, IntPtr buf)
		{
			context = ctx;
			buffer = buf;
		}

		public void Fill (UInt32 color)
		{
			var attribs = new int[] {
				(int)BlitAttribute.SCREEN_BLIT_COLOR,
				(int)color,
				(int)BlitAttribute.SCREEN_BLIT_END
			};
			screen_fill (context.Handle, buffer, attribs);
		}

		public void Blit (Buffer src, int srcX, int srcY, int width, int height, int dstX, int dstY)
		{
			var attribs = new int[] {
				(int)BlitAttribute.SCREEN_BLIT_SOURCE_X,
				srcX,
				(int)BlitAttribute.SCREEN_BLIT_SOURCE_Y,
				srcY,
				(int)BlitAttribute.SCREEN_BLIT_SOURCE_WIDTH,
				width,
				(int)BlitAttribute.SCREEN_BLIT_SOURCE_HEIGHT,
				height,
				(int)BlitAttribute.SCREEN_BLIT_DESTINATION_X,
				dstX,
				(int)BlitAttribute.SCREEN_BLIT_DESTINATION_Y,
				dstY,
				(int)BlitAttribute.SCREEN_BLIT_DESTINATION_WIDTH,
				width,
				(int)BlitAttribute.SCREEN_BLIT_DESTINATION_HEIGHT,
				height,
				(int)BlitAttribute.SCREEN_BLIT_END
			};
			if (screen_blit (context.Handle, buffer, src.buffer, attribs) != 0) {
				throw new Exception ("Error blitting.");
			}
		}
	}
	
}
