using DeepDiff.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace DeepDiff.Extensions.Microsoft.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDeepDiff(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            if (services.Any(sd => sd.ServiceType == typeof(IDeepDiff)))
                return services;

            var diffConfiguration = new DiffConfiguration();
            diffConfiguration.AddProfiles(assembliesToScan);

            var deepDiff = diffConfiguration.CreateDeepDiff();
            services.AddSingleton(typeof(IDeepDiff), deepDiff);

            return services;
        }
    }
}