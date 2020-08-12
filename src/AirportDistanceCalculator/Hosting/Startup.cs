using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AirportDistanceCalculator.Hosting
{
    /// <summary>ASP.NET Core application configurator</summary>
    public class Startup
    {
        private static ILogger Logger { get; } = Log.ForContext<Startup>();

        /// <summary>Constructor</summary>
        /// <remarks>Called by ASP.NET Core infrastructure</remarks>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        /// <summary>Initializes IoC-container with services</summary>
        /// <remarks>Called by ASP.NET Core infrastructure</remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHealthChecks();

            services
                .AddControllers()
                .AddJsonOptions(o =>
                {
                    // Перечисления - строками из [EnumMember]
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());

                    // Для диагностики и отладки удобнее с отступами, если вдруг это начнет жать - отключить просто
                    o.JsonSerializerOptions.WriteIndented = true;

                    // Общее соглашение почти везде: camelCase в JSON
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

                    // Еще несколько опций для снижения строгости формата
                    o.JsonSerializerOptions.AllowTrailingCommas = true;
                    o.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            Logger.Debug("Services configured");
        }

        /// <summary>Configures ASP.NET Core request processing pipeline</summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health-check");
            });

            Logger.Debug("ASP.NET Core request pipeline configured");
        }
    }
}
