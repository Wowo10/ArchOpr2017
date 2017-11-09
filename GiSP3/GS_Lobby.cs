using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceWars
{
    class GS_Lobby : GameState
    {
        Color background = new Color(0, 110, 0);

        public GS_Lobby() : base()
        {

        }

        public override void Update()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.M))
            {
                nextstate = States.GS_MENU;
                stateaction = StateActions.PUSH;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.V))
            {
                stateaction = StateActions.POP;
            }
        }

        public override void Render(RenderWindow window)
        { 
            window.Clear(background);
        }
    }
}
