using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IdentityServer.Services {
    public abstract class BaseApi {
        private const string ClientName = "Client";

        private readonly string _baseUrl;
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _client;
        
        protected BaseApi(string baseUrl,
            IHttpClientFactory clientFactory) {
            _baseUrl = baseUrl;
            _clientFactory = clientFactory;
            _client = _clientFactory.CreateClient(ClientName);
        }

        protected async Task<T> GetAsync<T>(string uriPart, params IUriParameter[] parameters) {
            var uri = new Uri(new Uri(_baseUrl), uriPart);
            var prependChar = uri.PathAndQuery.Contains("?") ? "&" : "?";

            var query = string.Empty;
            if(parameters == null) parameters = new IUriParameter[] {};
            foreach(var p in parameters) {
                query += prependChar;
                query += $"{p.Name}={p.GetValue()}";

                if(prependChar == "?") prependChar = "&";
            }
            var uriString = uri.ToString() + query;

            var response = await _client.GetAsync(uriString);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }

        protected interface IUriParameter {
            string Name { get; set; }
            string GetValue();
        }

        protected class UriParameter<T> : IUriParameter {
            public string Name { get; set; }
            public T Value { get; set; }

            public string GetValue() {
                return Convert.ToString(Value);
            }
        }
    }
}