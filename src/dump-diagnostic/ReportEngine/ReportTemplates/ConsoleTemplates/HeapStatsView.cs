using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    class HeapStatsView : TableView<(string type, int count, long size)>
    {
        public HeapStatsView(MemoryReportView reportView)
        {
            var count = reportView.HeapStats.Count() < 50 ? reportView.HeapStats.Count() : 50;
            Items = reportView.HeapStats.Take(count).ToList();
            AddColumn(h => h.type, "Type");
            AddColumn(h => h.count, "Count");
            AddColumn(h => h.size, "Size");
        }
    }
}
