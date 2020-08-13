using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using AirportDistanceCalculator.Application;
using AirportDistanceCalculator.RemoteServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
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
            services.AddControllers().AddJsonOptions(ConfigureControllersJson);

            AddMemoryCache(services);
            AddPolicies(services);
            AddHttpClients(services);
            AddApplicationServices(services);

            Logger.Debug("Services configured");
        }

        private static void ConfigureControllersJson(JsonOptions options)
        {
            // Перечисления - строками из [EnumMember]
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());

            // Для диагностики и отладки удобнее с отступами, если вдруг это начнет жать - отключить просто
            options.JsonSerializerOptions.WriteIndented = true;

            // Общее соглашение почти везде: camelCase в JSON
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

            // Еще несколько опций для снижения строгости формата
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
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
                    client.BaseAddress = new Uri("https://places-dev.cteleport.com/zzz/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddPolicyHandlerFromRegistry(PolicyNames.DefaultRetry);
            // TODO: .AddPolicyHandler(GetCircuitBreakerPolicy());
            // TODO: Move policy definition to the service
        }

        private class PolicyNames
        {
            internal const string DefaultRetry = nameof(DefaultRetry);
            internal const string DefaultJsonCache = nameof(DefaultJsonCache);
        }

        private static void AddPolicies(IServiceCollection services)
        {
            services.AddSingleton<IReadOnlyPolicyRegistry<string>, PolicyRegistry>(
                serviceProvider =>
                    new PolicyRegistry
                        {
                            { PolicyNames.DefaultJsonCache, GetDefaultCachePolicy(serviceProvider) },
                            { PolicyNames.DefaultRetry, GetDefaultRetryPolicy() }
                        });
        }

        private static AsyncCachePolicy<JsonDocument> GetDefaultCachePolicy(IServiceProvider serviceProvider)
            => Policy.CacheAsync(
                serviceProvider
                    .GetRequiredService<IAsyncCacheProvider>()
                    .AsyncFor<JsonDocument>(),
                // TODO: to config
                TimeSpan.FromMinutes(15),
                (ctx, key, e) => Logger.Error(e, "JsonDocument cache error for key {Key}", key));

        private static void AddMemoryCache(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetDefaultRetryPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(
                    3,
                    attempt => attempt == 1
                        ? TimeSpan.Zero
                        : TimeSpan.FromSeconds(1 << (attempt - 1)));

        /// <summary>Configures ASP.NET Core request processing pipeline</summary>
        public void Configure(IApplicationBuilder app)
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
