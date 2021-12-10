using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusquedasRPI.Utilities
{
    public class SearchFunctions
    {
        public static String CleanString(String st)
        {
            String vReturn = st == null ? "" : st;

            vReturn = vReturn.ToLower().Trim();
            vReturn = vReturn.Replace("'", "");
            vReturn = vReturn.Replace("%", "");

            return vReturn;
        }
    }
}
