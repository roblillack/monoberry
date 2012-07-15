using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BlackBerry.Screen
{
	[Flags]
	public enum ContextType : int {
		SCREEN_APPLICATION_CONTEXT = 0,
		SCREEN_WINDOW_MANAGER_CONTEXT = (1 << 0),
		SCREEN_INPUT_PROVIDER_CONTEXT = (1 << 1),  
		SCREEN_POWER_MANAGER_CONTEXT = (1 << 2), 
		SCREEN_DISPLAY_MANAGER_CONTEXT = (1 << 3)
	}

	public enum Property : int {
		SCREEN_PROPERTY_BUFFER_SIZE = 5,
		SCREEN_PROPERTY_EGL_HANDLE = 12,
		SCREEN_PROPERTY_FORMAT = 14,
		SCREEN_PROPERTY_INTERLACED = 22,
		SCREEN_PROPERTY_PHYSICALLY_CONTIGUOUS = 32,
		SCREEN_PROPERTY_PLANAR_OFFSETS = 33,
		SCREEN_PROPERTY_POINTER = 34,
		SCREEN_PROPERTY_PROTECTED = 36,
		SCREEN_PROPERTY_STRIDE = 44,
		SCREEN_PROPERTY_PHYSICAL_ADDRESS = 55,
		SCREEN_PROPERTY_NATIVE_IMAGE = 113,
		SCREEN_PROPERTY_DISPLAY_COUNT = 59,
		SCREEN_PROPERTY_DISPLAYS = 60,
		SCREEN_PROPERTY_DEVICE_COUNT = 98,
		SCREEN_PROPERTY_DEVICES = 101,
		SCREEN_PROPERTY_GROUP_COUNT = 104,
		SCREEN_PROPERTY_GROUPS = 105,
		SCREEN_PROPERTY_PIXMAP_COUNT = 106,
		SCREEN_PROPERTY_PIXMAPS = 107,
		SCREEN_PROPERTY_WINDOW_COUNT = 108,
		SCREEN_PROPERTY_WINDOWS = 109,
		SCREEN_PROPERTY_KEYMAP = 110,
		SCREEN_PROPERTY_DISPLAY = 11,
		SCREEN_PROPERTY_ID_STRING = 20,
		SCREEN_PROPERTY_TYPE = 47,
		SCREEN_PROPERTY_USER_DATA = 49,
		SCREEN_PROPERTY_WINDOW = 52,
		SCREEN_PROPERTY_POWER_MODE = 88,
		SCREEN_PROPERTY_CONTEXT = 95,
		SCREEN_PROPERTY_GAMMA = 2,
		SCREEN_PROPERTY_ROTATION = 38,
		SCREEN_PROPERTY_SIZE = 40,
		SCREEN_PROPERTY_TRANSPARENCY = 46,
		SCREEN_PROPERTY_MIRROR_MODE = 58,
		SCREEN_PROPERTY_ATTACHED = 64,
		SCREEN_PROPERTY_DETACHABLE = 65,
		SCREEN_PROPERTY_NATIVE_RESOLUTION = 66,
		SCREEN_PROPERTY_PROTECTION_ENABLE = 67,
		SCREEN_PROPERTY_PHYSICAL_SIZE = 69,
		SCREEN_PROPERTY_FORMAT_COUNT = 70,
		SCREEN_PROPERTY_FORMATS = 71,
		SCREEN_PROPERTY_VIEWPORT_POSITION = 74,
		SCREEN_PROPERTY_VIEWPORT_SIZE = 75,
		SCREEN_PROPERTY_IDLE_STATE = 81,
		SCREEN_PROPERTY_KEEP_AWAKES = 82,
		SCREEN_PROPERTY_IDLE_TIMEOUT = 83,
		SCREEN_PROPERTY_KEYBOARD_FOCUS = 84,
		SCREEN_PROPERTY_MTOUCH_FOCUS = 85,
		SCREEN_PROPERTY_POINTER_FOCUS = 86,
		SCREEN_PROPERTY_ID = 87,
		SCREEN_PROPERTY_MODE_COUNT = 89,
		SCREEN_PROPERTY_MODE = 90,
		SCREEN_PROPERTY_BUTTONS = 6,
		SCREEN_PROPERTY_DEVICE = 10,
		SCREEN_PROPERTY_DEVICE_INDEX = 10,
		SCREEN_PROPERTY_GROUP = 18,
		SCREEN_PROPERTY_INPUT_VALUE = 21,
		SCREEN_PROPERTY_JOG_COUNT = 23,
		SCREEN_PROPERTY_KEY_CAP = 24,
		SCREEN_PROPERTY_KEY_FLAGS = 25,
		SCREEN_PROPERTY_KEY_MODIFIERS = 26,
		SCREEN_PROPERTY_KEY_SCAN = 27,
		SCREEN_PROPERTY_KEY_SYM = 28,
		SCREEN_PROPERTY_NAME = 30,
		SCREEN_PROPERTY_POSITION = 35,
		SCREEN_PROPERTY_SOURCE_POSITION = 41,
		SCREEN_PROPERTY_SOURCE_SIZE = 42,
		SCREEN_PROPERTY_EFFECT = 62,
		SCREEN_PROPERTY_TOUCH_ID = 73,
		SCREEN_PROPERTY_TOUCH_ORIENTATION = 76,
		SCREEN_PROPERTY_TOUCH_PRESSURE = 77,
		SCREEN_PROPERTY_TIMESTAMP = 78,
		SCREEN_PROPERTY_SEQUENCE_ID = 79,
		SCREEN_PROPERTY_MOUSE_WHEEL = 94,
		SCREEN_PROPERTY_OBJECT_TYPE = 100,
		SCREEN_PROPERTY_MOUSE_HORIZONTAL_WHEEL = 111,
		SCREEN_PROPERTY_TOUCH_TYPE = 112,
		SCREEN_PROPERTY_USER_HANDLE = 50,
		SCREEN_PROPERTY_BUFFER_POOL = 99,
		SCREEN_PROPERTY_ALPHA_MODE = 1,
		SCREEN_PROPERTY_COLOR_SPACE = 8,
		SCREEN_PROPERTY_RENDER_BUFFERS = 37,
		SCREEN_PROPERTY_USAGE = 48,
		SCREEN_PROPERTY_BRIGHTNESS = 3,
		SCREEN_PROPERTY_BUFFER_COUNT = 4,
		SCREEN_PROPERTY_CLASS = 7,
		SCREEN_PROPERTY_CONTRAST = 9,
		SCREEN_PROPERTY_FLIP = 13,
		SCREEN_PROPERTY_FRONT_BUFFER = 15,
		SCREEN_PROPERTY_GLOBAL_ALPHA = 16,
		SCREEN_PROPERTY_PIPELINE = 17,
		SCREEN_PROPERTY_HUE = 19,
		SCREEN_PROPERTY_MIRROR = 29,
		SCREEN_PROPERTY_OWNER_PID = 31,
		SCREEN_PROPERTY_SATURATION = 39,
		SCREEN_PROPERTY_STATIC = 43,
		SCREEN_PROPERTY_SWAP_INTERVAL = 45,
		SCREEN_PROPERTY_VISIBLE = 51,
		SCREEN_PROPERTY_RENDER_BUFFER_COUNT = 53,
		SCREEN_PROPERTY_ZORDER = 54,
		SCREEN_PROPERTY_SCALE_QUALITY = 56,
		SCREEN_PROPERTY_SENSITIVITY = 57,
		SCREEN_PROPERTY_CBABC_MODE = 61,
		SCREEN_PROPERTY_FLOATING = 63,
		SCREEN_PROPERTY_SOURCE_CLIP_POSITION = 68,
		SCREEN_PROPERTY_SOURCE_CLIP_SIZE = 72,
		SCREEN_PROPERTY_IDLE_MODE = 80,
		SCREEN_PROPERTY_CLIP_POSITION = 91,
		SCREEN_PROPERTY_CLIP_SIZE = 92,
		SCREEN_PROPERTY_COLOR = 93,
		SCREEN_PROPERTY_DEBUG = 96,
		SCREEN_PROPERTY_ALTERNATE_WINDOW = 97,
		SCREEN_PROPERTY_SELF_LAYOUT = 103
	}

	[Flags]
	public enum Usage : int {
		SCREEN_USAGE_DISPLAY = (1 << 0),
		SCREEN_USAGE_READ = (1 << 1),
		SCREEN_USAGE_WRITE = (1 << 2),
		SCREEN_USAGE_NATIVE = (1 << 3),
		SCREEN_USAGE_OPENGL_ES1 = (1 << 4),
		SCREEN_USAGE_OPENGL_ES2 = (1 << 5),
		SCREEN_USAGE_OPENVG = (1 << 6),
		SCREEN_USAGE_VIDEO = (1 << 7),
		SCREEN_USAGE_CAPTURE = (1 << 8),
		SCREEN_USAGE_ROTATION = (1 << 9),
		SCREEN_USAGE_OVERLAY = (1 << 10)
	}

	public enum WindowType : int {
		SCREEN_APPLICATION_WINDOW = 0,
		SCREEN_CHILD_WINDOW = 1,
		SCREEN_EMBEDDED_WINDOW = 2,
		SCREEN_ROOT_WINDOW = 3
	}

	public enum BlitAttribute : int {
		SCREEN_BLIT_END = 0,
		SCREEN_BLIT_SOURCE_X = 1,
		SCREEN_BLIT_SOURCE_Y = 2,
		SCREEN_BLIT_SOURCE_WIDTH = 3,
		SCREEN_BLIT_SOURCE_HEIGHT = 4,
		SCREEN_BLIT_DESTINATION_X = 5,
		SCREEN_BLIT_DESTINATION_Y = 6,
		SCREEN_BLIT_DESTINATION_WIDTH = 7,
		SCREEN_BLIT_DESTINATION_HEIGHT = 8,
		SCREEN_BLIT_GLOBAL_ALPHA = 9,
		SCREEN_BLIT_TRANSPARENCY = 10,
		SCREEN_BLIT_SCALE_QUALITY = 11,
		SCREEN_BLIT_COLOR = 12
	}

	[Flags]
	public enum Flushing : int {
		SCREEN_WAIT_IDLE = (1 << 0),
		SCREEN_PROTECTED = (1 << 1),
		SCREEN_DONT_FLUSH = (1 << 2),
		SCREEN_POST_RESUME = (1 << 3)   
	}

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

	public class Window : IDisposable
	{
		[DllImport ("screen")]
		static extern int screen_create_window_type (out IntPtr pwin, IntPtr ctx, WindowType type);

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

		public int GetIntProperty (Property p)
		{
			var result = new int[] { 0 };
			screen_get_window_property_iv (handle, p, result);
			return result [0];
		}

		public void SetIntProperty (Property p, int val)
		{
			screen_set_window_property_iv (handle, Property.SCREEN_PROPERTY_USAGE, ref val);
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

		public Window (Context ctx, WindowType type)
		{
			context = ctx;
			if (screen_create_window_type (out handle, ctx.Handle, type) != 0) {
				// TODO: read errno
				throw new Exception ("Unable to create window");
			}
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

