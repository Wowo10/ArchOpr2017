using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace DiceWars
{
    class Hex : VertexArray, IMouseInteraction
    {
        private VertexArray border;
        private Color occupation;
        private int diceCount;
        private Text text;

        public Hex(float size)
        {
            occupation = Color.Black;
            Vector2f position = new Vector2f(0, 0);
            border = new VertexArray();
            border.PrimitiveType = PrimitiveType.LinesStrip;

            PrimitiveType = PrimitiveType.TrianglesFan;
            Append(new Vertex(position, occupation));
            for (float i = 0; i < 7; i++)
            {
                float x = (float)(Math.Sin(i / 6 * 2 * Math.PI) * size);
                float y = (float)(Math.Cos(i / 6 * 2 * Math.PI) * size);
                Append(new Vertex(new Vector2f(position.X + x, position.Y + y), new Color(100, 100, 100)));
                border.Append(new Vertex(new Vector2f(position.X + x, position.Y + y), Color.Black));
            }

            Font f = Program.LoadFont("Font.otf");
            text = new Text(new Random().Next(0, 10).ToString(), f, 20);
            //TODO: liczba kostek pobierana z serwera.
        }

        static float DotProduct(Vector2f a, Vector2f b)
        {
            float ret = 0;
            ret = a.X * b.Y - a.Y * b.X;
            return ret;
        }

        private bool isInside(Vector2f MousePos)
        {
            float data;
            for (uint i = 1; i < VertexCount - 1; i++)
            {
                Vector2f vec = new Vector2f(this[i].Position.X - this[i + 1].Position.X, this[i].Position.Y - this[i + 1].Position.Y);
                Vector2f vec2 = new Vector2f(this[i].Position.X - MousePos.X, this[i].Position.Y - MousePos.Y);
                data = DotProduct(vec2, vec);

                if (Math.Sign(data) < 0)
                    return false;
            }
            return true;
        }

        public Color Occupation
        {
            get { return occupation; }
            set
            {
                occupation = value;
                this[0] = new Vertex(positioin, value);
            }
        }

        public int DiceCount
        {
            get { return diceCount; }
            set
            {
                diceCount = value;
                text.DisplayedString = diceCount.ToString();
            }
        }

        private Vector2f positioin;
        public Vector2f Position
        {
            get { return positioin; }
            set
            {
                positioin = value;
                text.Position = new Vector2f(this[0].Position.X + value.X, this[0].Position.Y + value.Y);
                for (uint i = 0; i < VertexCount; i++)
                {
                    this[i] = new Vertex(new Vector2f(this[i].Position.X + value.X, this[i].Position.Y + value.Y), this[i].Color);
                }

                for (uint i = 0; i < border.VertexCount; i++)
                {
                    border[i] = new Vertex(new Vector2f(border[i].Position.X + value.X, border[i].Position.Y + value.Y), border[i].Color);
                }
            }
        }


        public void Clicked(float x, float y)
        {
            if (isInside(new Vector2f(x, y)))
            {
                this[0] = new Vertex(this[0].Position, Color.Blue);
            }
        }

        public void MouseMove(float x, float y)
        {
            if (isInside(new Vector2f(x, y)))
                this[0] = new Vertex(this[0].Position, Color.Red);
            else
                this[0] = new Vertex(this[0].Position, occupation);
        }

        public void Released(float x, float y)
        {
            if (isInside(new Vector2f(x, y)))
                this[0] = new Vertex(this[0].Position, Color.Red);
        }

        public new void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            target.Draw(border, states);
            target.Draw(text);
        }
    }

    class Map : Drawable, IMouseInteraction
    {
        private List<Hex> hex;
        private List<int> mine, opponents;

        int mapWidth = 6;
        int mapHeight = 6;
        int tiles;

        public Map()
        {
            float hexSize = 40;
            float d = (float)(hexSize * Math.Sqrt(3) / 2); //odległość od centrum do środka ściany
            tiles = mapWidth * mapHeight;

            hex = new List<Hex>();
            for (float i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    hex.Add(new Hex(hexSize));
                    hex[hex.Count - 1].Position = new Vector2f((j + 1) * 2 * d + (i % 2) * d, (i + 1) * 2 * hexSize - hexSize * (i / 2));
                }
            }

            InitializePlayers();
        }

        //TODO: get value from server
        private void InitializePlayers()
        {
            mine = new List<int>();
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                int n = r.Next(tiles);
                if (mine.Contains(n))
                    i--;
                else
                    mine.Add(n);
            }

            opponents = new List<int>();
            r = new Random();
            for (int i = 0; i < 10; i++)
            {
                int n = r.Next(tiles);
                if (opponents.Contains(n) || mine.Contains(n))
                    i--;
                else
                    opponents.Add(n);
            }

            mine.ForEach(z => hex[z].Occupation = Color.Green);
            opponents.ForEach(z => hex[z].Occupation = Color.Yellow);
        }

        public void Clicked(float x, float y)
        {
            mine.ForEach(w => hex[w].Clicked(x, y));
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            hex.ForEach(x => target.Draw(x, states));
        }

        public void MouseMove(float x, float y)
        {
            mine.ForEach(w => hex[w].MouseMove(x, y));
        }

        public void Released(float x, float y)
        {
            mine.ForEach(w => hex[w].Released(x, y));
        }
    }
}
