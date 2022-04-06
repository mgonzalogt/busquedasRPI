using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections;
using BusquedasRPI.Models;
using Newtonsoft.Json;

namespace BusquedasRPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration Configuration;

        public HomeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Reporte(string usuario, string searchParams)
        {
            //Get data
            MarcasController marcasController = new MarcasController(Configuration);

            //Get params
            SearchParameter vSearchParams = JsonConvert.DeserializeObject<SearchParameter>(searchParams);

            //Set param and datasource
            Models.Reporte reporte = new Models.Reporte();
            reporte.Usuario = usuario;
            reporte.TextoBuscado = vSearchParams.Text.Trim();
            reporte.Marcas = marcasController.DoSearchMarcas(searchParams);

            return PartialView("Reporte", reporte);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Public()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View();
        }
    }
}
