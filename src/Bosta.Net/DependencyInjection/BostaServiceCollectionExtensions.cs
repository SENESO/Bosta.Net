using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bosta.Net.DependencyInjection
{
    /// <summary>
    /// ASP.NET Core dependency-injection helpers for Bosta.Net.
    /// </summary>
    public static class BostaServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="BostaClient"/> as a transient service, backed by
        /// <c>IHttpClientFactory</c> when available.
        /// </summary>
        public static IServiceCollection AddBosta(this IServiceCollection services, Action<BostaOptions> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new BostaOptions();
            configure(options);
            options.Validate();

            services.AddHttpClient("Bosta.Net", client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl.EndsWith("/") ? options.BaseUrl : options.BaseUrl + "/");
                client.Timeout = options.Timeout;
            });

            services.AddTransient<BostaClient>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return new BostaClient(options, factory.CreateClient("Bosta.Net"));
            });

            return services;
        }

        /// <summary>
        /// Registers <see cref="BostaClient"/> with an API key.
        /// </summary>
        public static IServiceCollection AddBosta(this IServiceCollection services, string apiKey)
        {
            return AddBosta(services, o => o.ApiKey = apiKey);
        }
    }
}
