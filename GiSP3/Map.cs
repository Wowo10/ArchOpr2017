using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiceWars
{
    class Map
    {
        static int counter;
        public char 
        struct Region
        {            
            char label;
            public char Label
            {
                get { return label; }
            }
            public int owner; //playernumber
            public int diceamount;
            static Region()
            {
                counter = 0;
            }
            public Region(int ownernumber, int dices)
            {
                label = (char)(char.GetNumericValue('a') + counter++);
                owner = ownernumber;
                diceamount = dices;
            }
        }

        struct Edge
        {
            char begin, end;
            public char Begin
            {
                get { return begin; }
            }
            public char End
            {
                get { return end; }
            }
            public Edge(char start, char stop)
            {
                begin = start;
                end = stop;
            }
            public bool Contains(char label)
            {
                return (begin == label || end == label);
            }
        }

        List<Region> regions;
        List<Edge> edges;

        public Map()
        {
            regions = new List<Region>();
            edges = new List<Edge>();
        }

        public void AddRegion(int playernumber, int dices, char[] neighbours)
        {
            regions.Add(new Region(playernumber, dices));

            foreach (var neighbour in neighbours)
            {
                edges.Add(new Edge(neighbour,));
            }
        }

        public void Clear()
        {
            regions.Clear();
            edges.Clear();
        }

        public void ReadFromFile()
        {
            Clear();

            if (!File.Exists("resources/user/map.csv"))
            {
                Console.WriteLine("Map Reading Error");
            }
            else
            {
                
            }

        }

        public void Randomize()
        {
            //Clear()
            //kek - later or never
        }
    }
}
