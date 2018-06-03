using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebMia.CustomControls.MailService;

namespace WebMia.Interfaces
{
    public interface IMail
    {
    /// <summary>
    /// Envio de emails
    /// </summary>
    /// <param name="mailTo"></param>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="mailOperation"></param>
    /// <param name="product"></param>
         void SendMail(string mailTo, string user, string password, MailOperationType mailOperation,string product = null);
    }
}
