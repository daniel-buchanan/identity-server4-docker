using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServerApi.Models {
    public class DbConfigurationOptions {
        public List<Client> Clients { get; set; }
        public List<IdentityResource> IdentityResources { get; set; }
        public List<ApiResource> ApiResources { get; set; }
    }
}