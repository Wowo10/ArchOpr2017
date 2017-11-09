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
        Color background = new Color(110, 0, 0);

        public GS_Settings() : base()
        {

        }

        public override void Update()
        {
            base.Update();
        }

        public override void Render(RenderWindow window)
        {
            window.Clear(background);
        }
    }
}
