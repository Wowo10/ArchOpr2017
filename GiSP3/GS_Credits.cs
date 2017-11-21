﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace DiceWars
{
    class GS_Credits : GameState
    {
        Button btnBack;
        List<CuteText> authors;
        CuteText credits;

        public GS_Credits() : base()
        {
            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");
            int buttonWidth = Program.LoadIntSetting("buttonWidth");
            int buttonHeight = Program.LoadIntSetting("buttonHeight");

            btnBack = new Button(buttonWidth, buttonHeight);
            btnBack.setPosition(new Vector2f(40, resy - buttonHeight - 40));
            btnBack.ButtonText = "Back";
            mouseInteractionList.Add(btnBack);

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
            if (btnBack.isActive)
            {
                stateaction = StateActions.POP;
            }
        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
            window.Draw(credits);
            authors.ForEach(x => window.Draw(x));
        }
    }
}