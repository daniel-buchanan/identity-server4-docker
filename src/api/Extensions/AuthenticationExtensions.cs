using IdentityServerApi.Authorization;
using IdentityServerApi.Models;
using IdentityServerApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerApi.Extensions {
    public static class AuthenticationExtensions {
        public static void AddAuthentication(this IServiceCollection services, SecurityOptions options) {
            services.AddSingleton<ISecurityService>(provider => {
                var logger = provider.GetRequiredService<ILogger<SecurityService>>();
                return new SecurityService(options, logger);
            });
            
            services.AddScoped<AuthenticationFilter>();
        }
    }
}