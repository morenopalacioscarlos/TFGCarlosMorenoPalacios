using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMia.Models;
using WebMia.Interfaces;
using System.Data.SqlClient;
using System.Data;

namespace WebMia.Controllers
{
    public class MessagesController : Controller
    {
        private DataBaseContext context;
        private ILogged isLogged;
        private static List<Tuple<int, int, string>> messageList = null;

        public MessagesController(DataBaseContext context, ILogged isLogged)
        {
            this.context = context;
            this.isLogged = isLogged;
        }

        /// <summary>
        /// Devuelve View de los Mensajes
        /// </summary>
        /// <returns></returns>
       public IActionResult MessageGestion()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            if (messageList != null && messageList.Count > 0)
                ViewBag.MessageList = messageList;

            ViewBag.Users = context.Security.ToList();
            ViewBag.ActualUser = isLogged.GetUserIdLogged();
            

            return View();
        }

        /// <summary>
        /// Guarda los mensajes enviados a destinatarios en base de datos
        /// </summary>
        /// <param name="arrayofvalues"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult MessageSave()
        {
            var message = Request.Form["messageToSend"];
            int userIdFrom = Convert.ToInt32(Request.Form["userIdFrom"]);
            int userIdTo = Convert.ToInt32(Request.Form["userIdTo"]);


            if (message != string.Empty && userIdFrom != 0 && userIdTo != 0)
            {

                SqlConnection sqlConnection1 = new SqlConnection(context.GetDataBaseConectionString());
                SqlCommand cmd = new SqlCommand();


                string query = $"insert into Message values({userIdFrom}, '{message}', {userIdTo})";

                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();

                cmd.Connection.Close();
            }
            return PartialMessageView(new String[] { userIdTo.ToString() }); 
        }

        /// <summary>
        /// Devuelve los mensajes entre usuario actual y destinatario
        /// </summary>
        /// <param name="arrayofvalues"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PartialMessageView(string[] arrayofvalues)
        {
            if(messageList == null)
                   messageList = new List<Tuple<int, int, string>>();

            if (messageList.Count > 0)
                messageList.Clear();

            if (arrayofvalues.Length > 0)
            {
                
                //se añade a la tupla el usuario destinatario
                int UserIdTo = Convert.ToInt32(arrayofvalues.FirstOrDefault());
                messageList.Add(new Tuple<int, int, string>(0, UserIdTo, context.Security.Where(y => y.User_Id == UserIdTo).Select(x => x.Nombre).FirstOrDefault().ToString()));

                SqlConnection sqlConnection1 = new SqlConnection(context.GetDataBaseConectionString());
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader = null;
                int actualUser = isLogged.GetUserIdLogged();
                
                string query = $"select Message, UserIdFrom, UserIdTo  from Message where (UserIdFrom = {arrayofvalues.FirstOrDefault()} " +
                    $"and UserIdTo = {actualUser}) or (UserIdFrom = {actualUser}   and UserIdTo = {arrayofvalues.FirstOrDefault()})";

                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {   
                    while (reader.Read())
                    {
                        messageList.Add(new Tuple<int, int, string>((int)reader["UserIdFrom"], (int)reader["UserIdTo"],
                            (string)reader["Message"]));
                    }
                }

                cmd.Connection.Close();
            }
            return RedirectToAction("MessageGestion");
        }
    } 
}