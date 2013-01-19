using System;

namespace BlackBerry.Screen
{
	[Flags]
	internal enum KeyFlags : uint {
		KEY_DOWN =				0x00000001,	/* Key was pressed down */
		KEY_REPEAT =			0x00000002,	/* Key was repeated */
		KEY_SCAN_VALID =		0x00000020,	/* Scancode is valid */
		KEY_SYM_VALID =			0x00000040,	/* Key symbol is valid */
		KEY_CAP_VALID =			0x00000080,	/* Key cap is valid */
		KEY_SYM_VALID_EX =		0x00000100,	/* Key symbol is valid extended */
		KEY_MAPPING_CHANGED =	0x20000000,
		KEY_DEAD =				0x40000000,	/* Key symbol is a DEAD key */
		KEY_OEM_CAP =			0x80000000	/* Key cap is an OEM scan code from keyboard */
	}
}

