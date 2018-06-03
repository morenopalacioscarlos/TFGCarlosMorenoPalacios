using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebMia.Controllers;
using WebMia.Interfaces;
using WebMia.Models;

namespace WebMia.Services
{
    public class CheckLowStok : ICheckLowStock
    {
        private const int MINIMUM_ITEM = 5;
        private const int NO_ITEM = 0;

      
        public CheckLowStok() { }

        /// <summary>
        /// DEvuelve una lista de máquinas que tienen bajo stock de monedas
        /// </summary>
        /// <returns></returns>
        public List<InfoLowStockSave<StringBuilder, int>> CoinsStock()
        {
       
            List<InfoLowStockSave<StringBuilder, int>> InfoLowStockList = new List<InfoLowStockSave<StringBuilder, int>>();
            using (DataBaseContext context = new DataBaseContext())
            {
                HashSet<objectCoins> coins = new HashSet<objectCoins> ();
                var userMachineId = 0;
             
                //se recorren todos los tipos de monedas españolas
                foreach (var item in context.Change.Where(x => x.Coins_1_Cents <= MINIMUM_ITEM).Select(x => new {Name = "Moneda 1 Céntimo", Coin1Cent = x.Coins_1_Cents, IdMachine = x.Id_Machine }))
                    coins.Add(new objectCoins(item.Name, item.Coin1Cent, item.IdMachine, GetMachineUserID(context, item.IdMachine)));

                foreach (var item in context.Change.Where(x => x.Coins_2_Cents <= MINIMUM_ITEM).Select(x => new { Name = "Moneda 2 Céntimos", Coin2Cent = x.Coins_2_Cents, IdMachine = x.Id_Machine }))
                    coins.Add(new objectCoins(item.Name, item.Coin2Cent, item.IdMachine, GetMachineUserID(context, item.IdMachine)));

                foreach (var item in context.Change.Where(x => x.Coins_5_Cents <= MINIMUM_ITEM).Select(x => new { Name = "Moneda 5 Céntimos", Coin5Cent = x.Coins_5_Cents, IdMachine = x.Id_Machine }))
                    coins.Add(new objectCoins(item.Name, item.Coin5Cent, item.IdMachine, GetMachineUserID(context, item.IdMachine)));

                foreach (var item in context.Change.Where(x => x.Coins_10_Cents <= MINIMUM_ITEM).Select(x => new { Name = "Moneda 10 Céntimos", Coin10Cent = x.Coins_10_Cents, IdMachine = x.Id_Machine }))
                    coins.Add(new objectCoins(item.Name, item.Coin10Cent, item.IdMachine, GetMachineUserID(context, item.IdMachine)));

                foreach (var item in context.Change.Where(x => x.Coins_20_Cents <= MINIMUM_ITEM).Select(x => new { Name = "Moneda 20 Céntimos", Coin20Cent = x.Coins_20_Cents, IdMachine = x.Id_Machine }))
                    coins.Add(new objectCoins(item.Name, item.Coin20Cent, item.IdMachine, GetMachineUserID(context, item.IdMachine)));

                foreach (var item in context.Change.Where(x => x.Coins_50_Cents <= MINIMUM_ITEM).Select(x => new { Name = "Moneda 50 Céntimos", Coin50Cent = x.Coins_50_Cents, IdMachine = x.Id_Machine }))
                    coins.Add(new objectCoins(item.Name, item.Coin50Cent, item.IdMachine, GetMachineUserID(context, item.IdMachine)));

                foreach (var item in context.Change.Where(x => x.Coins_1_Eur <= MINIMUM_ITEM).Select(x => new { Name = "Moneda 1 Euros", Coin1Eur = x.Coins_1_Eur, IdMachine = x.Id_Machine }))
                    coins.Add(new objectCoins(item.Name, item.Coin1Eur, item.IdMachine, GetMachineUserID(context, item.IdMachine)));

                foreach (var item in context.Change.Where(x => x.Coins_2_Eur <= MINIMUM_ITEM).Select(x => new { Name = "Moneda 2 Euros", Coin2Eur = x.Coins_2_Eur, IdMachine = x.Id_Machine }))
                    coins.Add(new objectCoins(item.Name, item.Coin2Eur, item.IdMachine, GetMachineUserID(context, item.IdMachine)));

                var  coinsOrderedByIdMachine = coins.OrderBy(x => x.IdMachine).ToHashSet();
                var prevIdMachine = Convert.ToInt32(coinsOrderedByIdMachine.First().IdMachine);
                var prevUserID = Convert.ToInt32(coinsOrderedByIdMachine.First().UserId);

                StringBuilder sb = new StringBuilder();
                sb.Append($"La máquina con id {Convert.ToInt32(prevIdMachine)} tiene las siguientes monedas bajas de stock:.");
                sb.AppendLine();

                //se recorre toda la lista ordenada y se clasifican todas las monedas por ide de máquinas en una tupla con el string
                foreach (var item in coinsOrderedByIdMachine)
                {
                    if (item.IdMachine == prevIdMachine)
                    {
                        sb.AppendLine();
                        sb.Append($"{item.CoinName} con una cantidad de {Convert.ToInt32(item.Coin)} monedas.");
                        prevIdMachine = item.IdMachine;
                        prevUserID = item.UserId;

                    }
                    else
                    {
                        var returnData = new Tuple<StringBuilder, int>(new StringBuilder(sb.ToString()), prevUserID);
                        InfoLowStockList.Add(new InfoLowStockSave<StringBuilder, int>(returnData));
                        sb.Clear();
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.Append($"La máquina con id {Convert.ToInt32(item.IdMachine)} tiene las siguientes monedas bajas de stock:.");
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.Append($"{item.CoinName} con una cantidad de {Convert.ToInt32(item.Coin)} monedas.");
                        prevIdMachine = item.IdMachine;
                        prevUserID = item.UserId;

                    }
                }
              
                InfoLowStockList.Add(new InfoLowStockSave<StringBuilder, int>(new Tuple<StringBuilder, int>(new StringBuilder(sb.ToString()), prevUserID)));
            }

                return InfoLowStockList;
        }


        /// <summary>
        /// Devuelve el id del administrador de la máquina pasada por parñaetro
        /// </summary>
        /// <param name="context"></param>
        /// <param name="idMachine"></param>
        /// <returns></returns>
        private int GetMachineUserID(DataBaseContext context, int idMachine)
        {
            return Convert.ToInt32(context.Vending_Machine.Where(x => x.Id_Machine == idMachine).Select(x => x.UserAdministrator).FirstOrDefault());
        }


        /// <summary>5
        /// Devuelve las máquinas pertenecientes a un administrador que tengan un stock bajo de algún producto
        /// </summary>
        /// <returns></returns>
        public List<InfoLowStockSave<StringBuilder, int>> ProductsCheck()
        {
                List<InfoLowStockSave<StringBuilder, int>> InfoLowStockList = new List<InfoLowStockSave<StringBuilder, int>>();

            //bucle que se va a ejecutar siempre para comprobar los items de las máquinas

                using (DataBaseContext context = new DataBaseContext())
                {
                    ////selecciono todos los productos que tengan stock bajo
                    var LowStockProducts = (context.Slots.Where(x => x.Stock <= MINIMUM_ITEM).
                        Select(x => new { x.Id_Machine, x.Id_Product, x.Stock })).OrderBy(VendingMachine => VendingMachine.Id_Machine);

                    var idUser = 0;
                    var prevIdUser = 0;
                    var address = string.Empty;
                    var productName = string.Empty;
  

                    StringBuilder sb = new StringBuilder();

                    foreach (var item in LowStockProducts)
                    {
                        //Se unifican todas las máquinas y productos de un mismo administrador de maquinaria

                        if (idUser == 0 || prevIdUser == 0)
                        {
                            prevIdUser = context.Vending_Machine.Where(x => x.Id_Machine == item.Id_Machine).Select(x => x.UserAdministrator).FirstOrDefault();
                            idUser = prevIdUser;

                            if (item.Id_Product != NO_ITEM)
                            {
                                address = context.Vending_Machine.Where(x => x.Id_Machine == item.Id_Machine).Select(y => y.Address).FirstOrDefault().ToString();
                                productName = context.Items.Where(x => x.Id == item.Id_Product).Select(x => x.ItemName).FirstOrDefault().ToString();
                                sb.Append($"Maquina: {item.Id_Machine} sita en {address}");
                                sb.Append($" con producto: {productName} con un stock actual de :{item.Stock} unidades. \n");
                                sb.AppendLine();
                            }
                            continue;
                        }

                        idUser = context.Vending_Machine.Where(x => x.Id_Machine == item.Id_Machine).Select(x => x.UserAdministrator).FirstOrDefault();

                        if (prevIdUser == idUser)
                        {
                            if (item.Id_Product != NO_ITEM)
                            {
                                address = context.Vending_Machine.Where(x => x.Id_Machine == item.Id_Machine).Select(y => y.Address).FirstOrDefault().ToString();
                                productName = context.Items.Where(x => x.Id == item.Id_Product).Select(x => x.ItemName).FirstOrDefault().ToString();
                                sb.Append($"Maquina: {item.Id_Machine} sita en {address}");
                                sb.Append($" con producto: {productName} con un stock actual de :{item.Stock} unidades. \n");
                                sb.AppendLine();
                            }
                        }
                        else
                        {
                        var returnData = new Tuple<StringBuilder, int>(new StringBuilder(sb.ToString()), prevIdUser);
                        sb.Clear();
                        prevIdUser = idUser;


                        InfoLowStockList.Add(new InfoLowStockSave<StringBuilder, int>(returnData));
                        }
                        
                }
                var returnDataFinally = new Tuple<StringBuilder, int>(new StringBuilder(sb.ToString()), prevIdUser);
                InfoLowStockList.Add(new InfoLowStockSave<StringBuilder, int>(returnDataFinally));
            }

            return InfoLowStockList;
        }

    }

    /// <summary>
    /// Clase interna utilizada para los objetos de de monedas , identidad de máquina y la id de su administrador
    /// </summary>
    internal class objectCoins
    {
        public int IdMachine { private set; get; }
        public string CoinName { private set; get; }
        public int Coin { private set; get; }
        public int UserId { private set; get; }

        public objectCoins(string coinName, int coin, int idMachine, int userId)
        {
            this.CoinName = coinName;
            this.Coin = coin;
            this.IdMachine = idMachine;
            this.UserId = userId;
        }
    }



}
