using System.Collections.Generic;

namespace IdentityServerApi.Models {
    public class SecurityOptions {
        public List<string> AcceptedHosts { get; set; }
        public List<AccessKey> AccessKeys { get; set; }
        public string UiServiceName { get; set; }    
    }

    public class AccessKey {
        public string Service { get; set; }
        public string Key { get; set; }
        public AccessRole Role { get; set; }
    }

    public enum AccessRole {
        ReadOnly,
        IdentityServerUi,
        Cleint
    }
}