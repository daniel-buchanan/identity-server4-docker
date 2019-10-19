namespace IdentityServerApi.Services {
    public sealed class SecurityHeader {
        public string Service { get; }
        public string Key { get; }

        public SecurityHeader() { }

        public SecurityHeader(string service, string key)
        {
            Service = service;
            Key = key;
        }

        public static bool TryParse(string header, out SecurityHeader output) {
            output = default(SecurityHeader);

            if(string.IsNullOrWhiteSpace(header)) return false;                

            var parts = header.Split(' ', System.StringSplitOptions.None);

            if(parts.Length != 2) return false;

            var service = parts[0];
            var key = parts[1];

            output = new SecurityHeader(service, key);
            
            return true;
        }
    }
}