using Microsoft.EntityFrameworkCore;
using System.IO;

namespace WebMia.Models
{
    //Clase Entity Framework para la conexion con la base de datos
    public class DataBaseContext : DbContext 
    {

        public DataBaseContext(){}

        
        public DbSet<Change> Change { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<Slots> Slots { get; set; }
        public DbSet<Vending_Machine> Vending_Machine { get; set; }
        public DbSet<Admin> Security { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<RolesPermissionXml> RolesPermissionXml { get; set; }
        public DbSet<PriceUpdated> PriceUpdated { get; set; }

        //Configuracion dbcontext
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            //aplico datos para la migracion, debere hacer por consola un dotnet ef migrations add Database y luego dotnet ef database update
            base.OnConfiguring(optionsBuilder);

            //Conexión a base de datos local
            optionsBuilder.UseSqlServer("Server =(localdb)\\MSSQLLocalDB;Database="+ GetCurrentDirectoyPath());

            }

        /// <summary>
        /// Devuelve el path del directorio donde se encuentra la base de datos local
        /// </summary>
        /// <returns></returns>
        public string GetCurrentDirectoyPath()
        {
                    
            return Directory.GetCurrentDirectory() + "\\BBDD\\MACHINEVENDING.MDF";
        }

        /// <summary>
        /// Devuelve la cadena de conexión de la base de datos par aoperaciones
        /// </summary>
        /// <returns></returns>
        public string GetDataBaseConectionString()
        {
            //Conexión a base de datos local
            return "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = " + GetCurrentDirectoyPath() + "; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
       }
    }
}
