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
    public class CheckLowStockMailService : ICheckLowStockMailService
    {
       private  IMail mail;
       private ICheckLowStock iCheckLowStock;
       private const int NO_ID = 0;

        public CheckLowStockMailService( IMail mail, ICheckLowStock iCheckLowStock)
        {
            this.mail = mail;
            this.iCheckLowStock = iCheckLowStock;
        }

        /// <summary>
        /// Obtiene todos los productos con bajo stock y monedas  y los envia por email
        /// </summary>
        public void SendEmailLowStock()
        {

            //bucle que se va a ejecutar siempre para comprobar los items de las máquinas
            while (true)
            {
               //enviará en email cada 12 horas
               Thread.Sleep(43200000);

               var mailDataInfo = new HashSet<InfoLowStockSave<StringBuilder, int>>();

                //obtengo los productos con stock bajo
                mailDataInfo.UnionWith(iCheckLowStock.ProductsCheck());

                //Obtengo las monedas de las máquinas que están bajas de stock
                mailDataInfo.UnionWith(iCheckLowStock.CoinsStock());
                
                //Obtendo los diferentes ids que tengo
                HashSet<int> ids = mailDataInfo.Select(X => X.id).ToHashSet();
                HashSet<StringBuilder> stringToSend;

                foreach ( var id in ids)
                {
                    stringToSend = mailDataInfo.Where(x => x.id == id && x.id != NO_ID).Select(y => y.sb).ToHashSet();
                    IntegrateSendEmail(id, stringToSend);
                }
            }
        }

        /// <summary>
        /// Envia el email al usuario adecuado con todos los datos que esten por debajo del stock
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="contentData"></param>
        private void IntegrateSendEmail(int idUser, HashSet<StringBuilder> contentData)
        {
            StringBuilder sb = new StringBuilder();

            foreach(var itemSb in contentData)
            {
                sb.Append(itemSb.ToString());
                sb.AppendLine();
            }

            //se envia el email cuando se han unificado todos los productos de la misma maquinaria y luego se limpia la lista
            using (var context = new DataBaseContext())
            {
                
                var user = context.Security.Where(x => x.User_Id == idUser).Select(y => new { y.Nombre, y.Email });
                var mailData = user.Select(x => x.Email).FirstOrDefault().ToString();
                var name = user.Select(y => y.Nombre).FirstOrDefault().ToString();

                mail.SendMail(mailData, name, string.Empty, CustomControls.MailService.MailOperationType.infoLowStock, sb.ToString());
            }
        }
     }
}
