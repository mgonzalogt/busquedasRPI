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
        private readonly IConfiguration Configuration;

        public MarcasController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpGet]
        public object SearchMarcas(DataSourceLoadOptions loadOptions, [FromQuery] String searchParams)
        {
            List<Marca> marcas = new();
            SearchParameter vSearchParams = JsonConvert.DeserializeObject<SearchParameter>(searchParams);
            String searchTable = Configuration.GetSection("CustomSettings").GetSection("SearchTable").Value.ToString();
            Int32 minSearchLength = Int32.Parse(Configuration.GetSection("CustomSettings").GetSection("MinSearchLength").Value);
            String searchWordCondition = vSearchParams.AllWords ? "AND" : "OR";
            Boolean searchSubstrings = vSearchParams.Substrings;
            String topRecordSearch = Configuration.GetSection("CustomSettings").GetSection("TopRecordSearch").Value.ToString();
            String searchCollation = Configuration.GetSection("CustomSettings").GetSection("SearchCollation").Value.ToString();
            Int32 searchTimeout = Int32.Parse(Configuration.GetSection("CustomSettings").GetSection("SearchTimeout").Value);

            if (vSearchParams != null 
                && vSearchParams.Text != null 
                && vSearchParams.Text.Trim() != "" 
                && vSearchParams.Text.Trim().Length >= minSearchLength)
            {
                string connetionString = ConfigurationExtensions.GetConnectionString(Configuration, "RPIBusquedas");
                SqlConnection cnn;
                cnn = new SqlConnection(connetionString);
                cnn.Open();
                try
                {
                    SqlCommand command = cnn.CreateCommand();
                    String classCondition = SearchFunctions.BuildClassCondition(vSearchParams.Classes);
                    SearchCondition searchCondition = SearchFunctions.BuildSearchCondition(
                        vSearchParams.Text.Trim(), 
                        vSearchParams.Type,
                        searchWordCondition,
                        searchSubstrings,
                        minSearchLength,
                        searchCollation);

                    command.CommandText = String.Format(("SELECT TOP {0} * FROM {1} B WITH (NOLOCK) " +
                        "WHERE 1=1 " +
                        "AND ( " + searchCondition.Condition + ") " +
                        classCondition), 
                        topRecordSearch, 
                        searchTable, 
                        SearchFunctions.CleanString(vSearchParams.Classes));

                    //Build params
                    int cnt = 0;
                    foreach (var word in searchCondition.Words)
                    {
                        command.Parameters
                            .Add("@SearchText" + cnt.ToString(), SqlDbType.Text)
                            .Value = SearchFunctions.GetSearchWordValue(word);
                        cnt++;
                    }

                    //Get result query
                    string query = command.CommandText;
                    foreach (SqlParameter p in command.Parameters)
                    {
                        query = query.Replace(p.ParameterName, p.Value.ToString());
                    }

                    //Set timeout
                    command.CommandTimeout = searchTimeout;

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
