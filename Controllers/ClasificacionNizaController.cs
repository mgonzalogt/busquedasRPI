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
            String vSearchCondition = "";
            if (vSearchText.Trim() != "")
            {
                vSearchCondition = "AND N.Detalle LIKE @SearchText ";
            }

            string connetionString = ConfigurationExtensions.GetConnectionString(Configuration, "RPIBusquedas");
            SqlConnection cnn;
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            try
            {
                SqlCommand command = cnn.CreateCommand();
                String tableName = "vwNiza";
                command.CommandText = String.Format("SELECT * FROM {0} N WITH (NOLOCK) " +
                    "WHERE 1=1 " + 
                    vSearchCondition + 
                    "ORDER BY N.Id ASC", tableName);
                command.Parameters.Add("@SearchText", System.Data.SqlDbType.Text).Value = "%" + SearchFunctions.CleanString(vSearchText) + "%";

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
