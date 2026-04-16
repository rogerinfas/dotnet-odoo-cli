using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OdooDotNetClient;

namespace DotnetOdooCli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddOdooClient(context.Configuration.GetSection("Odoo"));
                })
                .Build();

            var client = host.Services.GetRequiredService<IOdooClient>();

            Console.WriteLine("Iniciando conexión inyectada vía ASP.NET DI Container...");

            try
            {
                int? uid = await client.AuthenticateAsync();
                
                if (uid.HasValue)
                {
                    Console.WriteLine($"Conexión exitosa, User ID: {uid.Value}");
                }
                else
                {
                    Console.WriteLine("Error de autenticación: Credenciales no válidas.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError general de infraestructura o de configuración. ¿Configuraste tu appsettings.json?");
                Console.WriteLine($"Detalle técnico: {ex.Message}");
            }
        }
    }
}
