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
        private Button toLobby, toSettings, toCredits, exit;
        private CuteText dice;

        public GS_Menu() : base()
        {
            InitializeGui();
        }

        private void InitializeGui()
        {
            backgroundColor = new Color(110, 0, 0);

            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");
            int buttonWidth = Program.LoadIntSetting("buttonWidth");
            int buttonHeight = Program.LoadIntSetting("buttonHeight");
            int buttonSpace = 20;

            toLobby = new Button(buttonWidth, buttonHeight);
            toLobby.ButtonText = "Play";

            toSettings = new Button(buttonWidth, buttonHeight);
            toSettings.ButtonText = "Settings";

            toCredits = new Button(buttonWidth, buttonHeight);
            toCredits.ButtonText = "Credits";

            exit = new Button(buttonWidth, buttonHeight);
            exit.ButtonText = "Exit";

            mouseInteractionList.Add(toLobby);
            mouseInteractionList.Add(toSettings);
            mouseInteractionList.Add(toCredits);
            mouseInteractionList.Add(exit);

            for (int i = 0; i < mouseInteractionList.Count; i++)
            {
                ((Button)mouseInteractionList[i]).setPosition(new Vector2f(resx / 2 - buttonWidth / 2, resy / 3 + (i + 1) * (buttonHeight + buttonSpace)));
            }

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

            else if (toSettings.isActive)
            {
                nextstate = States.GS_SETTINGS;
                stateaction = StateActions.PUSH;
            }

            else if (toCredits.isActive)
            {
                nextstate = States.GS_CREDITS;
                stateaction = StateActions.PUSH;
            }

            else if (exit.isActive)
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
