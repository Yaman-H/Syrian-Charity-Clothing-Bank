using CharitySystem.Application.Interfaces;
using CharitySystem.Application.Mappings;
using CharitySystem.Application.Services;
using CharitySystem.Domain.Interfaces;
using CharitySystem.Infrastructure.Data;
using CharitySystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace CharitySystem.Infrastructure 
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
                };
            });

            var connectionString = configuration.GetConnectionString("OracleConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(connectionString,
                    b => 
                    {
                        b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);

                        b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    })
            );
            services.AddAutoMapper(cfg => {
                cfg.AddProfile<MappingProfile>(); 
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClothService, ClothService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IFamilyService, FamilyService>();

            return services; 
        }
    }
}