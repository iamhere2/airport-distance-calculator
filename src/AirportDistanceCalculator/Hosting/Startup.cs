using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
using Microsoft.OpenApi.Models;
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
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Called by convention")]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHealthChecks();
            services.AddControllers().AddJsonOptions(ConfigureControllersJson);

            AddSwagger(services);
            AddMemoryCache(services);
            AddPolicies(services);
            AddHttpClients(services);
            AddApplicationServices(services);

            Logger.Debug("Services configured");
        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc(
                    name: "v1",
                    new OpenApiInfo
                    {
                        Title = $"Airport distance API",
                        Version = "v1"
                    });

                // Set the comments path for the Swagger JSON and UI.
                var xmlDocFileName = $"{typeof(Startup).Assembly.GetName().Name}.xml";
                var xmlDocFullPath = Path.Combine(AppContext.BaseDirectory, xmlDocFileName);
                o.IncludeXmlComments(xmlDocFullPath, includeControllerXmlComments: true);
            });
        }

        private static void ConfigureControllersJson(JsonOptions options)
        {
            // Enums as string from [EnumMember]
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());

            // Much more convenient for disagnostics/debug
            // TODO: get from config
            options.JsonSerializerOptions.WriteIndented = true;

            // Usual JSON convention: camelCase
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

            // Some options for less restrictive parsing
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
                    // TODO: get from config
                    client.BaseAddress = new Uri("https://places-dev.cteleport.com/zzz/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                .AddPolicyHandlerFromRegistry(PolicyNames.DefaultRetry);
            // TODO: .AddPolicyHandler(GetCircuitBreakerPolicy());
            // TODO: Move policy definition to the service
            // TODO: Consider developing non-default policy for returning last good value from cache if the response is error (see https://github.com/App-vNext/Polly/issues/648)
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
                // TODO: get from config / from the target service
                TimeSpan.FromHours(24),
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
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Called by convention")]
        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint(
                    url: "/swagger/v1/swagger.json",
                    name: $"Airport distance API v1");

                o.EnableDeepLinking();

                Logger.Information("Swagger UI configured at /swagger path");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health-check");
                endpoints.MapSwagger();
            });

            Logger.Debug("ASP.NET Core request pipeline configured");
        }
    }
}
