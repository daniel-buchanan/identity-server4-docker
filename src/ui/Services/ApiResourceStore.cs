using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Services {
    public class ApiResourceStore : BaseApi, IResourceStore
    {
        public ApiResourceStore(string baseUrl, IHttpClientFactory clientFactory) : base(baseUrl, clientFactory)
        {
        }

        public async Task<ApiResource> FindApiResourceAsync(string name) => await base.GetAsync<ApiResource>($"resources/api/{name}");

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var scopes = string.Empty;
            if(scopeNames != null) {
                scopes = string.Join(',', scopeNames);
            }
            
            return await base.GetAsync<IEnumerable<ApiResource>>("resources/api", UriParameter.Create("scopes", scopes));
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var scopes = string.Empty;
            if(scopeNames != null) {
                scopes = string.Join(',', scopeNames);
            }

            return await base.GetAsync<IEnumerable<IdentityResource>>("resources/identity", UriParameter.Create("scopes", scopes));
        }

        public async Task<Resources> GetAllResourcesAsync() => await base.GetAsync<Resources>("resources");
    }
}