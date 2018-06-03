using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMia.Interfaces;
using System.Text;
using WebMia.Services;

namespace WebMia.Controllers
{
    public class InitialAlerts : Controller, IInitialAlert
    {
        private ICheckLowStock checkLowStock;


        public InitialAlerts(ICheckLowStock checkLowStock)
        {
            this.checkLowStock = checkLowStock;
        }

        /// <summary>
        /// Obtiene todos los productos con un stock bajo para el usuario actual
        /// </summary>
        /// <param name="iLogged"></param>
        /// <returns></returns>
        public IEnumerable<InfoLowStockSave<StringBuilder, int>> ReturnInfoLowStockProducts(ILogged iLogged)
        {
           var userLowStockProducts = new HashSet<InfoLowStockSave<StringBuilder, int>>();


            //Solo devuelve los productos con bajo stock que se correspondan con el usuario logueado
            foreach (var item in checkLowStock.ProductsCheck())
                if (item.id == iLogged.GetUserIdLogged())
                    userLowStockProducts.Add(item);

            return userLowStockProducts;
        }

        /// <summary>
        /// Obtiene todos las monedas que tengan un stock bajo para el usuario actual
        /// </summary>
        /// <param name="iLogged"></param>
        /// <returns></returns>
        public IEnumerable<InfoLowStockSave<StringBuilder, int>> ReturnInfoLowStockCoins(ILogged iLogged)
        {
            var userLowStockCoins = new HashSet<InfoLowStockSave<StringBuilder, int>>();

            //Solo devuelve las monedas con bajo stock que se correspondan con el usuario logueado
            foreach (var item in checkLowStock.CoinsStock())
                if (item.id == iLogged.GetUserIdLogged())
                    userLowStockCoins.Add(item);

            return userLowStockCoins;
        }
    }
}