using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMia.CustomControls;
using WebMia.Interfaces;
using WebMia.Models;

namespace WebMia.Controllers
{
    public class UsersController : Controller
    {
        #region fields

        private ILogged isLogged;
        private DataBaseContext context;
        private ILoggingTime loggingTime;
        private const string ADMIN_USER = "1007";

        #endregion

        #region Constructor

        public UsersController(ILogged isLogged, DataBaseContext context, ILoggingTime loggingTime)
        {
            this.isLogged = isLogged;
            this.context = context;
            this.loggingTime = loggingTime;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Control para añadir nuevos usuarios en la bbdd.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult NewUserControl()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            var inputUser = SecurityConversor.Md5Calculator((Request.Form["user"]).ToString().ToLower());
            var inputPassword = SecurityConversor.Md5Calculator(Request.Form["password"]);
            var confirmInputPassword = SecurityConversor.Md5Calculator(Request.Form["confirmPassword"]);
            var email = Request.Form["email"];
            var name = Request.Form["UserName"];
            var rol = Request.Form["rol"];

            var securityContextList = context.Security.ToList();

            if (securityContextList.Exists(x => x.User_Name == inputUser))
            {
                TempData["InfoPass"] = "El usuario introducido ya existe";
                return RedirectToAction("NewUser", "Users");
            }
            else if (inputPassword != confirmInputPassword)
            {
                TempData["InfoPass"] = "Las Contraseñas no coinciden";
                return RedirectToAction("NewUser", "Users");
            }
            else
            {
                context.Security.Add(new Admin()
                {
                    Password = inputPassword,
                    User_Name = inputUser,
                    Email = email,
                    Nombre = name,
                    UserRolId = Convert.ToInt32(rol)
                });

                if (context.SaveChanges() > 0)
                    TempData["InfoPass"] = "Usuario Almacenado Correctamente";
            }

            return RedirectToAction("NewUser", "Users");
        }


        /// <summary>
        /// Metodo para dar de Alta nuevos Usuarios
        /// </summary>
        /// <returns></returns>
        public IActionResult NewUser()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Envio de roles disponibles a la vista
            ViewBag.Roles = context.Rol.ToList();

            return View();
        }

        /// <summary>
        /// Borrado de usuarios
        /// </summary>
        /// <returns></returns>
        public IActionResult UserDelete()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            var actualUserId = isLogged.GetUserIdLogged();

            var userId = Request.Form["RolDeleteUser"];

            if (userId != ADMIN_USER && userId.ToString() != "Default" && userId != actualUserId)
            {
                var userIdToDelete = context.Security.Find(Convert.ToInt32(userId));
                context.Security.Remove(userIdToDelete);
                context.SaveChanges();
                TempData["DeleteUsers"] = "Usuario eliminado";
            }
            //no se puede eliminar al administrador principal
            else if (userId == ADMIN_USER)
            {
                TempData["DeleteUsers"] = "No se puede eleminar al administrador principal";
            }
            //no se puede eliminar al usuario logueado
            else if (userId == actualUserId)
            {
                TempData["DeleteUsers"] = "No se puede eleminar el actual usuario logueado";
            }


            return RedirectToAction("DeleteUsers", "Users");
        }


        /// <summary>
        /// Obtiene datos de los usuarios registrados en la aplicación
        /// </summary>
        /// <returns></returns>
        public IActionResult GetUsers()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            var datos = context.Security.ToList();
            var roles = context.Rol.ToList();

            Dictionary<string, string> rolesDescription = new Dictionary<string, string>();

            foreach (var item in datos)
            {
                rolesDescription.Add(item.Nombre, roles.Where(x => x.UserRolId == item.UserRolId).Select(x => x.RolDescription).FirstOrDefault());
            }

            ViewBag.ActiveUsers = rolesDescription.ToList();

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }

        /// <summary>
        /// Borra los usuarios seleccionados de la aplicación
        /// </summary>
        /// <param name="arrayofvalues"></param>
        /// <returns></returns>
        public IActionResult DeleteUsers(string[] arrayofvalues)
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            var datos = context.Security.ToList();
            var roles = context.Rol.ToList();
            ViewBag.ActiveUsers = datos;
            ViewBag.ActualUser = isLogged.GetUserIdLogged();
            ViewBag.Roles = roles;

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }


        /// <summary>
        /// Devuelve la vista de los ultimos usuarios registrados
        /// </summary>
        /// <returns></returns>
        public IActionResult LastUserLogin()
        {
            //comprueba si el usuario está logueado
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            //Obtiene los datos de la tabla de usuarios logueados
            ViewBag.LastUserLogin = loggingTime.GetUserLogin();

            return View();
        }


        /// <summary>
        /// Devuelve la vista de la página de asignación de máquinas a usuarios
        /// </summary>
        /// <returns></returns>
        public IActionResult SetMachineAdmin()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");


            ViewBag.UserActive = context.Security.ToList();
            ViewBag.MachineActive = context.Vending_Machine.ToList();

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();

            return View();
        }

        /// <summary>
        /// Devuelve la vista para cambiar el password del usuario
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangePassword()
        {
            if (!isLogged.IsUserLogged())
                return RedirectToAction("Login", "Security");

            //Se manda info sobre el siguiente control para determinar la visibilidad de los links
            ViewBag.Allowed = isLogged.IsAllowed();
            return View();
        }

        /// <summary>
        /// Método que comprueba y cambia las contraseñas del usuario actual
        /// </summary>
        /// <returns></returns>
        public IActionResult SetNewPassword()
        {
            var actualUser = isLogged.GetActualUser();
            var actualPassword = SecurityConversor.Md5Calculator(Request.Form["Password"].ToString().ToLower());
            var newPassword = SecurityConversor.Md5Calculator(Request.Form["NewPassword"].ToString().ToLower());
            var confirmNewPassword = SecurityConversor.Md5Calculator(Request.Form["ConfirmPassword"].ToString().ToLower());


            if (actualUser.Password != actualPassword)
                TempData["InfoPass"] = "La contraseña no coincide con la actual";
            else
            {
                if (newPassword.Equals(confirmNewPassword))
                {
                    actualUser.Password = newPassword;
                    context.Security.Update(actualUser);
                    context.SaveChanges();

                      TempData["InfoPass"] = "Cambiada correctamente";
                }
                else
                {
                    TempData["InfoPass"] = "La contraseñas no son iguales";
                }
            }

            return  RedirectToAction("ChangePassword");
        }

        /// <summary>
        /// Actualizacion base de datos con los cambios de la asignación de máquinas a usuarios
        /// </summary>
        /// <param name="arrayofvalues"></param>
        public void SaveChangeMachineAssign(string[] arrayofvalues)
        {

            int userId = 0;
            int machineID = 0;

            for (int i = 3; i < arrayofvalues.Length; i++)
            {
                switch (i % 3)
                {
                    //user id
                    case 0:
                        machineID = Convert.ToInt32(arrayofvalues[i]);
                        break;
                    //direccion de maquinaria, no se cambia
                    case 1:
                        break;
                    //nombre producto
                    case 2:
                        userId = Convert.ToInt32(arrayofvalues[i]);

                        //una vez recorrida la primera la primera fila agregamos el cambio en el contexto sobre la máquina
                        Vending_Machine vending = context.Vending_Machine.Find(machineID);
                        vending.UserAdministrator = userId;

                        context.Vending_Machine.Update(vending);

                        //confirmar cambios en base de datos
                        context.SaveChanges();
                        break;
                }
            }
        }
         #endregion
    }
}
