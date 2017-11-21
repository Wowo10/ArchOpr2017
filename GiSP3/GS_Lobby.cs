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
            return data.Trim();
        }
    }

    class GS_Lobby : GameState
    {
        static bool isCorrect;
        //TODO: check if it is a valid ip (regexp)
        static bool isValidIPAddress(string ip)
        {
            Program.ip = null;
            string[] tmp = ip.Trim().Split('.');
            if (tmp.Length == 4)
            {
                try
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int octet = Convert.ToInt16(tmp[i]);
                        if (octet > 255 || octet < 0)
                        {
                            isCorrect = false;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    isCorrect = false;
                    return false;
                }

                Program.ip = ip;
                isCorrect = true;
                return true;

            }
            else
            {
                isCorrect = false;
                return false;
            }
                 
        }

        Button btnToMenu, btnPasteIP, btnConnectToGame;
        CuteText text;
        
        public GS_Lobby() : base()
        {
            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");
            int buttonWidth = Program.LoadIntSetting("buttonWidth");
            int buttonHeight = Program.LoadIntSetting("buttonHeight");

            text = new CuteText("", new Vector2f(50,50));
            text.setString("IP: ");

            btnToMenu = new Button(buttonWidth, buttonHeight);
            btnToMenu.setPosition(new Vector2f(40, resy - buttonHeight - 40));
            btnToMenu.ButtonText = "Back";

            btnPasteIP = new Button(buttonWidth * 1.6f, buttonHeight);
            btnPasteIP.setPosition(new Vector2f(resx - buttonWidth * 1.6f - 50, 120));
            btnPasteIP.ButtonText = "Paste IP address";

            btnConnectToGame = new Button(buttonWidth, buttonHeight);
            btnConnectToGame.setPosition(new Vector2f(resx - buttonWidth - 40, resy - buttonHeight - 40));
            btnConnectToGame.ButtonText = "Connect";

            mouseInteractionList.Add(btnToMenu);
            mouseInteractionList.Add(btnPasteIP);
            mouseInteractionList.Add(btnConnectToGame);

            backgroundColor = new Color(0, 110, 0);

            //TODO display ip addresses uploaded to a website (grzesieks.16mb.com)
        }

        public override void Update()
        {
            if (btnToMenu.isActive)
            {
                stateaction = StateActions.POP;
            }

            if (btnPasteIP.isActive)
            {
                string tmpIP = fastClipboard.GetText();
                if (isValidIPAddress(tmpIP))
                {
                    text.setString("IP: " + tmpIP);
                }
                else
                {
                    text.setString("IP: " + "incorrect IP");
                }
            }

            if (btnConnectToGame.isActive && isCorrect)
            {
                Console.WriteLine("Connecting...");
                stateaction = StateActions.PUSH;
                nextstate = States.GS_GAMEPLAY;
            }
        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
            window.Draw(text);
        }
    }
}
