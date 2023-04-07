using Common.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabitMq(this IServiceCollection services)
        {
            services.AddMassTransit(bus =>
            {
                bus.AddConsumers(Assembly.GetEntryAssembly());
                bus.UsingRabbitMq((context, configurator) =>
                {
                    var configuration = context.GetService<IConfiguration>();
                    if (configuration == null)
                    {
                        throw new Exception("Cannot resolve IConfiguration service for MassTransit registration");
                    }
                    var settings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    if (settings == null)
                    {
                        throw new Exception("Settings for rabbitMQ not found");
                    }

                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    if (serviceSettings == null)
                    {
                        throw new Exception("No serviceSettings section in configuration");
                    }
                    configurator.Host(settings.Host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    configurator.UseMessageRetry(conf =>
                    {
                        conf.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            return services;
        }
    }
}
