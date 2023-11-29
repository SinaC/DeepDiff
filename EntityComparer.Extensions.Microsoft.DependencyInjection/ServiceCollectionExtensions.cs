using EntityComparer.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EntityComparer.Extensions.Microsoft.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityComparer(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            var assembliesToScanArray = assembliesToScan as Assembly[] ?? assembliesToScan?.ToArray();

            var compareConfiguration = new CompareConfiguration();

            if (assembliesToScanArray != null && assembliesToScanArray.Length > 0)
            {
                var allTypes = assembliesToScanArray
                    .Where(a => !a.IsDynamic && a.GetName().Name != nameof(EntityComparer))
                    .Distinct()
                    .SelectMany(a => a.DefinedTypes)
                    .ToArray();

                //services.Configure<MapperConfigurationExpression>(options => options.AddMaps(assembliesToScanArray));

                //var openTypes = new[]
                //{
                //    typeof(IValueResolver<,,>),
                //    typeof(IMemberValueResolver<,,,>),
                //    typeof(ITypeConverter<,>),
                //    typeof(IValueConverter<,>),
                //    typeof(IMappingAction<,>)
                //};
                //foreach (var type in openTypes.SelectMany(openType => allTypes
                //    .Where(t => t.IsClass
                //        && !t.IsAbstract
                //        && t.AsType().ImplementsGenericInterface(openType))))
                //{
                //    // use try add to avoid double-registration
                //    services.TryAddTransient(type.AsType());
                //}
            }

            if (services.Any(sd => sd.ServiceType == typeof(IEntityComparer)))
                return services;

            //var entityComparer = compareConfiguration.CreateComparer();
            //services.AddSingleton(typeof(IEntityComparer), entityComparer);

            //services.AddSingleton<IConfigurationProvider>(sp =>
            //{
            //    // A mapper configuration is required
            //    var options = sp.GetRequiredService<IOptions<MapperConfigurationExpression>>();
            //    return new MapperConfiguration(options.Value);
            //});

            //services.Add(new ServiceDescriptor(typeof(IMapper),
            //    sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService), serviceLifetime));

            return services;
        }
    }
}