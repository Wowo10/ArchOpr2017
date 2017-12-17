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

            //InitializeHex();
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

        

        public void UpdateMap(int players, int myNumber, int[] state, int[,] dieces)
        {

            List<Color> colors = new List<Color>();
            colors.Add(Color.Green);
            colors.Add(Color.Magenta);
            colors.Add(Color.Cyan);
            colors.Add(Color.Red);
            colors.Add(Color.Black);

            Color mine = colors[myNumber - 1];

            colors.RemoveAt(myNumber - 1);

            int k = 0;
            for (int i = 0; i < dieces.GetLength(0); i++)
            {
                for (int j = 0; j < dieces.GetLength(1); j++)
                {
                    if (dieces[i,j] != hex[k].DiceCount)
                    {
                        hex[k].DiceCount = dieces[i,j];
                    }
                    k++;
                }
            }

            InitColors(players, mine, colors);
            InitializePlayers(myNumber, players, state, mineColor, opponentColors);

        }
                
        public static Map ReadMap(string map)
        {
            string[] tmp = map.Split(';');
            int myNumber = Convert.ToInt16(tmp[tmp.Length- 2]);
            int players = Convert.ToInt16(tmp[tmp.Length-1]);
            int[] state = new int[tmp.Length-2];
            int[,] dieces = new int[6, 6];

            int j = 0;
            int k = 0;

            for (int i = 0; i < tmp.Length-2; i++)
            {
                string[] tmp2 = tmp[i].Split(':');
                state[i] = Convert.ToInt32(tmp2[0]);
                if (i - k > 5)
                {
                    j++;
                    k += 6;                   
                }
                dieces[j, i - k] = Convert.ToInt16(tmp2[1]);

            }

            return new Map(players,myNumber,state,dieces);
        }

        public static void ReadMap(ref Map map,string textMap)
        {
            if (textMap != "no")
            {

                string[] tmp = textMap.Split(';');
                int myNumber = Convert.ToInt16(tmp[tmp.Length - 2]);
                int players = Convert.ToInt16(tmp[tmp.Length - 1]);
                int[] state = new int[tmp.Length - 2];
                int[,] dieces = new int[6, 6];

                int j = 0;
                int k = 0;

                for (int i = 0; i < tmp.Length - 2; i++)
                {
                    string[] tmp2 = tmp[i].Split(':');
                    state[i] = Convert.ToInt32(tmp2[0]);
                    if (i - k > 5)
                    {
                        j++;
                        k += 6;
                    }
                    dieces[j, i - k] = Convert.ToInt16(tmp2[1]);

                }
                if (map != null)
                {
                    map.UpdateMap(players, myNumber, state, dieces);
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
                    if (states[i]>myNumber)
                        opponents[states[i] - 2].Add(i);
                    else if(states[i]<myNumber)
                        opponents[states[i] - 1].Add(i);                     
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
                        if (!mine.Contains(neighbours[i]))
                        {

                            string s = "*;"+focusedHex.ToString() + ";" + neighbours[i];

                            Client client = new Client();
                            string respond = client.Connect(Program.ip, s);

                            if (respond != null)
                            {
                                string[] tmp = respond.Split(';');

                                hex[focusedHex].DiceCount = Convert.ToInt32(tmp[1]);
                                hex[neighbours[i]].DiceCount = Convert.ToInt32(tmp[3]);
                                if (Convert.ToInt32(tmp[0]) > Convert.ToInt32(tmp[2]))
                                {
                                    for (int j = 0; j < opponents.Length; j++)
                                    {
                                        if (opponents[j].Contains(neighbours[i]))
                                            opponents[j].Remove(neighbours[i]);
                                    }
                                    mine.Add(neighbours[i]);
                                }
                            }
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
        /// odświerzanie mapy
        /// </summary>
        /// <param name="player">Kolor gracza</param>
        /// <param name="players">Kolory przeciwników</param>
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
