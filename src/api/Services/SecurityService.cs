using System;
using System.Linq;
using IdentityServerApi.Models;
using Microsoft.Extensions.Logging;

namespace IdentityServerApi.Services {
    public sealed class SecurityService : ISecurityService
    {
        private readonly ILogger _logger;
        private readonly SecurityOptions _options;

        public SecurityService(SecurityOptions options, ILogger<SecurityService> logger)
        {
            _options = options;
            _logger = logger;
        }

        /// <summary>
        ///     Determine if the service is authenticated
        /// </summary>
        /// <param name="header">
        ///     The Authorize header value
        /// </param>
        /// <returns>
        ///     True if the service is authenticated, False if not.
        /// </returns>
        public bool Authenticate(string header) => CallMethod(header, Authenticate);

        /// <summary>
        ///     Determine if the service is authenticated
        /// </summary>
        /// <param name="service">
        ///     The name of the service
        /// </param>
        /// <param name="key">
        ///     The access key
        /// </param>
        /// <returns>
        ///     True if the service is authenticated, False if not.
        /// </returns>
        public bool Authenticate(string service, string key)
        {
            _logger.LogInformation("SSE :: Authenticating {0}", service);
            var found = GetService(service, key);

            var authenticated = found != null;
            _logger.LogInformation("SSE :: {0} is {1}", service, authenticated ? "Authenticated" : "not Authenticated");

            return authenticated;
        }

        /// <summary>
        ///     Get the service, based on the service name and access key provided.
        /// </summary>
        /// <param name="header">
        ///     The Authorize header value
        /// </param>
        /// <returns>
        ///     The access role for the found service, or throws a SecurityException.
        /// </returns>
        public AccessRole GetAccess(string header) => CallMethod(header, GetAccess);

        /// <summary>
        ///     Get the access role service, based on the service name and access key provided.
        /// </summary>
        /// <param name="service">
        ///     The name of the service
        /// </param>
        /// <param name="key">
        ///     The access key
        /// </param>
        /// <returns>
        ///     The access role for the found service, or throws a SecurityException.
        /// </returns>
        public AccessRole GetAccess(string service, string key)
        {
            _logger.LogInformation("SSE :: Getting Access for {0}", service);
            
            var found = GetService(service, key);
            
            if(found == null) {
                _logger.LogError("SSE :: Service not found, or Key was invalid.");
                throw new SecurityException("Service not found, or Key invalid.");
            } 

            _logger.LogInformation("SSE :: {0} has access {1}", service, found.Role.ToString());
            return found.Role;
        }

        /// <summary>
        ///     Call an overload of the the method, parsing the header
        /// </summary>
        /// <param name="header">
        ///     The value of the Authorize header
        /// </param>
        /// <param name="method">
        ///     The overload method to call
        /// </param>
        /// <returns>
        ///     The value of the overloaded method provided
        /// </returns>
        private T CallMethod<T>(string header, Func<string, string, T> method) {
            var parsed = SecurityHeader.TryParse(header, out var parsedHeader);

            if(!parsed) {
                _logger.LogError("SSE :: No Security Header provided, or it was in the wrong format.");
                throw new SecurityException("No Security Header provided, or it was in the wrong format.");
            } 

            return method(parsedHeader.Service, parsedHeader.Key);
        }

        /// <summary>
        ///     Get the service, based on the service name and access key provided.
        /// </summary>
        /// <param name="service">
        ///     The name of the service
        /// </param>
        /// <param name="key">
        ///     The access key
        /// </param>
        private AccessKey GetService(string service, string key) {
            return _options.AccessKeys.FirstOrDefault(s => 
                string.Equals(s.Service, service, StringComparison.InvariantCultureIgnoreCase) && 
                string.Equals(s.Key, key, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class SecurityException : Exception {
        public SecurityException() { }

        public SecurityException(string message) : base(message) { }
    }
}