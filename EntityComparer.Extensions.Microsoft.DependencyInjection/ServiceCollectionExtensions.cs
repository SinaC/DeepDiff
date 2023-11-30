using EntityComparer.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace EntityComparer.Extensions.Microsoft.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityComparer(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            if (services.Any(sd => sd.ServiceType == typeof(IEntityComparer)))
                return services;

            var compareConfiguration = new CompareConfiguration();
            compareConfiguration.AddProfiles(assembliesToScan);

            var entityComparer = compareConfiguration.CreateComparer();
            services.AddSingleton(typeof(IEntityComparer), entityComparer);

            return services;
        }
    }
}