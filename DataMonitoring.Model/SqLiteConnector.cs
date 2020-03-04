using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    // TODO : sur la base de SQL Server
    public class SqLiteConnector : Connector
    {
        [StringLength(30)]
        public string HostName { get; set; }

        [StringLength(30)]
        public string DatabaseName { get; set; }

        public bool UseIntegratedSecurity { get; set; }

        [StringLength(30)]
        public string UserName { get; set; }

        [StringLength(30)]
        public string Password { get; set; }

        [NotMapped()]
        public string ConnectionString
        {
            get
            {
               var connectionString = $"Data Source={HostName};Initial Catalog={DatabaseName};Persist Security Info=True;";
                connectionString += UseIntegratedSecurity ? "Integrated Security=True;" : $"Integrated Security=False;User ID={UserName};Password={Password};";
                return connectionString;
            }
        }
    }
}
