using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebMia.Models;
using WebMia.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Net;
using System.Text.RegularExpressions;

namespace WebMia.Controllers
{
    public enum OperationType
    {
        Delete,
        Update,
        Add
    }

    public class MachineController : Controller
    {

        #region Fields
        private DataBaseContext context;
        private ILogged isLogged;
        private ILoggingTime loggingTime;


        //Listas usadas en la comunicación entre partial views y views
        private static List<UnionProducts> unionList;
        private static List<Object> unionListChange;
        private static List<UnionProducts> unionListStock;

        #endregion

        /// <summary>
        /// Obtencion del contexto de Entity Framework, loggin 
        /// </summary>
        /// <param name="context"></param>
        public MachineController(DataBaseContext context, ILogged isLogged, ILoggingTime loggingTime)
        {
            this.context = context;
            this.isLogged = isLogged;
            this.loggingTime = loggingTime;
        }



        /// <summary>
        /// Obtencion de los datos de la base de datos de las listas de máquinas vending disponibles
        /// </summary>
        /// <returns></returns>
        public IActionResult MachineList()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //prepara los datos de la lista de Maquinas Dadas de alta
            var datos = context.Vending_Machine.ToList();
            ViewBag.ListMachineList = datos;

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }

        
        /// <summary>
        /// Devuelve la vista de la ubicación de las máquinas
        /// </summary>
        /// <returns></returns>
        public IActionResult MachineUbication()
        {

            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            var machine = context.Vending_Machine.ToList();
            ViewBag.ListMachineList = machine;

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }


        /// <summary>
        /// Return View Alta Nueva Maquina
        /// </summary>
        /// <returns></returns>
        public IActionResult NewMachine()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }

        /// <summary>
        /// Alta nueva Maquina Vending
        /// </summary>
        [HttpPost]
        public IActionResult SaveNewMachine()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            var modelo = Request.Form["Model"];
            var ciudad = Request.Form["City"];
            var direccion = Request.Form["Address"];
            var poblacion = Request.Form["Poblation"];
            var slots = Convert.ToInt32(Request.Form["Slots"]);
            var tokenPassWord = isLogged.getNewToken();

            context.Vending_Machine.Add(new Vending_Machine() { Machine_Model = modelo,
                Address = direccion, City = ciudad, Poblation = poblacion, TokenId = tokenPassWord });

            if (context.SaveChanges() > 0)
            {
                var newIdMachine = context.Vending_Machine
                      .Where(x => x.Id_Machine == context.Vending_Machine.Last().Id_Machine).First();


                for (int slot = 1; slot <= slots; slot++)
                {
                    context.Slots.Add(new Slots() { Slot_Number = slot, Id_Machine = newIdMachine.Id_Machine, Id_Product = 0, Price = 0, Stock = 0 });
                    context.SaveChanges();
                }

                TempData["InfoNewMachine"] = "Alta realizada correctamente";
            }

            return RedirectToAction("NewMachine", "Machine");
        }

        /// <summary>
        /// Guardado de los datos de las máquinas en la base de datos
        /// </summary>
        /// <param name="vendingMachine"></param>
        /// <param name="operationType"></param>
        public void ChangeVendingContext(Object vendingMachine, OperationType operationType)
        {
            Vending_Machine vending_Machine = vendingMachine as Vending_Machine;
            if (vendingMachine != null)
            {

                if (vending_Machine.Id_Machine >= 0 &&
                    vending_Machine.Machine_Model != null &&
                    vending_Machine.City != null &&
                    vending_Machine.Address != null &&
                    vending_Machine.Poblation != null)
                {

                    var vendingMachineOperation = context.Vending_Machine.Find(vending_Machine.Id_Machine);

                    if (operationType == OperationType.Update)
                    {
                        vendingMachineOperation.Address = vending_Machine.Address;
                        vendingMachineOperation.City = vending_Machine.City;
                        vendingMachineOperation.Machine_Model = vending_Machine.Machine_Model;
                        vendingMachineOperation.Poblation = vending_Machine.Poblation;

                        context.Vending_Machine.Update(vendingMachineOperation);
                    }


                    if (operationType == OperationType.Delete)
                    {
                        //eliminamos todos los datos de las tablas que tengan relaciones con clave foranea
                        try
                        {
                            var changeElementToDelete = context.Change.Find(context.Change.Where(x => x.Id_Machine == vending_Machine.Id_Machine)
                                .FirstOrDefault().Id_Change); //Para eliminar vending machine hay que eliminar antes change

                            foreach (var item in context.Slots.ToList().Where(x => x.Id_Machine == vending_Machine.Id_Machine))
                                context.Slots.Remove(item);

                            context.Change.Remove(changeElementToDelete);
                        }
                        catch { }
                        finally
                        {
                            context.Vending_Machine.Remove(vendingMachineOperation);
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Borrado de los items
        /// </summary>
        /// <param name="itemToDelete"></param>
        public void DeleteContextItem(object itemToDelete)
        {
            ChangeVendingContext(itemToDelete, OperationType.Delete);
        }

        
        /// <summary>
        /// Devuelve la vista con los productos disponibles
        /// </summary>
        /// <returns></returns>
        public IActionResult Products()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            ViewBag.ListMachineList = context.Vending_Machine.ToList();
            ViewBag.ItemList = context.Items.ToList();

            if (unionList != null && unionList.Count > 0)
            {
                ViewBag.Products = unionList.ToList();
            }

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }

        /// <summary>
        /// Clase con el join de distintas tablas
        /// </summary>
         public class UnionProducts
         {
            public string Product= string.Empty;
            public int Slot_Number = 0;
            public int Stock = 0;
            public decimal Price = 0;
            public int Id_Machine = 0;
            public int Id_Slot = 0;
            public int Id_Product = 0;
         }

        /// <summary>
        /// Devuelve un view con el stock de los productos
        /// </summary>
        /// <returns></returns>
        public IActionResult Stock()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            ViewBag.Items = context.Items.ToList();

            if  (unionListStock != null && unionListStock.Count > 0)
                ViewBag.MachineItems = unionListStock;

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }


        /// <summary>
        /// Devuelve un view con el cambio de las máquinas
        /// </summary>
        /// <returns></returns>
        public IActionResult Change()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            if (unionListChange != null && unionListChange.Count > 0 && unionListChange.First() != null)
                ViewBag.Change = unionListChange.ToList();
            else
                ViewBag.Change = null;
                    
           ViewBag.ListMachineList = context.Vending_Machine.ToList();

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }


        /// <summary>
        /// Devuelve una partialView con los datos de los items
        /// </summary>
        /// <param name="arrayofvalues"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PartialItemsView(string arrayofvalues)
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            if (unionListStock == null)
                unionListStock = new List<UnionProducts>();
            else
                unionListStock.Clear();

            IQueryable<int> idListMachine = null;

            if (arrayofvalues == null)
                RedirectToAction("Stock", "Machine");

             idListMachine = context.Slots.Where(x => x.Id_Product == Convert.ToInt32(arrayofvalues)).Select(x=> x.Id_Machine);
            

            foreach (var slot in idListMachine)
            {
                var unionProducts = (from a in context.Slots
                                     join c in context.Items 
                                      on a.Id_Product equals c.Id
                                     where a.Id_Machine == slot 
                                     && a.Id_Product == Convert.ToInt32(arrayofvalues)
                                     select new UnionProducts()
                                     {
                                         Product = c.ItemName,
                                         Slot_Number = a.Slot_Number,
                                         Stock = a.Stock,
                                         Price = a.Price,
                                         Id_Machine = a.Id_Machine,
                                         Id_Slot = a.Id_Slot,
                                         Id_Product = a.Id_Product
                                     });

                unionListStock.Add(unionProducts.FirstOrDefault());
            }
           
            return RedirectToAction("Stock");
        }


        /// <summary>
        /// Devuelve una partial view con los datos de los slots de las máquinas
        /// </summary>
        /// <param name="arrayofvalues"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PartialChangeView(string arrayofvalues)
        {
            if (unionListChange == null)
                unionListChange = new List<object>();
            else
                unionListChange.Clear();

            if (arrayofvalues == string.Empty)
                RedirectToAction("Change", "Machine");

            unionListChange.Add(context.Change.Where(x => x.Id_Machine == Convert.ToInt32(arrayofvalues))
                .Select(x => new Change{Id_Machine = x.Id_Machine, Id_Change = x.Id_Change, Coins_1_Cents = x.Coins_1_Cents,
                    Coins_2_Cents = x.Coins_2_Cents,Coins_5_Cents = x.Coins_5_Cents,Coins_10_Cents = x.Coins_10_Cents,
                   Coins_20_Cents = x.Coins_20_Cents,Coins_50_Cents = x.Coins_50_Cents,Coins_1_Eur = x.Coins_1_Eur,Coins_2_Eur = x.Coins_2_Eur }).FirstOrDefault() as Change);

           return RedirectToAction("Change");
        }


        /// <summary>
        /// Devuelve una partial view con los datos de los slots de las máquinas
        /// </summary>
        /// <param name="arrayofvalues"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PartialSlotsView(string arrayofvalues)
        {
            if (unionList == null)
                unionList = new List<UnionProducts>();
            else
                unionList.Clear();

            IQueryable<int> idListSlots = null;

            if (arrayofvalues == string.Empty)
                RedirectToAction("Products", "Machine");

            else if (arrayofvalues == "Refresh")
                idListSlots = context.Slots.Where(x => x.Id_Machine == (context.Vending_Machine.FirstOrDefault().Id_Machine))
                    .Select(x => x.Id_Slot);
            else
                idListSlots = context.Slots.Where(x => x.Id_Machine == Convert.ToInt32(arrayofvalues)).Select(x => x.Id_Slot);

            
            foreach (var slot in idListSlots)
            {
                var unionProducts = (from a in context.Slots
                            join c in context.Items on a.Id_Product equals c.Id
                            where a.Id_Slot == slot
                            select new UnionProducts()
                            {
                                Product = c.ItemName,
                                Slot_Number = a.Slot_Number,
                                Stock = a.Stock,
                                Price = a.Price,
                                Id_Machine = a.Id_Machine,
                                Id_Slot = a.Id_Slot,
                                Id_Product = a.Id_Product

                            });
                
                unionList.Add(unionProducts.FirstOrDefault());
            }
           
          
            return RedirectToAction("Products");
        }

        /// <summary>
        /// Actualizacion base de datos con los cambios de los productos disponibles
        /// </summary>
        /// <param name="arrayofvalues"></param>
        public void SaveChangeDataProducts(string[] arrayofvalues)
        {
            int machineId = 0;
            string productId = string.Empty;
            int slot = 0;
            int stock = 0;
            decimal price = 0;
            int idSlot = 0;

            string regularExpZeroDecimal = @"(#[0-9]{1}#)";
            string regularExpOneDecimal = @"(#[0-9]{1}(\,)[0-9]{1}#)";
            string multipleInteger = @"(#[0-9]*#)";


            Regex regex;


            for (int i = 7; i < arrayofvalues.Length; i++)
            {
                switch (i % 7)
                {
                    //null siempre
                    case 0:
                        break;
                    //id Máquina
                    case 1:
                        machineId = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //nombre producto
                    case 2:
                        productId = arrayofvalues[i];
                        break;
                    //slot de la maquina
                    case 3:
                        slot = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //stockaje
                    case 4:
                        stock = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //precio
                    case 5:

                        //El precio está estandarizado a 2 decimales, por lo que se controla que sea así
                        if (arrayofvalues[i].Contains("."))
                            arrayofvalues[i] = arrayofvalues[i].Replace(".", ",");

                        var number = "#" + arrayofvalues[i] + "#";
                        
                        regex = new Regex(regularExpZeroDecimal);
                        if (regex.IsMatch(number))
                        {
                            number =  number.Replace("#", "");
                            number += ",00";
                            price = Convert.ToDecimal(number);
                            break;

                        }

                        regex = new Regex(regularExpOneDecimal);
                        if (regex.IsMatch(number))
                        {
                            number = number.Replace("#", "");
                            number += "0";
                            price = Convert.ToDecimal(number);
                            break;
                        }

                        regex = new Regex(multipleInteger);
                        if (regex.IsMatch(number))
                        {  
                            price = 0;
                        }

                        string value = arrayofvalues[i].Substring(0, 4);
                        price = Convert.ToDecimal(value);
                        break;

                        //id del slot
                    case 6:
                        idSlot = Convert.ToInt32(arrayofvalues[i]);

                        //una vez recorrida la primera la primera fila agregamos el cambio en el contexto
                        //Actualizacion de slots de máquina

                        Slots slotObject = context.Slots.Find(idSlot);
                        slotObject.Id_Product = Convert.ToInt32(productId);
                        slotObject.Id_Machine = machineId;

                        //Si el precio no es el mismo que el anterior, se debe incluir la actualización en otra tabla
                        if (slotObject.Price != price)
                        {
                            var updatePrice = new PriceUpdated();
                            updatePrice.IdMachine = machineId;
                            updatePrice.IdProduct = Convert.ToInt32(productId);
                            updatePrice.IdSlot = idSlot;
                            updatePrice.NewPrice = price;
                            context.PriceUpdated.Add(updatePrice);
                            context.SaveChanges();
                        }
                            
                        slotObject.Price = price;
                        slotObject.Slot_Number = slot;
                        slotObject.Stock = stock;

                        context.Slots.Update(slotObject);

                        //confirmar cambios en base de datos
                        context.SaveChanges();
                        break;
                }
            }
            PartialSlotsView("Refresh");
        }


        
        /// <summary>
        /// Devuelve una vista con los productos disponibles y permite editarlos
        /// </summary>
        /// <returns></returns>
        public IActionResult ProductsEdition()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            ViewBag.ProductsEdition = context.Items.ToList();

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }
        

        /// <summary>
        /// Actualizacion base de datos con los cambios de la tabla del  cambio de cada máquina
        /// </summary>
        /// <param name="arrayofvalues"></param>
        [HttpPost]
        public IActionResult SaveChangeDataChange(string[] arrayofvalues)
        {

            int changeId = 0;
            int machineId = 0;
            int oneCent = 0;
            int twoCent = 0;
            int fiveCent = 0;
            int tenCent = 0;
            int twentyCent = 0;
            int fiftyCent = 0;
            int oneEur = 0;
            int twoEur = 0;


            //Si no es una nueva row, se recorre para eliminar o editar
            for (int i = 1; i < arrayofvalues.Length; i++)
                {
                    switch (i % 11)
                    {
                    //Ide del change 
                    case 1:
                        changeId = Convert.ToInt32(arrayofvalues[i]);
                        break;

                    //Ide de l máquina
                    case 2:
                        machineId = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //Moneda de 1 céntimo
                    case 3:
                        oneCent = Convert.ToInt32(arrayofvalues[i]);
                        break;

                    //Moneda de 2 céntimo
                    case 4:
                        twoCent = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //Moneda de 5 céntimo
                    case 5:
                        fiveCent = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //Moneda de 10 céntimo
                    case 6:
                        tenCent = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //Moneda de 20 céntimo
                    case 7:
                        twentyCent = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //Moneda de 50 céntimo
                    case 8:
                        fiftyCent = Convert.ToInt32(arrayofvalues[i]);
                        break;

                    //Moneda de 1 Euro
                    case 9:
                        oneEur = Convert.ToInt32(arrayofvalues[i]);
                        break;

                    //Moneda de 2 Euros
                    case 10:
                        twoEur = Convert.ToInt32(arrayofvalues[i]);

                            Change change = null;

                            try { change = context.Change.Find(changeId); } catch (Exception e) { }

                            change.Coins_1_Cents = oneCent;
                            change.Coins_2_Cents = twoCent;
                            change.Coins_5_Cents = fiveCent;
                            change.Coins_10_Cents = tenCent;
                            change.Coins_20_Cents = twoCent;
                            change.Coins_50_Cents = fiftyCent;
                            change.Coins_1_Eur = oneEur;
                            change.Coins_2_Eur = twoEur;

                            break;
                    }
            }
            context.SaveChanges();
            return Json(new { success = true });
        }


        /// <summary>
        /// Actualizacion base de datos con los cambios de la tabla de los items
        /// </summary>
        /// <param name="arrayofvalues"></param>
        [HttpPost]
        public IActionResult SaveChangeDataItems(string[] arrayofvalues)
        {

            int id = 0;
            string ItemName = string.Empty;
            const int NEWROW = -2;

            //Si es una nueva Row se debe crear un nuevo elemento
            if(Convert.ToInt32(arrayofvalues[0]) == NEWROW)
            {
                Items newItem = new Items();
                newItem.ItemName = arrayofvalues[4];
                ChangeItemContext(newItem, OperationType.Add);
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {

                //Si no es una nueva row, se recorre para eliminar o editar
                for (int i = 3; i < arrayofvalues.Length; i++)
                {
                    switch (i % 3)
                    {
                        //Ide del item
                        case 0:
                            id = Convert.ToInt32(arrayofvalues[i]);
                            break;
                        //INombre del producto
                        case 1:
                            ItemName = arrayofvalues[i];
                            break;

                        //Procesado de la info
                        case 2:
                            Items item = null;

                            try {  item = context.Items.Find(id); } catch (Exception e) { }
                        
                            item.ItemName = ItemName;

                            if (Convert.ToInt32(arrayofvalues[0]) == -1 && arrayofvalues[1] == "eliminar" && arrayofvalues[2] == "eliminar")
                                ChangeItemContext(item, OperationType.Delete);
                            else
                                ChangeItemContext(item, OperationType.Update);

                            break;
                    }
                }
            }
            context.SaveChanges();
            return Json(new { success = true });
        }

        /// <summary>
        /// Actualizacion base de datos con los cambios de la tabla de los items
        /// </summary>
        /// <param name="arrayofvalues"></param>
        [HttpPost]
        public IActionResult SaveChangeDataStockProducts(string[] arrayofvalues)
        {


            int idMachine = 0;
            string ItemName = string.Empty;
            int productsStock = 0;
            int idSlot = 0;

                //Si no es una nueva row, se recorre para eliminar o editar
                for (int i = 6; i < arrayofvalues.Length; i++)
                {
                    switch (i % 6)
                    {
                        //null, casilla vacia
                        case 0:
                            break;

                        //Id máquina
                        case 1:
                            idMachine = Convert.ToInt32(arrayofvalues[i]);
                            break;

                        //Nombre producto
                        case 2:

                            ItemName = arrayofvalues[i];
                            break;

                        //Stock
                    case 3:

                        productsStock = Convert.ToInt32(arrayofvalues[i]);
                        break;
    
                        //id Slog

                    case 4:

                        idSlot = Convert.ToInt32(arrayofvalues[i]);
                        break;

                        //id producto
                    case 5:
                        Slots slot = null;

                            try { slot = context.Slots.Find(idSlot); } catch (Exception e) { }

                        slot.Stock = productsStock;

                        context.Slots.Update(slot);
                        context.SaveChanges();
                            break;
                }
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// Almacena en la base de datos los cambios en los items
        /// </summary>
        /// <param name="item"></param>
        /// <param name="operation"></param>
        public void ChangeItemContext(Items item, OperationType operation)
        {
            if(operation == OperationType.Delete)
            {
                context.Items.Remove(item);
                context.SaveChanges();
            }

            if (operation == OperationType.Add)
            {
                context.Items.Add(item);
                context.SaveChanges();
            }
        }

        
        /// <summary>
        /// Actualizacion base de datos con los cambios de la tabla de las maquinas disponibles
        /// </summary>
        /// <param name="arrayofvalues"></param>
        public void SaveChangeData(string[] arrayofvalues)
        {
            int id = 0;
            string modelo = string.Empty;
            string ciudad = string.Empty;
            string direccion = string.Empty;
            string poblacion = string.Empty;

            for (int i = 6; i < arrayofvalues.Length; i++)
            {
                switch (i % 6)
                {
                    //Ide de la máquina
                    case 0:
                            id = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //Modelo Máquina
                    case 1:
                            modelo = arrayofvalues[i];
                        break;
                    //Ciudad
                    case 2:
                            ciudad = arrayofvalues[i];
                        break;
                    //Poblacion
                    case 3:
                            poblacion = arrayofvalues[i];
                        break;
                    //Direccion
                    case 4:
                        direccion = arrayofvalues[i];                        
                        break;
                    //proceso de eliminado y modificado
                    case 5:

                        //una vez recorrida la primera la primera fila agregamos el cambio en el contexto
                        Vending_Machine vending_Machine = context.Vending_Machine.Find(id);

                        vending_Machine.Machine_Model = modelo;
                        vending_Machine.City = ciudad;
                        vending_Machine.Address = direccion;
                        vending_Machine.Poblation = poblacion;
                        
                        if (Convert.ToInt32(arrayofvalues[0]) == -1 && arrayofvalues[1] == "eliminar"
                            && arrayofvalues[2] == "eliminar" && arrayofvalues[3] == "eliminar" && arrayofvalues[4] == "eliminar")
                            ChangeVendingContext(vending_Machine, OperationType.Delete);
                        else
                            ChangeVendingContext(vending_Machine, OperationType.Update);

                        break;
                }
            }
        }
    }
}
