using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MFA_Server.Models;
using MFA_Server.Entities;

namespace MFA_Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly MFATestDatabaseContext context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MFATestDatabaseContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [Route("")]
        [Route("/Home")]
        public IActionResult Index()
        {
            //this.context.Database.EnsureDeleted();
            //this.context.Database.EnsureCreated();
            //this.context.Users.Add(new TblUser("testuser", "testpassword", 0));
            //this.context.SaveChanges();
            return View();
        }

        [Route("/Home/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
