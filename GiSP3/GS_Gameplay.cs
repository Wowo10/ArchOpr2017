using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DiceWars
{
    class GS_Gameplay : GameState
    {
        Button btnBack;
        Client client;
        string s;

        public void SendAndReceive(object data)
        {
            while (!Program.exit)
            {
                Thread.Sleep(2000);
                //s = "wtf";
                Console.WriteLine(Program.ip);
                client = new Client();
                client.Connect(Program.ip, s);
            }                     
        }

        public GS_Gameplay() : base()
        {

            s = "wtf";
            Thread th = new Thread(new ParameterizedThreadStart(SendAndReceive));
            th.Start(Program.ip);            


            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");
            int buttonWidth = Program.LoadIntSetting("buttonWidth");
            int buttonHeight = Program.LoadIntSetting("buttonHeight");

            btnBack = new Button(buttonWidth, buttonHeight);
            btnBack.setPosition(new Vector2f(40, resy - buttonHeight - 40));
            btnBack.ButtonText = "Back";

            backgroundColor = new Color(40, 50, 90);

            mouseInteractionList.Add(btnBack);
            
        }

        public override void Update()
        {
            base.Update();

            if (btnBack.isActive)
            {
                client.Connect(Program.ip, (s+="a"));
            }
        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
        }
    }
}
