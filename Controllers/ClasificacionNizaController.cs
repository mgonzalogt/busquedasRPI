using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BusquedasRPI.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;

using Microsoft.Extensions.Configuration;
using BusquedasRPI.Utilities;
using System.Data;

namespace BusquedasRPI.Controllers
{
    public class ClasificacionNizaController : Controller
    {
        private IConfiguration Configuration;

        public ClasificacionNizaController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpGet]
        public object GetClasificacion(DataSourceLoadOptions loadOptions, String searchText)
        {
            List<ClasificacionNiza> clases = new();
            String vSearchText = searchText == null ? "" : searchText.Trim();
            String tableName = Configuration.GetSection("CustomSettings").GetSection("NizaTable").Value.ToString();
            Int32 minSearchLength = Int32.Parse(Configuration.GetSection("CustomSettings").GetSection("MinSearchLength").Value);
            String searchCollation = Configuration.GetSection("CustomSettings").GetSection("SearchCollation").Value.ToString();
            Int32 searchTimeout = Int32.Parse(Configuration.GetSection("CustomSettings").GetSection("SearchTimeout").Value);
            List<String> searchWords = new();
            searchWords.Add(vSearchText);
                
            SearchCondition searchCondition = SearchFunctions.BuildNizaCondition(
                        searchWords,
                        "N.DetalleText",
                        "OR",
                        minSearchLength,
                        searchCollation);

            string connetionString = ConfigurationExtensions.GetConnectionString(Configuration, "RPIBusquedas");
            SqlConnection cnn;
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            try
            {
                SqlCommand command = cnn.CreateCommand();
                String vCondition = searchCondition.Words.Count == 0 ? "" : "AND ( " + searchCondition.Condition + ") ";
                command.CommandText = String.Format("SELECT * FROM {0} N WITH (NOLOCK) " +
                    "WHERE 1=1 " +
                    vCondition +
                    "ORDER BY N.Id ASC", tableName);

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
                /*
                string query = command.CommandText;
                foreach (SqlParameter p in command.Parameters)
                {
                    query = query.Replace(p.ParameterName, p.Value.ToString());
                }
                */

                //Set timeout
                command.CommandTimeout = searchTimeout;

                SqlDataReader result = command.ExecuteReader();

                while (result.Read())
                {
                    ClasificacionNiza el = new();
                    el.Id = result["Id"].ToString();
                    el.Codigo = result["Codigo"].ToString();
                    el.Descripcion = result["Descripcion"].ToString();
                    el.Detalle = result["Detalle"].ToString();
                    el.ProductosQueAmpara = result["ProductosQueAmpara"].ToString();
                    clases.Add(el);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            cnn.Close();

            return DataSourceLoader.Load(clases, loadOptions);
        }

    }
}
