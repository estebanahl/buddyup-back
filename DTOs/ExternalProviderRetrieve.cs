namespace buddyUp.DTOs
{
    public class ExternalProviderRetrieve
    { 
        public int iat { get; set; }
        public int exp { get; set; }
        public string jti { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
