using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusquedasRPI.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;

namespace BusquedasRPI.Controllers
{ 

    [Route("api/[controller]")]
    public class MarcaDataController : Controller
    {

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            return DataSourceLoader.Load(MarcaData.ListaMarcas, loadOptions);
        }

    }
}
