using System.Reflection;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerApi.Extensions {
    public static class DatabaseExtensions {
        public static void AddIdentityServerDatabase(this IServiceCollection services, string connectionString) {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddConfigurationStore(option =>
                           option.ConfigureDbContext = builder => builder.UseNpgsql(connectionString, options =>
                           options.MigrationsAssembly(migrationsAssembly)))
                    .AddOperationalStore(option =>
                           option.ConfigureDbContext = builder => builder.UseNpgsql(connectionString, options =>
                           options.MigrationsAssembly(migrationsAssembly)));


            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}