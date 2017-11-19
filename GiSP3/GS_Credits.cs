using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace DiceWars
{
    class GS_Credits : GameState
    {
        Button toMenu;

        public GS_Credits() : base()
        {
            toMenu = new Button(100, 100);
            toMenu.ButtonText = "toMenu";
            mouseInteractionList.Add(toMenu);

            backgroundColor = new Color(130, 20, 80);
        }

        public override void Update()
        {
            if (toMenu.isActive)
            {
                stateaction = StateActions.POP;
            }
        }

        public override void Render(RenderWindow window)
        {
            window.Clear(backgroundColor);
            DrawMouseInteractive(window);
        }
    }
}
