// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        13.09.2019 10:15
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MFA_QR_CODE.Models;
using Microsoft.AspNetCore.Authorization;

namespace MFA_QR_CODE.Controllers
{
    /// <summary>
    /// Der Home Controller. Das <see cref="AutoValidateAntiforgeryTokenAttribute"/> ist gesetzt, um XSRF Attacken zu mitigieren.
    /// </summary>
    /// AntiforgeryToken sollte normalerweise aktiviert sein. Für das Demo ist er deaktiviert.
    // [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="HomeController"/> Klasse.
        /// </summary>
        /// <param name="logger"></param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Index Methode des Home Controllers.
        /// </summary>
        /// <returns>Ein View Objekt.</returns>
        [Route("")]
        [Route("Home")]
        [Route("Home/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Privacy Methode des Home Controllers.
        /// </summary>
        /// <returns>Ein View Objekt.</returns>
        [Route("Home/Privacy")]
        [Route("Privacy")]
        [Authorize]
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Eine Methode die eine unsichere Handlung darstellt, welche gegen XSRF geschützt werden muss, zb Banküberweisung.
        /// </summary>
        /// <param name="model">Das Modell für die Transaktion die simuliert wird.</param>
        /// <returns>Ein View Objekt.</returns>
        [Route("Home/DoSomethingUnsafe")]
        [Authorize]
        [HttpPost]
        public IActionResult DoSomethingUnsafe(TransactionAmountModel model)
        {
            return View(model);
        }

        /// <summary>
        /// Eine Methode die eine sichere Handlung darstellt, die trotz POST Requests nicht vor XSRF geschützt werden muss.
        /// </summary>
        /// <returns>Ein View Objekt.</returns>
        [Route("Home/DoSomethingSafe")]
        [Authorize]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult DoSomethingSafe()
        {
            return View();
        }

        [Route("Home/XSRF")]
        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public IActionResult XSRF()
        {
            return View(new TransactionAmountModel());
        }

        [Route("Home/About")]
        [HttpGet]
        [IgnoreAntiforgeryToken]
        public IActionResult About()
        {
            return View();
        }
    }
}
