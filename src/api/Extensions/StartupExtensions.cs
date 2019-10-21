using System;
using IdentityServerApi.Models;
using Newtonsoft.Json;

namespace IdentityServerApi.Extensions {
    public static class StartupExtensions {
        private const string EnvErrorFormat = "The {0} environment variable was not set. Please set it before running.";
        private const string ConnectionString = "CONNECTION_STRING";
        private const string DbOptions = "DATABASE_OPTIONS";
        private const string SecurityOptions = "SECURITY_OPTIONS";

        public static DbConfigurationOptions GetDatabaseOptions(this Startup startup) {
            var dbOptionsJson = Environment.GetEnvironmentVariable(DbOptions);

            if(string.IsNullOrWhiteSpace(dbOptionsJson))
                throw new Exception(string.Format(EnvErrorFormat, DbOptions));

            return JsonConvert.DeserializeObject<DbConfigurationOptions>(dbOptionsJson);
        }

        public static string GetConnectionString(this Startup startup) {
            var connectionString = Environment.GetEnvironmentVariable(ConnectionString);

            if(string.IsNullOrWhiteSpace(connectionString))
                throw new Exception(string.Format(EnvErrorFormat, ConnectionString));

            return connectionString;
        }

        public static SecurityOptions GetSecurityOptions(this Startup startup) {
            var securityOptionsJson = Environment.GetEnvironmentVariable(SecurityOptions);

            if(string.IsNullOrWhiteSpace(securityOptionsJson))
                throw new Exception(string.Format(EnvErrorFormat, SecurityOptions));

            return JsonConvert.DeserializeObject<SecurityOptions>(securityOptionsJson);
        }
    }
}