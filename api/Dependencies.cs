using JwtAuthentication.Data;
using JwtAuthentication.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            #region JwtAuthentication Schema definition

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>                 {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["AuthSettings:Issuer"],
                        ValidateAudience = true,
                        ValidateLifetime = true,

                        ValidAudience = configuration["AuthSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["AuthSettings:TokenKey"] ?? string.Empty)
                        ),
                        ClockSkew = TimeSpan.Zero
                    };
                }
                );


            #endregion

            services.AddScoped<IAuthUserService, AuthUserService>();

            return services;
        }
    }
}
