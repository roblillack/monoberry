using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BlackBerry.Screen
{

	public class Window : IDisposable
	{
		[DllImport ("screen")]
		static extern int screen_create_window_type (out IntPtr pwin, IntPtr ctx, WindowType type);

		[DllImport ("screen")]
		static extern int screen_create_window_group (IntPtr win, string name);

		[DllImport ("screen")]
		static extern int screen_destroy_window (IntPtr win);

		[DllImport ("screen")]
		static extern int screen_set_window_property_iv (IntPtr win, Property pname, ref int param);

		[DllImport ("screen")]
		static extern int screen_get_window_property_iv (IntPtr win, Property pname, [Out] int[] param);

		[DllImport ("screen")]
		static extern int screen_get_window_property_pv (IntPtr win, Property pname, [Out] IntPtr[] param);

		[DllImport ("screen")]
		static extern int screen_create_window_buffers (IntPtr win, int count);

		[DllImport ("screen")]
		static extern int screen_set_window_property_cv (IntPtr win, Property pname, int len, byte[] param);

		[DllImport ("screen")]
		static extern int screen_post_window (IntPtr win, IntPtr buffer, int rect_count, [In] int[] dirty_rects, Flushing flushing);

		Context context;
		IntPtr handle;
		public IntPtr Handle { get { return handle; } }

		public Usage Usage {
			get {
				return (Usage)GetIntProperty (Property.SCREEN_PROPERTY_USAGE);
			}

			set {
				SetIntProperty (Property.SCREEN_PROPERTY_USAGE, (int)value);
			}
		}

		public bool IsVisible {
			get {
				return GetIntProperty (Property.SCREEN_PROPERTY_VISIBLE) == 1;
			}
			set {
				SetIntProperty (Property.SCREEN_PROPERTY_VISIBLE, value ? 1 : 0);
			}
		}

		public int GetIntProperty (Property p)
		{
			var result = new int[] { 0 };
			if (screen_get_window_property_iv (handle, p, result) != 0) {
				throw new Exception ("Unable to read window property " + p);
			}
			return result [0];
		}

		public void SetIntProperty (Property p, int val)
		{
			if (screen_set_window_property_iv (handle, p, ref val) != 0) {
				throw new Exception ("Unable to set window property " + p);
			}
		}

		public void AddBuffers (int count)
		{
			screen_create_window_buffers (handle, count);
		}

		public void AddBuffer ()
		{
			AddBuffers (1);
		}

		public List<Buffer> Buffers {
			get {
				int count = GetIntProperty (Property.SCREEN_PROPERTY_RENDER_BUFFER_COUNT);
				if (count == 0) {
					return new List<Buffer> ();
				}

				var bufs = new IntPtr[count];
				screen_get_window_property_pv (handle, Property.SCREEN_PROPERTY_RENDER_BUFFERS, bufs);
				var list = new List<Buffer>();
				foreach (var i in bufs) {
					list.Add (new Buffer (context, i));
				}
				return list;
			}
		}

		public int Width {
			get {
				var rect = new int [2];
				screen_get_window_property_iv (handle, Property.SCREEN_PROPERTY_BUFFER_SIZE, rect);
				return rect [0];
			}
		}

		public int Height {
			get {
				var rect = new int [2];
				screen_get_window_property_iv (handle, Property.SCREEN_PROPERTY_BUFFER_SIZE, rect);
				return rect [1];
			}
		}

		public string Identifier {
			set {
				byte[] chars = Encoding.UTF8.GetBytes (value);
				screen_set_window_property_cv(handle, Property.SCREEN_PROPERTY_ID_STRING, chars.Length, chars);
			}
		}

		public void Render (Buffer buffer)
		{
			var dirty = new int[] { 0, 0, Width, Height };
			screen_post_window (handle, buffer.buffer, 1, dirty, Flushing.SCREEN_WAIT_IDLE);
		}

		public Window (Context ctx, WindowType type = WindowType.SCREEN_APPLICATION_WINDOW)
		{
			context = ctx;
			if (screen_create_window_type (out handle, ctx.Handle, type) != 0) {
				throw new Exception ("Unable to create window");
			}
			if (type != WindowType.SCREEN_APPLICATION_WINDOW &&
			    type != WindowType.SCREEN_CHILD_WINDOW) {
				return;
			}
			if (screen_create_window_group (handle, handle.ToString ()) != 0) {
				throw new Exception ("Unable to create window group");
			}
		}

		public Window (Context ctx, IntPtr hnd)
		{
			context = ctx;
			handle = hnd;
		}

		public void Dispose ()
		{
			if (screen_destroy_window (handle) != 0) {
				// TODO: read errno
				throw new Exception ("Unable to destroy window");
			}
		}

	}
}
