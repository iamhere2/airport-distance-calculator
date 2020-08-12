using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using AirportDistanceCalculator.Application;
using AirportDistanceCalculator.RemoteServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Extensions.Http;
using Polly.Registry;
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

            AddMemoryCache(services);
            AddPolicies(services);
            AddHttpClients(services);
            AddApplicationServices(services);

            Logger.Debug("Services configured");
        }

        private static void AddApplicationServices(IServiceCollection services)
        {
            services.AddSingleton<DistanceCalculator>();
        }

        private static void AddHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<IAirportLocator, PlacesService>(
                client =>
                {
                    // TODO: to config
                    client.BaseAddress = new Uri("https://places-dev.cteleport.com");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddPolicyHandlerFromRegistry("Cache")
                .AddPolicyHandler(GetRetryPolicy());
            // TODO: .AddPolicyHandler(GetCircuitBreakerPolicy());
        }

        private static void AddPolicies(IServiceCollection services)
        {
            services.AddSingleton<IReadOnlyPolicyRegistry<string>, PolicyRegistry>(
                serviceProvider =>
                    new PolicyRegistry
                        {
                            {
                                "Cache",
                                Policy.CacheAsync(
                                    serviceProvider
                                        .GetRequiredService<IAsyncCacheProvider>()
                                        .AsyncFor<HttpResponseMessage>(),
                                    TimeSpan.FromMinutes(15))
                            }
                        });
        }

        private static void AddMemoryCache(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

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
