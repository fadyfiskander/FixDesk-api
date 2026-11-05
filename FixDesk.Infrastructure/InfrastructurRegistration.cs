using FixDesk.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FixDesk.Infrastructure
{
    public static class InfrastructurRegistration
    {
        public static IServiceCollection AddInfrastructurRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SQLiteConnection") 
                ?? throw new ArgumentNullException("SQLiteConnection cannot be null");

            services.AddApplicationService();

            return services;
        }
    }
}