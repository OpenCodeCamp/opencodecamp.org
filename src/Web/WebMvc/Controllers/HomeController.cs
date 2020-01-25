using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenCodeCamp.WebMvc.Models;

namespace OpenCodeCamp.WebMvc.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //ViewData["Baseline2"] = _localizer[Baseline];
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public string ShowMeCulture()
        //{
        //    return $"CurrentCulture:{System.Globalization.CultureInfo.CurrentCulture.Name}, CurrentUICulture:{System.Globalization.CultureInfo.CurrentUICulture.Name}";
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //public IActionResult Map_Test()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult Team()
        //{
        //    return View();
        //}
    }
}
