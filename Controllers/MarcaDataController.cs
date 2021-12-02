using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusquedasRPI.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;

using System.Data;
using System.Data.SqlClient;

using Microsoft.Extensions.Configuration;

namespace BusquedasRPI.Controllers
{ 

    [Route("api/[controller]")]
    public class MarcaDataController : Controller
    {
        private IConfiguration Configuration;

        public MarcaDataController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            List<Marca> marcas = new();

            string connetionString = ConfigurationExtensions.GetConnectionString(Configuration, "RPIBusquedas");
            SqlConnection cnn;
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            try
            {
                SqlCommand command = cnn.CreateCommand();
                command.CommandText = "SELECT TOP 50 * FROM Marcas WHERE Denominacion LIKE 'peps%'";
                SqlDataReader result = command.ExecuteReader();
                
                while (result.Read())
                {
                    Marca el = new();
                    el.Id = result["Id"].ToString();
                    el.ExpedienteId = result["ExpedienteId"].ToString();
                    el.TipoDeMarca = result["TipoDeMarca"].ToString();
                    el.Denominacion = result["Denominacion"].ToString();
                    el.Traduccion = result["Traduccion"].ToString();
                    el.Registro = result["Registro"].ToString();
                    el.UltimaRenovacion = result["UltimaRenovacion"].ToString();
                    marcas.Add(el);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            cnn.Close();

            return DataSourceLoader.Load(marcas, loadOptions);
        }

    }
}
