using System.Collections.Generic;

namespace DataMonitoring.Model
{
    public class Connector
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string TimeZone { get; set; }

        public ICollection<IndicatorQuery> IndicatorDefinitions { get; set; }
    }

    public enum ConnectorType
    {
        Api,        // 0
        SQLServer,  // 1
        SQLite,     // 2
        PostgreSQL, // 3
        MySQL,      // 4 
        OracleDB,   // 5
        Firebird,   // 6
    }
}
