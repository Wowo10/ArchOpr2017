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
        Button btnBack, btnEndOfTurn;
        Client client;
        string s;
        private bool turn;

        CuteText yourTurnCuteText, notCuteText;

        public GS_Gameplay() : base()
        {
            InitializeGui();
            turn = true;
        }

        private void InitializeGui()
        {
            backgroundColor = new Color(40, 50, 90);

            int resx = Program.LoadIntSetting("resx");
            int resy = Program.LoadIntSetting("resy");
            int buttonWidth = Program.LoadIntSetting("buttonWidth");
            int buttonHeight = Program.LoadIntSetting("buttonHeight");

            btnBack = new Button(buttonWidth, buttonHeight);
            btnBack.setPosition(new Vector2f(40, resy - buttonHeight - 40));
            btnBack.ButtonText = "Back";

            btnEndOfTurn = new Button(buttonWidth, buttonHeight);
            btnEndOfTurn.setPosition(new Vector2f(resx - buttonWidth - 20, resy - buttonHeight - 160));
            btnEndOfTurn.ButtonText = "End Turn";

            yourTurnCuteText = new CuteText("", new Vector2f(resx - 160, 60));
            yourTurnCuteText.setString("Your\r\nTurn");

            notCuteText = new CuteText("", new Vector2f(resx - 160, 25));
            notCuteText.setString("Not");

            mouseInteractionList.Add(btnBack);
            mouseInteractionList.Add(btnEndOfTurn);
        }

        public override void Update()
        {
            base.Update();

            if (DateTime.Now.Second % 4 == 0 && !btnEndOfTurn.getClickable())
            {
                btnEndOfTurn.setClickable(true);
            }

            if (btnBack.isActive)
            {
                btnEndOfTurn.setClickable(false);
            }

            if (btnEndOfTurn.isActive)
            {
                turn = !turn;
            }
        }

        public override void Render(RenderWindow window)
        {
            base.Render(window);
            window.Draw(yourTurnCuteText);
            if (!turn)
                window.Draw(notCuteText);
        }
    }
}
