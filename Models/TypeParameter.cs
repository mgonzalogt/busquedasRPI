using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusquedasRPI.Models
{
    public class TypeParameter
    {
        public static string fldSearchType { get; set; }

        public static IEnumerable<SelectListItem> SearchTypeList
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Fonetica", Value = "0"},
                    new SelectListItem { Text = "Exacta", Value = "1"},
                    new SelectListItem { Text = "Titular", Value = "2"},
                    new SelectListItem { Text = "Traduccion", Value = "3"}
                };
            }
        }
    }
}
