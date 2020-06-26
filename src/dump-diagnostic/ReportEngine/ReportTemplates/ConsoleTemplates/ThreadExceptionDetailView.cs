using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    class ThreadExceptionDetailView : ContentView
    {
        public ThreadExceptionDetailView(int ManagedThreadId, uint OSThreadId, string stackFrames, string exceptionDetails)
        {
            Span = new ContentSpan($@"
Managed Thread ID: {ManagedThreadId}, OS Thread ID: {OSThreadId}
{stackFrames}
{exceptionDetails}");
        }
    }
}
