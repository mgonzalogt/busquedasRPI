﻿using Microsoft.AspNetCore.Mvc;
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
    public class ExpedienteController : Controller
    {
        private IConfiguration Configuration;

        public ExpedienteController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpGet]
        public object GetMarcaByExpedienteId([FromQuery] String expedienteId)
        {
            Marca marca = new();

            String connetionString = ConfigurationExtensions.GetConnectionString(Configuration, "RPIBusquedas");
            String tableName = Configuration.GetSection("CustomSettings").GetSection("SearchTable").Value.ToString();
            Int32 searchTimeout = Int32.Parse(Configuration.GetSection("CustomSettings").GetSection("SearchTimeout").Value);

            SqlConnection cnn;
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            try
            {
                SqlCommand command = cnn.CreateCommand();
                command.CommandText = String.Format("SELECT * FROM {0} WITH (NOLOCK) WHERE ExpedienteId = @ExpedienteId", tableName);
                command.Parameters.Add("@ExpedienteId", System.Data.SqlDbType.NVarChar).Value = SearchFunctions.CleanString(expedienteId).Replace("-", "");

                //Set timeout
                command.CommandTimeout = searchTimeout;

                SqlDataReader result = command.ExecuteReader();

                while (result.Read())
                {
                    Marca el = new();
                    el.Id = result["Id"].ToString();
                    el.ExpedienteId = result["ExpedienteId"].ToString();
                    el.Denominacion = result["Denominacion"].ToString();
                    el.Clase = result["ClaseId"].ToString();
                    marca = el;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            cnn.Close();

            return marca;
        }

    }
}
