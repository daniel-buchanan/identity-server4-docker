using System;
using System.Reflection;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServerApi.Data;
using IdentityServerApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace api
{
    public class Startup
    {
        private const string ConnectionString = "CONNECTION_STRING";
        private const string DbOptions = "DATABASE_OPTIONS";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable(ConnectionString);

            if(string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"The {ConnectionString} environment variable was not set. Please set it before running.");

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var dbOptionsJson = Environment.GetEnvironmentVariable(DbOptions);

            if(string.IsNullOrWhiteSpace(dbOptionsJson))
                throw new Exception($"The {DbOptions} environment variable was not set. Please set it before running.");

            var dbOptions = JsonConvert.DeserializeObject<DbConfigurationOptions>(dbOptionsJson);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Initialize(dbOptions);
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
