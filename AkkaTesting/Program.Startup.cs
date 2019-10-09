using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AkkaTesting.Infra.Helper;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AkkaTesting
{
    public partial class Program
    {
        public static IServiceProvider RootServiceProvider;

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.Seq("http://seqserver.sicluster:5341", compact: true)
                .CreateLogger();

            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                var sp = services.BuildServiceProvider();
                DIProps.ServiceProvider = RootServiceProvider = sp;

                Log.Information("Running...");
                await Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled Exception");

                if (Debugger.IsAttached)
                    Debugger.Break();
            }
            finally
            {
                Log.Information("Press ENTER to exit...");
                Console.ReadLine();
            }
        }

        public static T GetService<T>() => RootServiceProvider.GetRequiredService<T>();
    }
}
