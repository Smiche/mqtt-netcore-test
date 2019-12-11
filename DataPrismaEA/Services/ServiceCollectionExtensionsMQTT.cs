using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataPrismaEA.Services
{
    public static class ServiceCollectionExtensionsMQTT
    {
        public static IServiceCollection AddMQTTClientService(this IServiceCollection services, IConfiguration configuration)
        {
           // services.AddMQTTClientService(configuration);

            return services;
        }

    }
}
