using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace DiceWars
{
    class Button : RectangleShape, IMouseInteraction
    {
        public bool clicked;
        private Font font;
        public Button()
        {
            font = new Font("resources/fonts/Font.otf");
            OutlineThickness = 2;

            clicked = false;

            text = new Text("", font, 20);
            text.Color = Color.Black;
        }
        public Button(float x, float y) : this()
        {
            Size = new Vector2f(x, y);
            text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, text.GetGlobalBounds().Height / 2);
            text.Position = new Vector2f(x / 2, y / 2);
        }

        public Button(Vector2f size) : this(size.X, size.Y) { }

        public void setPosition(Vector2f pos)
        {
            Position = pos;
            text.Position = new Vector2f(pos.X + text.Position.X, pos.Y + text.Position.Y);
        }

        private Text text;
        public string ButtonText
        {
            get { return text.DisplayedString; }
            set
            {
                text.DisplayedString = value;
                text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, text.GetGlobalBounds().Height / 2);
                //TODO jeśli szerokość większa od przycisku
            }
        }

        public bool isClicked
        {
            get
            {
                if (clicked)
                {
                    clicked = false;
                    return true;
                }
                else
                    return false;
            }
        }

        public void Clicked(float x, float y)
        {
            if (GetGlobalBounds().Contains(x, y))
            {
                FillColor = Color.Green;
                clicked = true;
            }
        }

        public void Released(float x, float y)
        {
            if (clicked || GetGlobalBounds().Contains(x, y))
            {
                FillColor = Color.White;
                clicked = false;
            }
        }

        public void MouseMove(float x, float y)
        {
            if (GetGlobalBounds().Contains(x, y))
            {
                onMouseEnter();
            }
            else
            {
                onMouseLeave();
            }
        }
        public void onMouseEnter()
        {
            OutlineColor = Color.Black;
        }
        public void onMouseLeave()
        {
            OutlineColor = Color.White;
        }
        

        public new void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            target.Draw(text, states);
        }
    }
}
