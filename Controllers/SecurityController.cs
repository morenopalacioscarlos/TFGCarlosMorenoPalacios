using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMia.Models;
using WebMia.CustomControls;
using WebMia.Interfaces;
using static WebMia.CustomControls.MailService;
using WebMia.Services;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;

namespace WebMia.Controllers
{
    public class SecurityController : Controller, ILogged
    {
        #region Fields
        
        private DataBaseContext securityContext;
        private IMail sendEmail;
        private  ILoggingTime logginTime;
        public static UserLogged userLogged { get; private set; }

        #endregion

        #region Constructor
        public SecurityController(DataBaseContext securityContext, IMail sendEmail, ILoggingTime logginTime)
        {
            this.securityContext = securityContext;
            this.sendEmail = sendEmail;
            this.logginTime = logginTime;
        }

        #endregion


        /// <summary>
        /// Control login con base de datos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult LoginControl()
        {
            var InputUser = SecurityConversor.Md5Calculator(Request.Form["User"]);
            var InputPassword = SecurityConversor.Md5Calculator(Request.Form["Password"]);
            var securityContextList = securityContext.Security.ToList();

            if (securityContextList.Exists(x => x.User_Name == InputUser && x.Password == InputPassword))
            {
                //instancia singleton de login
                userLogged = UserLogged.GetInstance(InputUser);

                //inserción de datos de loging en la tabla correspondiente
                logginTime.NewUserLogin(GetUserIdLogged());

                if (!IsUserLogged())
                    return RedirectToAction("Login", "Security");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorLogin"] = "Usuario y/o Contraseña incorrectos";
                return RedirectToAction("Login", "Security");
            }
        }

        /// <summary>
        /// Devuelve el usuario logueado
        /// </summary>
        /// <returns></returns>
       public string GetUserLogued()
        {
            return userLogged.User;
        }

        /// <summary>
        /// Obtiene al usuario actual
        /// </summary>
        /// <returns></returns>
        public Admin GetActualUser()
        {
            return securityContext.Security.Where(x => x.User_Id == GetUserIdLogged()).First();
        }

        /// <summary>
        /// Devuelve el id del usuario logueado
        /// </summary>
        /// <returns></returns>
        public int GetUserIdLogged()
        {
            return securityContext.Security.Where(x => x.User_Name == userLogged.User).Select(x => x.User_Id).FirstOrDefault();
        }

        /// <summary>
        /// Devuelve el rolid del usuario logueado
        /// </summary>
        public int GetUserRolId()
        {
            return securityContext.Security.Where(x => x.User_Id == GetUserIdLogged()).Select(x => x.UserRolId).FirstOrDefault();
        }

        
        /// <summary>
        /// Carga pantalla inicial del Login
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Control para ver si el usuario está logueado
        /// </summary>
        /// <returns></returns>
        public bool IsUserLogged()
        {
            return (userLogged == null) ? false : true;
        }


        /// <summary>
        /// Cerrar sesión de usuario
        /// </summary>
        /// <returns></returns>
        public IActionResult CloseSession()
        {
            userLogged = null;
            UserLogged.Dispose();
            return RedirectToAction("Login", "Security");
        }

        [HttpPost]
        public IActionResult SendEMail()
        {

                string user = Request.Form["user"];
                string inputUserMdf = SecurityConversor.Md5Calculator(user);
                string email = Request.Form["UserMail"];
                var existUser = securityContext.Security.Where(x => x.User_Name == inputUserMdf && x.Email == email).Select(x => new { x.User_Name , x.User_Id});
                string newRandomPassword = string.Empty;
                bool ok = false;

                try
                {
                    existUser.First();
                    newRandomPassword = GetNewRandomPassword();
                    Admin contextToUpdate = securityContext.Security.Find(existUser.FirstOrDefault().User_Id);
                    contextToUpdate.Password = SecurityConversor.Md5Calculator(newRandomPassword);
                    securityContext.Security.Update(contextToUpdate);        
                    securityContext.SaveChanges();
                    ok = true;
                    }
                catch
                {
                    Exception e;
                }
                if (ok)
                {
                    sendEmail.SendMail(email, user, newRandomPassword, MailOperationType.mailRecovery);
                    TempData["EmailPass"] = "Email enviado Correctamente";
                }
                else
                    TempData["EmailPass"] = "Los Datos introducidos no son correctos";

         return RedirectToAction("PassRecovery", "Security");
   
        }


        /// <summary>
        /// Control para recuperar contraseña
        /// </summary>
        /// <returns></returns>
        public IActionResult PassRecovery()
        {
             return View();
        }


        /// <summary>
        /// Devolverá si un elemento será visible o no según el xml de configuración
        /// </summary>
        /// <returns></returns>
        public List<string> IsAllowed()
        {

            var xml =  securityContext.RolesPermissionXml.Select(x=>x.xmlInfo).FirstOrDefault();

            XDocument doc = XDocument.Parse(xml);

            List<string> itemsAllowed = new List<string>();
            foreach( var item in doc.Root.Elements())
            {
                
                if(GetUserRolId() == Convert.ToInt32(item.Attribute("idValue").Value))
                {
                    foreach(var subitem in item.Elements())
                    {
                        var valuesTemp = subitem.Value;
                        itemsAllowed.Add(valuesTemp);
                    }
                }
            }
            return itemsAllowed;
        }


        /// <summary>
        /// Crea numeros aleatorios
        /// </summary>
        /// <returns></returns>
        public string GetNewRandomPassword()
        {
            Random newRandom = new Random();
            return newRandom.Next(100000000, 999999999).ToString();
        }

        /// <summary>
        /// Devuelve un token con cifrado hash md5
        /// </summary>
        /// <returns></returns>
        public string getNewToken()
        {
            var token = GetNewRandomPassword();
            return SecurityConversor.Md5Calculator(token);

        }
    }
}