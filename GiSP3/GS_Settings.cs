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
        Button toMenu;

        public GS_Settings() : base()
        {
            toMenu = new Button(100, 100);
            toMenu.ButtonText = "toMenu";
            mousInteractionList.Add(toMenu);

            backgroundColor = new Color(30, 110, 60);
            mousInteractionList.Add(toMenu);
        }

        public override void Update()
        {
            if (toMenu.isClicked)
            {
                nextstate = States.GS_MENU;
                stateaction = StateActions.PUSH;
            }
        }

        public override void Render(RenderWindow window)
        {
            window.Clear(backgroundColor);
            DrawMouseInteractive(window);
        }
    }
}
