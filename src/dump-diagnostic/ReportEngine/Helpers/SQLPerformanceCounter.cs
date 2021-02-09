using System;
using System.Collections.Generic;
using System.Text;

namespace ReportEngine.Helpers
{
    public class SQLPerformanceCounter
    {
        public long HardConnectsPerSecond { get; set; }
        public long HardDisconnectsPerSecond { get; set; }
        public long NumberOfNonPooledConnections { get; set; }
        public long NumberOfPooledConnections { get; set; }
        public long NumberOfActiveConnectionPoolGroups { get; set; }
        public long NumberOfInactiveConnectionPoolGroups { get; set; }
        public long NumberOfActiveConnectionPools { get; set; }
        public long NumberOfInactiveConnectionPools { get; set; }
        public long NumberOfActiveConnections { get; set; }
        public long NumberOfFreeConnections { get; set; }
        public long NumberOfStasisConnections { get; set; }
        public long NumberOfReclaimedConnections { get; set; }

        public long SoftConnectsPerSecond { get; set; }
        public long SoftDisconnectsPerSecond { get; set; }
    }
}
