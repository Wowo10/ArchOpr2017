using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceWars
{
    class GS_Settings : GameState
    {
        Button btnToMenu;
        CuteText textSettings;

        public GS_Settings() : base()
        {
            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");
            int buttonWidth = Program.LoadIntSetting("buttonWidth");
            int buttonHeight = Program.LoadIntSetting("buttonHeight");

            textSettings = new CuteText("", new Vector2f(50, 50));
            textSettings.setString("IP: ");

            btnToMenu = new Button(buttonWidth, buttonHeight);
            btnToMenu.setPosition(new Vector2f(40, resy - buttonHeight - 40));
            btnToMenu.ButtonText = "Back";

            backgroundColor = new Color(30, 110, 60);

            mouseInteractionList.Add(btnToMenu);
        }

        public override void Update()
        {
            if (btnToMenu.isActive)
            {
                stateaction = StateActions.POP;
            }
        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
        }
    }
}
