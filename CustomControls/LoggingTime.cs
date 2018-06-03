using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebMia.Interfaces;
using WebMia.Models;

namespace WebMia.CustomControls
{
    public class LoggingTime : ILoggingTime
    {
        private DataBaseContext context;

        public LoggingTime(DataBaseContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Inserta en la base de datos la hora actual de logueado
        /// </summary>
        public void NewUserLogin(int userId)
        {

            SqlConnection sqlConnection1 = new SqlConnection(context.GetDataBaseConectionString());
            SqlCommand cmd = new SqlCommand();

            string actualTime = DateTime.Now.ToString();
            string query = "INSERT into LoginTime (UserID,Time) "
                 + " VALUES ("
                 + userId
                 + ", '"
                 + actualTime + "')";

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        /// <summary>
        /// Obtiene de la base de datos los datos del registro de login
        /// </summary>
        public List<Tuple<int, string>> GetUserLogin()
        {

            SqlConnection sqlConnection1 = new SqlConnection(context.GetDataBaseConectionString());
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            List<Tuple<int, string>> registryLogginInfo = null;


            string query = "select * from   LoginTime";

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            cmd.Connection.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                registryLogginInfo = new List<Tuple<int, string>>();
                while (reader.Read())
                {
                    registryLogginInfo.Add(new Tuple<int, string>((int)reader["UserID"],
                        (string)reader["Time"]));
                }
            }
            else
            {
                registryLogginInfo.Add(new Tuple<int, string>(0,
                     "NoData"));
            }
            cmd.Connection.Close();

            return registryLogginInfo;
        }
    }
}
