using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependecyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //Add database connectivity
            // JWT Add Authentication Scheme
            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

            //Create Dependecy Injection
            services.AddScoped<IUser, UserRepository>();
            
            return services;
        }

        public static IApplicationBuilder UserInfrastucturpolicy(this IApplicationBuilder app)
        {
            //Register middleware such as :
            //Global Exception: Handle external errors.
            //Listen Only TO API Gateway : block all; outsiders call

            SharedServiceContainer.UserSharedPolicies(app);
            return app;
        }
    }
}
