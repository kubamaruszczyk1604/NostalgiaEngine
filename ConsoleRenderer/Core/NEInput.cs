using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace NostalgiaEngine.Core
{

    public enum NEButton : int
    {
        LeftMouseButton = 0x01, RightMouseButton = 0x02, CancelMouse = 0x03, MiddleMouseButton = 0x04,
        X1MouseButton = 0x05, X2MouseButton = 0x06
    }

    public enum NEKey : int
    {

        Backspace = 0x08, Tab = 0x09, Clear = 0x0C,
        Enter = 0x0D, Shift = 0x10, Control = 0x11,
        Alt = 0x12, Pause = 0x13, CapsLock = 0x14,
        Escape = 0x1B, Space = 0x20, PageUp = 0x21,
        PageDown = 0x22, End = 0x23, Home = 0x24,
        LeftArrow = 0x25, UpArrow = 0x26, RightArrow = 0x27,
        DownArrow = 0x28, Select = 0x29, Print = 0x2A,
        Execute = 0x2B, PrintScreen = 0x2C, Insert = 0x2D,
        Delete = 0x2E, Help = 0x2F,

        IMEKana = 0x15, IMEHagul = 0x15, IMEJunja = 0x17, IMEFinal = 0x18, IMEHanja = 0x19, IMEKanji = 0x19,
        IMEConvert = 0x1C, IMENoConvert = 0x1D, IMEAccept = 0x1E, IMEModeChange = 0x1F,


        Key_0 = 0x30, Key_1 = 0x31, Key_2 = 0x32, Key_3 = 0x33, Key_4 = 0x34, Key_5 = 0x35, Key_6 = 0x36,
        Key_7 = 0x37, Key_8 = 0x38, Key_9 = 0x39, Key_A = 0x41, Key_B = 0x42, Key_C = 0x43, Key_D = 0x44,
        Key_E = 0x45, Key_F = 0x46, Key_G = 0x47, Key_H = 0x48, Key_I = 0x49, Key_J = 0x4A, Key_K = 0x4B,
        Key_L = 0x4C, Key_M = 0x4D, Key_N = 0x4E, Key_O = 0x4F, Key_P = 0x50, Key_Q = 0x51, Key_R = 0x52,
        Key_S = 0x53, Key_T = 0x54, Key_U = 0x55, Key_V = 0x56, Key_W = 0x57, Key_X = 0x58, Key_Y = 0x59,
        Key_Z = 0x5A,

        LeftWindowsKey = 0x5B, RightWindowsKey = 0x5C, ApplicationsKey = 0x5D, Sleep = 0x5F,
        NumPad0 = 0x60, NumPad1 = 0x61, NumPad2 = 0x62, NumPad3 = 0x63, NumPad4 = 0x64,
        NumPad5 = 0x65, NumPad6 = 0x66, NumPad7 = 0x67, NumPad8 = 0x68, NumPad9 = 0x69,

        Multiply = 0x6A, Plus = 0x6B, Separator = 0x6C,
        Subtract = 0x6D, Decimal = 0x6E, Divide = 0x6F,

        F1 = 0x70, F2 = 0x71, F3 = 0x72, F4 = 0x73,F5 = 0x74, F6 = 0x75, F7 = 0x76, F8 = 0x77,
        F9 = 0x78, F10 = 0x79, F11 = 0x7A, F12 = 0x7B, F13 = 0x7C, F14 = 0x7D, F15 = 0x7E, F16 = 0x7F,
        F17 = 0x80, F18 = 0x81, F19 = 0x82, F20 = 0x83, F21 = 0x84, F22 = 0x85, F23 = 0x86, F24 = 0x87,

        NumLock = 0x90, Scroll = 0x91,
        LeftShift = 0xA0, RightShift = 0xA1,
        LeftControl = 0xA2, RightControl = 0xA3,
        LeftMenu = 0xA4, RightMenu = 0xA5,

        VolumeMute = 0xAD,
        VolumeDown = 0xAE,
        VolumeUP = 0xAF,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        OEM_1 = 0xBA,
        ///<summary>
        ///Windows 2000/XP: For any country/region, the '+' key
        ///</summary>
        OEM_PLUS = 0xBB,
        OEM_COMMA = 0xBC,
        OEM_MINUS = 0xBD,
        OEM_PERIOD = 0xBE,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        OEM_2 = 0xBF,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        OEM_3 = 0xC0,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        OEM_4 = 0xDB,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        OEM_5 = 0xDC,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        OEM_6 = 0xDD,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        OEM_7 = 0xDE,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        OEM_8 = 0xDF,
        ///<summary>
        ///Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
        ///</summary>
        OEM_102 = 0xE2,
        ///<summary>
        ///Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
        ///</summary>
        PROCESSKEY = 0xE5,
        ///<summary>
        ///Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
        ///</summary>
        PACKET = 0xE7,
        ///<summary>
        ///Attn key
        ///</summary>
        ATTN = 0xF6,
        ///<summary>
        ///CrSel key
        ///</summary>
        CRSEL = 0xF7,
        ///<summary>
        ///ExSel key
        ///</summary>
        EXSEL = 0xF8,
        ///<summary>
        ///Erase EOF key
        ///</summary>
        EREOF = 0xF9,
        ///<summary>
        ///Play key
        ///</summary>
        PLAY = 0xFA,
        ///<summary>
        ///Zoom key
        ///</summary>
        ZOOM = 0xFB,
        ///<summary>
        ///Reserved
        ///</summary>
        NONAME = 0xFC,
        ///<summary>
        ///PA1 key
        ///</summary>
        PA1 = 0xFD,
        ///<summary>
        ///Clear key
        ///</summary>
        OEM_CLEAR = 0xFE
    }


    public class NEInput
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(
         [MarshalAs(UnmanagedType.U4)] int vKey);


        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref NEPoint lpPoint);



        private static NEPoint c_MousePosition = new NEPoint();

        public static bool CheckKeyDown(ConsoleKey key)
        {
            return ((GetAsyncKeyState((int)key) << 16) !=0);
        }

        public static bool CheckKeyDown(NEKey key)
        {
            return ((GetAsyncKeyState((int)key) << 16) != 0);
        }

        public static bool CheckKeyDown(NEButton button)
        {
            return ((GetAsyncKeyState((int)button) << 16) != 0);
        }

        public static bool CheckKeyPress(ConsoleKey key)
        {
            int output = GetAsyncKeyState((int)key);
            return (((output << 16) != 0) && ((output & 1) != 0));
        }

        public static bool CheckKeyPress(NEKey key)
        {
            int output = GetAsyncKeyState((int)key);
            return (((output << 16) != 0) && ((output & 1) != 0));
        }

        public static bool CheckKeyPress(NEButton button)
        {
            int output = GetAsyncKeyState((int)button);
            return (((output << 16) != 0) && ((output & 1) != 0));
        }


        public static NEPoint GetMousePostion()
        {
            NEPoint wPos = NEWindowControl.GetWindowPosition();
            NEPoint mScrPos = new NEPoint();
            GetCursorPos(ref mScrPos);
            c_MousePosition.X = (short)(mScrPos.X - wPos.X);
            c_MousePosition.Y = (short)(mScrPos.Y - wPos.Y);
            return c_MousePosition;
        }

        public static void FlushKeyboard()
        {
            foreach (var val in Enum.GetValues(typeof(NEKey)))
            {
                CheckKeyPress((NEKey)val);
            }
        }

        public static void FlushMouse()
        {
            foreach (var val in Enum.GetValues(typeof(NEButton)))
            {
                CheckKeyPress((NEButton)val);
            }
        }
    }
}
