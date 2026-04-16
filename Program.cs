using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DotnetOdooCli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            string? odooUrl = config["Odoo:Url"];
            string? odooDb = config["Odoo:Db"];
            string? odooUser = config["Odoo:User"];
            string? odooPassword = config["Odoo:Password"];

            if (string.IsNullOrEmpty(odooUrl) || string.IsNullOrEmpty(odooDb) || 
                string.IsNullOrEmpty(odooUser) || string.IsNullOrEmpty(odooPassword))
            {
                Console.WriteLine("Error: Falta configuración de Odoo. Revisa el archivo appsettings.json.");
                return;
            }

            Console.WriteLine($"Conectando a {odooUrl} (DB: {odooDb})...");

            var client = new OdooClient(odooUrl, odooDb, odooUser, odooPassword);

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
                Console.WriteLine("Excepción capturada:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
