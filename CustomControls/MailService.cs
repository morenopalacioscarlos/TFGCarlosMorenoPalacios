

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using WebMia.Interfaces;

namespace WebMia.CustomControls
{
    public class MailService : IMail
    {
        public enum MailOperationType
        {
            mailRecovery,
            infoLowStock
        }

        /// <summary>
        /// Manda un email a un usuario dependiendo del tipo de operacion realizada al usuario especificado. Esto puede ser el stockaje de los producos, una incidencia
        /// o una nueva contraseña generada
        /// </summary>
        /// <param name="mailTo"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="mailOperation"></param>
        /// <param name="product"></param>
        public void SendMail(string mailTo, string user, string password, MailOperationType mailOperation, string product= null)
        {
            var mail = new MailMessage();
            string mailAdmin = "TFGCarlosMorenoPalacios@gmail.com";
            SmtpClient smtp = new SmtpClient();
            mail.From = new MailAddress(mailAdmin);
            mail.To.Add(mailTo);

            switch (mailOperation)
            {
                case (MailOperationType.mailRecovery):
                    mail.Subject = "Recuperación de contraseña";
                    mail.Body = "Buenos días, le mandamos a continuación sus datos de usuario y su nueva contraseña."
                        +"\n\n" + $"Su usuario es : {user}" + "\n"+
                        $"Su password es : {password}";
                    break;

                case (MailOperationType.infoLowStock):
                    mail.Subject = "Stock bajo de producto";
                    mail.Body = $"Buenos días {user}, se ha detectado un nivel bajo de los siguientes:\n\n {product}";
                    break;

                default:
                    mail.Subject = "Incidencia";
                    mail.Body = "Se ha detectado una incidencia en su cuenta, por favor, contacte con el administrador" +
                        $"en la dirección siguiente: {mailAdmin}";
                    break;
            }

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            smtp.Credentials = new NetworkCredential(mailAdmin, "adminTFG");
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(mail);
            }catch(Exception e) { }
            finally
            {
            smtp.Dispose();
            }
 

        }
    }
}
