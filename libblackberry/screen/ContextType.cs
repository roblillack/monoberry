using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BlackBerry.Screen
{
	[Flags]
	public enum ContextType : int {
		Application = 0,
		WindowManager = (1 << 0),
		InputProvider = (1 << 1),  
		PowerManager = (1 << 2), 
		DisplayManager = (1 << 3)
	}

}
