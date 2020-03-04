using System.ComponentModel.DataAnnotations;

namespace DataMonitoring.Model
{
    public class ApiConnector : Connector
    {
        [StringLength(100)]
        public string BaseUrl { get; set; }

        [StringLength(50)]
        public string AcceptHeader { get; set; }

        public AutorisationType AutorisationType { get; set; }

        [StringLength(100)]
        public string AccessTokenUrl { get; set; }

        [StringLength(100)]
        public string ClientId { get; set; }

        [StringLength(100)]
        public string ClientSecret { get; set; }

        public GrantType? GrantType { get; set; }

        [StringLength(10)]
        public string HttpMethod { get; set; }
    }

    public enum AutorisationType
    {
        NoAuth,
        OAuth20,
    }

    public enum GrantType
    {
        ClientCredentials,
        PasswordCredentials,
    }
}
