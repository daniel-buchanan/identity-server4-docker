using System;
using IdentityServerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerApi.Authorization {
    public sealed class AuthenticationAttribute : ServiceFilterAttribute
    {
        public AuthenticationAttribute() : this(AccessRole.ReadOnly)
        {
            
        }

        public AuthenticationAttribute(AccessRole role) : base(typeof(AuthenticationFilter))
        {
            Role = role;
        }

        public AccessRole Role { get; }
    }
}