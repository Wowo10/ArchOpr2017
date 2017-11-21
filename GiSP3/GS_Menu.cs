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
        Button toLobby, toSettings, toCredits, exit;
        CuteText dice;

        public GS_Menu() : base()
        {
            backgroundColor = new Color(110, 0, 0);

            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");

            int buttonH = 40;
            int buttonW = 130;
            int buttonSpace = 20;

            toLobby = new Button(buttonW, buttonH);
            toLobby.setPosition(new Vector2f(resx / 2 - buttonW / 2, resy / 3 + buttonH + buttonSpace));
            toLobby.ButtonText = "toLobby";

            toSettings = new Button(buttonW, buttonH);
            toSettings.setPosition(new Vector2f(resx / 2 - buttonW / 2, resy / 3 + 2 * (buttonH + buttonSpace)));
            toSettings.ButtonText = "toSettings";

            toCredits = new Button(buttonW, buttonH);
            toCredits.setPosition(new Vector2f(resx / 2 - buttonW / 2, resy / 3 + 3 * (buttonH + buttonSpace)));
            toCredits.ButtonText = "toCredits";

            exit = new Button(buttonW, buttonH);
            exit.setPosition(new Vector2f(resx / 2 - buttonW / 2, resy / 3 + 4 * (buttonH + buttonSpace)));
            exit.ButtonText = "Exit!";

            mouseInteractionList.Add(toLobby);
            mouseInteractionList.Add(toSettings);
            mouseInteractionList.Add(toCredits);
            mouseInteractionList.Add(exit);

            dice = new CuteText("DiceWars!");
            dice.Position = new Vector2f(resx / 2, resy / 3);
        }

        public override void Update()
        {
            if (toLobby.isActive)
            {
                nextstate = States.GS_LOBBY;
                stateaction = StateActions.PUSH;
            }

            if (toSettings.isActive)
            {
                nextstate = States.GS_SETTINGS;
                stateaction = StateActions.PUSH;
            }

            if (toCredits.isActive)
            {
                nextstate = States.GS_CREDITS;
                stateaction = StateActions.PUSH;
            }

            if (exit.isActive)
            {
                stateaction = StateActions.POP;
            }
        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
            window.Draw(dice);
        }
    }
}
