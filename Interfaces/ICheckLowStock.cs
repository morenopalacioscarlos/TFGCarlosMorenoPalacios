using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMia.Services;

namespace WebMia.Interfaces
{
   public interface ICheckLowStock
    {
        /// <summary>
        /// Deuvelve los productos con bajo stock
        /// </summary>
        /// <returns></returns>
        List<InfoLowStockSave<StringBuilder, int>> ProductsCheck();

        /// <summary>
        /// Devuelve todas las monedas con bajo stock
        /// </summary>
        /// <returns></returns>
        List<InfoLowStockSave<StringBuilder, int>> CoinsStock();

    }
}
