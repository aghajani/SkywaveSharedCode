using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSC
{
    public static class Random<T>
    {
        public static Random RAND;

        static Random()
        {
            if (RAND == null) RAND = new Random();
        }

        public static List<T> RandomizeOrder(List<T> toRandomize)
        {
            List<T> r = new List<T>();
            List<T> copyOfToRandomize = new List<T>();
            copyOfToRandomize.AddRange(toRandomize);
            int randomIndex;
            while (copyOfToRandomize.Count > 0)
            {
                randomIndex = RAND.Next(copyOfToRandomize.Count);
                r.Add(copyOfToRandomize[randomIndex]);
                copyOfToRandomize.RemoveAt(randomIndex);
            }
            return r;
        }
    }
}
