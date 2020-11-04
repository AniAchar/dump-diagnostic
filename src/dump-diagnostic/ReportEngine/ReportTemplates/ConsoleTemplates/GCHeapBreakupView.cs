using Microsoft.Diagnostics.Runtime;
using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    class GCHeapBreakupView : TableView<(string GCtype, MemoryRange commitedMemory, MemoryRange reservedMemory)>
    {
        public GCHeapBreakupView(MemoryReportView reportView)
        {
            Items = reportView.GCHeapBrekup.ToList();
            AddColumn(h => h.GCtype, "GC Type");
            AddColumn(h => h.commitedMemory.Length, "Committed  memory in bytes");
            AddColumn(h => h.reservedMemory.Length, "Reserved Memory in bytes");
        }
    }
}
