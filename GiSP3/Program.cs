using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

using System.IO;

namespace DiceWars
{
    static class Program
    {
        static RenderWindow app;
        public static bool exit;
        static string settingspath;
        static string imagespath;
        static string fontpath;

        static Dictionary<string, string> loadedsettings;
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
        static Dictionary<string, Texture> loadedtextures;
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