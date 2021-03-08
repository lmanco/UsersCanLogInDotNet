using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace UsersCanLogIn
{
    public class Program
    {
        public const string AppName = "UsersCanLogIn";
        public const string AppDisplayName = "Users Can Log In";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
