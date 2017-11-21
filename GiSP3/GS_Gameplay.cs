using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceWars
{
    class GS_Gameplay : GameState
    {
        Button btnBack;

        public GS_Gameplay() : base()
        {
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
                stateaction = StateActions.POP;
            }
        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
        }
    }
}
