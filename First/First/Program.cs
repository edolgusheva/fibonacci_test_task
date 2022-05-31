using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using First.FibonacciRestClient;
using First.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace First
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new CommandLineArgs { Args = args });
                    services.AddHostedService<FibonacciCalculatorRunner>();
                    services.AddHostedService<RabbitMqListener>();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
                .UseConsoleLifetime()
                .Build();

            await host.StartAsync();
        }

        private static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<FibonacciHttpClient>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<FibonacciCalculator>().AsSelf();
            builder.RegisterType<FibonacciCalculatorRunner>().AsSelf();
            builder.RegisterType<RabbitMqListener>().AsSelf();
        }
    }
}