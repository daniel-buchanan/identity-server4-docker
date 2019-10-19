using IdentityServerApi.Models;

namespace IdentityServerApi.Services {
    public interface ISecurityService {
        bool Authenticate(string header);
        bool Authenticate(string service, string key);
        AccessRole GetAccess(string header);
        AccessRole GetAccess(string service, string key);
    }
}