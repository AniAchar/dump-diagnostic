using ReportEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportEngine.ReportView
{
    public class CrashReportView : BaseReportView
    {
        public IEnumerable<AnalysisThread> ThreadDetails { get; set; }
        public IEnumerable<(int ManagedThreadId, uint OSThreadId, string stackFrames, string exceptionDetails)> ThreadsWithExceptions 
        { get; set; }
    }
}
