using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace CalculatorApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Starting App");
            //limit threads for testing load with limited threads available - use with WebSurge (or similar tool)
            //ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

            // NLog: setup the logger first to catch all startup errors
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                //NLog: catch setup errors
                logger.Error(ex, "Unable to start CalcuatorApi");
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Error);
                })
                .UseNLog()
                .Build();
    }
}
