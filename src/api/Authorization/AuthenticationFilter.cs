using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServerApi.Models;
using IdentityServerApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace IdentityServerApi.Authorization {
    public class AuthenticationFilter : AuthorizeFilter {
        private const string AuthHeader = "Authorize";

        private readonly ILogger _logger;
        private readonly ISecurityService _securityService;

        public AuthenticationFilter(ILogger<AuthenticationFilter> logger,
            ISecurityService securityService)
        {
            _logger = logger;
            _securityService = securityService;
        }

        public override Task OnAuthorizationAsync(AuthorizationFilterContext context) {
            var header = GetHeader(context.HttpContext.Request);
            SecurityHeader.TryParse(header, out var parsed);

            if(string.IsNullOrWhiteSpace(header)) {
                SetUnauthorizedResult(context);
                return Task.CompletedTask;
            }

            var authenticated = _securityService.Authenticate(header);

            if(!authenticated) {
                SetUnauthorizedResult(context);
                return Task.CompletedTask;
            }

            var requiredRole = GetRequiredRole(context);
            var serviceRole = _securityService.GetAccess(header);

            if(requiredRole < serviceRole) return Task.CompletedTask;

            SetForbiddenResult(context, parsed?.Service);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Gets the header from the request
        /// </summary>
        /// <param name="request">
        ///     The Http Request to get the header from.
        /// </param>
        /// <returns>
        ///     Returns the value of the header, if present.
        /// </returns>
        private string GetHeader(HttpRequest request) {
            if(!request.Headers.ContainsKey(AuthHeader)) return null;

            if(!request.Headers.TryGetValue(AuthHeader, out var values))
                return null;

            if(values.Count == 1) return values[0];
            return null;
        }

        /// <summary>
        ///     Gets the required user type for this request
        /// </summary>
        /// <param name="context">
        ///     The context representing the request
        /// </param>
        /// <returns>
        ///     Returns the required service role. If no role is defined for this request,
        ///     <see cref="AccessRole.ReadOnly"/> is returned
        /// </returns>
        private AccessRole GetRequiredRole(AuthorizationFilterContext context)
        {
            var filter = context.ActionDescriptor.FilterDescriptors
                .Where(f => f.Filter is AuthenticationAttribute)
                .LastOrDefault()?.Filter as AuthenticationAttribute;

            return filter?.Role ?? AccessRole.ReadOnly;
        }

        /// <summary>
        ///     Sets the result of the request to forbidden
        /// </summary>
        /// <param name="context">
        ///     The request context on which the response is set
        /// </param>
        /// <param name="service">
        ///     The Service attempting this request
        /// </param>
        private void SetForbiddenResult(AuthorizationFilterContext context, string service)
        {
            _logger.LogInformation($"Service {service} not authorized for {context.ActionDescriptor?.DisplayName ?? "Unknown"}");
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }

        /// <summary>
        ///     Sets the result of the request to unauthorized
        /// </summary>
        /// <param name="context">
        ///     The request context on which the response is set
        /// </param>
        private void SetUnauthorizedResult(AuthorizationFilterContext context)
        {
            _logger.LogInformation($"User not authenticated for {context.ActionDescriptor?.DisplayName ?? "Unknown"}");
            context.Result = new UnauthorizedResult();
        }
    }
}