using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceWars
{
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
}
