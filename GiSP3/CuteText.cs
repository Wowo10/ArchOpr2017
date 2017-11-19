using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace DiceWars
{
    class CuteText : Drawable
    {
        private Text mainText, lowerText, upperText;
        private Font font;
        private string text;
        private uint charSize;
        private int lx = -1, ux = 3, ly = -1, uy = 3;

        public CuteText(string text)
        {
            charSize = 40;
            this.text = text;
            font = Program.LoadFont("Font.otf");
            mainText = getText(text, charSize, font, Color.White);
            upperText = getText(text, charSize, font, Color.Black, ux, uy);
            lowerText = getText(text, charSize, font, Color.Black, lx, ly);
        }

        private Text getText(string txt, uint charSize, Font font, Color color)
        {
            Text tmp;
            tmp = new Text(txt, font);
            tmp.Color = color;
            tmp.Origin = new Vector2f(tmp.GetGlobalBounds().Width / 2, tmp.GetGlobalBounds().Height / 2);

            return tmp;
        }

        private Text getText(string txt, uint charSize, Font font, Color color, int x, int y)
        {
            Text tmp = getText(txt, charSize, font, color);
            tmp.Position = new Vector2f(tmp.Position.X + x, tmp.Position.Y + y);
            return tmp;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(upperText, states);
            target.Draw(lowerText, states);
            target.Draw(mainText, states);
        }

        private Vector2f position;

        public Vector2f Position
        {
            get { return position; }
            set
            {
                position = value;
                mainText.Position = position;
                upperText.Position = new Vector2f(Position.X + ux, Position.Y + uy);
                lowerText.Position = new Vector2f(Position.X + lx, Position.Y + ly);
            }
        }

    }
}
