using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServerApi.Authorization;
using IdentityServerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerApi.Controllers {
    [Authentication(AccessRole.ReadOnly)]
    public class ResourcesController : ControllerBase {
        private readonly IResourceStore _resourceStore;

        public ResourcesController(IResourceStore resourceStore)
        {
            _resourceStore = resourceStore;
        }

        [HttpGet("api/resources")]
        public async Task<Resources> All(bool? enabled, string scopes) {
            string[] parts = null;

            if(!string.IsNullOrWhiteSpace(scopes)) {
                parts = scopes.Split(',', System.StringSplitOptions.None);
            }

            if(enabled != true && parts == null) {
                return await _resourceStore.GetAllResourcesAsync();
            }
            else if (enabled != true && parts != null) {
                return await _resourceStore.FindResourcesByScopeAsync(parts);
            }
            
            if(parts == null) {
                return await _resourceStore.GetAllEnabledResourcesAsync(); 
            }
            else { 
                return await _resourceStore.FindEnabledResourcesByScopeAsync(parts);
            }
        }

        [HttpGet("api/resources/api/{name}")]
        public async Task<ApiResource> FindApiResource(string name) {
            return await _resourceStore.FindApiResourceAsync(name);
        }

        [HttpGet("api/resources/api")]
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScope(string scope) {
            return await _resourceStore.FindApiResourcesByScopeAsync(new[] { scope });
        }

        [HttpGet("api/resources/identity")]
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourceByScope(bool? enabled, string scopes) {
            string[] parts = null;

            if(!string.IsNullOrWhiteSpace(scopes)) {
                parts = scopes.Split(',', System.StringSplitOptions.None);
            }

            if(enabled == true) {
                return await _resourceStore.FindEnabledIdentityResourcesByScopeAsync(parts);
            }

            return await _resourceStore.FindIdentityResourcesByScopeAsync(parts);
        }
    }
}