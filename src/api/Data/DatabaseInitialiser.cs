using System.Collections.Generic;
using System.Linq;
using IdentityServer.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServerApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerApi.Data {
    public static class DatabaseInitializer
    {
        public static void Initialize(this IApplicationBuilder app, DbConfigurationOptions options)
        {
            var context = app.ApplicationServices.GetService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            InitialiseDatabase(app, options);

            context.SaveChanges();
        }

        private static void InitialiseDatabase(IApplicationBuilder app, DbConfigurationOptions options)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()
                   .CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
                    .Database.Migrate();

                var context = scope.ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>();
                
                context.Database.Migrate();

                InitialiseClients(context, options.Clients);
                InitialiseIdentityResources(context, options.IdentityResources);
                InitialiseApiResources(context, options.ApiResources);
            }
        }

        private static void InitialiseClients(ConfigurationDbContext context, List<Client> clients) {
            foreach (var client in clients)
            {
                var existing = context.Clients.FirstOrDefault(c => c.ClientId == client.ClientId);
                if(existing == null) 
                {
                    context.Clients.Add(client);
                    continue;
                }

                UpdateFromOptions(existing, client);                       
            }

            context.SaveChanges();
        }

        private static void InitialiseIdentityResources(ConfigurationDbContext context, List<IdentityResource> resources) {
            foreach (var resource in resources)
            {
                var existing = context.IdentityResources.FirstOrDefault(r => r.Name == resource.Name);
                if(existing == null) 
                {
                    context.IdentityResources.Add(resource);
                    continue;
                }

                UpdateFromOptions(existing, resource);                       
            }

            context.SaveChanges();
        }

        private static void InitialiseApiResources(ConfigurationDbContext context, List<ApiResource> resources) {
            foreach (var resource in resources)
            {
                var existing = context.ApiResources.FirstOrDefault(r => r.Name == resource.Name);
                if(existing == null) 
                {
                    context.ApiResources.Add(resource);
                    continue;
                }

                UpdateFromOptions(existing, resource);                       
            }

            context.SaveChanges();
        }

        private static void UpdateFromOptions<T>(T existing, T incoming) 
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            foreach(var p in properties) 
            {
                var incomingValue = p.GetValue(incoming);
                p.SetValue(existing, incomingValue);
            }
        }
    }
}