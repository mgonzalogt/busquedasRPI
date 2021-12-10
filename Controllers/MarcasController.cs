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
using Newtonsoft.Json;
using BusquedasRPI.Utilities;

namespace BusquedasRPI.Controllers
{ 

    [Route("api/[controller]")]
    public class MarcasController : Controller
    {
        private IConfiguration Configuration;

        public MarcasController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpGet]
        public object SearchMarcas(DataSourceLoadOptions loadOptions, [FromQuery] String searchParams)
        {
            List<Marca> marcas = new();
            SearchParameter vSearchParams = JsonConvert.DeserializeObject<SearchParameter>(searchParams);

            if (vSearchParams != null 
                && vSearchParams.Text != null 
                && vSearchParams.Text.Trim() != "" 
                && vSearchParams.Text.Trim().Length > 2)
            {
                string connetionString = ConfigurationExtensions.GetConnectionString(Configuration, "RPIBusquedas");
                SqlConnection cnn;
                cnn = new SqlConnection(connetionString);
                cnn.Open();
                try
                {
                    SqlCommand command = cnn.CreateCommand();
                    String tableName = "vwBusquedas";

                    String classCondition = "";
                    if (vSearchParams.Classes != null && vSearchParams.Classes.Trim() != "")
                    {
                        classCondition = "AND B.ClaseId IN ({1}) ";
                    }

                    String searchCondition = "";
                    if (vSearchParams.Text != null && vSearchParams.Text.Trim() != "")
                    {
                        if (vSearchParams.Type == "0" || vSearchParams.Type == "1")
                        {
                            searchCondition = "AND B.Denominacion LIKE @SearchText ";
                        }

                        if (vSearchParams.Type == "2")
                        {
                            searchCondition = "AND B.TitularNombre LIKE @SearchText ";
                        }

                    }

                    command.CommandText = String.Format("SELECT TOP 500 * FROM {0} B " +
                        "WHERE 1=1 " +
                        searchCondition +
                        classCondition, tableName, SearchFunctions.CleanString(vSearchParams.Classes));
                    command.Parameters.Add("@SearchText", System.Data.SqlDbType.Text).Value = "%" + SearchFunctions.CleanString(vSearchParams.Text.Trim()) + "%";

                    SqlDataReader result = command.ExecuteReader();
                    while (result.Read())
                    {
                        Marca el = new();
                        el.Id = result["Id"].ToString();
                        el.ExpedienteId = result["ExpedienteId"].ToString();
                        el.Clase = result["Clase"].ToString();
                        el.ClaseDetalle = result["ClaseDetalle"].ToString();
                        el.TipoDeMarca = result["TipoDeMarca"].ToString();
                        el.Denominacion = result["Denominacion"].ToString();
                        el.Traduccion = result["Traduccion"].ToString();
                        el.Registro = result["Registro"].ToString();
                        el.UltimaRenovacion = result["UltimaRenovacion"].ToString();
                        el.Titular = result["Titular"].ToString();
                        el.TitularNombre = result["TitularNombre"].ToString();
                        el.Estado = result["EstatusDescripcion"].ToString();
                        el.ExpedienteRenovacion = result["ExpedienteRenovacion"].ToString();
                        el.ExpedienteAnotacion = result["ExpedienteAnotacion"].ToString();
                        marcas.Add(el);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                cnn.Close();
            }

            return DataSourceLoader.Load(marcas, loadOptions);
        }

    }
}
