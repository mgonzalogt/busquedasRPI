using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusquedasRPI.Models
{
    public class Reporte
    {
        public string Usuario { get; set; }
        public List<Marca> Marcas { get; set; }
        public string TextoBuscado { get; set; }
    }
}
