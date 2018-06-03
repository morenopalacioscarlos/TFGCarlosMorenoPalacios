using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMia.Interfaces;

namespace WebMia.Services
{
    public class SinglentonCheckService
    {
        private static  SinglentonCheckService singlentonCheckService = null;
        protected SinglentonCheckService() { }


        /// <summary>
        /// Devuelve la instancia que será utilizada para detectar si el usuario se ha logueado y para cerrar sesión
        /// </summary>
        /// <returns></returns>
        public static SinglentonCheckService GetInstance()
        {
            if (singlentonCheckService == null)
                singlentonCheckService = new SinglentonCheckService();

            return singlentonCheckService;
        }

        /// <summary>
        /// Devuelve el objeto de la instancia
        /// </summary>
        /// <returns></returns>
        public static SinglentonCheckService ReturnObject()
        {
            return singlentonCheckService;
        }





    }
}
