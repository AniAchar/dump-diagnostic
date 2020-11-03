using System;
using System.Collections.Generic;
using System.Text;

namespace ReportEngine.Helpers
{
    public class SQLConnectionInfo
    {
        public string ConnectionString { get; set; }
        public int OpenConnectionCount{ get; set; }
        public int MaxPoolSize { get; set; }
        public int MinPoolSize { get; set; }
        public int CurrentConnectionCount { get; set; }
    }
}
