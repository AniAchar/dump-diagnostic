using ReportEngine.Helpers;
using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Text;
using System.Threading;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    class ThreadsDetailView : TableView<AnalysisThread>
    {
        public ThreadsDetailView(CrashReportView reportView)
        {
            Items = reportView.ThreadDetails.ToList();
            AddColumn(t => t.OSThreadId, "OS Thread ID");
            AddColumn(t => t.ManagedThreadId, "Managed thread ID");
            AddColumn(t => t.Address, "Address");
            AddColumn(t => t.StackBase, "Stack Base");
            AddColumn(t => t.StackLimit, "Stack Limit");
            AddColumn(t => t.LockCount, "Lock");
            AddColumn(t => t.GCMode, "GC Mode");
            AddColumn(t => t.Apartment, "Apartment");
            AddColumn(t => t.Finalizer, "Finalizer thread");
            AddColumn(t => t.CurrentException, "Exception");
        }
    }
}
