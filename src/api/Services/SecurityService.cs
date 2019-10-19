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

        public bool Authenticate(string header) => CallMethod(header, Authenticate);

        public bool Authenticate(string service, string key)
        {
            _logger.LogInformation("SSE :: Authenticating {0}", service);
            var found = _options.AccessKeys.FirstOrDefault(s => s.Service == service && s.Key == key);

            var authenticated = found != null;
            _logger.LogInformation("SSE :: {0} is {1}", service, authenticated ? "Authenticated" : "not Authenticated");

            return authenticated;
        }

        public AccessRole GetAccess(string header) => CallMethod(header, GetAccess);

        public AccessRole GetAccess(string service, string key)
        {
            _logger.LogInformation("SSE :: Getting Access for {0}", service);
            
            var found = _options.AccessKeys.FirstOrDefault(s => s.Service == service && s.Key == key);
            
            if(found == null) {
                _logger.LogError("SSE :: Service not found, or Key was invalid.");
                throw new SecurityException("Service not found, or Key invalid.");
            } 

            _logger.LogInformation("SSE :: {0} has access {1}", service, found.Role.ToString());
            return found.Role;
        }

        private T CallMethod<T>(string header, Func<string, string, T> method) {
            var parsed = SecurityHeader.TryParse(header, out var parsedHeader);

            if(!parsed) {
                _logger.LogError("SSE :: No Security Header provided, or it was in the wrong format.");
                throw new SecurityException("No Security Header provided, or it was in the wrong format.");
            } 

            return method(parsedHeader.Service, parsedHeader.Key);
        }
    }

    public class SecurityException : Exception {
        public SecurityException() { }

        public SecurityException(string message) : base(message) { }
    }
}