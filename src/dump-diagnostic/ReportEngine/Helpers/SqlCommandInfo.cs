using System;
using System.Collections.Generic;
using System.Text;

namespace ReportEngine.Helpers
{
    public class SqlCommandInfo
    {
        public string ConnectionString { get; set; }
        public string CommandString { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string ConnectionState { get; set; }
    }
}
