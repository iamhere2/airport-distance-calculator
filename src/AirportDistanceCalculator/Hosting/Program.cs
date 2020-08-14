using System;
using CTeleport.AirportDistanceCalculator.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CTeleport.AirportDistanceCalculator
{
    /// <summary>Process entry points</summary>
    public static class Program
    {
        /// <summary>Main entry point</summary>
        public static void Main(string[] args)
        {
            Console.WriteLine($"Service airport-distance-calculator: process started");

            try
            {
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                ConfigureInitialLogging();
                RunHost(args);
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
            finally
            {
                AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            }

            Console.WriteLine($"Process shut down gracefully");
        }

        private static void ConfigureInitialLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Debug("Initial logging configured");
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException(e.ExceptionObject as Exception);

            if (e.IsTerminating)
            {
                Log.CloseAndFlush();
            }
        }

        private static void LogException(Exception? e)
        {
            if (Log.Logger != null)
            {
                if (e is null)
                {
                    Log.Logger.Fatal("Unhandled exception: <Unknown exception, null>");
                }
                else
                {
                    Log.Logger.Fatal(e, "Unhandled exception: {Exception}", e.ToString());
                }
            }

            // We'll duplicate the message to console independently,
            // due to the exception severity
            var errorMessage = $"FATAL ERROR! Unhandled exception: {e?.ToString() ?? "<Unknown exception>"}";
            Console.WriteLine(errorMessage);
        }

        private static void RunHost(string[] args) =>
            CreateHostBuilder(args)
            .Build()
            .Run();

        private const string SerilogConfigurationSectionName = "Serilog";

        /// <summary>Creates instance of <see cref="IHostBuilder"/></summary>
        /// <remarks>
        /// Can be called directly by dotnet tools.
        /// </remarks>
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("Config.json",
                        optional: false,
                        reloadOnChange: true);
                })
                .UseSerilog((ctx, sp, loggingConfiguration) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    if (configuration.GetSection(SerilogConfigurationSectionName).Exists())
                    {
                        loggingConfiguration.ReadFrom.Configuration(configuration, SerilogConfigurationSectionName);
                        Log.Logger.Information("Logging has finally configured from general configuration source");
                    }
                    else
                    {
                        Log.Logger.Warning(
                            "Configuration section for Serilog (\"{SerilogConfigurationSectionName}\") was not found",
                            SerilogConfigurationSectionName);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
