using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DiceWars
{
    public static class fastClipboard
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetClipboardData(uint uFormat);
        [DllImport("user32.dll")]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
        [DllImport("user32.dll")]
        static extern bool IsClipboardFormatAvailable(uint format);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        static extern bool GlobalUnlock(IntPtr hMem);

        const uint CF_UNICODETEXT = 13;

        public static bool SetText(string data)
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                return false;
            if (!OpenClipboard(IntPtr.Zero))
                return false;

            var ptr = Marshal.StringToHGlobalUni(data);
            var res = SetClipboardData(CF_UNICODETEXT, ptr);
            CloseClipboard();
            if (res != IntPtr.Zero)
                return true;
            else
                return false;
        }

        public static string GetText()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                return null;
            if (!OpenClipboard(IntPtr.Zero))
                return null;

            string data = null;
            var hGlobal = GetClipboardData(CF_UNICODETEXT);
            if (hGlobal != IntPtr.Zero)
            {
                var lpwcstr = GlobalLock(hGlobal);
                if (lpwcstr != IntPtr.Zero)
                {
                    data = Marshal.PtrToStringUni(lpwcstr);
                    GlobalUnlock(lpwcstr);
                }
            }
            CloseClipboard();
            return data;
        }
    }
    class GS_Lobby : GameState
    {
        Button toMenu;

        public GS_Lobby() : base()
        {
            toMenu = new Button(100,100);
            toMenu.ButtonText = "toMenu";
            mousInteractionList.Add(toMenu);

            backgroundColor = new Color(0, 110, 0);
            mousInteractionList.Add(toMenu);
        }

        public override void Update()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.V))
            {
                stateaction = StateActions.POP;
            }

            if(toMenu.isActive)
            {
                stateaction = StateActions.POP;
            }
        }

        public override void Render(RenderWindow window)
        {
            window.Clear(backgroundColor);
            DrawMouseInteractive(window);
        }
    }
}
