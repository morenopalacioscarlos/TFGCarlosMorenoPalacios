using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMia.Controllers;
using WebMia.Services;

namespace WebMia.Interfaces
{
    public interface IInitialAlert
    {
        /// <summary>
        /// Devuelve los productos con bajo stock para un usuario específico
        /// </summary>
        /// <param name="iLogged"></param>
        /// <returns></returns>
        IEnumerable<InfoLowStockSave<StringBuilder, int>> ReturnInfoLowStockProducts(ILogged iLogged);

        /// <summary>
        /// Devuelve las monedas con un stock bajo para un usuario específico
        /// </summary>
        /// <param name="iLogged"></param>
        /// <returns></returns>
        IEnumerable<InfoLowStockSave<StringBuilder, int>> ReturnInfoLowStockCoins(ILogged iLogged);
    }
}
