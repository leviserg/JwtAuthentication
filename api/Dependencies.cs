using JwtAuthentication.Data;
using JwtAuthentication.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthentication
{
    public static class Dependencies
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<IConfiguration>(configuration);

            #region SqlServer

            var dbLogin = Environment.GetEnvironmentVariable("DEVELOPER_LOGIN") ?? string.Empty;
            var dbPassword = Environment.GetEnvironmentVariable("DEVELOPER_PWD") ?? string.Empty;
            var dbName = Environment.GetEnvironmentVariable("DEVELOPER_DB") ?? string.Empty;

            var connectionString = string.Format(configuration.GetConnectionString("AppDb") ?? string.Empty, dbLogin, dbPassword, dbName);

            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            #endregion

            services.AddScoped<IAuthUserService, AuthUserService>();

            return services;
        }
    }
}
