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
        private Button btnBack, btnEndOfTurn, btnStart;
        private Client client;
        private bool turn;
        private Map map;
        private bool exit;

        public void SendAndReceive()
        {
            while (!Program.exit && !exit)
            {
                Thread.Sleep(500);
                Console.WriteLine(Program.ip);
                client = new Client();
                if (map != null)
                {
                    string tmpMap = client.Connect(Program.ip, "!");
                    Map.ReadMap(ref map, tmpMap);
                    if (turn)
                    {
                        client.Connect(Program.ip, "#");
                    }
                    else
                    {                                              
                        if (client.Connect(Program.ip, "?") == "Y")
                        {
                            turn = true;
                            btnEndOfTurn.setClickable(true);
                            mouseInteractionList.Add(map);
                        }                       
                    }
                }
                else
                {
                    string tmpMap = client.Connect(Program.ip, "!");
                    if (tmpMap != "no")
                    {
                        map = Map.ReadMap(tmpMap);
                    }
                }                                              
            }                     
        }

        CuteText yourTurnCuteText, notCuteText;

        public GS_Gameplay() : base()
        {
            turn = false;
            Client client = new Client();
            map = null;            
            InitializeGui();

            Thread th = new Thread(SendAndReceive);
            th.Start();
        }

        private void InitializeGui()
        {
            backgroundColor = new Color(40, 50, 90);

            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");
            int buttonWidth = Program.LoadIntSetting("buttonWidth");
            int buttonHeight = Program.LoadIntSetting("buttonHeight");

            btnBack = new Button(buttonWidth, buttonHeight);
            btnBack.setPosition(new Vector2f(40, resy - buttonHeight - 40));
            btnBack.ButtonText = "Back";

            btnEndOfTurn = new Button(buttonWidth, buttonHeight);
            btnEndOfTurn.setPosition(new Vector2f(resx - buttonWidth - 20, resy - buttonHeight - 160));
            btnEndOfTurn.ButtonText = "End Turn";
            btnEndOfTurn.setClickable(false);

            btnStart = new Button(buttonWidth, buttonHeight);
            btnStart.setPosition(new Vector2f(resx - buttonWidth - 20, resy - buttonHeight - 110));
            btnStart.ButtonText = "Start";


            yourTurnCuteText = new CuteText("", new Vector2f(resx - 160, 60));
            yourTurnCuteText.setString("Your\r\nTurn");

            notCuteText = new CuteText("", new Vector2f(resx - 160, 25));
            notCuteText.setString("Not");

            mouseInteractionList.Add(btnBack);
            mouseInteractionList.Add(btnEndOfTurn);
            mouseInteractionList.Add(btnStart);
        }

        public override void Update()
        {
            base.Update();

            //if (DateTime.Now.Second % 4 == 0 && !btnEndOfTurn.getClickable())
            //{
            //    btnEndOfTurn.setClickable(true);
            //}

            if (btnBack.isActive)
            {
                exit = true;
                stateaction = StateActions.POP;
                //btnEndOfTurn.setClickable(false);               
            }

            if (btnStart.isActive)
            {
                if (btnStart.getClickable())
                {
                    string nothing = client.Connect(Program.ip, "$");
                    btnStart.setClickable(false);
                }
                
            }


            if (btnEndOfTurn.isActive)
            {
                if(btnEndOfTurn.getClickable())
                {
                    mouseInteractionList.Remove(map);
                    string nothing = client.Connect(Program.ip, "&");
                    turn = false;
                    btnEndOfTurn.setClickable(false);
                }
            }
        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
            window.Draw(yourTurnCuteText);
            if (!turn)
                window.Draw(notCuteText);
            if (map !=null)
            {
                window.Draw(map);
            }
            
        }
    }
}
