using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HW_06.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace HW_06.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Quote()
        {
            var httpClient = new HttpClient();
            try
            {
                var json = await httpClient.GetStringAsync("https://quotes.rest/qod");
                var response = JsonConvert.DeserializeObject<QuoteResponse>(json);
                ViewData["Quotes"] = response.Contents.Quotes.First();
            }
            catch (Exception ex)
            {
                ViewData["Quotes"] = new Quotes { Quote = "The best things come to those who wait", Author = "-Tanner" };
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
