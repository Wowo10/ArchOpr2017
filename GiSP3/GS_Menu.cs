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
        Sprite kek; //trial

        Button toLobby;

        public GS_Menu() : base()
        {
            backgroundColor = new Color(110, 0, 0);

            kek = new Sprite(Program.LoadTexture("knight"));
            kek.Position = new Vector2f(50, 50);

            toLobby = new Button(100, 100);
            toLobby.setPosition(new Vector2f(100, 100));
            toLobby.ButtonText = "toLobby";
            mousInteractionList.Add(toLobby);
        }

        public override void Update()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.C))
            {
                stateaction = StateActions.POP;
            }

            if (toLobby.isClicked)
            {
                nextstate = States.GS_LOBBY;
                stateaction = StateActions.PUSH;
            }
        }

        public override void Render(RenderWindow window)
        {
            window.Clear(backgroundColor);
            window.Draw(kek);
            DrawMouseInteractive(window);
        }
    }
}
