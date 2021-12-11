using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusquedasRPI.Models
{
    public class SearchCondition
    {
        public string Condition { get; set; }
        public List<SearchWord> Words { get; set; }
    }
}
