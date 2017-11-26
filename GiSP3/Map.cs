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

        private Color noOccupationColor, hoverColor, neighbourColor, clickedColor;

        public Hex(float size, int a)
        {
            InitColors();
            occupation = Color.Black;

            diceCount = a;


            border = new VertexArray();
            border.PrimitiveType = PrimitiveType.LinesStrip;


            PrimitiveType = PrimitiveType.TrianglesFan;
            Vector2f position = new Vector2f(0, 0);
            Append(new Vertex(position, occupation));
            for (float i = 0; i < 7; i++)
            {
                float x = (float)(Math.Sin(i / 6 * 2 * Math.PI) * size);
                float y = (float)(Math.Cos(i / 6 * 2 * Math.PI) * size);
                Append(new Vertex(new Vector2f(position.X + x, position.Y + y), new Color(100, 100, 100)));
                border.Append(new Vertex(new Vector2f(position.X + x, position.Y + y), occupation));
            }

            Font f = Program.LoadFont("Font.otf");
            Random w = new Random(DateTime.Now.Millisecond);
            text = new Text(a.ToString(), f, 20);
            //TODO: liczba kostek pobierana z serwera.
        }

        private void InitColors()
        {
            noOccupationColor = Color.Black;
            hoverColor = Color.Yellow;
            neighbourColor = Color.Magenta;
            clickedColor = Color.Blue;
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

        private Color borderColor;

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                for (uint i = 0; i < border.VertexCount; i++)
                {
                    border[i] = new Vertex(border[i].Position, value);
                }
            }
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


        public bool Clicked(float x, float y)
        {
            if (isInside(new Vector2f(x, y)))
            {
                this[0] = new Vertex(this[0].Position, clickedColor);
                return true;
            }
            return false;
        }

        public void MouseMove(float x, float y)
        {
            if (isInside(new Vector2f(x, y)))
                this[0] = new Vertex(this[0].Position, hoverColor);
            else
                this[0] = new Vertex(this[0].Position, occupation);
        }

        public void Released(float x, float y)
        {
            if (isInside(new Vector2f(x, y)))
                this[0] = new Vertex(this[0].Position, occupation);
        }

        public new void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            target.Draw(border, states);
            target.Draw(text);
        }
    }

    class Set : List<int>
    {
        public Set() : base()
        {

        }

        public Set(int[] tab) : base()
        {
            for (int i = 0; i < tab.Length; i++)
            {
                Add(tab[i]);
            }
        }

        public static Set operator &(Set l, Set r) // union of sets ∩
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (!r.Contains(l[i]))
                    l.Remove(l[i]);
            }
            return l;
        }

        public static Set operator |(Set l, Set r) //sum of sets ∪
        {
            for (int i = 0; i < r.Count; i++)
            {
                if (!l.Contains(r[i]))
                    l.Add(r[i]);
            }
            return l;
        }

        public override string ToString()
        {
            string w = String.Join(", ", this);
            return w;
        }
    }

    class Map : Drawable, IMouseInteraction
    {
        private List<Hex> hex;
        private Set mine, opponents, neighbours;
        private Color mineColor, opponentColor, neighbourColor, defaultColor;

        int mapWidth = 6;
        int mapHeight = 6;
        int tiles;

        private int focusedHex;

        public Map()
        {
            focusedHex = -1;
            neighbours = new Set();
            float hexSize = 40;
            float d = (float)(hexSize * Math.Sqrt(3) / 2); //odległość od centrum do środka ściany
            tiles = mapWidth * mapHeight;
            Console.WriteLine("sum: " + (new Set(new int[] { 1, 2, 3, 4 }) | new Set(new int[] { 1, 3, 5, 7 })));
            Console.WriteLine("intesection: " +(new Set(new int[] { 1, 2, 3, 4 }) & new Set(new int[] { 1, 3, 5, 7 })));

            hex = new List<Hex>();
            Random r = new Random();
            for (float i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    hex.Add(new Hex(hexSize - 0.5f, r.Next(0, 10)));
                    hex[hex.Count - 1].Position = new Vector2f((j + 1) * 2 * d + (i % 2) * d, (i + 1) * 2 * hexSize - hexSize * (i / 2));
                }
            }

            InitializePlayers();
            InitColors();
        }

        private Set GetNeighbours(int a)
        {
            Set tmp = new Set();

            int odd = a / mapWidth % 2;

            if (a % mapWidth != 0)                                           //lewy
                tmp.Add(a - 1);
            if (a % mapWidth != mapWidth - 1)                                //prawy
                tmp.Add(a + 1);
            if (odd == 1)
            {
                if (a / mapWidth < mapWidth - 1)
                    tmp.Add(a + mapWidth - 1 + odd);
                if (a / mapWidth > 0)
                    tmp.Add(a - mapWidth - 1 + odd);
                if (a % mapWidth != mapWidth - 1 && a / mapWidth < mapWidth - 1) //prawy dolny
                {
                    tmp.Add(a + mapWidth + odd);
                }
                if (a % mapWidth != mapWidth - 1 && a / mapWidth > 0)            //prawy górny
                {
                    tmp.Add(a - mapWidth + odd);
                }
            }
            else
            {
                if (a / mapWidth < mapWidth - 1)
                    tmp.Add(a + mapWidth + odd);
                if (a / mapWidth > 0)
                    tmp.Add(a - mapWidth + odd);
                if (a % mapWidth != 0 && a / mapWidth < mapWidth - 1)            //lewy dolny
                {
                    tmp.Add(a + mapWidth - 1 + odd);
                }
                if (a % mapWidth != 0 && a / mapWidth > 0)                       //lewy górny
                {
                    tmp.Add(a - mapWidth - 1 + odd);
                }
            }

            return tmp;
        }

        //TODO: get value from server
        private void InitializePlayers()
        {
            mine = new Set();
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                int n = r.Next(tiles);
                if (mine.Contains(n))
                    i--;
                else
                    mine.Add(n);
            }

            opponents = new Set();
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
            opponents.ForEach(z => hex[z].Occupation = Color.Red);
        }

        private void InitColors()
        {
            mineColor = Color.Green;
            opponentColor = Color.Red;
            neighbourColor = Color.Magenta;
            defaultColor = Color.Black;
        }

        public bool Clicked(float x, float y)
        {
            if (focusedHex < 0)
            {
                for (int i = 0; i < mine.Count; i++)
                {
                    if (hex[mine[i]].Clicked(x, y))
                    {
                        focusedHex = mine[i];
                        hex[focusedHex].BorderColor = Color.Blue;
                        neighbours = GetNeighbours(mine[i]);
                        neighbours.ForEach(q => hex[q].BorderColor = Color.White);
                    }
                }
            }
            else
            {
                for (int i = 0; i < neighbours.Count; i++)
                {
                    if (hex[neighbours[i]].Clicked(x, y))
                    {
                        //attack from focused to clicked
                        Console.WriteLine(String.Format("Move #{0}({1}) -> #{2}({3})",
                            focusedHex, hex[focusedHex].DiceCount, neighbours[i], hex[neighbours[i]].DiceCount));

                        if (mine.Contains(neighbours[i]))
                        {
                            if (hex[focusedHex].DiceCount >= 2)
                            {
                                hex[neighbours[i]].DiceCount += hex[focusedHex].DiceCount - 2;
                                hex[focusedHex].DiceCount = 1;
                            }
                        }

                        else if (hex[focusedHex].DiceCount > hex[neighbours[i]].DiceCount)
                        {
                            if (opponents.Contains(neighbours[i]))
                                opponents.Remove(neighbours[i]);
                            if (!mine.Contains(neighbours[i]))
                            {
                                mine.Add(neighbours[i]);
                                hex[neighbours[i]].DiceCount = hex[focusedHex].DiceCount - hex[neighbours[i]].DiceCount - 2;
                                hex[focusedHex].DiceCount = 1;
                            }
                        }
                    }
                }
                hex[focusedHex].BorderColor = Color.Black;
                neighbours.ForEach(q => hex[q].BorderColor = Color.Black);

                for (int i = 0; i < neighbours.Count; i++)
                {
                    hex[neighbours[i]].Released(x, y);
                }

                neighbours.Clear();
                focusedHex = -1;
                RefreshMap();
            }
            return true;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            hex.ForEach(x => target.Draw(x, states));
        }

        public void MouseMove(float x, float y)
        {
            if (focusedHex < 0)
                mine.ForEach(w => hex[w].MouseMove(x, y));
            else
            {
                neighbours.ForEach(w => hex[w].MouseMove(x, y));
                hex[focusedHex].MouseMove(x, y);
            }
        }

        public void Released(float x, float y)
        {
            for (int i = 0; i < mine.Count; i++)
            {
                hex[mine[i]].Released(x, y);
            }
        }

        private void RefreshMap()
        {
            mine.ForEach(z => hex[z].Occupation = Color.Green);
            opponents.ForEach(z => hex[z].Occupation = Color.Red);
        }
    }
}
