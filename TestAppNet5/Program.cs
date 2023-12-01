using Autofac;
using Autofac.Extensions.DependencyInjection;
using DeepDiff.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TestAppNet5.Entities;

namespace TestAppNet5
{
    internal class Program
    {
        private const string LogFile = "TestAppNet5.log";

        static void Main(string[] args)
        {
            ILogger logger = new LoggerConfiguration()
                   .Enrich.FromLogContext()
                   .WriteTo.File(LogFile, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: false)
                   .CreateLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDeepDiff(typeof(Program).Assembly);
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
}
