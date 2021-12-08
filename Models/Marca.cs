using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusquedasRPI.Models
{
    public class Marca
    {
        public string Id { get; set; }
        public string ExpedienteId { get; set; }
        public string Clase { get; set; }
        public string ClaseDetalle { get; set; }
        public string TipoDeMarca { get; set; }
        public string Denominacion { get; set; }
        public string Traduccion { get; set; }
        public string Registro { get; set; }
        public string UltimaRenovacion { get; set; }
        public string Titular { get; set; }
        public string TitularNombre { get; set; }
        public string Estado { get; set; }
        public string ExpedienteRenovacion { get; set; }
        public string ExpedienteAnotacion { get; set; }
    }
}
