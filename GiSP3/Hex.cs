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
        /*
        */
        private VertexArray border; //obwódka dokoła pola sześciokąta, służy do oznaczania pól
        private Color occupation; // kolor środkowego wierzchołka hexa
        private int diceCount; // liczba kostek na polu
        private Text text; // wyświetlany tekst na heksie

        private Color noOccupationColor, hoverColor, neighbourColor, clickedColor; //jakieś domyślne kolory

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
            //Random w = new Random(DateTime.Now.Millisecond);
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

        #region auxiliary methods
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
        #endregion

        /// <summary>
        /// robi obwódkę dokoła pola
        /// this.BorderColor = Color.White
        /// </summary>
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

        /// <summary>
        /// Zmienia kolor środkowego wierzchołka 
        /// this.Occupation = Color.Green
        /// </summary>
        public Color Occupation
        {
            get { return occupation; }
            set
            {
                occupation = value;
                this[0] = new Vertex(positioin, value);
            }
        }

        /// <summary>
        /// Zmienia liczbę kostek - liczba kostek to cyfra na polu
        /// </summary>
        public int DiceCount
        {
            get { return diceCount; }
            set
            {
                diceCount = value;
                text.DisplayedString = diceCount.ToString();
            }
        }

        /// <summary>
        /// Zmienia pozycję hexa o zadany wektor - zmiana pozycji obwódki, tekstu i pól wewnątrz
        /// </summary>
        private Vector2f positioin;
        public Vector2f Position
        {
            get { return positioin; }
            set
            {
                positioin = value;
                text.Position = new Vector2f(this[0].Position.X + value.X, this[0].Position.Y + value.Y); //przesunięcie textu
                for (uint i = 0; i < VertexCount; i++)
                {
                    // przesunięcie hexa
                    this[i] = new Vertex(new Vector2f(this[i].Position.X + value.X, this[i].Position.Y + value.Y), this[i].Color);
                }

                for (uint i = 0; i < border.VertexCount; i++)
                {
                    //przesunięcie obwódki
                    border[i] = new Vertex(new Vector2f(border[i].Position.X + value.X, border[i].Position.Y + value.Y), border[i].Color);
                }
            }
        }


        #region Metody interfejsów
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
            this[0] = new Vertex(this[0].Position, occupation);
        }
        #endregion

        public new void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            target.Draw(border, states);
            target.Draw(text);
        }
    }
}
