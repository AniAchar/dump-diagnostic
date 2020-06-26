using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    class StackFramesStackView : StackLayoutView
    {
        public StackFramesStackView(CrashReportView reportView)
        {
            foreach(var (ManagedThreadId, OSThreadId, stackFrames, exceptionDetails) in reportView.ThreadsWithExceptions)
            {
                Add(new ThreadExceptionDetailView(ManagedThreadId, OSThreadId, stackFrames, exceptionDetails));
            }
        }
    }
}
