using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMia.Interfaces;
using WebMia.Models;
using System.Data.SqlClient;
using System.Data;

namespace WebMia.Controllers
{
    public class StatsController : Controller
    {
        private ILogged isLogged;
        private DataBaseContext context;
        private static HashSet<DateTime> dateInfo = null;
        private static string itemSelected = string.Empty;

        public StatsController(ILogged isLogged, DataBaseContext context)
        {
            this.isLogged = isLogged;
            this.context = context;
        }

        /// <summary>
        /// Devuelve la vista de la gestión de estadística principal
        /// </summary>
        /// <returns></returns>
        public IActionResult StatsGestion()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }


        //Limpia los datos de la parcial View y devuelve la vista de las estadística de los productos
        public IActionResult ProductsStatsChange()
        {
           if(dateInfo != null)
                dateInfo.Clear();

            return RedirectToAction("ProductsStats");
        }

        /// <summary>
        /// Devuelve la vista de las estadísticas de los productos
        /// </summary>
        /// <returns></returns>
        public IActionResult ProductsStats()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            //Si hay datos para las estadísticas se manda la información con objetos dinámicos
            if (dateInfo != null && dateInfo.Count > 0)
            {
                ViewBag.DateInfo = dateInfo;
                ViewBag.itemSelected = itemSelected;
            }

            ViewBag.Items = context.Items.ToList();

            return View();
        }

        /// <summary>
        /// Devuelve la información para generar una gráfica sobre las estadísticas de las máquinas del último año
        /// </summary>
        /// <returns></returns>
        public IActionResult PartialMachineStats(string arrayofvalues)
        {
            //Se almacena el id de la máquina seleccionada
            itemSelected = arrayofvalues;

            if (dateInfo == null)
                dateInfo = new HashSet<DateTime>();

            if (dateInfo.Count > 0)
                dateInfo.Clear();

            SqlConnection sqlConnection1 = new SqlConnection(context.GetDataBaseConectionString());
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            DateTime currentDate = DateTime.Now;
            var currentYear = currentDate.Year;

            string query = $"select date from solded where IdMachine = {arrayofvalues} and Date >= '{currentYear}' ";

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            cmd.Connection.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dateInfo.Add((DateTime)reader["Date"]);
                }
            }

            cmd.Connection.Close();

           return  RedirectToAction("MachineStats");
        }
        

        /// <summary>
        /// Devuelve la información para generar una gráfica sobre las estadísticas de los productos
        /// </summary>
        /// <returns></returns>
        public IActionResult PartialProductsStats(string arrayofvalues)
        {
            //Se almacena el nombre del item seleccionado
            itemSelected = context.Items.Where(x=>x.Id == Convert.ToInt32(arrayofvalues)).Select(x=>x.ItemName).First();

            if (dateInfo == null)
                dateInfo = new HashSet<DateTime>();

            if (dateInfo.Count > 0)
                dateInfo.Clear();

            SqlConnection sqlConnection1 = new SqlConnection(context.GetDataBaseConectionString());
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            DateTime currentDate = DateTime.Now;
            var currentYear = currentDate.Year;

            string query = $"select date from solded where IdProduct = {arrayofvalues} and Date >= '{currentYear}'";

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            cmd.Connection.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dateInfo.Add((DateTime)reader["Date"]);
                }
            }

            cmd.Connection.Close();

            return RedirectToAction("ProductsStats");
        }

        //Limpia los datos de la parcial View y devuelve la vista de las estadística de las máquinas
        public IActionResult MachineStatsChange()
        {
            if(dateInfo != null)
                 dateInfo.Clear();

            return RedirectToAction("MachineStats");
        }


        /// <summary>
        /// Devuelve la vista de las estadísticas de las máquinas
        /// </summary>
        /// <returns></returns>
        public IActionResult MachineStats()
        {

            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            if (dateInfo != null && dateInfo.Count > 0)
            {
                ViewBag.DateInfo = dateInfo;
                ViewBag.itemSelected = itemSelected;
            } 

            ViewBag.ListMachineList = context.Vending_Machine.ToList();

            return View();
        }
    }
}
