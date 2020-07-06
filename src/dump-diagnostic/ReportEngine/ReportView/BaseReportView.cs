using System;
using System.Collections.Generic;
using System.Text;

namespace ReportEngine.ReportView
{
    public class BaseReportView
    {
        public string ClrVersion { get; set; }
        public string DotnetFlavor { get; set; }
        public string MainModuleName { get; set; }

        public string GCMode { get; set; }

        public string DumpPath { get; set; }

    }
}
