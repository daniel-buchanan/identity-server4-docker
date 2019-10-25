using IdentityServer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class ApiServicesExtensions {
        /// <summary>
        /// Adds the api identity resources
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddApiIdentityResources(this IIdentityServerBuilder builder)
        {
            builder.AddResourceStore<ApiResourceStore>();

            return builder;
        }
    }
}