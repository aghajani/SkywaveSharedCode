using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSC.Linq
{
    public static class Extensions
    {
        public static string ToSeperated<T>(this IEnumerable<T> q, string seperator = ",", bool removeNullOrEmpty = false)
        {
            string r = "";
            if (q != null)
            {
                string str1;
                foreach (T fe1 in q)
                {
                    if (fe1 == null)
                        str1 = "";
                    else
                        str1 = fe1.ToString();
                    if (!removeNullOrEmpty || !string.IsNullOrEmpty(str1))
                        r += str1 + seperator;
                }
            }
            if (r.Length > 0)
                r = r.Substring(0, r.Length - seperator.Length);
            return r;
        }
    }
}
