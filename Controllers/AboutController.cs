using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMia.Interfaces;

namespace WebMia.Controllers
{
    public class AboutController : Controller
    {
        private ILogged isLogged;
        public AboutController(ILogged isLogged)
        {
            this.isLogged = isLogged;
        }

        /// <summary>
        /// Devuelve la view de información de la web y su administración
        /// </summary>
        /// <returns></returns>
        public IActionResult About()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            return View();
        }

    }
}
