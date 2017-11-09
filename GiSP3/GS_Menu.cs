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
        Color background = new Color(110, 0, 0);

        Sprite kek; //trial

        public GS_Menu() : base()
        {
            kek = new Sprite(Program.LoadTexture("knight"));
            kek.Position = new Vector2f(50,50);
        }

        public override void Update()
        {
            if(Keyboard.IsKeyPressed(Keyboard.Key.N))
            {
                nextstate = States.GS_LOBBY;
                stateaction = StateActions.PUSH;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.C))
            {   
                stateaction = StateActions.POP;
            }
        }

        public override void Render(RenderWindow window)
        {
            window.Clear(background);

            window.Draw(kek);
        }
    }
}
