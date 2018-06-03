using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using WebMia.Interfaces;
using WebMia.Services;

namespace WebMia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogged isLogged;
        private IInitialAlert iInitialAlert;

        public HomeController(ILogged isLogged, IInitialAlert iInitialAlert)
        {
            this.isLogged = isLogged;
            this.iInitialAlert = iInitialAlert;
        }

        /// <summary>
        /// DEvuelve la vista inicial de la aplicación web
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            if (!isLogged.IsUserLogged())
            {
                return RedirectToAction("Login", "Security");
            }


            //Obtengo datos de alerta de stock bajo de productos y de monedas correspondiente al usuario logueado
            List<InfoLowStockSave<StringBuilder, int>> lowStockList = new List<InfoLowStockSave<StringBuilder, int>>();

            //se añade la información de los productos con bajo stock
            ViewBag.LowStockAlert = iInitialAlert.ReturnInfoLowStockProducts(isLogged);

            //se añade la información de las moendas con baja cantidad
            ViewBag.LowCoinsAlert = iInitialAlert.ReturnInfoLowStockCoins(isLogged);


            ViewBag.UserLoggedId = isLogged.GetUserIdLogged();

            return View();
        }

        /// <summary>
        /// Devuelve la vista de la gestión de las máquinas
        /// </summary>
        /// <returns></returns>
        public IActionResult MachineGestion()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }

        /// <summary>
        /// Devuelve la vista de la gestión de los usuarios de la plataforma
        /// </summary>
        /// <returns></returns>
        public IActionResult UserGestion()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }
    }
}

