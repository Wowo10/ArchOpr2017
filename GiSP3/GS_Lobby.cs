using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


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
        private Button btnToMenu, btnPasteIP, btnConnectToGame;

        private CuteText text;

        List<CuteText> players;

        public GS_Lobby() : base()
        {
            players = new List<CuteText>();

            InitializeGui();
        }

        private void InitializeGui()
        {
            backgroundColor = new Color(0, 110, 0);

            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");
            int buttonWidth = Program.LoadIntSetting("buttonWidth");
            int buttonHeight = Program.LoadIntSetting("buttonHeight");

            text = new CuteText("", new Vector2f(50, 50));
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
        }

        public override void Update()
        {
            if (btnToMenu.isActive)
            {
                Program.Disconnect();
                stateaction = StateActions.POP;
            }

            if (btnPasteIP.isActive)
            {
                string tmpIP = fastClipboard.GetText();
                if (isValidIPAddress(tmpIP))
                {
                    text.setString("IP: " + tmpIP);

                    Program.endpoint = new IPEndPoint(IPAddress.Parse(tmpIP),
                                    Program.LoadIntSetting("port"));
                }
                else
                {
                    text.setString("IP: " + "incorrect IP");
                }
            }

            if (btnConnectToGame.isActive)
            {
                Console.WriteLine("Connecting...");

                Program.Connect();
            }

            ///////////////////////////////////////////

            Program.Receive();
            if (Program.receivedstack.Count != 0)
            {
                Console.WriteLine(Program.receivedstack.Pop());
            }

        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
            window.Draw(text);
        }

        static bool isValidIPAddress(string ip)
        {
            return ip.Split('.').Length == 4;
        }

    }
}
