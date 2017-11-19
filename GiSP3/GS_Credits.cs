using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace DiceWars
{
    class GS_Credits : GameState
    {
        Button toMenu;
        List<CuteText> authors;
        CuteText credits;

        public GS_Credits() : base()
        {
            toMenu = new Button(100, 100);
            toMenu.ButtonText = "toMenu";
            mouseInteractionList.Add(toMenu);

            authors = new List<CuteText>();
            authors.Add(new CuteText("Wojciech Płatek"));
            authors.Add(new CuteText("Grzegorz Sołdatowski"));
            authors.Add(new CuteText("Maciej Wesołowski"));

            //nie patrzeć na to
            int x, y;
            x = Program.LoadIntSetting("resx");
            y = Program.LoadIntSetting("resy");
            int space = 0;
            authors.ForEach(a => a.Position = new Vector2f(x / 2, y / 3 + (space+=60)));

            credits = new CuteText("CREDITS:", new Vector2f(x / 2, y / 3 - 20));

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
            window.Draw(credits);
            authors.ForEach(x => window.Draw(x));
            DrawMouseInteractive(window);
        }
    }
}
