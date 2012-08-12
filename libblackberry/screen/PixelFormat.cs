using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BlackBerry.Screen
{
	public enum PixelFormat : int {
		SCREEN_FORMAT_BYTE                     = 1,
        SCREEN_FORMAT_RGBA4444                 = 2,
        SCREEN_FORMAT_RGBX4444                 = 3,
        SCREEN_FORMAT_RGBA5551                 = 4,
        SCREEN_FORMAT_RGBX5551                 = 5,
        SCREEN_FORMAT_RGB565                   = 6,
        SCREEN_FORMAT_RGB888                   = 7,
        SCREEN_FORMAT_RGBA8888                 = 8,
        SCREEN_FORMAT_RGBX8888                 = 9,
        SCREEN_FORMAT_YVU9                     = 10,
        SCREEN_FORMAT_YUV420                   = 11,
        SCREEN_FORMAT_NV12                     = 12,
        SCREEN_FORMAT_YV12                     = 13,
        SCREEN_FORMAT_UYVY                     = 14,
        SCREEN_FORMAT_YUY2                     = 15,
        SCREEN_FORMAT_YVYU                     = 16,
        SCREEN_FORMAT_V422                     = 17,
        SCREEN_FORMAT_AYUV                     = 18
	}
}
