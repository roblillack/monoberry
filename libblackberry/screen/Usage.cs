using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BlackBerry.Screen
{

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

}
