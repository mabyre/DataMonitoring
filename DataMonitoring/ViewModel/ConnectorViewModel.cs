using System.ComponentModel.DataAnnotations;
using DataMonitoring.Model;

namespace DataMonitoring.ViewModel
{
    public class ConnectorViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string TimeZone { get; set; }

        public ApiConnectorViewModel ApiConnector { get; set; } // 0

        public SqlServerConnectorViewModel SqlServerConnector { get; set; } // 1

        public SqLiteConnectorViewModel SqLiteConnector { get; set; } // 2
    }

    public class ApiConnectorViewModel
    {
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

    public class SqlServerConnectorViewModel
    {
        public string HostName { get; set; }
        public string DatabaseName { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class SqLiteConnectorViewModel
    {
        public string HostName { get; set; }
        public string DatabaseName { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
