using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusquedasRPI.Models
{
    public class WildParameter
    {
        public static string CriteriaType { get; set; }

        public static IEnumerable<SelectListItem> CriteriaTypeList
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Contiene", Value = "0"},
                    new SelectListItem { Text = "Empieza", Value = "1"},
                    new SelectListItem { Text = "Termina", Value = "2"},
                    new SelectListItem { Text = "Igual", Value = "3"}
                };
            }
        }
    }
}
