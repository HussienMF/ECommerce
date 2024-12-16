using eCommerce.SharedLibrary.DependencyInjection;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;
using Polly;
using Polly.Retry;
using System.Linq.Expressions;

namespace OrderApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastrutureService(this IServiceCollection services, IConfiguration config)
        {
            //Add DataBase Connectivity
            //Add Authentications scheme

            SharedServiceContainer.AddSharedServices<OrdreDbContext>(services, config, config["MySerilog:FileName"]!);

            //Create Dependecy Injection
            services.AddScoped<IOrder, OrderRepository>();


            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            //register middleware sucha as:
            //Global Exception -> handleenternal error 
            //ListenToApiGeteway Only -> block all outsiders calls

            SharedServiceContainer.UserSharedPolicies(app);
            return app;
        }
    }
}
