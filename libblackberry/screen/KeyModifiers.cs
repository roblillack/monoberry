using System;

namespace BlackBerry.Screen
{
	enum Bits
	{
		KEYMODBIT_SHIFT = 0,
		KEYMODBIT_CTRL = 1,
		KEYMODBIT_ALT = 2,
		KEYMODBIT_ALTGR = 3,
		KEYMODBIT_SHL3 = 4,
		KEYMODBIT_MOD6 = 5,
		KEYMODBIT_MOD7 = 6,
		KEYMODBIT_MOD8 = 7,
		
		KEYMODBIT_SHIFT_LOCK = 8,
		KEYMODBIT_CTRL_LOCK = 9,
		KEYMODBIT_ALT_LOCK = 10,
		KEYMODBIT_ALTGR_LOCK = 11,
		KEYMODBIT_SHL3_LOCK = 12,
		KEYMODBIT_MOD6_LOCK = 13,
		KEYMODBIT_MOD7_LOCK = 14,
		KEYMODBIT_MOD8_LOCK = 15,
		
		KEYMODBIT_CAPS_LOCK = 16,
		KEYMODBIT_NUM_LOCK = 17,
		KEYMODBIT_SCROLL_LOCK = 18
	}

	[Flags]
	public enum KeyModifiers : uint
	{
		Shift = (1 << Bits.KEYMODBIT_SHIFT),
		Ctrl = (1 << Bits.KEYMODBIT_CTRL),
		Alt = (1 << Bits.KEYMODBIT_ALT),
		AltGr = (1 << Bits.KEYMODBIT_ALTGR),
		ShL3 = (1 << Bits.KEYMODBIT_SHL3),
		Mod6 = (1 << Bits.KEYMODBIT_MOD6),
		Mod7 = (1 << Bits.KEYMODBIT_MOD7),
		Mod8 = (1 << Bits.KEYMODBIT_MOD8),
				
		ShiftLock = (1 << Bits.KEYMODBIT_SHIFT_LOCK),
		CtrlLock = (1 << Bits.KEYMODBIT_CTRL_LOCK),
		AltLock = (1 << Bits.KEYMODBIT_ALT_LOCK),
		AltGrLock = (1 << Bits.KEYMODBIT_ALTGR_LOCK),
		ShL3Lock = (1 << Bits.KEYMODBIT_SHL3_LOCK),
		Mod6Lock = (1 << Bits.KEYMODBIT_MOD6_LOCK),
		Mod7Lock = (1 << Bits.KEYMODBIT_MOD7_LOCK),
		Mod8Lock = (1 << Bits.KEYMODBIT_MOD8_LOCK),
				
		CapsLock = (1 << Bits.KEYMODBIT_CAPS_LOCK),
		NumLock = (1 << Bits.KEYMODBIT_NUM_LOCK),
		ScrollLock = (1 << Bits.KEYMODBIT_SCROLL_LOCK)
	}
}

