using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceWars
{
    class GS_Menu : GameState
    {
        Button toLobby, toSettings;
        Button exitButton;
        Text menuText, menuText2, menuText3;

        public GS_Menu() : base()
        {
            backgroundColor = new Color(110, 0, 0);

            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");

            int buttonH = 40;
            int buttonW = 130;
            int buttonSpace = 20;
            toLobby = new Button(buttonW, buttonH);
            toLobby.setPosition(new Vector2f(resx/2 - 50, resy/3 + buttonH + buttonSpace));
            toLobby.ButtonText = "toLobby";

            toSettings = new Button(buttonW, buttonH);
            toSettings.setPosition(new Vector2f(resx / 2 - 50, resy / 3 +2*(buttonH + buttonSpace)));
            toSettings.ButtonText = "toSettings";

            exitButton = new Button(30, 30);
            exitButton.setPosition(new Vector2f(Program.LoadIntSetting("resx") - 30, 0));
            exitButton.ButtonText = "X";
            exitButton.ButtonTextSize = 30;

            string txt = "DiceWars!";
            uint charSize = 40;
            Font font = Program.LoadFont("Font.otf");

            //class CuteText
            menuText = new Text(txt, font, charSize);
            menuText.Position = new Vector2f(resx / 2, resy / 3);
            menuText.Origin = new Vector2f(menuText.GetGlobalBounds().Width / 2, menuText.GetGlobalBounds().Height / 2);
            menuText.Color = Color.White;

            menuText2 = new Text(txt, font, charSize);
            menuText2.Position = new Vector2f(resx / 2 - 1, resy / 3 - 1);
            menuText2.Origin = new Vector2f(menuText2.GetGlobalBounds().Width / 2, menuText2.GetGlobalBounds().Height / 2);
            menuText2.Color = Color.Black;

            menuText3 = new Text(txt, font, charSize);
            menuText3.Position = new Vector2f(resx / 2 + 3, resy / 3 + 3);
            menuText3.Origin = new Vector2f(menuText3.GetGlobalBounds().Width / 2, menuText3.GetGlobalBounds().Height / 2);
            menuText3.Color = Color.Black;

            mousInteractionList.Add(toLobby);
            mousInteractionList.Add(exitButton);
            mousInteractionList.Add(toSettings);
        }

        public override void Update()
        {
            if (toLobby.isActive)
            {
                nextstate = States.GS_LOBBY;
                stateaction = StateActions.PUSH;
            }

            if (exitButton.isActive)
            {
                stateaction = StateActions.POP;
            }
            
            if(toSettings.isActive)
            {
                nextstate = States.GS_SETTINGS;
                stateaction = StateActions.PUSH;
            }
        }

        public override void Render(RenderWindow window)
        {
            window.Clear(backgroundColor);
            window.Draw(menuText2);
            window.Draw(menuText3);
            window.Draw(menuText);
            DrawMouseInteractive(window);
        }
    }
}
