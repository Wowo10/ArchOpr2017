using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace DiceWars
{
    class Map : Drawable, IMouseInteraction
    {
        /// <summary>
        /// Mapa to zbiór hexów - do wyświetlenia
        /// Set to lista intów. Numery pól należące do gracza.
        /// </summary>
        private List<Hex> hex;
        private Set mine, opponents, neighbours;
        private Color mineColor, opponentColor, neighbourColor, defaultColor;

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
            neighbours = new Set();
            InitializePlayers();
            InitColors();
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
        private void InitializePlayers()
        {
            mine = new Set();
            Random r = new Random(); // tutaj
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
                            if (opponents.Contains(neighbours[i]))
                                opponents.Remove(neighbours[i]);

                            mine.Add(neighbours[i]);
                            hex[neighbours[i]].DiceCount = hex[focusedHex].DiceCount - hex[neighbours[i]].DiceCount - 2;
                            hex[focusedHex].DiceCount = 1;
                        }
                    }
                }
                //zresetuj widok mapy
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
            mine.ForEach(m => hex[m].Released(x, y));
        }

        /// <summary>
        /// Odświeżenie mapy    
        /// </summary>
        private void RefreshMap()
        {
            hex[focusedHex].BorderColor = Color.Black;
            neighbours.ForEach(q => hex[q].BorderColor = Color.Black);
            neighbours.ForEach(q => hex[q].Released(-100,-100));

            neighbours.Clear();
            focusedHex = -1;

            mine.ForEach(z => hex[z].Occupation = Color.Green);
            opponents.ForEach(z => hex[z].Occupation = Color.Red);
        }
    }
}
