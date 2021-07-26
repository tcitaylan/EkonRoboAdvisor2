using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Helpers.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RoboAdvisorApi.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Report()
        {
            List<BacktestResult> items;

            using (StreamReader r = new StreamReader("Backtest.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<BacktestResult>>(json);
            }

            return View("Views/Report/Report.cshtml", items);
        }
    }
}
