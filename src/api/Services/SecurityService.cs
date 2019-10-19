using System;
using System.Linq;
using IdentityServerApi.Models;
using Microsoft.Extensions.Logging;

namespace IdentityServerApi.Services {
    public sealed class SecurityService : ISecurityService
    {
        private readonly ILogger _logger;
        private readonly SecurityOptions _options;

        public SecurityService(SecurityOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;
        }

        public bool Authenticate(string header) => CallMethod(header, Authenticate);

        public bool Authenticate(string service, string key)
        {
            var found = _options.AccessKeys.FirstOrDefault(s => s.Service == service && s.Key == key);

            return found != null;
        }

        public AccessRole GetAccess(string header) => CallMethod(header, GetAccess);

        public AccessRole GetAccess(string service, string key)
        {
            var found = _options.AccessKeys.FirstOrDefault(s => s.Service == service && s.Key == key);
            if(found == null) throw new SecurityException("Service not found, or Key invalid.");
            return found.Role;
        }

        private T CallMethod<T>(string header, Func<string, string, T> method) {
            var parsed = SecurityHeader.TryParse(header, out var parsedHeader);

            if(!parsed) throw new SecurityException("No Security Header provided, or it was in the wrong format.");

            return method(parsedHeader.Service, parsedHeader.Key);
        }
    }

    public class SecurityException : Exception {
        public SecurityException() { }

        public SecurityException(string message) : base(message) { }
    }
}