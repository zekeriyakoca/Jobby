

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Upwork.SDK
{
    public static class UpworkServiceCollectionExtensions
    {
        public static void AddUpworkService(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider()
                .GetRequiredService<IConfiguration>();

            AddUpworkService(services, configuration);
        }

        public static void AddUpworkService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ConnectionManager>();
            services.AddHttpClient<IUpworkService, UpworkService>()
                .ConfigureHttpClient(client => {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("Upwork:BaseUrl"));
                });
        }
    }
}