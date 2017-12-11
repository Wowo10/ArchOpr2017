using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using NetworkModule;

namespace DiceWars
{
    static class Program
    {
        private static RenderWindow app;
        public static bool exit;
        private static string settingspath;
        private static string imagespath;
        private static string fontpath;

        //WowoUpdExperimentalsheet

        public static UdpClient client;
        public static Task sending, receiving;
        public static IPEndPoint endpoint;
        private static bool connected;

        public static void Connect()
        {
            connected = true;
            client.Connect(endpoint);
            Send(new Packet(PacketType.JOIN));
        }

        public static void Disconnect()
        { //need of also cancel active tasks!
            Console.WriteLine("Disconnecting!");
            if (connected)
            {
                connected = false;
                SendNow(new Packet(PacketType.QUIT));
                client.Close();
            }
        }

        public static void SendNow(Packet message)
        {
            byte[] temp = message.ToByteArray();
            client.Send(temp, temp.Length);
        }

        public async static void Send(Packet message)
        {
            sending = Task.Run(() =>
            {
                //Console.WriteLine("Sending: "+message+", to: "+endpoint);
                byte[] data = message.ToByteArray();
                client.Send(data, data.Length);
            });

            if (sending.IsCompleted)
                await sending;
        }

        public static Stack<string> receivedstack;
        public async static void Receive()
        {
            if (connected)
            {
                receiving = Task.Run(() =>
                {
                    if (connected) //trust me this makes sense with async
                    {
                        var receivedData = client.Receive(ref endpoint);
                        receivedstack.Push(Encoding.ASCII.GetString(receivedData));
                        //Console.WriteLine("Received: " + temp);
                    }
                });

                if (receiving.IsCompleted)
                    await receiving;
            }
        }

        //end of WowoUpdExperimentalsheet

        static Program()
        {
            settingspath = "resources/user/settings.csv";
            imagespath = "resources/images/";
            fontpath = "resources/fonts/";
            loadedsettings = new Dictionary<string, string>();
            loadedtextures = new Dictionary<string, Texture>();

            app = new RenderWindow(new VideoMode(
                Convert.ToUInt32(LoadSetting("resx")),
                Convert.ToUInt32(LoadSetting("resy"))), "Dice Wars");

            //Wowosheet
            client = new UdpClient();
            endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);

            connected = false;

            receivedstack = new Stack<string>();
        }

        public static Dictionary<string, string> loadedsettings;
        public static string LoadSetting(string name)
        {
            //checking if setting was loaded
            if (loadedsettings.ContainsKey(name))
            {
                return loadedsettings[name];
            }

            //if not then read from file
            if (File.Exists(settingspath))
            {
                StreamReader sr = new StreamReader(settingspath);

                string line = "";

                while ((line = sr.ReadLine()) != null)
                {
                    string[] temp = line.Split(';');

                    if (temp[0] == name)
                    {
                        loadedsettings.Add(name, temp[1]);
                        sr.Close();

                        return temp[1];
                    }
                }
                Console.WriteLine("No Such setting: " + name);
            }
            else
                Console.WriteLine("Setting read error!");

            //all hope is gone
            return "";
        }

        public static int LoadIntSetting(string name)
        {
            int x = int.Parse(LoadSetting(name));
            return x;
        }

        //quite similar to loading settings
        public static Dictionary<string, Texture> loadedtextures;
        public static Texture LoadTexture(string name)
        {
            if (loadedtextures.ContainsKey(name))
            {
                return loadedtextures[name];
            }

            Texture temp;
            if (File.Exists(imagespath + name + ".png"))
            {
                temp = new Texture(imagespath + name + ".png");

                loadedtextures.Add(name, temp);
                return temp;
            }
            else
                Console.WriteLine("Texture read error");

            return new Texture(1, 1); //empty with size x = 1 y = 1!
        }

        public static Font LoadFont(string name)
        {
            if (File.Exists(fontpath + name))
            {
                return new Font(fontpath + name);
            }
            else
            {
                Console.WriteLine("Font read error - no such file");
                return new Font(fontpath + "Font.otf");
            }
        }

        //////////////////////////////////////////////////////////

        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        //Events handled above gamestates. And in more elegant way
        static void OnKeyPress(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                exit = true;

            else if (Keyboard.IsKeyPressed(Keyboard.Key.F1))
            {
                Console.WriteLine("F1 pressed");
            }
        }

        static void Main()
        {
            app.Closed += new EventHandler(OnClose);
            app.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPress);
            app.SetFramerateLimit(100);

            StateManager sm = new StateManager();
            sm.EventsUpdate(app);

            while (app.IsOpen && !exit)
            {
                app.DispatchEvents();

                sm.MainLoop(app);

                app.Display();
            }

            app.Close();

            //Console.WriteLine("[Press ENTER Key to close terminal]");
            //Console.Read();
        }
    }
}
