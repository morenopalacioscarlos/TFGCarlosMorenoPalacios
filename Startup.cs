using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebMia.Models;
using WebMia.Controllers;
using WebMia.Interfaces;
using WebMia.CustomControls;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading;
using WebMia.Services;

namespace WebMia
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;


        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //agrego datos a los servicios para acceder al contexto de la bbdd e inyeccion de dependencias 
            //de las interfaces
      
            services.AddDbContext<DataBaseContext>();
            services.AddTransient<ILogged, SecurityController>();
            services.AddScoped<IInitialAlert, InitialAlerts>();
            services.AddTransient<IMail, MailService>();
            services.AddScoped<ICheckLowStock, CheckLowStok>();
            services.AddScoped<IInitialAlert, InitialAlerts>();
            services.AddScoped<ICheckLowStockMailService, CheckLowStockMailService>();
            services.AddScoped<ILoggingTime, LoggingTime>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ICheckLowStockMailService checkLowStockMailService)
        {
            //Creacion de un hilo  que alerta automáticamente al administrador de la máquina 
            //cuando un producto se encuentra bajo de stock.

            //primero compruebo si ya está instanciado el singlenton del servicio de chekeo

            if (SinglentonCheckService.ReturnObject() == null)
            {
                ThreadStart checkServiceDelegate = new ThreadStart(checkLowStockMailService.SendEmailLowStock);
                Thread checkServiceThread = new Thread(checkServiceDelegate);
                checkServiceThread.Start();
                SinglentonCheckService.GetInstance();
            }
 

            app.UseStaticFiles(new StaticFileOptions() {

                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot"))
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "default",
                   template: "{controller}/{action}/{id?}",
                   defaults: new
                   {
                       controller = "Security",
                       action = "Login",
                       id = ""
                   }
                );
   
            });

          
        }
    }
}
