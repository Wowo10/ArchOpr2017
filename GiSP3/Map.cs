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
        /// <summary>
        /// Mapa to zbiór hexów - do wyświetlenia
        /// Set to lista intów. Numery pól należące do gracza.
        /// </summary>
        private List<Hex> hex;
        private Set mine, neighbours;
        private Color mineColor, defaultColor, neighbourColor;
        private Color[] opponentColors;
        private Set[] opponents;

        private int mapWidth = 6;
        private int mapHeight = 6;
        private int tiles;
        private float hexSize = 40; //długość od wierzchołka środkowego do wierzchołka na krawędzi

        //kliknięty hex, jeśli żaden nie jest kliknięty, ma wartość -1
        private int focusedHex = -1;

        public Map()
        {
            tiles = mapWidth * mapHeight;

            InitializeHex();
            //neighbours = new Set();
            //InitializePlayers();
            InitColors();
        }

        public Map(int players, int myNumber,int[]state,int[,]dieces)
        {

            List<Color> colors = new List<Color>();
            colors.Add(Color.Green);
            colors.Add(Color.Magenta);
            colors.Add(Color.Cyan);
            colors.Add(Color.Red);
            colors.Add(Color.Black);

            Color mine = colors[myNumber-1];

            colors.RemoveAt(myNumber - 1);

            //tiles = mapWidth * mapHeight;
            InitializeHex(dieces);
            neighbours = new Set();
            InitColors(players, mine, colors);
            InitializePlayers(myNumber, players, state,mineColor,opponentColors);

        }

        private void InitializeHex()
        {
            float d = (float)(hexSize * Math.Sqrt(3) / 2); //odległość od centrum do środka ściany, pomocnicza zmienna
            hex = new List<Hex>();
            Random r = new Random();

            for (float i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    hex.Add(new Hex(hexSize - 0.5f, r.Next(0, 10))); //-0.5f to wizualna, kosmetyczna zmiana
                    //zmiana pozycji; 2*d to stała szerokość pomiędzy wierzchołkami hexów w poziomie.
                    //jeśli jest nieparzysty wiersz należy przesunąć o pół szerokości (o 1*d) w poziomie.
                    //w pionie kolejny wiersz jest niżej o 1.75 hexSize - wynika to z przesunięcia w poziomie nieparzystych wierszy
                    hex[hex.Count - 1].Position = new Vector2f((j + 1) * 2 * d + (i % 2) * d, (i + 1 - i / 4) * 2 * hexSize);
                }
            }
        }


        private void InitializeHex(int[,]dieces)
        {
            float d = (float)(hexSize * Math.Sqrt(3) / 2); //odległość od centrum do środka ściany, pomocnicza zmienna
            hex = new List<Hex>();
            Random r = new Random();

            for (float i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    hex.Add(new Hex(hexSize - 0.5f, dieces[Convert.ToInt32(i),j])); //-0.5f to wizualna, kosmetyczna zmiana
                    //zmiana pozycji; 2*d to stała szerokość pomiędzy wierzchołkami hexów w poziomie.
                    //jeśli jest nieparzysty wiersz należy przesunąć o pół szerokości (o 1*d) w poziomie.
                    //w pionie kolejny wiersz jest niżej o 1.75 hexSize - wynika to z przesunięcia w poziomie nieparzystych wierszy
                    hex[hex.Count - 1].Position = new Vector2f((j + 1) * 2 * d + (i % 2) * d, (i + 1 - i / 4) * 2 * hexSize);
                }
            }
        }

        /// <summary>
        /// Zwraca zbiór sąsiadów a-tego hexa
        /// </summary>
        /// <param name="n">hex number</param>
        /// <returns></returns>
        private Set GetNeighbours(int n)
        {
            Set tmp = new Set();
            int odd = n / mapWidth % 2;

            if (n % mapWidth != 0)                                           //lewy
                tmp.Add(n - 1);
            if (n % mapWidth != mapWidth - 1)                                //prawy
                tmp.Add(n + 1);
            if (odd == 1)
            {
                if (n / mapWidth < mapWidth - 1)                                    //lewy dolny
                    tmp.Add(n + mapWidth - 1 + odd);
                if (n / mapWidth > 0)                                               //lewy górny
                    tmp.Add(n - mapWidth - 1 + odd);
                if (n % mapWidth != mapWidth - 1 && n / mapWidth < mapWidth - 1)    //prawy dolny
                    tmp.Add(n + mapWidth + odd);
                if (n % mapWidth != mapWidth - 1 && n / mapWidth > 0)               //prawy górny
                    tmp.Add(n - mapWidth + odd);
            }
            else
            {
                if (n / mapWidth < mapWidth - 1)                                //prawy dolny
                    tmp.Add(n + mapWidth + odd);
                if (n / mapWidth > 0)                                           //prawy górny
                    tmp.Add(n - mapWidth + odd);
                if (n % mapWidth != 0 && n / mapWidth < mapWidth - 1)            //lewy dolny
                    tmp.Add(n + mapWidth - 1 + odd);
                if (n % mapWidth != 0 && n / mapWidth > 0)                       //lewy górny
                    tmp.Add(n - mapWidth - 1 + odd);
            }

            return tmp;
        }

        //TODO: get value from server
   //    private void InitializePlayers()
   // {
   //     mine = new Set();
   //     Random r = new Random(); // tutaj
   //     for (int i = 0; i < 10; i++)
   //     {
   //         int n = r.Next(tiles);
   //         if (mine.Contains(n))
   //             i--;
   //         else
   //             mine.Add(n);
   //     }
   //
   //     opponents = new Set();
   //     r = new Random();
   //     for (int i = 0; i < 10; i++)
   //     {
   //         int n = r.Next(tiles);
   //         if (opponents.Contains(n) || mine.Contains(n))
   //             i--;
   //         else
   //             opponents.Add(n);
   //     }
   //
   //     mine.ForEach(z => hex[z].Occupation = Color.Green);
   //     opponents.ForEach(z => hex[z].Occupation = Color.Red);
   // }


        private void InitializePlayers(int myNumber,int players, int[] states, Color player, Color[]colors)
        {
            mine = new Set();
            opponents = new Set[players-1];
            for (int i = 0; i < opponents.Length; i++)
            {
                opponents[i] = new Set();
            }

            for (int i = 0; i < states.Length; i++)
            {
                if (states[i] == myNumber)
                {
                    mine.Add(i);
                }
                else
                    switch (myNumber)
                    {
                        case 1:
                            opponents[states[i] - 2].Add(i);
                            break;
                        case 2:
                            if (states[i] == 1)
                                opponents[states[i] - 1].Add(i);
                            else if (states[i] == 3)
                                opponents[states[i] - 2].Add(i);
                            break;
                        case 3:
                            opponents[states[i] - 1].Add(i);
                            break;                        
                    }
                
            }
        

            mine.ForEach(z => hex[z].Occupation = player);
            for (int i = 0; i < opponents.Length; i++)
            {
                opponents[i].ForEach(z => hex[z].Occupation = colors[i]);
            }
            
        }

        private void InitColors()
        {
            mineColor = Color.Green;
            //opponentColor = Color.Red;
            neighbourColor = Color.Magenta;
            defaultColor = Color.Black;
        }

        private void InitColors(int players, Color mine, List<Color> color)
        {
            mineColor = mine;
            opponentColors = new Color[players-1];
            for (int i = 0; i < opponentColors.Length; i++)
            {
                opponentColors[i] = color[i];
            }
            neighbourColor = Color.Magenta;
            defaultColor = Color.Black;
        }

        public bool Clicked(float x, float y)
        {
            //Jeśli żaden hex nie został kliknięty
            if (focusedHex < 0)
            {
                for (int i = 0; i < mine.Count; i++)
                {
                    if (hex[mine[i]].Clicked(x, y))
                    {
                        focusedHex = mine[i];
                        hex[focusedHex].BorderColor = Color.Blue;

                        neighbours = GetNeighbours(focusedHex);
                        neighbours.ForEach(q => hex[q].BorderColor = Color.White);
                    }
                }
            }
            else
            {
                //Sprawdzenie czy zaatakowano sąsiadującego hexa
                for (int i = 0; i < neighbours.Count; i++)
                {
                    if (hex[neighbours[i]].Clicked(x, y))
                    {
                        Console.WriteLine(String.Format("Move #{0}({1}) -> #{2}({3})",
                            focusedHex, hex[focusedHex].DiceCount, neighbours[i], hex[neighbours[i]].DiceCount));

                        //jeśli wykonano ruch na pole gracza
                        if (mine.Contains(neighbours[i]))
                        {
                            if (hex[focusedHex].DiceCount >= 2)
                            {
                                hex[neighbours[i]].DiceCount += hex[focusedHex].DiceCount - 1;
                                hex[focusedHex].DiceCount = 1;
                            }
                        }
                        // jeśli na inne pole. jeśli mamy więcej kostek niż przciwnik
                        else if (hex[focusedHex].DiceCount > hex[neighbours[i]].DiceCount)
                        {
                            for (int j = 0; j < opponents.Length; j++)
                            {
                                if (opponents[j].Contains(neighbours[i]))
                                    opponents[j].Remove(neighbours[i]);
                            }
                           

                            mine.Add(neighbours[i]);
                            hex[neighbours[i]].DiceCount = hex[focusedHex].DiceCount - hex[neighbours[i]].DiceCount - 2;
                            hex[focusedHex].DiceCount = 1;
                        }
                    }
                }
                //zresetuj widok mapy
                RefreshMap(mineColor,opponentColors);
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

        /// <summary>
        /// Odświeżenie mapy    
        /// </summary>
        private void RefreshMap()
        {
            hex[focusedHex].BorderColor = Color.Black;
            neighbours.ForEach(q => hex[q].BorderColor = Color.Black);

            for (int i = 0; i < neighbours.Count; i++)
                hex[neighbours[i]].Released(-100, -100);

            neighbours.Clear();
            focusedHex = -1;

            mine.ForEach(z => hex[z].Occupation = Color.Green);
            for (int i = 0; i < opponents.Length; i++)
            {
                opponents[i].ForEach(z => hex[z].Occupation = Color.Red);
            }
            
        }
        private void RefreshMap(Color player, Color[] players)
        {
            hex[focusedHex].BorderColor = Color.Black;
            neighbours.ForEach(q => hex[q].BorderColor = Color.Black);

            for (int i = 0; i < neighbours.Count; i++)
                hex[neighbours[i]].Released(-100, -100);

            neighbours.Clear();
            focusedHex = -1;

            mine.ForEach(z => hex[z].Occupation = player);
            for (int i = 0; i < opponents.Length; i++)
            {
                opponents[i].ForEach(z => hex[z].Occupation = players[i]);
            }

        }
    }
}
