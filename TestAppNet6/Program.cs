using Autofac;
using Autofac.Extensions.DependencyInjection;
using DeepDiff;
using DeepDiff.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TestAppNet6.Entities;

namespace TestAppNet6;

class Program
{
    private const string LogFile = "TestApp.log";

    static void Main(string[] args)
    {
        ILogger logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.File(LogFile, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: false)
                    .CreateLogger();

        var serviceCollection = new ServiceCollection();
        var diffConfiguration = new DeepDiffConfiguration();
        diffConfiguration.AddProfiles(typeof(Program).Assembly);
        var deepDiff = diffConfiguration.CreateDeepDiff();
        serviceCollection.AddSingleton(typeof(IDeepDiff), deepDiff);
        serviceCollection.AddSingleton(logger);

        var containerBuilder = new ContainerBuilder();

        // Once you've registered everything in the ServiceCollection, call
        // Populate to bring those registrations into Autofac. This is
        // just like a foreach over the list of things in the collection
        // to add them to Autofac.
        containerBuilder.Populate(serviceCollection);

        containerBuilder.RegisterType<Calculate>().As<ICalculate>();

        // Creating a new AutofacServiceProvider makes the container
        // available to your app using the Microsoft IServiceProvider
        // interface so you can use those abstractions rather than
        // binding directly to Autofac.
        var container = containerBuilder.Build();
        var serviceProvider = new AutofacServiceProvider(container);

        var calculate = serviceProvider.GetService<ICalculate>();
        calculate!.Perform(Date.Today);
    }
}