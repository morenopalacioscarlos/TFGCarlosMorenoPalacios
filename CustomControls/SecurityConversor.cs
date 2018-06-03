using System.Security.Cryptography;
using System.Text;

namespace WebMia.CustomControls
{
    public class SecurityConversor
    {
        /// <summary>
        /// Calculadora del MD5 de un string como parámetro.
        /// </summary>
        /// <param name="valueToChange"></param>
        /// <returns></returns>
        public static string Md5Calculator(string valueToChange)
        {
            using (MD5 md5Object = MD5.Create())
            {
                SecurityConversor securityConversor = new SecurityConversor();
                string md5 = securityConversor.GetNewHashValue(md5Object, valueToChange);
                return md5;
            }
        }

        private  string GetNewHashValue(MD5 md5Object, string valueTochange)
        {
            byte[] dataByte = md5Object.ComputeHash(Encoding.UTF8.GetBytes(valueTochange));
            StringBuilder stringByteMd5 = new StringBuilder();

            for (int i = 0; i < dataByte.Length; i++)
            {
                stringByteMd5.Append(dataByte[i].ToString("x4"));
            }
            return stringByteMd5.ToString();
        }
    }
}
