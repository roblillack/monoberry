using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

		[DllImport ("screen")]
		static extern int screen_get_buffer_property_iv (IntPtr buf, Property pname, out int param);
		
		[DllImport ("screen")]
		static extern int screen_set_buffer_property_iv (IntPtr buf, Property pname, ref int param);
		
		[DllImport ("screen")]
		static extern int screen_set_buffer_property_iv (IntPtr buf, Property pname, ref uint param);
		
		[DllImport ("screen")]
		static extern int screen_set_buffer_property_iv (IntPtr buf, Property pname, [In] int[] param);
		
		[DllImport ("screen")]
		static extern int screen_get_buffer_property_iv (IntPtr buf, Property pname, [Out] int[] param);
		
		[DllImport ("screen")]
		static extern int screen_get_buffer_property_pv (IntPtr buf, Property pname, [Out] IntPtr[] param);

		Context context;
		Window window;
		internal IntPtr handle;

		public Buffer (Context ctx, Window win, IntPtr hndl)
		{
			context = ctx;
			window = win;
			handle = hndl;
		}

		public void Fill (UInt32 color)
		{
			var attribs = new int[] {
				(int)BlitAttribute.SCREEN_BLIT_COLOR,
				(int)color,
				(int)BlitAttribute.SCREEN_BLIT_END
			};
			screen_fill (context.Handle, handle, attribs);
		}

		public void Blit (Buffer src, Rectangle rect)
		{
			Console.WriteLine ("Blitting {0}", rect);
			Blit (src, rect.X, rect.Y, rect.Width, rect.Height, rect.X, rect.Y);
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
			if (screen_blit (context.Handle, handle, src.handle, attribs) != 0) {
				throw new Exception ("Error blitting.");
			}
		}

		public void Render (Bitmap bitmap)
		{
			var rect = new Rectangle (0, 0, bitmap.Width, bitmap.Height);
			var format = window.PixelFormat.ToSDI ();
			var lockData = bitmap.LockBits (rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, format);
			var data = new byte [Length];
			Marshal.Copy (lockData.Scan0, data, 0, Length);
			Marshal.Copy (data, 0, Pointer, Length);
			bitmap.UnlockBits (lockData);
		}

		public Bitmap Bitmap {
			get {
				return new Bitmap (window.Width, window.Height, Stride, window.PixelFormat.ToSDI (), Pointer);
			}
		}

		/*public Graphics GraphicsContext {
			get {
				return new Graphics (Bitmap);
			}
		}*/

		public IntPtr Pointer {
			get {
				var ptrs = new IntPtr [1];
				screen_get_buffer_property_pv (handle, Property.SCREEN_PROPERTY_POINTER, ptrs);
				return ptrs [0];
			}
		}

		public int Stride {
			get {
				int result;
				if (screen_get_buffer_property_iv (handle, Property.SCREEN_PROPERTY_STRIDE, out result) != 0) {
					throw new Exception ("Unable to read buffer stride.");
				}
				return result;
			}
		}

		public int Length {
			get {
				int result;
				if (screen_get_buffer_property_iv (handle, Property.SCREEN_PROPERTY_SIZE, out result) != 0) {
					throw new Exception ("Unable to read buffer size.");
				}
				return result;
			}
		}
	}
	
}
